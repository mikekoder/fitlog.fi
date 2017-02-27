using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Nutrition
{
    public class MealNutrient : NutrientAmount
    {
        public Guid MealId { get; set; }
    }
}
