﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Web.Models.Training
{
    public class ExerciseSummaryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int UsageCount { get; set; }
    }
}