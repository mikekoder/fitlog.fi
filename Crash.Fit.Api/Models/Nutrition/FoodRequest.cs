using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Nutrition
{
    public class FoodRequest
    {
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public NutrientAmountModel[] Nutrients { get; set; }
        public PortionRequest[] Portions { get; set; }
    }
}
