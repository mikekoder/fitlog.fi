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
    public class FoodsController : ApiControllerBase
    {
        private readonly INutritionRepository nutritionRepository;
        public FoodsController(INutritionRepository nutritionRepository)
        {
            this.nutritionRepository = nutritionRepository;
        }

        [HttpGet]
        [Route("search")]
        public IEnumerable<FoodMinimal> Search(string name)
        {
            var foods = nutritionRepository.SearchFoods(name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            return foods;
        }
        [HttpGet]
        [Route("{id}")]
        public FoodDetails Details(Guid id)
        {
            var food = nutritionRepository.GetFood(id);
            return food;
        }
        [HttpPost]
        [Route("")]
        public FoodDetails Create(FoodRequest request)
        {
            var food = AutoMapper.Mapper.Map<FoodDetails>(request);
            if (food.IsRecipe)
            {
                CalculateNutrients(food);
            }
            nutritionRepository.CreateFood(food);
            return food;
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update(Guid id, FoodRequest request)
        {
            var food = nutritionRepository.GetFood(id);
            if (food.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            AutoMapper.Mapper.Map(request, food);
            nutritionRepository.UpdateFood(food);
            return Ok(food);
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete(Guid id)
        {
            var food = nutritionRepository.GetFood(id);
            if(food.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            nutritionRepository.DeleteFood(food);
            return Ok();
        }

        private void CalculateNutrients(FoodDetails food)
        {

        }
    }
}