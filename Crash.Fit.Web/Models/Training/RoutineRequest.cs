using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Web.Models.Training
{
    public class RoutineRequest
    {
        public string Name { get; set; }
        public RoutineWorkoutRequest[] Workouts { get; set; }
    }
    public class RoutineWorkoutRequest
    {
        public string Name { get; set; }
        public RoutineExerciseRequest[] Exercises { get; set; }
    }
    public class RoutineExerciseRequest
    {
        public Guid? ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
    }
}
