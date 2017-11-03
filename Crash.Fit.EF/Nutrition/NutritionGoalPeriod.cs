using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Nutrition
{
    public partial class NutritionGoalPeriod
    {
        public NutritionGoalPeriod()
        {
            Meals = new HashSet<NutritionGoalMeal>();
            Values = new HashSet<NutritionGoalValue>();
        }

        public Guid Id { get; set; }
        public Guid NutritionGoalId { get; set; }
        public int Index { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        public bool ExerciseDay { get; set; }
        public bool RestDay { get; set; }
        public bool WholeDay { get; set; }

        public NutritionGoal NutritionGoal { get; set; }
        public ICollection<NutritionGoalMeal> Meals { get; set; }
        public ICollection<NutritionGoalValue> Values { get; set; }
    }
}
