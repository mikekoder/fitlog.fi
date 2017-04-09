using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Web.Models.Training
{
    public class WorkoutSummaryResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset Time { get; set; }
        public string Name { get; set; }
        public Dictionary<Guid, int> MuscleGroupSets { get; set; }
    }
}
