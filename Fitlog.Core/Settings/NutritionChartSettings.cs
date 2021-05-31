using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Settings
{
    [SettingsKey("NutritionChart")]
    public class NutritionChartSettings
    {
        public int?[] Nutrients { get; set; }
    }
}
