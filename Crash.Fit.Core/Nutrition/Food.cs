using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Nutrition
{
    public class Food : Entity
    {
        public Guid? UserId { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public bool IsRecipe { get; set; }
        public string FineliId { get; set; }
        /// <summary>
        /// For which portion nutrient amounts are given
        /// </summary>
        public Guid? NutrientPortionId { get; set; }
        public string Ean { get; set; }
    }
    public class FoodSearchResult : Food
    {
        public int UsageCount { get; set; }
        public DateTimeOffset? LatestUse { get; set; }
    }
    public class FoodSearchNutrientResult : Food
    {
        public int UsageCount { get; set; }
        public DateTimeOffset? LatestUse { get; set; }
        public decimal NutrientAmount { get; set; }
    }
    public class FoodSummary : Food
    {
        public int UsageCount { get; set; }
        public int NutrientCount { get; set; }
    }
    public class FoodDetails : Food
    {
        public RecipeIngredient[] Ingredients { get; set; }
        public Portion[] Portions { get; set; }
        public FoodNutrientAmount[] Nutrients { get; set; }
        public decimal? CookedWeight { get; set; }
        public Guid? MostUsedPortionId { get; set; }
    }
}
