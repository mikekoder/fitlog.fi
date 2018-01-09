using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Api.Models.Activities
{
    public class EnergyExpenditureRequest
    {
        public DateTimeOffset Time { get; set; }
        public string ActivityName { get; set; }
        public decimal? EnergyKcal { get; set; }
        public Guid? ActivityId { get; set; }
        public int? Hours { get; set; }
        public int? Minutes { get; set; }
    }
}
