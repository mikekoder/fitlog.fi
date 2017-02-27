using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Web.Models.Nutrition
{
    public class MealRow
    {
        public Guid FoodId { get; set; }
        public decimal Quantity { get; set; }
        public Guid? PortionId { get; set; }
    }
}
