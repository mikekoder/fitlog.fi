using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.External
{
    public class ExternalFood
    {
        public string Ean { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Manufacturer { get; set; }
        public decimal? Kj { get; set; }
        public decimal? Kcal { get; set; }
        public decimal? Protein { get; set; }
        public decimal? Carbohydrate { get; set; }
        public decimal? Sugar { get; set; }
        public decimal? Fat { get; set; }
        public decimal? FatSaturated { get; set; }
        public decimal? Fiber { get; set; }
    }
}
