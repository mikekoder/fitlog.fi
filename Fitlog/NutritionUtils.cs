using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Web
{
    public static class NutritionUtils
    {
        public static void AppendComputedNutrients(Dictionary<int,decimal> nutrients)
        {
            var energy = nutrients.ContainsKey(Constants.Nutrition.EnergyKcalId) ? nutrients[Constants.Nutrition.EnergyKcalId] : null as decimal?;
            var protein = nutrients.ContainsKey(Constants.Nutrition.ProteinId) ? nutrients[Constants.Nutrition.ProteinId] : null as decimal?;
            var carbs = nutrients.ContainsKey(Constants.Nutrition.CarbId) ? nutrients[Constants.Nutrition.CarbId] : null as decimal?;
            var fat = nutrients.ContainsKey(Constants.Nutrition.FatId) ? nutrients[Constants.Nutrition.FatId] : null as decimal?;
            var calculatedEnergy = 4 * protein + 4 * carbs + 9 * fat;

            if (energy.HasValue || calculatedEnergy.HasValue)
            {
                if ((calculatedEnergy ?? energy).Value == 0)
                {
                    return;
                }

                if (protein.HasValue)
                {
                    nutrients.Add(Constants.Nutrition.ProteinEnergyId, (4 * protein.Value) / (calculatedEnergy ?? energy).Value * 100);
                }
                if (carbs.HasValue)
                {
                    nutrients.Add(Constants.Nutrition.CarbEnergyId, (4 * carbs.Value) / (calculatedEnergy ?? energy).Value * 100);
                }
                if (fat.HasValue)
                {
                    nutrients.Add(Constants.Nutrition.FatEnergyId, (9 * fat.Value) / (calculatedEnergy ?? energy).Value * 100);
                }
            }
        }
    }
}
