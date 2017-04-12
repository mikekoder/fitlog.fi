using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Web.Models.Nutrition
{
    public class NutrientAmountResponse
    {
        public Guid NutrientId { get; set; }
        public decimal Amount { get; set; }
    }
}
