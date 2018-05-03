using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Web
{
    public static class Constants
    {
        public static class Nutrition
        {
            public static readonly int EnergyDistributionId = 36;
            public static readonly int EnergyKcalId = 34;
            public static readonly int EnergyKjId = 51;
            public static readonly int ProteinId = 8;
            public static readonly int CarbId = 77;
            public static readonly int FatId = 52;
            public static readonly int ProteinEnergyId = 63;
            public static readonly int CarbEnergyId = 79;
            public static readonly int FatEnergyId = 6;
            public static readonly int FatSaturatedId = 80;
            public static readonly int SugarId = 44;
            public static readonly int FiberId = 69;
            public static readonly int[] ComputedNutrientIds = new[]
            {
                EnergyDistributionId,
                ProteinEnergyId,
                CarbEnergyId,
                FatEnergyId
            };
        }
        public static class Training
        {
            public static readonly decimal WorkoutEnergyExpenditure = 0.06m;
            
        }
        public static class Measurements
        {
            public static readonly Guid HeightId = Guid.Parse("A3E87CD7-D5DC-4F2F-A614-96CDE9575E80");
            public static readonly Guid WeightId = Guid.Parse("A86E5B0C-AB63-4CFF-B192-EE68BB99AF84");
            public static readonly Guid RmrId = Guid.Parse("08DB7A7C-1BB9-4657-861F-DDDF034D7210");
        }
        public static class Activities
        {
            public static readonly decimal SleepFactor = 0.9m;
            public static readonly decimal InactivityFactor = 1.3m;
            public static readonly decimal LightActivityFactor = 1.8m;
            public static readonly decimal ModerateActivityFactor = 2.4m;
            public static readonly decimal HeavyActivityFactor = 4m;
        }
    }
}
