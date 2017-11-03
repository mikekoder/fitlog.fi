using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Training
{
    public partial class TrainingGoalExercise
    {
        public Guid Id { get; set; }
        public Guid TrainingGoalId { get; set; }
        public int Index { get; set; }
        public Guid ExerciseId { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public decimal Frequency { get; set; }

        public Exercise Exercise { get; set; }
        public TrainingGoal TrainingGoal { get; set; }
    }
}
