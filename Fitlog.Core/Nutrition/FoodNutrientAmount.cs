using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Nutrition
{
    public class FoodNutrientAmount : NutrientAmount
    {
        // Amount in a portion
        public decimal? PortionAmount { get; set; }
    }
}
