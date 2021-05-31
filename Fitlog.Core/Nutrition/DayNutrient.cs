using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Nutrition
{
    public class DayNutrient
    {
        public Guid UserId { get; set; }
        public DateTimeOffset Date { get; set; }
        public Dictionary<int,decimal> Nutrients { get; set; }
    }
}
