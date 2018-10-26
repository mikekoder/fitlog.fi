using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Crash.Fit.External.KRuoka
{
    public class KRuokaClient : IStoreClient
    {
        private readonly string baseUrl = "https://www.k-ruoka.fi/kr-api";
        private readonly string storeId = "N106";
        public async Task<ExternalFood> FindFood(string ean)
        {
            var products = await FindProducts(ean, storeId);
            var product = products.FirstOrDefault(p => p.Ean == ean);
            if(product == null)
            {
                return null;
            }

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{baseUrl}/products/{product.UrlSlug}?storeId={storeId}");
                var json = await response.Content.ReadAsStringAsync();
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new JsonPathConverter());
                var details = JsonConvert.DeserializeObject<ProductDetailsResponse>(json, settings);
                if(details == null)
                {
                    return null;
                }
                return new ExternalFood
                {
                    Brand = details.Brand,
                    Carbohydrate = details.Carbohydrate,
                    Ean = details.Ean,
                    Fat = details.Fat,
                    FatSaturated = details.FatSaturated,
                    Fiber = details.Fiber,
                    Kcal = details.Kcal,
                    Kj = details.Kj,
                    Name = details.Name,
                    Protein = details.Protein,
                    Sugar = details.Sugar
                };
            }
        }
        private async Task<IEnumerable<ProductResponse>> FindProducts(string text, string storeId)
        {
            var products = new List<ProductResponse>();
            using (var client = new HttpClient())
            {
                while (true)
                {
                    var response = await client.PostAsync($"{baseUrl}/product-search/{text}?storeId={storeId}&offset={products.Count}", new StringContent(""));
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ProductSearchResult>(json);
                    products.AddRange(result.Result);
                    if (products.Count >= result.TotalHits)
                    {
                        break;
                    }
                }
            }

            foreach (var product in products)
            {
                try
                {
                    decimal.Parse(product.VisiblePrice.Value, CultureInfo.InvariantCulture);
                    decimal.Parse(product.ReferencePrice.Value, CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    ;
                }
            }
            return products;
        }
        private class ProductSearchResult
        {
            public ProductResponse[] Result { get; set; }
            public int TotalHits { get; set; }
        }
    }
    
}
