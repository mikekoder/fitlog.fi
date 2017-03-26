using Crash.Fit.Nutrition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Web.Models.Nutrition
{
    public class MealResponse
    {
        public Guid Id { get; set; }
        public DateTimeOffset Time { get; set; }
        public string Name { get; set; }
        public MealRow[] Rows { get; set; }
        public Dictionary<Guid,decimal> Nutrients { get; set; }
    }
}
