using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Web.Models.Nutrition
{
    public class MealRequest
    {
        public DateTimeOffset Time { get; set; }
        public MealRow[] Rows { get; set; }
    }
}
