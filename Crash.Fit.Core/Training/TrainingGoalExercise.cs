using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Training
{
    public class TrainingGoalExercise
    {
        public Guid Id { get; set; }
        public Guid ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public decimal Frequency { get; set; }
    }
}
