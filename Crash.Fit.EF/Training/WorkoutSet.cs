using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Training
{
    public partial class WorkoutSet
    {
        public Guid Id { get; set; }
        public Guid WorkoutId { get; set; }
        public int Index { get; set; }
        public Guid ExerciseId { get; set; }
        public int Reps { get; set; }
        public decimal? Weights { get; set; }
        public decimal? Load { get; set; }
        public decimal? BodyWeightLoad { get; set; }

        public Exercise Exercise { get; set; }
        public Workout Workout { get; set; }
    }
}
