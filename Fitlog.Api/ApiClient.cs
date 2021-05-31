using Fitlog.Api.Models.Nutrition;
using Fitlog.Api.Models.Profile;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Fitlog.Api
{
    public class ApiClient : IApiClient
    {
        public static readonly string Web = "web";
        public static readonly string Mobile = "mobile";

        private readonly ITokenProvider tokenProvider;
        private readonly string baseUrl;
        private readonly string refreshToken;
        private string accessToken;
        private readonly DateTimeOffset accessTokenExpires;

        public ApiClient(string baseUrl, ITokenProvider tokenProvider)
        {
            this.baseUrl = baseUrl;
            this.tokenProvider = tokenProvider;

            var token = tokenProvider.GetToken();
            if (token != null)
            {
                this.refreshToken = token.RefreshToken;
                this.accessToken = token.AccessToken;
                this.accessTokenExpires = token.Expires;
            }
        }
        public ApiResult<ProfileResponse> GetProfile()
        {
            var url = $"{baseUrl}/api/users/me/";
            return Get<ProfileResponse>(url, null);
        }
        public ApiResult<MealDetailsResponse[]> GetMeals(DateTimeOffset? start, DateTimeOffset? end)
        {
            var url = $"{baseUrl}/api/meals/";
            var query = new Dictionary<string, object> { { "start", start }, { "end", end } };
            return Get<MealDetailsResponse[]>(url, query);
        }
        private void CheckAccessToken()
        {
            if(DateTimeOffset.UtcNow > accessTokenExpires)
            {
                using var client = new HttpClient();
                var url = baseUrl + "/api/users/refress-token";
                var query = new Dictionary<string, object> { { "refreshToken", refreshToken } };
                var response = client.GetAsync(url + GetQueryString(query)).Result;
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(response.Content.ReadAsStringAsync().Result);

                accessToken = tokenResponse.AccessToken;

                tokenProvider.UpdateToken(new Token
                {
                    RefreshToken = tokenResponse.RefreshToken,
                    AccessToken = tokenResponse.AccessToken,
                    Expires = tokenResponse.Expires
                });
            }
        }
        private ApiResult<T> Get<T>(string url, Dictionary<string,object> query)
        {
            CheckAccessToken();
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = client.GetAsync(url + GetQueryString(query)).Result;
                return new ApiResult<T>
                {
                    Success = response.IsSuccessStatusCode,
                    Status = response.StatusCode,
                    Message = response.ReasonPhrase,
                    Data = JsonSerializer.Deserialize<T>(response.Content.ReadAsStringAsync().Result)
                };
            }
            catch
            {
                return new ApiResult<T>
                {
                    Success = false
                };
            }
        }

        private static string GetQueryString(Dictionary<string, object> values)
        {
            if(values == null ||values.Count == 0)
            {
                return "";
            }
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
            return "?" + query[1..];
        }
    }
}
