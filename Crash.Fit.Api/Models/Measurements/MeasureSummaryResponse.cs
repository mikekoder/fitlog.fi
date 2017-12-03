using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Measurements
{
    public class MeasureSummaryResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public DateTimeOffset? LatestTime { get; set; }
        public decimal? LatestValue { get; set; }
    }
}
