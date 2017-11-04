﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Training
{
    public class ExerciseRequest
    {
        public string Name { get; set; }
        public decimal? PercentageBW { get; set; }
        public Guid[] Targets { get; set; }
    }
}
