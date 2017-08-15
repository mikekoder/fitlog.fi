using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Nutrition
{
    public class RecipeIngredientModel
    {
        public Guid FoodId { get; set; }
        public decimal Quantity { get; set; }
        public Guid? PortionId { get; set; }
    }
}
