using Movie.Client.Interfaces;

namespace Movie.Client.Services
{
    public class CRUDService : IIntegrationService
    {
        private static HttpClient _httpClient = new HttpClient();
        public CRUDService()
        {
            _httpClient.BaseAddress = new Uri("http://localhost:57863/");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);

        }
        public async Task Run()
        {
            await GetResource();
        }

        public async Task GetResource()
        {
            var response = await _httpClient.GetAsync("api/movies");
            response.EnsureSuccessStatusCode();
         }
    }
}
