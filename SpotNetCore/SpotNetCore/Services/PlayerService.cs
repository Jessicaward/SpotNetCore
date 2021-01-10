using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using SpotNetCore.Models;
using AuthenticationManager = SpotNetCore.Implementation.AuthenticationManager;

namespace SpotNetCore.Services
{
    public class PlayerService
    {
        private readonly AuthenticationManager _authenticationManager;

        public PlayerService(AuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }
        
        public async Task PlayCurrentTrack()
        {
            using var httpClient = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue(_authenticationManager.Token.TokenType,
                        _authenticationManager.Token.AccessToken)
                }
            };

            var response = await httpClient.PutAsync("https://api.spotify.com/v1/me/player/play", null);
            
            response.EnsureSuccessStatusCode();
        }

        public async Task PauseCurrentTrack()
        {
            using var httpClient = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue(_authenticationManager.Token.TokenType,
                        _authenticationManager.Token.AccessToken)
                }
            };
                
            var response = await httpClient.PutAsync("https://api.spotify.com/v1/me/player/pause", null);

            response.EnsureSuccessStatusCode();
        }

        public async Task NextTrack()
        {
            using var httpClient = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue(_authenticationManager.Token.TokenType,
                        _authenticationManager.Token.AccessToken)
                }
            };
            
            var response = await httpClient.PostAsync("https://api.spotify.com/v1/me/player/next", null);
                
            response.EnsureSuccessStatusCode();
        }

        public async Task<SpotifyCurrentlyPlaying> GetCurrentlyPlaying()
        {
            using var httpClient = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue(_authenticationManager.Token.TokenType,
                        _authenticationManager.Token.AccessToken)
                }
            };
                
            var response = await httpClient.GetAsync("https://api.spotify.com/v1/me/player");

            response.EnsureSuccessStatusCode();

            return JsonSerializer.Deserialize<SpotifyCurrentlyPlaying>(await response.Content.ReadAsStringAsync());
        }

        public async Task PreviousTrack()
        {
            using var httpClient = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue(_authenticationManager.Token.TokenType,
                        _authenticationManager.Token.AccessToken)
                }
            };
            
            var response = await httpClient.PostAsync("https://api.spotify.com/v1/me/player/previous", null);

            response.EnsureSuccessStatusCode();
        }
    }
}