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
        public MealDetails Create(MealRequest request)
        {
            var meal = AutoMapper.Mapper.Map<MealDetails>(request);
            meal.UserId = CurrentUserId;
            CalculateNutrients(meal);
            nutritionRepository.CreateMeal(meal);
            return meal;
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
            nutritionRepository.UpdateMeal(meal);
            return Ok(meal);
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

        }
    }
}