using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Api.Models.Nutrition
{
    public class NutrientSettingResponse
    {
        public int NutrientId { get; set; }
        public int Order { get; set; }
        public bool? HideSummary { get; set; }
        public bool? HideDetails { get; set; }
        public int? HomeOrder { get; set; }
    }
}
