﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Web.Models.Measurements
{
    public class MeasureSummaryResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset? LatestTime { get; set; }
        public decimal? LatestValue { get; set; }
    }
}
