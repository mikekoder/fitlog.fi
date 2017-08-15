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
    public class RecipesController : ApiControllerBase
    {
        private readonly INutritionRepository nutritionRepository;
        public RecipesController(INutritionRepository nutritionRepository, ILogRepository logger) : base(logger)
        {
            this.nutritionRepository = nutritionRepository;
        }
        [HttpGet("")]
        public IActionResult List()
        {
            var recipes = nutritionRepository.SearchRecipes(CurrentUserId);

            var response = AutoMapper.Mapper.Map<RecipeSummaryResponse[]>(recipes);
            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult Details(Guid id)
        {
            var food = nutritionRepository.GetFood(id);
            if (!food.IsRecipe)
            {
                return NotFound();
            }
            var result = AutoMapper.Mapper.Map<RecipeDetailsResponse>(food);
            return Ok(food);
        }
        [HttpPost("")]
        public IActionResult Create([FromBody]RecipeRequest request)
        {
            var recipe = AutoMapper.Mapper.Map<FoodDetails>(request);
            recipe.UserId = CurrentUserId;
            recipe.IsRecipe = true;
            
            CalculateNutrients(recipe);
            CalculatePortionWeights(recipe);
            if(!nutritionRepository.CreateFood(recipe))
            {
                return BadRequest();
            }

            var result = AutoMapper.Mapper.Map<RecipeDetailsResponse>(recipe);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody]RecipeRequest request)
        {
            var recipe = nutritionRepository.GetFood(id);
            if (!recipe.IsRecipe)
            {
                return NotFound();
            }
            if (recipe.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
           
            AutoMapper.Mapper.Map(request, recipe);
            CalculateNutrients(recipe);
            CalculatePortionWeights(recipe);
            if (!nutritionRepository.UpdateFood(recipe))
            {
                return BadRequest();
            }
           
            var result = AutoMapper.Mapper.Map<RecipeDetailsResponse>(recipe);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var recipe = nutritionRepository.GetFood(id);
            if (recipe.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            nutritionRepository.DeleteFood(recipe);

            return Ok();
        }

        private void CalculateNutrients(FoodDetails recipe)
        {
            recipe.Ingredients = recipe.Ingredients.Where(i => i.FoodId != Guid.Empty).ToArray();

            var foodIds = recipe.Ingredients.Select(i => i.FoodId);
            var foods = nutritionRepository.GetFoods(foodIds);
            var recipeNutrients = new List<NutrientAmount>();
            var recipeWeight = 0m;
            foreach(var ingredient in recipe.Ingredients)
            {
                var food = foods.Single(f => f.Id == ingredient.FoodId);
                if (ingredient.PortionId.HasValue)
                {
                    var portion = food.Portions.Single(p => p.Id == ingredient.PortionId);
                    ingredient.Weight = ingredient.Quantity * portion.Weight;
                }
                else
                {
                    ingredient.Weight = ingredient.Quantity;
                }
                foreach (var foodNutrient in food.Nutrients)
                {
                    var recipeNutrient = recipeNutrients.SingleOrDefault(n => n.NutrientId == foodNutrient.NutrientId);
                    if(recipeNutrient == null)
                    {
                        recipeNutrient = new NutrientAmount
                        {
                            NutrientId = foodNutrient.NutrientId
                        };
                        recipeNutrients.Add(recipeNutrient);
                    }
                    recipeNutrient.Amount += (ingredient.Weight * foodNutrient.Amount) / 100m;
                    
                }
                recipeWeight += ingredient.Weight;
            }
            if (recipe.CookedWeight.HasValue)
            {
                recipeWeight = recipe.CookedWeight.Value;
            }
            recipeNutrients.ForEach(na => {
                na.Amount = na.Amount * 100 / recipeWeight;
            });
            recipe.Nutrients = recipeNutrients.ToArray();
        }
        private void CalculatePortionWeights(FoodDetails recipe)
        {
            var recipeWeight = recipe.Ingredients.Sum(i => i.Weight);
            if (recipe.CookedWeight.HasValue)
            {
                recipeWeight = recipe.CookedWeight.Value;
            }
            foreach (var portion in recipe.Portions)
            {
                if (portion.Amount.HasValue)
                {
                    portion.Weight = recipeWeight / portion.Amount.Value;
                }
            }
        }
    }
}