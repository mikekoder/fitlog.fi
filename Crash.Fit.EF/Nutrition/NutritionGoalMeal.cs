using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Nutrition
{
    public partial class NutritionGoalMeal
    {
        public Guid NutritionGoalPeriodId { get; set; }
        public Guid MealDefinitionId { get; set; }

        public MealDefinition MealDefinition { get; set; }
        public NutritionGoalPeriod NutritionGoalPeriod { get; set; }
    }
}
