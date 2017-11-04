using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Measurements
{
    public class Measurement : Entity
    {
        public Guid UserId { get; set; }
        public Guid MeasureId { get; set; }
        public DateTimeOffset Time { get; set; }
        public decimal Value { get; set; }
    }
}
