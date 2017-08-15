using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Measurements
{
    public class MeasurementRequest
    {
        public DateTimeOffset Time { get; set; }
        public Measurement[] Measurements { get; set; }

        public class Measurement
        { 
            public Guid? MeasurementId { get; set; }
            public Guid? MeasureId { get; set; }
            public string MeasureName { get; set; }
            public decimal Value { get; set; }
        }
    }
}
