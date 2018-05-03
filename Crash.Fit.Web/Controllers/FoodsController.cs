using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Crash.Fit.Nutrition;
using Crash.Fit.Api.Models.Nutrition;
using Microsoft.AspNetCore.Mvc.Filters;
using Crash.Fit.Logging;
using Microsoft.AspNetCore.Authorization;
using Crash.Fit.External.Foodin;

namespace Crash.Fit.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class FoodsController : ApiControllerBase
    {
        private readonly INutritionRepository nutritionRepository;
        public FoodsController(INutritionRepository nutritionRepository, ILogRepository logger):base(logger)
        {
            this.nutritionRepository = nutritionRepository;
        }
        [HttpGet("")]
        public IActionResult List()
        {
            var foods = nutritionRepository.SearchUserFoods(CurrentUserId).OrderBy(f => f.Name);

            var response = AutoMapper.Mapper.Map<FoodSummaryResponse[]>(foods);
            return Ok(response);
        }
        [HttpGet("search")]
        [AllowAnonymous]
        public IActionResult Search(string name)
        {
            var foods = nutritionRepository.SearchFoods(name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries), CurrentUserId);
            foods = foods.OrderBy(f => f.LatestUse.HasValue || f.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase) ? 1 : 2)
                .ThenBy(f => f.Name.StartsWith(name, StringComparison.CurrentCultureIgnoreCase) ? 1 : 2)
                .ThenBy(f => f.Name);
            var response = AutoMapper.Mapper.Map<FoodSearchResultResponse[]>(foods);
            return Ok(response);
        }
        [HttpGet("search-external")]
        [AllowAnonymous]
        public IActionResult SearchExternal(string ean)
        {
            var foodinClient = new FoodinClient("https://www.foodie.fi");
            var entry = foodinClient.GetEntry(ean);
            if(entry != null)
            {
                var nutrients = new List<FoodNutrientAmountResponse>();
                if (entry.Carbohydrate.HasValue)
                {
                    nutrients.Add(new FoodNutrientAmountResponse
                    {
                        NutrientId = Constants.Nutrition.CarbId,
                        Amount = entry.Carbohydrate.Value
                    });
                }
                if (entry.Fat.HasValue)
                {
                    nutrients.Add(new FoodNutrientAmountResponse
                    {
                        NutrientId = Constants.Nutrition.FatId,
                        Amount = entry.Fat.Value
                    });
                }
                if (entry.FatSaturated.HasValue)
                {
                    nutrients.Add(new FoodNutrientAmountResponse
                    {
                        NutrientId = Constants.Nutrition.FatSaturatedId,
                        Amount = entry.FatSaturated.Value
                    });
                }
                if (entry.Fiber.HasValue)
                {
                    nutrients.Add(new FoodNutrientAmountResponse
                    {
                        NutrientId = Constants.Nutrition.FiberId,
                        Amount = entry.Fiber.Value
                    });
                }
                if (entry.Kcal.HasValue)
                {
                    nutrients.Add(new FoodNutrientAmountResponse
                    {
                        NutrientId = Constants.Nutrition.EnergyKcalId,
                        Amount = entry.Kcal.Value
                    });
                }
                if (entry.Kj.HasValue)
                {
                    nutrients.Add(new FoodNutrientAmountResponse
                    {
                        NutrientId = Constants.Nutrition.EnergyKjId,
                        Amount = entry.Kj.Value
                    });
                }
                if (entry.Protein.HasValue)
                {
                    nutrients.Add(new FoodNutrientAmountResponse
                    {
                        NutrientId = Constants.Nutrition.ProteinId,
                        Amount = entry.Protein.Value
                    });
                }
                if (entry.Sugar.HasValue)
                {
                    nutrients.Add(new FoodNutrientAmountResponse
                    {
                        NutrientId = Constants.Nutrition.SugarId,
                        Amount = entry.Sugar.Value
                    });
                }
                var response = new FoodExternalDetailsResponse
                {
                    Ean = entry.Ean,
                    Name = entry.Name,
                    Manufacturer = entry.Subname,
                    Nutrients = nutrients.ToArray()
                };
                return Ok(response);
            }
            return NoContent();
        }
        [HttpGet("search/most-nutrients")]
        [AllowAnonymous]
        public IActionResult SearchMostNutrients(int nutrientId, int? count)
        {
            var foods = nutritionRepository.SearchFoodsTopNutrients(nutrientId, CurrentUserId, count ?? 50);
 
            var response = AutoMapper.Mapper.Map<FoodSearchNutrientResultResponse[]>(foods);
            return Ok(response);
        }
        [HttpGet("search/least-nutrients")]
        [AllowAnonymous]
        public IActionResult SearchLeastNutrients(int nutrientId, int? count)
        {
            var foods = nutritionRepository.SearchFoodsTopNutrients(nutrientId, CurrentUserId, count ?? 50, false);

            var response = AutoMapper.Mapper.Map<FoodSearchNutrientResultResponse[]>(foods);
            return Ok(response);
        }
        [HttpGet("latest")]
        public IActionResult Latest(int? count)
        {
            var foods = nutritionRepository.SearchLatestFoods(CurrentUserId, count ?? 10);
            var response = AutoMapper.Mapper.Map<FoodSearchResultResponse[]>(foods);
            return Ok(response);
        }
        [HttpGet("most-used")]
        public IActionResult MostUsed(int? count)
        {
            var foods = nutritionRepository.SearchMostUsedFoods(CurrentUserId, count ?? 10);
            var response = AutoMapper.Mapper.Map<FoodSearchResultResponse[]>(foods);
            return Ok(response);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
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
        [HttpPost("")]
        public IActionResult Create([FromBody]FoodRequest request)
        {
            CheckNutrientPortion(request);
            var food = AutoMapper.Mapper.Map<FoodDetails>(request);
            food.UserId = CurrentUserId;
            CalculatePortionNutrients(request, food);
            nutritionRepository.CreateFood(food);

            var response = AutoMapper.Mapper.Map<FoodDetailsResponse>(food);
            return Ok(food);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody]FoodRequest request)
        {
            var food = nutritionRepository.GetFood(id);
            if (food.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            CheckNutrientPortion(request);
            AutoMapper.Mapper.Map(request, food);
            CalculatePortionNutrients(request, food);
            nutritionRepository.UpdateFood(food);

            var response = AutoMapper.Mapper.Map<FoodDetailsResponse>(food);
            return Ok(food);
        }

        [HttpDelete("{id}")]
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

        private void CheckNutrientPortion(FoodRequest request)
        {
            var nutrientPortion = request.Portions?.FirstOrDefault(p => p.NutrientPortion);
            if (nutrientPortion != null && nutrientPortion.Id == Guid.Empty)
            {
                nutrientPortion.Id = Guid.NewGuid();
            }
        }
        private void CalculatePortionNutrients(FoodRequest request, FoodDetails food)
        {
            var nutrientPortion = request.Portions.FirstOrDefault(p => p.NutrientPortion);
            if (nutrientPortion != null)
            {
                food.NutrientPortionId = nutrientPortion.Id;

                var portionWeight = nutrientPortion.Weight;
                foreach(var nutrientAmount in food.Nutrients)
                {
                    nutrientAmount.PortionAmount = nutrientAmount.Amount;
                    nutrientAmount.Amount = nutrientAmount.Amount * (100m / portionWeight);
                }
            }
        }
    }
}