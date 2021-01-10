using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using SpotNetCore.Implementation;
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

        public async Task<SpotifyPlayerContext> GetPlayerContext()
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

            return JsonSerializer.Deserialize<SpotifyPlayerContext>(await response.Content.ReadAsStringAsync());
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

        public async Task RestartTrack()
        {
            using var httpClient = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue(_authenticationManager.Token.TokenType,
                        _authenticationManager.Token.AccessToken)
                }
            };

            var response = await httpClient.PutAsync("https://api.spotify.com/v1/me/player/seek?position_ms=0", null);

            response.EnsureSuccessStatusCode();
        }

        /// <param name="requestedShuffleState">Nullable bool depending on whether user specifies state or not.</param>
        public async Task ShuffleToggle(bool? requestedShuffleState)
        {
            using var httpClient = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue(_authenticationManager.Token.TokenType,
                        _authenticationManager.Token.AccessToken)
                }
            };

            var shuffleState = requestedShuffleState == null ? !(await GetPlayerContext()).ShuffleState : requestedShuffleState.Value;

            var response = await httpClient.PutAsync($"https://api.spotify.com/v1/me/player/shuffle?state={shuffleState}", null);

            response.EnsureSuccessStatusCode();
        }
    }
}