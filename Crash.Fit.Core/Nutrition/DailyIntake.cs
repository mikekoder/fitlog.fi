using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Nutrition
{
    public class DailyIntake
    {
        public Gender Gender { get; set; }
        public TimeSpan StartAge { get; set; }
        public DateTime EndAge { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
    }
}
