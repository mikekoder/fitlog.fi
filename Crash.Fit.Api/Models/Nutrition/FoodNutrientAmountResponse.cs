using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Nutrition
{
    public class FoodNutrientAmountResponse
    {
        public Guid NutrientId { get; set; }
        public decimal Amount { get; set; }
        public decimal? PortionAmount { get; set; }
    }
}
