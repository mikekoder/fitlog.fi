using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Api.Models.Activities
{
    public class ActivityPresetDayResponse
    {
        public DateTimeOffset Date { get; set; }
        public Guid ActivityPresetId { get; set; }
    }
}
