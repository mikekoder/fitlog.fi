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
    internal class StoreResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("chain")]
        public string Chain { get; set; }
        [JsonProperty("shortName")]
        public string ShortName { get; set; }
        [JsonProperty("shortestName")]
        public string ShortestName { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
        [JsonProperty("geo")]
        public Geo Geo { get; set; }
    }
    internal class Geo
    {
        [JsonProperty("latitude")]
        public decimal Latitude { get; set; }
        [JsonProperty("longitude")]
        public decimal Longitude { get; set; }
    }
}
