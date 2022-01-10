using System.Net.Http;
using System.Threading.Tasks;
using SpotNetCore.Models;

namespace SpotNetCore.Endpoints
{
    public class PlayerEndpoint
    {
        private readonly HttpClient _httpClient;

        internal PlayerEndpoint(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> Play()
        {
            var response = await _httpClient.PutAsync("/v1/me/player/play", null);
            return response;
        }

        public async Task<HttpResponseMessage> Pause()
        {
            var response = await _httpClient.PutAsync("/v1/me/player/pause", null);
            return response;
        }

        public async Task<HttpResponseMessage> Next()
        {
            var response = await _httpClient.PostAsync("/v1/me/player/next", null);
            return response;
        }

        public async Task<HttpResponseMessage> Previous()
        {
            var response = await _httpClient.PostAsync("/v1/me/player/previous", null);
            return response;
        }

        public async Task<HttpResponseMessage> Seek(int milliseconds)
        {
            var response = await _httpClient.PutAsync($"/v1/me/player/seek?position_ms={milliseconds}", null);
            return response;
        }

        public async Task<HttpResponseMessage> Shuffle(bool state)
        {
            var response = await _httpClient.PutAsync($"/v1/me/player/shuffle?state={state}", null);
            return response;
        }

        public async Task<HttpResponseMessage> Queue(string trackUri)
        {
            var response = await _httpClient.PostAsync($"/v1/me/player/queue?uri={trackUri}", null);
            return response;
        }

        public async Task<SpotifyPlayerContext> Player()
        {
            return await _httpClient.GetFromSpotifyJsonAsync<SpotifyPlayerContext>("/v1/me/player");
        }
    }
}