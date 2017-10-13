using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Training
{
    public class WorkoutDetailsResponse : WorkoutSummaryResponse
    {
        public WorkoutSetResponse[] Sets { get; set; }
    }
    public class WorkoutSetResponse
    {
        public Guid Id { get; set; }
        public Guid ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        public int Reps { get; set; }
        public decimal Weights { get; set; }
        public decimal? OneRepMax { get; set; }
    }
}
