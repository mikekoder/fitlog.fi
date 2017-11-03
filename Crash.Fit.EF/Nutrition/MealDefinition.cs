using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Nutrition
{
    public partial class MealDefinition
    {
        public MealDefinition()
        {
            Meal = new HashSet<Meal>();
            NutritionGoalMeal = new HashSet<NutritionGoalMeal>();
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public TimeSpan? Start { get; set; }
        public TimeSpan? End { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Deleted { get; set; }

        public Profile User { get; set; }
        public ICollection<Meal> Meal { get; set; }
        public ICollection<NutritionGoalMeal> NutritionGoalMeal { get; set; }
    }
}
