﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Nutrition
{
    public class NutrientSetting
    {
        public Guid UserId { get; set; }
        public int NutrientId { get; set; }
        public int Order { get; set; }
        public bool? HideSummary { get; set; }
        public bool? HideDetails { get; set; }
        public int? HomeOrder { get; set; }
    }
}
