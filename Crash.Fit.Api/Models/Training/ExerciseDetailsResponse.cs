using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Training
{
    public class ExerciseDetailsResponse : ExerciseSummaryResponse
    {
        public Guid[] Targets { get; set; }
        public ExerciseImageResponse[] Images { get; set; }
    }
}
