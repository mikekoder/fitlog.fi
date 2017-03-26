using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Nutrition
{
    public class Nutrient
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Unit { get; set; }

        public string FineliId { get; set; }
        public string FineliClass { get; set; }
        public string FineliGroup { get; set; }

        public int? UIOrder { get; set; }
        public bool UIVisible { get; set; }
        public int Precision { get; set; }
    }
}
