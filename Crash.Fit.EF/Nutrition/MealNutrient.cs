using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Nutrition
{
    public partial class MealNutrient
    {
        public Guid MealId { get; set; }
        public Guid NutrientId { get; set; }
        public decimal Amount { get; set; }

        public Meal Meal { get; set; }
        public Nutrient Nutrient { get; set; }
    }
}
