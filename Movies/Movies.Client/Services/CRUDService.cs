using Movies.Client.Interfaces;
using Movies.Client.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Serialization;

namespace Movies.Client.Services
{
    public class CRUDService : IIntegrationService
    {
        private static HttpClient _httpClient = new HttpClient();
        public CRUDService()
        {
            _httpClient.BaseAddress = new Uri("http://localhost:57863/");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 0.9));
            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        }
        public async Task Run()
        {
            //await GetResource();
            //await GetResourceThroughHttpRequestMessage();
            //await CreateResource();
            //await UpdateResourse();
            await DeleteResource();
        }

        public async Task GetResource()
        {
            var response = await _httpClient.GetAsync("api/movies");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var movies = new List<Movie>();
            if (response.Content.Headers.ContentType.MediaType == "application/json")
            {
                movies = JsonConvert.DeserializeObject<List<Movie>>(content);
            }
            else if(response.Content.Headers.ContentType.MediaType == "application/xml")
            {
                var serializer = new XmlSerializer(typeof(List<Movie>));
                movies = (List<Movie>)serializer.Deserialize(new StringReader(content));
            }

        }

        public async Task GetResourceThroughHttpRequestMessage()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/movies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var movies = JsonConvert.DeserializeObject<List<Movie>>(content);
        }

        public async Task CreateResource()
        {
            var movieToCreate = new MovieForCreation()
            {
                Title = "Reservoir Dogs",
                Description = "After a simple jewelry heist goes terribly wrong, the " +
                 "surviving criminals begin to suspect that one of them is a police informant.",
                DirectorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                ReleaseDate = new DateTimeOffset(new DateTime(1992, 9, 2)),
                Genre = "Crime, Drama"
            };

            var serializedMovieToCreate = JsonConvert.SerializeObject(movieToCreate);

            var request = new HttpRequestMessage(HttpMethod.Post, "api/movies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = new StringContent(serializedMovieToCreate);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var createdMovie = JsonConvert.DeserializeObject<Movie>(content);
        }
       
        private async Task PostResourceShortCut()
        {
            var movieToCreate = new MovieForCreation()
            {
                Title = "Reservoir Dogs",
                Description = "After a simple jewelry heist goes terribly wrong, the " +
                 "surviving criminals begin to suspect that one of them is a police informant.",
                DirectorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                ReleaseDate = new DateTimeOffset(new DateTime(1992, 9, 2)),
                Genre = "Crime, Drama"
            };

            var response = await _httpClient.PostAsync(
                "api/movies",
                new StringContent(
                    JsonConvert.SerializeObject(movieToCreate),
                    Encoding.UTF8,
                    "application/json"));

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var createdMovie = JsonConvert.DeserializeObject<Movie>(content);
        }

        public async Task UpdateResourse()
        {
            var movieToUpdate = new MovieForUpdate()
            {
                Title = "Pulp Fiction",
                Description = "The movie with Zed.",
                DirectorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                ReleaseDate = new DateTimeOffset(new DateTime(1992, 9, 2)),
                Genre = "Crime, Drama"
            };

            var serializeMoviedToUpdate = JsonConvert.SerializeObject(movieToUpdate);

            var request = new HttpRequestMessage(HttpMethod.Put, "api/movies/5B1C2B4D-48C7-402A-80C3-CC796AD49C6B");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(serializeMoviedToUpdate);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var updatedMovie = JsonConvert.DeserializeObject<Movie>(content);
        }

        private async Task UpdateResourceShortCut()
        {
            var movieToUpdate = new MovieForUpdate()
            {
                Title = "Pulp Fiction",
                Description = "The movie with Zed.",
                DirectorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                ReleaseDate = new DateTimeOffset(new DateTime(1992, 9, 2)),
                Genre = "Crime, Drama"
            };

            var response = await _httpClient.PutAsync("api/movies/5B1C2B4D-48C7-402A-80C3-CC796AD49C6B",
                new StringContent(
                    JsonConvert.SerializeObject(movieToUpdate),
                    Encoding.UTF8,
                    "application/json"));

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var updatedMovie = JsonConvert.DeserializeObject<Movie>(content);
        }
        private async Task DeleteResource()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, "api/movies/5B1C2B4D-48C7-402A-80C3-CC796AD49C6B");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
        }

        private async Task DeleteResourceShortCut()
        {
            var response = await _httpClient.DeleteAsync("api/movies/5B1C2B4D-48C7-402A-80C3-CC796AD49C6B");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
        }
    }
}
