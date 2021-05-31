using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Nutrition
{
    public class NutritionGoalValue
    {
        public int NutrientId { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
    }
}
