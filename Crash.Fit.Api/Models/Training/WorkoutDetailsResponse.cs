using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Training
{
    public class WorkoutDetailsResponse : WorkoutResponse
    {
        public WorkoutSetResponse[] Sets { get; set; }
    }
    public class WorkoutSetResponse
    {
        public Guid ExerciseId { get; set; }
        public int Reps { get; set; }
        public decimal Weights { get; set; }
    }
}
