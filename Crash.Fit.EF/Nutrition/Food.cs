using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Nutrition
{
    public partial class Food
    {
        public Food()
        {
            Nutrients = new HashSet<FoodNutrient>();
            Portions = new HashSet<FoodPortion>();
            MealRow = new HashSet<MealRow>();
            RecipeIngredients = new HashSet<RecipeIngredient>();
            RecipeIngredientRecipe = new HashSet<RecipeIngredient>();
        }

        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string Name { get; set; }
        public bool IsRecipe { get; set; }
        public string FineliId { get; set; }
        public DateTimeOffset? Created { get; set; }
        public DateTimeOffset? Deleted { get; set; }
        public decimal? CookedWeight { get; set; }
        public Guid? NutrientPortionId { get; set; }

        public Profile User { get; set; }
        public ICollection<FoodNutrient> Nutrients { get; set; }
        public ICollection<FoodPortion> Portions { get; set; }
        public ICollection<MealRow> MealRow { get; set; }
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; }
        public ICollection<RecipeIngredient> RecipeIngredientRecipe { get; set; }
    }
}
