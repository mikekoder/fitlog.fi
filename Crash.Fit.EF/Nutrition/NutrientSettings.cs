using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Nutrition
{
    public partial class NutrientSettings
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid NutrientId { get; set; }
        public bool? HideSummary { get; set; }
        public bool? HideDetails { get; set; }
        public int? Order { get; set; }
        public int? HomeOrder { get; set; }

        public Nutrient Nutrient { get; set; }
        public Profile User { get; set; }
    }
}
