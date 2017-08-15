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
            CalculateNutrients(meal);
            if(!nutritionRepository.CreateMeal(meal))
            {
                return BadRequest();
            }

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
            MealDetails restoredMeal;
            nutritionRepository.RestoreMeal(meal.Id, out restoredMeal);

            var result = AutoMapper.Mapper.Map<MealDetailsResponse>(restoredMeal);
            return Ok(result);
        }

        private void CalculateNutrients(MealDetails meal)
        {
            var foodIds = meal.Rows.Select(r => r.FoodId);
            var foods = nutritionRepository.GetFoods(foodIds);
            var mealNutrients = new List<NutrientAmount>();
            foreach(var row in meal.Rows)
            {
                var food = foods.Single(f => f.Id == row.FoodId);
                if (row.PortionId.HasValue)
                {
                    var portion = food.Portions.Single(p => p.Id == row.PortionId);
                    row.Weight = row.Quantity * portion.Weight;
                }
                else
                {
                    row.Weight = row.Quantity;
                }
                foreach (var foodNutrient in food.Nutrients)
                {
                    var mealNutrient = mealNutrients.SingleOrDefault(n => n.NutrientId == foodNutrient.NutrientId);
                    if(mealNutrient == null)
                    {
                        mealNutrient = new NutrientAmount
                        {
                            NutrientId = foodNutrient.NutrientId
                        };
                        mealNutrients.Add(mealNutrient);
                    }
                    mealNutrient.Amount += (row.Weight * foodNutrient.Amount) / 100m;
                }
            }
            meal.Nutrients = mealNutrients.ToArray();
        }
    }
}