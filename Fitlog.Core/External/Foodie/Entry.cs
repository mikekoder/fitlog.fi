using System.Text.Json.Serialization;

namespace Fitlog.External.Foodie
{
    public class Entry
    {
        [JsonPropertyName("ean")]
        public long Ean { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("subname")]
        public string Subname { get; set; }
        [JsonPropertyName("brand_name")]
        public string BrandName { get; set; }
        [JsonPropertyName("kj")]
        public decimal? Kj { get; set; }
        [JsonPropertyName("kcal")]
        public decimal? Kcal { get; set; }
        [JsonPropertyName("protein")]
        public decimal? Protein { get; set; }
        [JsonPropertyName("carbohydrate")]
        public decimal? Carbohydrate { get; set; }
        [JsonPropertyName("sugar")]
        public decimal? Sugar { get; set; }
        [JsonPropertyName("fat")]
        public decimal? Fat { get; set; }
        [JsonPropertyName("fat_saturated")]
        public decimal? FatSaturated { get; set; }
        [JsonPropertyName("fiber")]
        public decimal? Fiber { get; set; }
    }
}
