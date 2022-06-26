using Movies.Client.Interfaces;
using Movies.Client.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace Movies.Client.Services
{
    internal class StreamService : IIntegrationService
    {
        private static HttpClient _httpClient = new HttpClient();

        public StreamService()
        {
            _httpClient.BaseAddress = new Uri("http://localhost:57863/");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();

        }
        public async Task Run()
        {
            //await GetPosterWithStream();
            //await GetPosterWithStreamAndCompletionMode();
            //await TestGetPosterWithoutStream(); 
            //await TestGetPosterWithStream();
            //await TestGetPosterWithStreamAndCompletionMode();
            await PostPosterWithStream();
        }

        private async Task GetPosterWithStream()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/movies/F9A16FEE-4C49-41BB-87A1-BBAAD0CD1174/posters/{Guid.NewGuid()}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                var posters = stream.ReadAndDeserializeFromJson<Poster>();
            }
        }

        private async Task GetPosterWithStreamAndCompletionMode()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/movies/F9A16FEE-4C49-41BB-87A1-BBAAD0CD1174/posters/{Guid.NewGuid()}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                var posters = stream.ReadAndDeserializeFromJson<Poster>();
            }
        }

        private async Task GetPosterWithoutStream()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/movies/F9A16FEE-4C49-41BB-87A1-BBAAD0CD1174/posters/{Guid.NewGuid()}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var posters = JsonConvert.DeserializeObject<Poster>(content);
        }

        private async Task PostPosterWithStream()
        {
            var random = new Random();
            var generatedBytes = new byte[524288];
            random.NextBytes(generatedBytes);

            var posterForCreation = new PosterForCreation()
            {
                Name = "A new poster for the big lebowski",
                Bytes = generatedBytes
            };

            var memoryContentSream = new MemoryStream();
            memoryContentSream.SerializeToJsonAndWrite(posterForCreation);

            memoryContentSream.Seek(0, SeekOrigin.Begin);
            using(var request = new HttpRequestMessage(HttpMethod.Post, $"api/movies/D8663E5E-7494-4F81-8739-6E0DE1BEA7EE/posters"))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using(var streamContent = new StreamContent(memoryContentSream))
                {
                    request.Content = streamContent;
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await _httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var createdContent = await response.Content.ReadAsStringAsync();
                    var createPoster = JsonConvert.DeserializeObject(createdContent);
                }
            }

        }

        private async Task PostAndReadPosterWithStream()
        {
            var random = new Random();
            var generatedBytes = new byte[524288];
            random.NextBytes(generatedBytes);

            var posterForCreation = new PosterForCreation()
            {
                Name = "A new poster for the big lebowski",
                Bytes = generatedBytes
            };

            var memoryContentSream = new MemoryStream();
            memoryContentSream.SerializeToJsonAndWrite(posterForCreation);

            memoryContentSream.Seek(0, SeekOrigin.Begin);
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"api/movies/D8663E5E-7494-4F81-8739-6E0DE1BEA7EE/posters"))
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var streamContent = new StreamContent(memoryContentSream))
                {
                    request.Content = streamContent;
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();

                        var stream = await response.Content.ReadAsStreamAsync();
                        var poster = stream.ReadAndDeserializeFromJson<Poster>();
                    }
                }
            }

        }

        private async Task TestGetPosterWithoutStream()
        {
            // warmup
            await GetPosterWithoutStream();

            var stopWatch = Stopwatch.StartNew();

            for (int i = 0; i < 200; i++)
            {
                await GetPosterWithoutStream();
            }

            stopWatch.Stop();
            Console.WriteLine($"Elapsed milliseconds without stream: " +
                $"{stopWatch.ElapsedMilliseconds}, " +
                $"averaging {stopWatch.ElapsedMilliseconds / 200} millisecons/request");
        }

        private async Task TestGetPosterWithStream()
        {
            // warmup
            await GetPosterWithoutStream();

            var stopWatch = Stopwatch.StartNew();

            for (int i = 0; i < 200; i++)
            {
                await GetPosterWithoutStream();
            }

            stopWatch.Stop();
            Console.WriteLine($"Elapsed milliseconds with stream: " +
                $"{stopWatch.ElapsedMilliseconds}, " +
                $"averaging {stopWatch.ElapsedMilliseconds / 200} millisecons/request");
        }

        private async Task TestGetPosterWithStreamAndCompletionMode()
        {
            // warmup
            await GetPosterWithStreamAndCompletionMode();

            var stopWatch = Stopwatch.StartNew();

            for (int i = 0; i < 200; i++)
            {
                await GetPosterWithStreamAndCompletionMode();
            }

            stopWatch.Stop();
            Console.WriteLine($"Elapsed milliseconds with stream and completionmode: " +
                $"{stopWatch.ElapsedMilliseconds}, " +
                $"averaging {stopWatch.ElapsedMilliseconds / 200} millisecons/request");
        }
    }
}
