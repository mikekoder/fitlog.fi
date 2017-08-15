using Crash.Fit.Api.Models.Nutrition;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Crash.Fit.MobileServices;

namespace Crash.Fit.Api
{
    public class ApiClient : IApiClient
    {
        private CookieContainer cookieContainer;
        private readonly string baseUrl;
        public ApiClient(string baseUrl, ICookieStore cookieStore)
        {
            this.baseUrl = baseUrl;
            cookieContainer = new CookieContainer();
            foreach(var cookie in cookieStore.GetCookies(baseUrl))
            {
                cookieContainer.Add(new Uri(baseUrl), cookie);
            }
        }
        public ApiResult<MealDetailsResponse[]> GetMeals(DateTimeOffset? start, DateTimeOffset? end)
        {
            var url = $"{baseUrl}/api/meals/";
            var query = new Dictionary<string, object> { { "start", start }, { "end", end } };
            return Get<MealDetailsResponse[]>(url, query);
        }
        private ApiResult<T> Get<T>(string url, Dictionary<string,object> query)
        {
            try
            {
                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                using (var client = new HttpClient(handler))
                {
                    var response = client.GetAsync(url + GetQueryString(query)).Result;
                    return new ApiResult<T>
                    {
                        Success = response.IsSuccessStatusCode,
                        Status = response.StatusCode,  
                        Message = response.ReasonPhrase,
                        Data = JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result)
                    };
                }
            }
            catch
            {
                return new ApiResult<T>
                {
                    Success = false
                };
            }
        }

        private string GetQueryString(Dictionary<string, object> values)
        {
            var query = "";
            foreach(var pair in values)
            {
                string value;
                if(pair.Value == null)
                {
                    value = "";
                }
                else if(pair.Value.GetType() == typeof(DateTimeOffset) || pair.GetType() == typeof(DateTimeOffset?))
                {
                    value = ((DateTimeOffset)pair.Value).ToString("o");
                }
                else
                {
                    value = pair.Value.ToString();
                }
                query += $"&{WebUtility.UrlEncode(pair.Key)}={WebUtility.UrlEncode(value)}";
            }
            
            if(query.Length == 0)
            {
                return "";
            }
            return "?" + query.Substring(1);
        }
    }
}
