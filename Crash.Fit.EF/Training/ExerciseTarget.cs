using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Training
{
    public partial class ExerciseTarget
    {
        public Guid ExerciseId { get; set; }
        public Guid MuscleGroupId { get; set; }

        public Exercise Exercise { get; set; }
        public MuscleGroup MuscleGroup { get; set; }
    }
}
