using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Api.Models.Training
{
    public class TrainingGoalRequest
    {
        public string Name { get; set; }
        public TrainingGoalExerciseRequest[] Exercises { get; set; }
    }
    public class TrainingGoalExerciseRequest
    {
        public Guid? ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public decimal Frequency { get; set; }
    }
}
