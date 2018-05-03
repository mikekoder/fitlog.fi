using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.External.Foodin
{
    public class Entry
    {
        [JsonProperty("ean")]
        public string Ean { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("subname")]
        public string Subname { get; set; }
        [JsonProperty("brand_name")]
        public string BrandName { get; set; }
        [JsonProperty("kj")]
        public decimal? Kj { get; set; }
        [JsonProperty("kcal")]
        public decimal? Kcal { get; set; }
        [JsonProperty("protein")]
        public decimal? Protein { get; set; }
        [JsonProperty("carbohydrate")]
        public decimal? Carbohydrate { get; set; }
        [JsonProperty("sugar")]
        public decimal? Sugar { get; set; }
        [JsonProperty("fat")]
        public decimal? Fat { get; set; }
        [JsonProperty("fat_saturated")]
        public decimal? FatSaturated { get; set; }
        [JsonProperty("fiber")]
        public decimal? Fiber { get; set; }
    }
}
