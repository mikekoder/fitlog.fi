using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Crash.Fit.Nutrition;
using Crash.Fit.Api.Models.Nutrition;
using Crash.Fit.Logging;

namespace Crash.Fit.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MealsController : ApiControllerBase
    {
        private readonly INutritionRepository nutritionRepository;
        public MealsController(INutritionRepository nutritionRepository, ILogRepository logger) : base(logger)
        {
            this.nutritionRepository = nutritionRepository;
        }
        [HttpGet("")]
        public IActionResult List(DateTimeOffset start, DateTimeOffset? end)
        {
            var meals = nutritionRepository.SearchMeals(CurrentUserId, start, end ?? DateTimeOffset.Now);

            var response = AutoMapper.Mapper.Map<MealDetailsResponse[]>(meals.OrderByDescending(m => m.Time));
            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult Details(Guid id)
        {
            var meal = nutritionRepository.GetMeal(id);

            var response = AutoMapper.Mapper.Map<MealDetailsResponse>(meal);
            return Ok(response);
        }
        [HttpPost("")]
        public IActionResult Create([FromBody]MealRequest request)
        {
            var meal = AutoMapper.Mapper.Map<MealDetails>(request);
            meal.UserId = CurrentUserId;
            meal.Created = DateTimeOffset.Now;
            AdjustTime(meal);
            CalculateNutrients(meal);
            nutritionRepository.CreateMeal(meal);

            var result = AutoMapper.Mapper.Map<MealDetailsResponse>(meal);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody]MealRequest request)
        {
            var meal = nutritionRepository.GetMeal(id);
            if(meal.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            AutoMapper.Mapper.Map(request, meal);
            AdjustTime(meal);
            CalculateNutrients(meal);
            if(!nutritionRepository.UpdateMeal(meal))
            {
                return BadRequest();
            }
           
            var result = AutoMapper.Mapper.Map<MealDetailsResponse>(meal);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var meal = nutritionRepository.GetMeal(id);
            if (meal.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            nutritionRepository.DeleteMeal(meal);

            return Ok();
        }

        [HttpPost("{id}/restore")]
        public IActionResult Restore(Guid id)
        {
            var meal = nutritionRepository.GetMeal(id);
            if(meal == null)
            {
                return NotFound();
            }
            if (meal.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            nutritionRepository.RestoreMeal(meal.Id, out MealDetails restoredMeal);

            var result = AutoMapper.Mapper.Map<MealDetailsResponse>(restoredMeal);
            return Ok(result);
        }
        [HttpGet("definitions")]
        public IActionResult GetDefinitions()
        {
            var definitions = nutritionRepository.GetMealDefinitions(CurrentUserId);
            var response = AutoMapper.Mapper.Map<MealDefinitionResponse[]>(definitions);
            return Ok(response);
        }
        [HttpPut("definitions")]
        public IActionResult UpdateDefinitions([FromBody] MealDefinitionRequest[] request)
        {
            var definitions = new List<MealDefinition>();
            foreach(var model in request)
            {
                var startParts = (model.Start ?? "").Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                var endParts = (model.End ?? "").Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                definitions.Add(new MealDefinition
                {
                    UserId = CurrentUserId,
                    Name = model.Name,
                    Start = startParts.Length > 0 ? new TimeSpan(int.Parse(startParts[0]), startParts.Length > 1 ? int.Parse(startParts[1]) : 0,0) : null as TimeSpan?,
                    End = endParts.Length > 0 ? new TimeSpan(int.Parse(endParts[0]), endParts.Length > 1 ? int.Parse(endParts[1]) : 0, 0) : null as TimeSpan?
                    /*
                    StartHour = startParts.Length > 0 ? int.Parse(startParts[0]) : null as int?,
                    StartMinute = startParts.Length > 1 ? int.Parse(startParts[1]) : null as int?,
                    EndHour = endParts.Length > 0 ? int.Parse(endParts[0]) : null as int?,
                    EndMinute = endParts.Length > 1 ? int.Parse(endParts[1]) : null as int?
                    */
                });
            }
            nutritionRepository.SaveMealDefinitions(CurrentUserId, definitions);
            return GetDefinitions();
        }
        [HttpPost("rows")]
        public IActionResult AddRow([FromBody]AddMealRowRequest request)
        {
            var dayStart = DateTimeUtils.ToLocal(request.Date).Date;
            var dayEnd = dayStart.AddDays(1).AddMilliseconds(-1);
            var mealRow = new MealRow
            {
                FoodId = request.FoodId,
                Quantity = request.Quantity,
                PortionId = request.PortionId
            };
            MealDetails meal;
            if (request.MealId.HasValue)
            {
                meal = nutritionRepository.GetMeal(request.MealId.Value);
            }
            else if (request.MealDefinitionId.HasValue)
            {
                meal = nutritionRepository.SearchMeals(CurrentUserId,dayStart,dayEnd).FirstOrDefault(m => m.DefinitionId == request.MealDefinitionId);
            }
            else
            {
                return BadRequest();
            }
            if(meal == null)
            {
                if (request.MealId.HasValue)
                {
                    return NotFound();
                }
                else if (request.MealDefinitionId.HasValue)
                {
                    var def = nutritionRepository.GetMealDefinitions(CurrentUserId).Single(d => d.Id == request.MealDefinitionId.Value);
                    if(def == null)
                    {
                        return BadRequest();
                    }

                    meal = new MealDetails
                    {
                        UserId = CurrentUserId,
                        DefinitionId = def.Id,
                        Time = DateTimeUtils.CreateLocal(dayStart, def.Time),
                        Rows = new MealRow[] { }
                    };
                }
            }
            if (meal.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            mealRow.MealId = meal.Id;
            meal.Rows = meal.Rows.Union(new[] { mealRow }).ToArray();
            CalculateNutrients(meal);
            if (meal.Id == Guid.Empty)
            {
                nutritionRepository.CreateMeal(meal);
            }
            else
            {
                nutritionRepository.UpdateMeal(meal);
            }

            var result = AutoMapper.Mapper.Map<MealRowModel>(mealRow);
            return Ok(result);
        }
        [HttpPut("{mealId}/rows/{id}")]
        public IActionResult UpdateRow(Guid mealId, Guid id, [FromBody]AddMealRowRequest request)
        {
            if (!request.MealId.HasValue)
            {
                return BadRequest();
            }
            var meal = nutritionRepository.GetMeal(request.MealId.Value);
            if(meal == null)
            {
                return NotFound();
            }
            if (meal.UserId != CurrentUserId)
            {
                return Unauthorized();
            }

            var row = meal.Rows.FirstOrDefault(r => r.Id == id);
            if(row == null)
            {
                return NotFound();
            }
            row.FoodId = request.FoodId;
            row.Quantity = request.Quantity;
            row.PortionId = request.PortionId;
            row.Nutrients = null;
            CalculateNutrients(meal);

            nutritionRepository.UpdateMeal(meal);

            var result = AutoMapper.Mapper.Map<MealRowModel>(row);
            return Ok(result);
        }
        [HttpDelete("{mealId}/rows/{id}")]
        public IActionResult DeleteRow(Guid mealId, Guid id)
        {
            var meal = nutritionRepository.GetMeal(mealId);
            if (meal == null)
            {
                return NotFound();
            }
            if (meal.UserId != CurrentUserId)
            {
                return Unauthorized();
            }

            meal.Rows = meal.Rows.Where(r => r.Id != id).ToArray();
            CalculateNutrients(meal);
            nutritionRepository.UpdateMeal(meal);

            return Ok();
        }
        private void AdjustTime(MealDetails meal)
        {
            if (meal.DefinitionId.HasValue)
            {
                var definition = nutritionRepository.GetMealDefinitions(CurrentUserId).FirstOrDefault(d => d.Id == meal.DefinitionId.Value);
                if (definition != null)
                {
                    var time = definition.Time;
                    var datetime = new DateTime(meal.Time.Year, meal.Time.Month, meal.Time.Day, time.Hours, time.Minutes, time.Seconds);
                    meal.Time = DateTimeUtils.CreateLocal(meal.Time, definition.Time); 
                }
            }
        }
        private void CalculateNutrients(MealDetails meal)
        {
            var foodIds = meal.Rows.Where(r => r.Nutrients == null || r.Nutrients.Length == 0).Select(r => r.FoodId);
            var foods = nutritionRepository.GetFoods(foodIds);
            foreach(var row in meal.Rows.Where(r => r.Nutrients == null || r.Nutrients.Length == 0))
            {
                var food = foods.Single(f => f.Id == row.FoodId);
                row.FoodName = food.Name;
                if (row.PortionId.HasValue)
                {
                    var portion = food.Portions.Single(p => p.Id == row.PortionId);
                    row.Weight = row.Quantity * portion.Weight;
                    row.PortionName = portion.Name;
                }
                else
                {
                    row.Weight = row.Quantity;
                }
                row.Nutrients = food.Nutrients.Select(n => new NutrientAmount
                {
                    NutrientId = n.NutrientId,
                    Amount = row.Weight * n.Amount / 100m
                }).ToArray();
            }
            meal.Nutrients = meal.Rows.SelectMany(r => r.Nutrients).GroupBy(n => n.NutrientId, n => n.Amount).Select(n => new NutrientAmount
            {
                NutrientId = n.Key,
                Amount = n.Sum()
            }).ToArray();
        }
    }
}