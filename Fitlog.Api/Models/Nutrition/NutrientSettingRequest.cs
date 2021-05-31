using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Api.Models.Nutrition
{
    public class NutrientSettingRequest
    {
        public int NutrientId { get; set; }
        public int? Order { get; set; }
        public bool? UserHideSummary { get; set; }
        public bool? UserHideDetails { get; set; }
    }
}
