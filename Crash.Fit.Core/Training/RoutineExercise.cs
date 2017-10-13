using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Training
{
    public class RoutineExercise
    {
        //public int Index { get; set; }
        public Guid ExerciseId { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public decimal? LoadFrom { get; set; }
        public decimal? LoadTo { get; set; }
    }
}
