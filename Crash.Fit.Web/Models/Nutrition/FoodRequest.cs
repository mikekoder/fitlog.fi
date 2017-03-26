﻿using Crash.Fit.Nutrition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Web.Models.Nutrition
{
    public class FoodRequest
    {
        public string Name { get; set; }
        public NutrientAmount[] Nutrients { get; set; }
        public Portion[] Portions { get; set; }
    }
}
