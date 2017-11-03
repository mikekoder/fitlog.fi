using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Training
{
    public partial class Workout
    {
        public Workout()
        {
            Sets = new HashSet<WorkoutSet>();
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset Time { get; set; }
        public DateTimeOffset? Deleted { get; set; }

        public Profile User { get; set; }
        public ICollection<WorkoutSet> Sets { get; set; }
    }
}
