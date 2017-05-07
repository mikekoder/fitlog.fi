﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Training
{
    public class Exercise : Entity
    {
        public Guid? UserId { get; set; }
        public string Name { get; set; }
    }
    public class ExerciseSummary : Exercise
    {
        public int UsageCount { get; set; }
    }
    public class ExerciseDetails : Exercise
    {
        public Guid[] Targets { get; set; }
    }
}
