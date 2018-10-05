using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Training
{
    public class ExerciseSummaryResponse : ExerciseResponse
    {
        public int UsageCount { get; set; }
        public decimal? OneRepMax { get; set; }
        public DateTimeOffset? LatestUse { get; set; }
    }
}
