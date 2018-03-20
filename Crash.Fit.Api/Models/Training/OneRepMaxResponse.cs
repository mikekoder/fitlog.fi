using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Api.Models.Training
{
    public class OneRepMaxResponse
    {
        public Guid ExerciseId { get; set; }
        public DateTimeOffset Time { get; set; }
        public decimal Max { get; set; }
        public decimal? MaxBW { get; set; }
        public decimal? MaxInclBW { get; set; }
    }
}
