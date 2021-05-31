using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Api.Models.Nutrition
{
    public class FoodSearchResultResponse : FoodResponse
    {
        public int UsageCount { get; set; }
        public DateTimeOffset? LatestUse { get; set; }
    }
}
