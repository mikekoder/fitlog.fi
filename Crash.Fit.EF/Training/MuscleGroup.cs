using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Training
{
    public partial class MuscleGroup
    {
        public MuscleGroup()
        {
            Exercises = new HashSet<ExerciseTarget>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public ICollection<ExerciseTarget> Exercises { get; set; }
    }
}
