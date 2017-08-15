using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Nutrition
{
    public class MealRequest
    {
        public DateTimeOffset Time { get; set; }
        public MealRowModel[] Rows { get; set; }
    }
}
