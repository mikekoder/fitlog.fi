using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Training
{
    public partial class Routine
    {
        public Routine()
        {
            Workouts = new HashSet<RoutineWorkout>();
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Deleted { get; set; }
        public bool Active { get; set; }

        public Profile User { get; set; }
        public ICollection<RoutineWorkout> Workouts { get; set; }
    }
}
