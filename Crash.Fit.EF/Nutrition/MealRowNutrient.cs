using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Nutrition
{
    public partial class MealRowNutrient
    {
        public Guid MealRowId { get; set; }
        public Guid NutrientId { get; set; }
        public decimal? Amount { get; set; }

        public MealRow MealRow { get; set; }
        public Nutrient Nutrient { get; set; }
    }
}
