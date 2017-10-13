using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Api.Models.Training
{
    public class TrainingGoalResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public TrainingGoalExerciseResponse[] Exercises { get; set; }
    }
    public class TrainingGoalExerciseResponse
    {
        public Guid ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public decimal Frequency { get; set; }
    }
}
