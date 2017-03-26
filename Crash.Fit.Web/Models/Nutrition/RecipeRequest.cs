using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Web.Models.Nutrition
{
    public class RecipeRequest
    {
        public string Name { get; set; }
        public RecipeIngredient[] Ingredients { get; set; }
        public Portion[] Portions { get; set; }
    }
}
