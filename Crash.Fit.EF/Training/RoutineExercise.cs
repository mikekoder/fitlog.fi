using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Training
{
    public partial class RoutineExercise
    {
        public Guid Id { get; set; }
        public Guid RoutineWorkoutId { get; set; }
        public int Index { get; set; }
        public Guid ExerciseId { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public decimal? LoadFrom { get; set; }
        public decimal? LoadTo { get; set; }

        public Exercise Exercise { get; set; }
        public RoutineWorkout RoutineWorkout { get; set; }
    }
}
