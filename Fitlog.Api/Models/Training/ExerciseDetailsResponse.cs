using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Api.Models.Training
{
    public class ExerciseDetailsResponse : ExerciseSummaryResponse
    {
        public Guid[] Targets { get; set; }
        public Guid[] SecondaryTargets { get; set; }
        public ExerciseImageResponse[] Images { get; set; }
        public Guid[] Equipments { get; set; }
    }
}
