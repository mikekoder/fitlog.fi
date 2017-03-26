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
    public class RecipesController : ApiControllerBase
    {
        private readonly INutritionRepository nutritionRepository;
        public RecipesController(INutritionRepository nutritionRepository)
        {
            this.nutritionRepository = nutritionRepository;
        }
        [HttpGet]
        [Route("")]
        public IEnumerable<FoodMinimal> List()
        {
            var recipes = nutritionRepository.SearchRecipes(CurrentUserId);
            return recipes;
        }
        [HttpGet]
        [Route("{id}")]
        public IActionResult Details(Guid id)
        {
            var food = nutritionRepository.GetFood(id);
            if (!food.IsRecipe)
            {
                return NotFound();
            }
            var result = AutoMapper.Mapper.Map<RecipeResponse>(food);
            return Ok(food);
        }
        [HttpPost]
        [Route("")]
        public IActionResult Create([FromBody]RecipeRequest request)
        {
            var recipe = AutoMapper.Mapper.Map<FoodDetails>(request);
            recipe.UserId = CurrentUserId;
            recipe.IsRecipe = true;
            
            CalculateNutrients(recipe);
            if(!nutritionRepository.CreateFood(recipe))
            {
                return BadRequest();
            }
            var result = AutoMapper.Mapper.Map<RecipeResponse>(recipe);
            return Ok(result);
        }

        [HttpPut]
        [Route("{id}")]
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
            if(!nutritionRepository.UpdateFood(recipe))
            {
                return BadRequest();
            }
           
            var result = AutoMapper.Mapper.Map<RecipeResponse>(recipe);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
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
            recipeNutrients.ForEach(na => {
                na.Amount = na.Amount * 100 / recipeWeight;
            });
            recipe.Nutrients = recipeNutrients.ToArray();
        }
    }
}