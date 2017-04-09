using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Web.Models.Training
{
    public class WorkoutResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset Time { get; set; }
        public string Name { get; set; }
        public WorkoutSetResponse[] Sets { get; set; }
    }
    public class WorkoutSetResponse
    {
        public Guid ExerciseId { get; set; }
        public int Reps { get; set; }
        public decimal Weights { get; set; }
    }
}
