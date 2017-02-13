using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Nutrition
{
    public class MealRow
    {
        public int Index { get; set; }
        public Guid FoodId { get; set; }
        public Guid? PortionId { get; set; }
        public decimal? PortionAmount { get; set; }
        public decimal Weight { get; set; }
    }
}
