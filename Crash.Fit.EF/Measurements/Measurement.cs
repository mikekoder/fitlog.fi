using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Measurements
{
    public partial class Measurement
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid MeasureId { get; set; }
        public DateTimeOffset Time { get; set; }
        public decimal Value { get; set; }

        public Measure Measure { get; set; }
        public Profile User { get; set; }
    }
}
