using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Training
{
    public partial class OneRepMax
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ExerciseId { get; set; }
        public DateTimeOffset Time { get; set; }
        public decimal Max { get; set; }
        public decimal? BodyWeightMax { get; set; }

        public Exercise Exercise { get; set; }
        public Profile User { get; set; }
    }
}
