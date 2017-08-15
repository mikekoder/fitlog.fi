using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Training
{
    public class WorkoutRequest
    {
        public DateTimeOffset Time { get; set; }
        public string Name { get; set; }
        public WorkoutSetRequest[] Sets { get; set; }
    }
    public class WorkoutSetRequest
    {
        public Guid? ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        public int Reps { get; set; }
        public decimal Weights { get; set; }
    }
}
