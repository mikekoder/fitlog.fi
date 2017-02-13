using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Nutrition
{
    public class Nutrient : Entity
    {
        public string Name { get; set; }
        public Unit Unit { get; set; }

        public string FineliId { get; set; }
    }
}
