using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Nutrition
{
    public partial class NutritionGoal
    {
        public NutritionGoal()
        {
            Periods = new HashSet<NutritionGoalPeriod>();
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Deleted { get; set; }

        public Profile User { get; set; }
        public ICollection<NutritionGoalPeriod> Periods { get; set; }
    }
}
