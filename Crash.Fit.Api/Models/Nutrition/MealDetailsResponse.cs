using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Nutrition
{
    public class MealDetailsResponse
    {
        public Guid Id { get; set; }
        public DateTimeOffset Time { get; set; }
        public MealRowModel[] Rows { get; set; }
        public Dictionary<Guid,decimal> Nutrients { get; set; }
    }
}
