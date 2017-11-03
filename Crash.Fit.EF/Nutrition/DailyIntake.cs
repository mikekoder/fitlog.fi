using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Nutrition
{
    public partial class DailyIntake
    {
        public Guid Id { get; set; }
        public Guid NutrientId { get; set; }
        public int Gender { get; set; }
        public decimal StartAge { get; set; }
        public decimal? EndAge { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }

        public Nutrient Nutrient { get; set; }
    }
}
