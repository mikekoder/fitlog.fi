using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Nutrition
{
    public class NutrientTarget
    {
        public Guid UserId { get; set; }
        public Guid NutrientId { get; set; }
        public Days Days { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
    }
}
