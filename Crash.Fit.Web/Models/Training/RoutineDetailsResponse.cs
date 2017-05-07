using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Web.Models.Training
{
    public class RoutineDetailsResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public RoutineWorkoutResponse[] Workouts { get; set; }
    }
    public class RoutineWorkoutResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public RoutineExerciseResponse[] Exercises { get; set; }
    }
    public class RoutineExerciseResponse
    {
        public Guid ExerciseId { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
    }
}
