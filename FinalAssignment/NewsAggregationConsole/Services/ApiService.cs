using System.Net.Http.Json;

namespace NewsAggregationConsole.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(string baseAddress)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(baseAddress) };
        }

        public void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<T> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            return await HandleResponse<T>(response);
        }

        public async Task<T> PostAsync<T>(string url, object data)
        {
            var response = await _httpClient.PostAsJsonAsync(url, data);
            return await HandleResponse<T>(response);
        }

        public async Task<T> PutAsync<T>(string url, object data)
        {
            var response = await _httpClient.PutAsJsonAsync(url, data);
            return await HandleResponse<T>(response);
        }

        public async Task DeleteAsync(string url)
        {
            var response = await _httpClient.DeleteAsync(url);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"API request failed: {response.StatusCode}");
        }

        private static async Task<T> HandleResponse<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                //throw new HttpRequestException($"API request failed: {response.StatusCode} : {response}");
                Console.WriteLine($"API request failed: {response.StatusCode} : {response}");
            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}
