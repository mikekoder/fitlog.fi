using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Api.Models.Nutrition
{
    public class RecipeSummaryResponse : RecipeResponse
    {
        public int UsageCount { get; set; }
    }
}
