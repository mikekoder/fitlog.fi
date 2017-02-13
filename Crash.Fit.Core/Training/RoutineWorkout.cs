using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Training
{
    public class RoutineWorkout : Entity
    {
        public string Name { get; set; }
        public RoutineSet[] Sets { get; set; }
    }
}
