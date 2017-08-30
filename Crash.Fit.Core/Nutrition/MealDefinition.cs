using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Nutrition
{
    public class MealDefinition : Entity
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public TimeSpan? Start { get; set; }
        public TimeSpan? End { get; set; }

        public TimeSpan Time
        {
            get
            {
                return new TimeSpan(((Start ?? TimeSpan.Zero) + (End ?? TimeSpan.FromHours(24))).Ticks / 2);
            }
        }
        /*
        public int? StartHour { get; set; }
        public int? StartMinute { get; set; }
        public int? EndHour { get; set; }
        public int? EndMinute { get; set; }
        */
    }
}
