using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Training
{
    public class RoutineWorkout
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal? Frequency { get; set; }
        public RoutineExercise[] Exercises { get; set; }
    }
}
