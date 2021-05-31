using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Api.Models.Training
{
    public class ExerciseRequest
    {
        public string Name { get; set; }
        public decimal? PercentageBW { get; set; }
        public Guid[] Targets { get; set; }
        public Guid[] SecondaryTargets { get; set; }
        public Guid[] Equipments { get; set; }
    }
}
