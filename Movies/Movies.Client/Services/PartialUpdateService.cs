using Microsoft.AspNetCore.JsonPatch;
using Movies.Client.Interfaces;
using Movies.Client.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Movies.Client.Services
{
    public class PartialUpdateService : IIntegrationService
    {
        private static HttpClient _httpClient = new HttpClient();

        public PartialUpdateService()
        {
            _httpClient.BaseAddress = new Uri("http://localhost:57863/");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
        }
        public async Task Run()
        {
            await PatchResource();
        }

        public async Task PatchResource()
        {
            var patchDoc = new JsonPatchDocument<MovieForUpdate>();

            patchDoc.Replace(m => m.Title, "Update title");
            patchDoc.Remove(m => m.Description);

            var serilizedChangeSet = JsonConvert.SerializeObject(patchDoc);

            var request = new HttpRequestMessage(HttpMethod.Patch, "api/movies/5B1C2B4D-48C7-402A-80C3-CC796AD49C6B");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(serilizedChangeSet);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json-patch+json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var updatedMovie = JsonConvert.DeserializeObject<MovieForUpdate>(content);
        }

        private async Task PatchResourceShortCut()
        {
            var patchDoc = new JsonPatchDocument<MovieForUpdate>();

            patchDoc.Replace(m => m.Title, "Update title");
            patchDoc.Remove(m => m.Description);

            var response = await _httpClient.PatchAsync("api/movies/5B1C2B4D-48C7-402A-80C3-CC796AD49C6B",
                new StringContent(
                    JsonConvert.SerializeObject(patchDoc),
                    Encoding.UTF8,
                    "application/json-patch+json"));

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var updatedMovie = JsonConvert.DeserializeObject<MovieForUpdate>(content);

        }
    }
}
