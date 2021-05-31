using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Nutrition
{
    public class NutritionGoal : Entity
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
    }
    public class NutritionGoalDetails : NutritionGoal
    {
        public NutritionGoalPeriod[] Periods { get; set; }
    }
}
