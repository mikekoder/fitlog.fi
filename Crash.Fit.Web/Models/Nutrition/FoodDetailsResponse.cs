using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Web.Models.Nutrition
{
    public class FoodDetailsResponse : FoodResponse
    {
        public NutrientAmountResponse[] Nutrients { get; set; }
        public Portion[] Portions { get; set; }
    }
}
