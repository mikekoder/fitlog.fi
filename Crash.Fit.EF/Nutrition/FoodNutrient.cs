using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Nutrition
{
    public partial class FoodNutrient
    {
        public Guid FoodId { get; set; }
        public Guid NutrientId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? PortionAmount { get; set; }

        public Food Food { get; set; }
        public Nutrient Nutrient { get; set; }
    }
}
