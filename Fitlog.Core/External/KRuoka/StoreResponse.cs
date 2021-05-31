using System.Text.Json.Serialization;

namespace Fitlog.External.KRuoka
{
    internal class StoreResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("chain")]
        public string Chain { get; set; }
        [JsonPropertyName("shortName")]
        public string ShortName { get; set; }
        [JsonPropertyName("shortestName")]
        public string ShortestName { get; set; }
        [JsonPropertyName("location")]
        public string Location { get; set; }
        [JsonPropertyName("geo")]
        public Geo Geo { get; set; }
    }
    internal class Geo
    {
        [JsonPropertyName("latitude")]
        public decimal Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public decimal Longitude { get; set; }
    }
}
