using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Api.Models.Nutrition
{
    public class FoodSearchNutrientResultResponse : FoodSearchResultResponse
    {
        public decimal NutrientAmount { get; set; }
    }
}
