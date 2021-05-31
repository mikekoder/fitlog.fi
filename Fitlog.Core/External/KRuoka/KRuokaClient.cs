using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Fitlog.External.KRuoka
{
    public class KRuokaClient : IStoreClient
    {
        private readonly string baseUrl = "https://www.k-ruoka.fi/kr-api/v2";
        private readonly string storeId = "N106";
        public async Task<ExternalFood> FindFood(string ean)
        {
            try
            {
                var products = await FindProducts(ean, storeId);
                var product = products.FirstOrDefault(p => p.Ean == ean);
                if (product == null)
                {
                    return null;
                }

                using var client = new HttpClient();
                var response = await client.GetAsync($"{baseUrl}/products/{product.UrlSlug}?storeId={storeId}");
                var json = await response.Content.ReadAsStringAsync();

                var details = JsonSerializer.Deserialize<ProductDetailsResponse>(json);
                if (details == null)
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
            catch
            {
                return null;
            }
        }
        private async Task<IEnumerable<ProductResponse>> FindProducts(string text, string storeId)
        {
            var products = new List<ProductResponse>();
            using (var client = new HttpClient())
            {
                while (true)
                {
                    var response = await client.PostAsync($"{baseUrl}/product-search/{text}?storeId={storeId}&offset=0&limit=30&clientUpdatedPSD2=1", new StringContent(""));
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ProductSearchResult>(json);
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
                catch
                {
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
