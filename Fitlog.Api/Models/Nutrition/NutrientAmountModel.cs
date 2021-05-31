using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Api.Models.Nutrition
{
    public class NutrientAmountModel
    {
        public int NutrientId { get; set; }
        public decimal Amount { get; set; }
    }
}
