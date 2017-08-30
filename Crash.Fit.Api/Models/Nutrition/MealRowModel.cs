using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Nutrition
{
    public class MealRowModel
    {
        public Guid Id { get; set; }
        public Guid MealId { get; set; }
        public int Index { get; set; }
        public Guid FoodId { get; set; }
        public string FoodName { get; set; }
        public decimal Quantity { get; set; }
        public Guid? PortionId { get; set; }
        public string PortionName { get; set; }
        public decimal Weight { get; set; }
        public Dictionary<Guid, decimal> Nutrients { get; set; }
    }
}
