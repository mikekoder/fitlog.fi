using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Nutrition
{
    public class NutrientSettingRequest
    {
        public Guid NutrientId { get; set; }
        public int? Order { get; set; }
        public bool? UserHideSummary { get; set; }
        public bool? UserHideDetails { get; set; }
    }
}
