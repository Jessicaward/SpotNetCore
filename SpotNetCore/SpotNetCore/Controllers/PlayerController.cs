using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using SpotNetCore.Models;
using AuthenticationManager = SpotNetCore.Implementation.AuthenticationManager;

namespace SpotNetCore.Controllers
{
    public class PlayerController
    {
        public async Task<bool> PlayCurrentTrack()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(AuthenticationManager.Token.TokenType,
                        AuthenticationManager.Token.AccessToken);

                return (await httpClient.PutAsync("https://api.spotify.com/v1/me/player/play", null)).StatusCode == HttpStatusCode.NoContent;
            }
        }

        public async Task<bool> PauseCurrentTrack()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(AuthenticationManager.Token.TokenType,
                        AuthenticationManager.Token.AccessToken);
                return (await httpClient.PutAsync("https://api.spotify.com/v1/me/player/pause", null)).StatusCode == HttpStatusCode.NoContent;
            }
        }

        public async Task<SpotifyCurrentlyPlaying> NextTrack()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(AuthenticationManager.Token.TokenType,
                        AuthenticationManager.Token.AccessToken);

                //Skip Track
                if ((await httpClient.PostAsync("https://api.spotify.com/v1/me/player/next", null)).StatusCode !=
                    HttpStatusCode.NoContent)
                {
                    throw new Exception("Could not skip track");
                }

                return await GetCurrentlyPlaying();
            }
        }

        public async Task<SpotifyCurrentlyPlaying> GetCurrentlyPlaying()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(AuthenticationManager.Token.TokenType,
                        AuthenticationManager.Token.AccessToken);
                
                var response = await httpClient.GetAsync("https://api.spotify.com/v1/me/player/currently-playing");

                response.EnsureSuccessStatusCode();

                return JsonSerializer.Deserialize<SpotifyCurrentlyPlaying>(await response.Content.ReadAsStringAsync());
            }
        }
    }
}