using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Training
{
    public class Routine : Entity
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }     
    }
    public class RoutineSummary : Routine
    {
        public int WorkoutCount { get; set; }
    }
    public class RoutineDetails : Routine
    {
        public RoutineWorkout[] Workouts { get; set; }
    }
}
