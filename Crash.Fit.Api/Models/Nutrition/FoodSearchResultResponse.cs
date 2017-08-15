using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Nutrition
{
    public class FoodSearchResultResponse : FoodResponse
    {
        public int UsageCount { get; set; }
    }
}
