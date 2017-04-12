using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Web.Models.Nutrition
{
    public class FoodDetailsResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public NutrientAmountResponse[] Nutrients { get; set; }
        public Portion[] Portions { get; set; }
    }
}
