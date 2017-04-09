using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Web.Models.Training
{
    public class WorkoutSetRequest
    {
        public Guid? ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        public int Reps { get; set; }
        public decimal Weights { get; set; }
    }
}
