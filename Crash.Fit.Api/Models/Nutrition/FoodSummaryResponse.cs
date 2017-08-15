using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Nutrition
{
    public class FoodSummaryResponse : FoodResponse
    {
        public int UsageCount { get; set; }
        public int NutrientCount { get; set; }
    }
}
