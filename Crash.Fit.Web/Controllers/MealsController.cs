using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Crash.Fit.Nutrition;
using Crash.Fit.Web.Models.Nutrition;

namespace Crash.Fit.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MealsController : ApiControllerBase
    {
        private readonly INutritionRepository nutritionRepository;
        public MealsController(INutritionRepository nutritionRepository)
        {
            this.nutritionRepository = nutritionRepository;
        }
        [HttpGet]
        [Route("")]
        public IEnumerable<MealDetails> List(DateTimeOffset start, DateTimeOffset? end)
        {
            var meals = nutritionRepository.SearchMeals(CurrentUserId, start, end ?? DateTimeOffset.Now);
            return meals;
        }
        [HttpGet]
        [Route("{id}")]
        public MealDetails Details(Guid id)
        {
            var meal = nutritionRepository.GetMeal(id);
            return meal;
        }
        [HttpPost]
        [Route("")]
        public IActionResult Create([FromBody]MealRequest request)
        {
            var meal = AutoMapper.Mapper.Map<MealDetails>(request);
            meal.UserId = CurrentUserId;
            CalculateNutrients(meal);
            if(!nutritionRepository.CreateMeal(meal))
            {
                return BadRequest();
            }
            var result = AutoMapper.Mapper.Map<MealResponse>(meal);
            return Ok(result);
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update(Guid id, MealRequest request)
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
           
            var result = AutoMapper.Mapper.Map<MealResponse>(meal);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
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