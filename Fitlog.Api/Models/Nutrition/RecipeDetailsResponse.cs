using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Api.Models.Nutrition
{
    public class RecipeDetailsResponse : RecipeResponse
    {
        public bool IsRecipe { get; set; }
        public RecipeIngredientModel[] Ingredients { get; set; }
        public PortionResponse[] Portions { get; set; }
        public NutrientAmountModel[] Nutrients { get; set; }
    }
}
