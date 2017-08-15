using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Training
{
    public class ExerciseDetailsResponse : ExerciseResponse
    {
        public Guid[] Targets { get; set; }
    }
}
