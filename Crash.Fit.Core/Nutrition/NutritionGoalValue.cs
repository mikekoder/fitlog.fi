﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Nutrition
{
    public class NutritionGoalValue
    {
        public Guid NutrientId { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
    }
}
