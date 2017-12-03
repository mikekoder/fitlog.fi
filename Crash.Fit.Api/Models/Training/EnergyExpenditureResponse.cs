using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Api.Models.Training
{
    public class EnergyExpenditureResponse
    {
        public Guid Id { get; set; }
        public DateTimeOffset Time { get; set; }
        public string ActivityName { get; set; }
        public decimal EnergyKcal { get; set; }
        public Guid? ActivityId { get; set; }
        public int? Hours { get; set; }
        public int? Minutes { get; set; }
        public Guid? WorkoutId { get; set; }
    }
}
