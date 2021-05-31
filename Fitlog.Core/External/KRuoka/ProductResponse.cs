using System.Text.Json.Serialization;

namespace Fitlog.External.KRuoka
{
    internal class ProductResponse
    {
        [JsonPropertyName("ean")]
        public string Ean { get; set; }
        [JsonPropertyName("urlSlug")]
        public string UrlSlug { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("contentUnit")]
        public string ContentUnit { get; set; }
        [JsonPropertyName("contentSize")]
        public decimal ContentSize { get; set; }
        [JsonPropertyName("visiblePrice")]
        public Price VisiblePrice { get; set; }
        [JsonPropertyName("referencePrice")]
        public Price ReferencePrice { get; set; }
    }
    internal class Price
    {
        public string Value { get; set; }
    }
}
