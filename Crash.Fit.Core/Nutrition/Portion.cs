using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Nutrition
{
    public class Portion
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// How many portions in a recipe
        /// </summary>
        public decimal? Amount { get; set; }
        public decimal Weight { get; set; }
    }
}
