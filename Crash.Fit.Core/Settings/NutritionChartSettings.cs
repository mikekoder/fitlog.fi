using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Settings
{
    [SettingsKey("NutritionChart")]
    public class NutritionChartSettings
    {
        public int?[] Nutrients { get; set; }
    }
}
