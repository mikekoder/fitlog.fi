using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Nutrition
{
    public class FoodMinimal : Entity
    {
        public Guid? UserId { get; set; }
        public string Name { get; set; }
        public bool IsRecipe { get; set; }
        public string FineliId { get; set; }
    }
    public class FoodDetails : FoodMinimal
    {
        public RecipeIngredient[] Ingredients { get; set; }
        public Portion[] Portions { get; set; }
        public NutrientAmount[] Nutrients { get; set; }
    }
}
