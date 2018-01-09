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
            public static readonly Guid EnergyDistributionId = Guid.Parse("bdc0b05d-38ac-48e5-a048-67cb5ec1ae66");
            public static readonly Guid EnergyKcalId = Guid.Parse("14cbc692-34c3-4f02-ae56-6599c132a794");
            public static readonly Guid ProteinId = Guid.Parse("1ddca711-0dcc-4708-93c2-1ac3b0b3d281");
            public static readonly Guid CarbId = Guid.Parse("fa5f03f8-6aeb-4d5f-9100-f41cd606d36b");
            public static readonly Guid FatId = Guid.Parse("9fa87e51-46af-4ca9-8c7d-98176cfa8b78");
            public static readonly Guid ProteinEnergyId = Guid.Parse("c04b0066-7c2d-4b6c-8f3c-b248fec34a9a");
            public static readonly Guid CarbEnergyId = Guid.Parse("0d0d71d6-e770-4e16-a53f-fdca0bcfcca6");
            public static readonly Guid FatEnergyId = Guid.Parse("6e2a3101-5d3a-44ae-a622-14bd43dafa30");
            public static readonly Guid[] ComputedNutrientIds = new[]
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
