using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Api.Models.Nutrition
{
    public class FoodDetailsResponse : FoodResponse
    {
        public FoodNutrientAmountResponse[] Nutrients { get; set; }
        public PortionResponse[] Portions { get; set; }
        public Guid? MostUsedPortionId { get; set; }
    }
}
