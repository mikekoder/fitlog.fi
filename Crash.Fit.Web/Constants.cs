using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Web
{
    public static class Constants
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
}
