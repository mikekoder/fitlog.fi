using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.External.Foodie
{
    public class FoodieClient : IStoreClient
    {
        private readonly string baseUrl;
        public FoodieClient() : this("https://www.foodie.fi")
        {

        }
        public FoodieClient(string baseUrl)
        {
            this.baseUrl = baseUrl.TrimEnd('/');
        }
        public async Task<ExternalFood> FindFood(string ean)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Accept", "application/json");
                var json = client.DownloadString($"{baseUrl}/entry/entry_data/{ean}?product_data_language=fi");
                var food = JsonConvert.DeserializeObject<Entry>(json);
                if(food == null)
                {
                    return null;
                }
                return new ExternalFood
                {
                    Brand = food.BrandName,
                    Carbohydrate = food.Carbohydrate,
                    Ean = food.Ean,
                    Fat = food.Fat,
                    FatSaturated=  food.FatSaturated,
                    Fiber = food.Fiber,
                    Kcal = food.Kcal,
                    Kj = food.Kj,
                    Manufacturer = food.Subname,
                    Name = food.Name,
                    Protein = food.Protein,
                    Sugar = food.Sugar
                };
            }
        }
    }
}
