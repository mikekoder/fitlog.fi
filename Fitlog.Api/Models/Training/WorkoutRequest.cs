using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Api.Models.Training
{
    public class WorkoutRequest
    {
        public DateTimeOffset Time { get; set; }
        public int? Hours { get; set; }
        public int? Minutes { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public WorkoutSetRequest[] Sets { get; set; }
        public decimal? EnergyExpenditure { get; set; }
    }
    public class WorkoutSetRequest
    {
        public Guid? ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        public decimal Reps { get; set; }
        public decimal Weights { get; set; }
    }
}
