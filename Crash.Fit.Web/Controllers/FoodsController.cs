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
        [Route("")]
        public IActionResult List()
        {
            var foods = nutritionRepository.SearchUserFoods(CurrentUserId);

            var response = AutoMapper.Mapper.Map<FoodSummaryResponse[]>(foods);
            return Ok(response);
        }
        [HttpGet]
        [Route("search")]
        public IActionResult Search(string name)
        {
            var foods = nutritionRepository.SearchFoods(name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries), CurrentUserId);

            var response = AutoMapper.Mapper.Map<FoodSummaryResponse[]>(foods);
            return Ok(response);
        }
        [HttpGet]
        [Route("{id}")]
        public IActionResult Details(Guid id)
        {
            var food = nutritionRepository.GetFood(id);
            if(food == null || (food.UserId != null && food.UserId != CurrentUserId))
            {
                return NotFound();
            }

            var response = AutoMapper.Mapper.Map<FoodDetailsResponse>(food);
            return Ok(response);
        }
        [HttpPost]
        [Route("")]
        public IActionResult Create([FromBody]FoodRequest request)
        {
            var food = AutoMapper.Mapper.Map<FoodDetails>(request);
            food.UserId = CurrentUserId;
            nutritionRepository.CreateFood(food);

            var response = AutoMapper.Mapper.Map<FoodDetailsResponse>(food);
            return Ok(food);
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update(Guid id, [FromBody]FoodRequest request)
        {
            var food = nutritionRepository.GetFood(id);
            if (food.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            AutoMapper.Mapper.Map(request, food);
            nutritionRepository.UpdateFood(food);

            var response = AutoMapper.Mapper.Map<FoodDetailsResponse>(food);
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
    }
}