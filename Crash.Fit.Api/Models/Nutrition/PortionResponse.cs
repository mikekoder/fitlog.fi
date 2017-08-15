using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Nutrition
{
    public class PortionResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        //public decimal? Amount { get; set; }
        public decimal Weight { get; set; }
    }
}
