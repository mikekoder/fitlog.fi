using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Training
{
    public class RoutineMinimal : Entity
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }     
    }
    public class RoutineDetails : RoutineMinimal
    {
        public RoutineWorkout[] Workouts { get; set; }
    }
}
