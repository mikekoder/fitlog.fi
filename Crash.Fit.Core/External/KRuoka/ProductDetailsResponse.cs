using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.External.KRuoka
{
    public class ProductDetailsResponse
    {
        [JsonProperty("ean")]
        public string Ean { get; set; }
        [JsonProperty("brand")]
        public string Brand { get; set; }
        [JsonProperty("name.finnish")]
        public string Name { get; set; }
        [JsonProperty("nutritionalDetails.nutritionalContent.energyKJ.amount")]
        public decimal? Kj { get; set; }
        [JsonProperty("nutritionalDetails.nutritionalContent.energyKC.amount")]
        public decimal? Kcal { get; set; }
        [JsonProperty("nutritionalDetails.nutritionalContent.protein.amount")]
        public decimal? Protein { get; set; }
        [JsonProperty("nutritionalDetails.nutritionalContent.carbohydrates.amount")]
        public decimal? Carbohydrate { get; set; }
        [JsonProperty("nutritionalDetails.nutritionalContent.carbohydratesSugar.amount")]
        public decimal? Sugar { get; set; }
        [JsonProperty("nutritionalDetails.nutritionalContent.fat.amount")]
        public decimal? Fat { get; set; }
        [JsonProperty("nutritionalDetails.nutritionalContent.fatSaturated.amount")]
        public decimal? FatSaturated { get; set; }
        [JsonProperty("nutritionalDetails.nutritionalContent.nutritionalFiber.amount")]
        public decimal? Fiber { get; set; }

        public class Nutrients
        {
            public decimal EnergyKj { get; set; }
            public decimal EnergyKcal { get; set; }
            public decimal Fat { get; set; }
            public decimal FatSaturated { get; set; }
        }
    }
}
