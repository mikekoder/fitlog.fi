using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Training
{
    public class OneRepMax
    {
        public Guid UserId { get; set; }
        public Guid ExerciseId { get; set; }
        public DateTimeOffset Time { get; set; }
        /// <summary>
        /// 1RM estimate (weights) calculated from weights only
        /// </summary>
        public decimal Max { get; set; }
        /// <summary>
        /// 1RM estimate (weights) calculated from weights + lifted body weight
        /// </summary>
        public decimal? MaxBW { get; set; }
        /// <summary>
        /// 1RM estimate (weights + lifted body weight) calculated from weights + lifted body weight
        /// </summary>
        public decimal? MaxInclBW { get; set; }
    }
}
