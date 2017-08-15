using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Nutrition
{
    public class DailyIntake
    {
        public string Gender { get; set; }
        public decimal? StartAge { get; set; }
        public decimal? EndAge { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
    }
}
