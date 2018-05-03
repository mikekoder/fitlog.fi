using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Api.Models.Nutrition
{
    public class FoodExternalDetailsResponse
    {
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Ean { get; set; }
        public FoodNutrientAmountResponse[] Nutrients { get; set; }
    }
}
