using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Fitlog.External.Foodie
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
            using var client = new WebClient();
            client.Headers.Add("Accept", "application/json");
            var json = client.DownloadString($"{baseUrl}/entry/entry_data/{ean}?product_data_language=fi");
            var food = JsonSerializer.Deserialize<Entry>(json);
            if (food == null)
            {
                return null;
            }
            return new ExternalFood
            {
                Brand = food.BrandName,
                Carbohydrate = food.Carbohydrate,
                Ean = food.Ean.ToString(),
                Fat = food.Fat,
                FatSaturated = food.FatSaturated,
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
