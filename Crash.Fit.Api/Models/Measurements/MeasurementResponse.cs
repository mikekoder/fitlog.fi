using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Api.Models.Measurements
{
    public class MeasurementResponse
    {
        public Guid MeasureId { get; set; }
        public DateTimeOffset Time { get; set; }
        public decimal Value { get; set; }
    }
}
