using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Training
{
    public class EnergyExpenditure : Entity
    {
        public Guid UserId { get; set; }
        public DateTimeOffset Time { get; set; }
        public string ActivityName { get; set; }
        public Guid? ActivityId { get; set; }
        public TimeSpan? Duration { get; set; }
        public decimal EnergyKcal { get; set; }
        public Guid? WorkoutId { get; set; }
    }
}
