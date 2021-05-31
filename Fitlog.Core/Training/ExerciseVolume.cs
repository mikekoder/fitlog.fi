using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Training
{
    public class ExerciseVolume
    {
        public Guid UserId { get; set; }
        public Guid ExerciseId { get; set; }
        public DateTimeOffset Time { get; set; }
        public decimal TotalVolume { get; set; }
        //public decimal? WorkVolume { get; set; }
    }
}
