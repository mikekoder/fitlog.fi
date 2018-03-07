using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Api.Models.Activities
{
    public class ActivityPresetDayRequest
    {
        public DateTimeOffset Date { get; set; }
        public Guid ActivityPresetId { get; set; }
    }
}
