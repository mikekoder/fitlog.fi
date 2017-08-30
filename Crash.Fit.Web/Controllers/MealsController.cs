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
            nutritionRepository.SaveMealDefinitions(definitions);
            return GetDefinitions();
        }
        [HttpPost("add-row")]
        public IActionResult AddRow([FromBody]AddMealRowRequest request)
        {
            
            var mealRow = new MealRow
            {
                FoodId = request.FoodId,
                Quantity = request.Amount,
                PortionId = request.PortionId
            };
            MealDetails meal;
            if (request.MealId.HasValue)
            {
                meal = nutritionRepository.GetMeal(request.MealId.Value);
                if(meal == null)
                {
                    return NotFound();
                }
                if(meal.UserId != CurrentUserId)
                {
                    return Unauthorized();
                }
                mealRow.MealId = meal.Id;
                meal.Rows = meal.Rows.Union(new[] { mealRow }).ToArray();
                CalculateNutrients(meal);
                nutritionRepository.AddMealRow(mealRow);
            }
            else if (request.MealDefinitionId.HasValue)
            {
                var start = DateTimeUtils.ToLocal(request.Date).Date;
                var end = start.AddDays(1).AddMilliseconds(-1);

                meal = nutritionRepository.SearchMeals(CurrentUserId,start,end).FirstOrDefault(m => m.DefinitionId == request.MealDefinitionId);
                if(meal == null)
                {
                    var def = nutritionRepository.GetMealDefinitions(CurrentUserId).Single(d => d.Id == request.MealDefinitionId.Value);
                    meal = new MealDetails
                    {
                        UserId = CurrentUserId,
                        DefinitionId = def.Id,
                        Time = DateTimeUtils.CreateLocal(start, def.Time),
                        Rows = new[]
                        {
                            mealRow
                        }
                    };
                    CalculateNutrients(meal);
                    nutritionRepository.CreateMeal(meal);
                }
            }

            var result = AutoMapper.Mapper.Map<MealRowModel>(mealRow);
            return Ok(result);
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