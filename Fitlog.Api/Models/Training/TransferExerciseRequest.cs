using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Api.Models.Training
{
    public class TransferExerciseRequest
    {
        public Guid FromExerciseId { get; set; }
        public Guid ToExerciseId { get; set; }
        public bool TransferWorkouts { get; set; }
        public bool TransferRoutines { get; set; }
        public bool Transfer1RM { get; set; }
    }
}
