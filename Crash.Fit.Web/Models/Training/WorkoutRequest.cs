﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Web.Models.Training
{
    public class WorkoutRequest
    {
        public DateTimeOffset Time { get; set; }
        public string Name { get; set; }
        public WorkoutSetRequest[] Sets { get; set; }
    }
    
}
