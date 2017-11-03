using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Nutrition
{
    public partial class NutritionGoalValue
    {
        public Guid NutritionGoalPeriodId { get; set; }
        public Guid NutrientId { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }

        public Nutrient Nutrient { get; set; }
        public NutritionGoalPeriod NutritionGoalPeriod { get; set; }
    }
}
