using System.Text.Json.Serialization;

namespace Fitlog.External.KRuoka
{
    public class ProductDetailsResponse
    {
        [JsonPropertyName("ean")]
        public string Ean { get; set; }
        [JsonPropertyName("brand")]
        public string Brand { get; set; }
        [JsonPropertyName("localizedName.finnish")]
        public string Name { get; set; }
        [JsonPropertyName("nutritionalDetails.nutritionalContent.energyKj")]
        public decimal? Kj { get; set; }
        [JsonPropertyName("nutritionalDetails.nutritionalContent.energyKcal")]
        public decimal? Kcal { get; set; }
        [JsonPropertyName("nutritionalDetails.nutritionalContent.protein.amount")]
        public decimal? Protein { get; set; }
        [JsonPropertyName("nutritionalDetails.nutritionalContent.carbohydrates.amount")]
        public decimal? Carbohydrate { get; set; }
        [JsonPropertyName("nutritionalDetails.nutritionalContent.carbohydratesSugar.amount")]
        public decimal? Sugar { get; set; }
        [JsonPropertyName("nutritionalDetails.nutritionalContent.fat.amount")]
        public decimal? Fat { get; set; }
        [JsonPropertyName("nutritionalDetails.nutritionalContent.fatSaturated.amount")]
        public decimal? FatSaturated { get; set; }
        [JsonPropertyName("nutritionalDetails.nutritionalContent.nutritionalFiber.amount")]
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
