using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Training
{
    public class WorkoutSummaryResponse : WorkoutResponse
    {
        public Dictionary<Guid, int> MuscleGroupSets { get; set; }
    }
}
