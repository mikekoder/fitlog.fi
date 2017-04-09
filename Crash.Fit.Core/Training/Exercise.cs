using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Training
{
    public class ExerciseMinimal : Entity
    {
        public Guid? UserId { get; set; }
        public string Name { get; set; }
    }
    public class ExerciseDetails : ExerciseMinimal
    {
        public Guid[] Targets { get; set; }
    }
}
