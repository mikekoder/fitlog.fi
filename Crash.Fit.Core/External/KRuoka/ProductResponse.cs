using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Crash.Fit.External.KRuoka
{
    internal class ProductResponse
    {
        [JsonProperty("ean")]
        public string Ean { get; set; }
        [JsonProperty("urlSlug")]
        public string UrlSlug { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("contentUnit")]
        public string ContentUnit { get; set; }
        [JsonProperty("contentSize")]
        public decimal ContentSize { get; set; }
        [JsonProperty("visiblePrice")]
        public Price VisiblePrice { get; set; }
        [JsonProperty("referencePrice")]
        public Price ReferencePrice { get; set; }
    }
    internal class Price
    {
        public string Value { get; set; }
    }
}
