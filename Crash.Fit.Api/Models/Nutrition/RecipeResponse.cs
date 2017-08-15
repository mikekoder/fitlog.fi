using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Nutrition
{
    public class RecipeSummaryResponse : RecipeResponse
    {
        public int UsageCount { get; set; }
    }
}
