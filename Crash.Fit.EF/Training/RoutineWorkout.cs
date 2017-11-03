using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Training
{
    public partial class RoutineWorkout
    {
        public RoutineWorkout()
        {
            Exercises = new HashSet<RoutineExercise>();
        }

        public Guid Id { get; set; }
        public Guid RoutineId { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public decimal? Frequency { get; set; }

        public Routine Routine { get; set; }
        public ICollection<RoutineExercise> Exercises { get; set; }
    }
}
