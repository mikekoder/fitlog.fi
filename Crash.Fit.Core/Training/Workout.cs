using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Training
{
    public class Workout : Entity
    {
        public Guid UserId { get; set; }
        public DateTimeOffset Time { get; set; }
    }
    public class WorkoutSummary : Workout
    {
        public Dictionary<Guid, int> MuscleGroupSets { get; set; }
    }
    public class WorkoutDetails : WorkoutSummary
    {
        public WorkoutSet[] Sets { get; set; }
    }
}
