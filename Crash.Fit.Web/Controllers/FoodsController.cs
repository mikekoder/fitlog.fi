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
using Crash.Fit.External.Foodie;
using System.Threading;
using Crash.Fit.External.KRuoka;
using Crash.Fit.External;

namespace Crash.Fit.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class FoodsController : ApiControllerBase
    {
        private readonly INutritionRepository nutritionRepository;
        public FoodsController(INutritionRepository nutritionRepository, ILogRepository logger) : base(logger)
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
        public async Task<IActionResult> Search(string name)
        {
            IEnumerable<FoodSearchResult> foods;
            if (EanUtils.IsAllNumbers(name))
            {
                foods = nutritionRepository.SearchFoodsByEan(name, CurrentUserId);
                if (!foods.Any())
                {
                    var extFood = await SearchExternalFood(name);
                    if (extFood != null)
                    {
                        var newFood = new FoodDetails
                        {
                            UserId = CurrentUserId,
                            Ean = name,
                            Manufacturer = extFood.Manufacturer,
                            Name = extFood.Name,
                            Nutrients = extFood.Nutrients.Select(n => new FoodNutrientAmount
                            {
                                NutrientId = n.NutrientId,
                                Amount = n.Amount
                            }).ToArray()
                        };

                        nutritionRepository.CreateFood(newFood);
                        foods = new[]
                        {
                            new FoodSearchResult
                            {
                                Id = newFood.Id,
                                Created = newFood.Created,
                                Ean = newFood.Ean,
                                Manufacturer = newFood.Manufacturer,
                                Name = newFood.Name,
                                UserId = newFood.UserId
                            }
                        };
                    }
                }
            }
            else
            {
                foods = nutritionRepository.SearchFoods(name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries), CurrentUserId);
            }
            foods = foods.OrderBy(f => f.LatestUse.HasValue || f.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase) ? 1 : 2)
                .ThenBy(f => f.Name.StartsWith(name, StringComparison.CurrentCultureIgnoreCase) ? 1 : 2)
                .ThenBy(f => f.Name);
            var response = AutoMapper.Mapper.Map<FoodSearchResultResponse[]>(foods);
            return Ok(response);
        }

        [HttpGet("search-external")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchExternal(string ean)
        {
            if (string.IsNullOrWhiteSpace(ean))
            {
                return NoContent();
            }

            if (EanUtils.IsInternalEan13(ean))
            {
                ean = EanUtils.NormalizeInternalEan13(ean);
            }

            var response = await SearchExternalFood(ean);
            if (response != null)
            {
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
            var food = nutritionRepository.GetFood(id, CurrentUserId);
            if (food == null || (food.UserId != null && food.UserId != CurrentUserId))
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
                if (!food.UserId.HasValue && FoodHasChanges(food, request))
                {
                    request.Name += " (oma)";
                    foreach(var portion in request.Portions)
                    {
                        portion.Id = Guid.Empty;
                    }
                    return Create(request);
                }
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
            if (food.UserId != CurrentUserId)
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
                if (food.Nutrients != null)
                {
                    foreach (var nutrientAmount in food.Nutrients)
                    {
                        nutrientAmount.PortionAmount = nutrientAmount.Amount;
                        nutrientAmount.Amount = nutrientAmount.Amount * (100m / portionWeight);
                    }
                }
            }
        }

        private async Task<FoodExternalDetailsResponse> SearchExternalFood(string ean)
        {
            var clients = new IStoreClient[]
            {
                new FoodieClient(),
                new KRuokaClient()
            };
            var cts = new CancellationTokenSource();
            var tasks = clients.Select(c => Task.Run(() => c.FindFood(ean), cts.Token)).ToList();
            while (tasks.Any())
            {
                var completed = await Task.WhenAny(tasks);
                tasks.Remove(completed);
                if(completed.IsCompleted && completed.Result != null)
                {
                    cts.Cancel();

                    var food = completed.Result;
                    var nutrients = new List<FoodNutrientAmountResponse>();
                    if (food.Carbohydrate.HasValue)
                    {
                        nutrients.Add(new FoodNutrientAmountResponse
                        {
                            NutrientId = Constants.Nutrition.CarbId,
                            Amount = food.Carbohydrate.Value
                        });
                    }
                    if (food.Fat.HasValue)
                    {
                        nutrients.Add(new FoodNutrientAmountResponse
                        {
                            NutrientId = Constants.Nutrition.FatId,
                            Amount = food.Fat.Value
                        });
                    }
                    if (food.FatSaturated.HasValue)
                    {
                        nutrients.Add(new FoodNutrientAmountResponse
                        {
                            NutrientId = Constants.Nutrition.FatSaturatedId,
                            Amount = food.FatSaturated.Value
                        });
                    }
                    if (food.Fiber.HasValue)
                    {
                        nutrients.Add(new FoodNutrientAmountResponse
                        {
                            NutrientId = Constants.Nutrition.FiberId,
                            Amount = food.Fiber.Value
                        });
                    }
                    if (food.Kcal.HasValue)
                    {
                        nutrients.Add(new FoodNutrientAmountResponse
                        {
                            NutrientId = Constants.Nutrition.EnergyKcalId,
                            Amount = food.Kcal.Value
                        });
                    }
                    if (food.Kj.HasValue)
                    {
                        nutrients.Add(new FoodNutrientAmountResponse
                        {
                            NutrientId = Constants.Nutrition.EnergyKjId,
                            Amount = food.Kj.Value
                        });
                    }
                    if (food.Protein.HasValue)
                    {
                        nutrients.Add(new FoodNutrientAmountResponse
                        {
                            NutrientId = Constants.Nutrition.ProteinId,
                            Amount = food.Protein.Value
                        });
                    }
                    if (food.Sugar.HasValue)
                    {
                        nutrients.Add(new FoodNutrientAmountResponse
                        {
                            NutrientId = Constants.Nutrition.SugarId,
                            Amount = food.Sugar.Value
                        });
                    }
                    return new FoodExternalDetailsResponse
                    {
                        Ean = food.Ean,
                        Name = food.Name,
                        Manufacturer = food.Manufacturer,
                        Nutrients = nutrients.ToArray()
                    };
                }
            }

            return null;
        }
        private bool FoodHasChanges(FoodDetails food, FoodRequest request)
        {
            if(food.Ean != request.Ean)
            {
                return true;
            }
            if (food.Manufacturer != request.Manufacturer)
            {
                return true;
            }
            if (food.Name != request.Name)
            {
                return true;
            }
            if (food.Nutrients != null)
            {
                foreach (var nutrient in food.Nutrients)
                {
                    var nutrient2 = request.Nutrients.FirstOrDefault(n => n.NutrientId == nutrient.NutrientId);
                    if (nutrient2 == null || nutrient2.Amount != nutrient.Amount)
                    {
                        return true;
                    }
                }
            }
            if (food.Portions != null)
            {
                if(food.Portions.Length != (request.Portions?.Length ?? 0))
                {
                    return true;
                }
                foreach (var portion in food.Portions)
                {
                    var portion2 = request.Portions.FirstOrDefault(p => p.Id == portion.Id);
                    if (portion2 == null || portion2.Name != portion.Name || portion2.Weight != portion.Weight)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}