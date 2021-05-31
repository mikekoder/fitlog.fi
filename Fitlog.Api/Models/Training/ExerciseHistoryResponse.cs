using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Api.Models.Training
{
    public class ExerciseHistoryResponse
    {
        public Guid ExerciseId { get; set; }
        public DateTimeOffset Time { get; set; }
        public decimal? Max { get; set; }
        public decimal? MaxBW { get; set; }
        public decimal? MaxInclBW { get; set; }
        public decimal? TotalVolume { get; set; }
    }
}
