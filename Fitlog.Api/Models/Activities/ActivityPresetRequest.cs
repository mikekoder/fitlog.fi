using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Api.Models.Activities
{
    public class ActivityPresetRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal? Sleep { get; set; }
        public decimal? Inactivity { get; set; }
        public decimal? LightActivity { get; set; }
        public decimal? ModerateActivity { get; set; }
        public decimal? HeavyActivity { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
    }
}
