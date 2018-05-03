using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Crash.Fit.External.Foodin
{
    public class FoodinClient
    {
        private readonly string baseUrl;
        public FoodinClient(string baseUrl)
        {
            this.baseUrl = baseUrl.TrimEnd('/');
        }
        public Entry GetEntry(string ean)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept", "application/json");
                var json = client.DownloadString($"{baseUrl}/entry/entry_data/{ean}?product_data_language=fi");
                return JsonConvert.DeserializeObject<Entry>(json);
            }
        }
    }
}
