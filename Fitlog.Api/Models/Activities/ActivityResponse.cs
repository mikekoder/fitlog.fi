using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Api.Models.Activities
{
    public class ActivityResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal EnergyExpenditure { get; set; }
    }
}
