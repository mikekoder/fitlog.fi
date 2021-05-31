using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Api.Models.Nutrition
{
    public class RecipeRequest
    {
        public string Name { get; set; }
        public RecipeIngredientModel[] Ingredients { get; set; }
        public PortionRequest[] Portions { get; set; }
        public decimal? CookedWeight { get; set; }
    }
}
