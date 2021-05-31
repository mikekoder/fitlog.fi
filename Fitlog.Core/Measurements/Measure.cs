using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Measurements
{
    public class Measure : Entity
    {
        public Guid? UserId { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
    }
    public class MeasureSummary : Measure
    {
        public DateTimeOffset? LatestTime { get; set; }
        public decimal? LatestValue { get; set; }
    }
}
