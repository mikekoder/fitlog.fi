using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Nutrition
{
    public class DayNutrient
    {
        public Guid UserId { get; set; }
        public DateTimeOffset Date { get; set; }
        public Dictionary<int,decimal> Nutrients { get; set; }
    }
}
