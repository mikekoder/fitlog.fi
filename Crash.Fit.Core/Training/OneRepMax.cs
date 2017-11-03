using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Training
{
    public class OneRepMax
    {
        public Guid UserId { get; set; }
        public Guid ExerciseId { get; set; }
        public DateTimeOffset Time { get; set; }
        public decimal Max { get; set; }
        public decimal? MaxBW { get; set; }
    }
}
