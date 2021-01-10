using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SpotNetCore.Models;
using AuthenticationManager = SpotNetCore.Implementation.AuthenticationManager;

namespace SpotNetCore.Services
{
    public class PlayerService : IDisposable
    {
        private readonly AuthenticationManager _authenticationManager;
        private readonly HttpClient _httpClient;

        public PlayerService(AuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
            _httpClient = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue(_authenticationManager.Token.TokenType,
                        _authenticationManager.Token.AccessToken)
                }
            };
        }
        
        ~PlayerService()
        {
            Dispose(false);
        }
        
        public async Task PlayCurrentTrack()
        {
            var response = await _httpClient.PutAsync("https://api.spotify.com/v1/me/player/play", null);
            
            response.EnsureSpotifySuccess();
        }

        public async Task PauseCurrentTrack()
        {
            var response = await _httpClient.PutAsync("https://api.spotify.com/v1/me/player/pause", null);

            response.EnsureSpotifySuccess();
        }

        public async Task<SpotifyPlayerContext> NextTrack()
        {
            var lastPlayingTrack = await GetPlayerContext();

            var response = await _httpClient.PostAsync("https://api.spotify.com/v1/me/player/next", null);
            response.EnsureSpotifySuccess();

            return response.StatusCode == HttpStatusCode.Forbidden
                ? lastPlayingTrack
                : await WaitForNextTrack(lastPlayingTrack);
        }
        
        public async Task<SpotifyPlayerContext> GetPlayerContext()
        {
            var response = await _httpClient.GetAsync("https://api.spotify.com/v1/me/player");

            response.EnsureSpotifySuccess();

            return JsonSerializer.Deserialize<SpotifyPlayerContext>(await response.Content.ReadAsStringAsync());
        }

        public async Task<SpotifyPlayerContext> PreviousTrack()
        {
            var lastPlayingTrack = await GetPlayerContext();

            var response = await _httpClient.PostAsync("https://api.spotify.com/v1/me/player/previous", null);
            response.EnsureSpotifySuccess();

            return response.StatusCode == HttpStatusCode.Forbidden
                ? lastPlayingTrack
                : await WaitForNextTrack(lastPlayingTrack);
        }

        public async Task RestartTrack()
        {
            var response = await _httpClient.PutAsync("https://api.spotify.com/v1/me/player/seek?position_ms=0", null);

            response.EnsureSpotifySuccess();
        }

        private async Task<SpotifyPlayerContext> WaitForNextTrack(SpotifyPlayerContext previousTrack)
        {
            var currentlyPlayingTrack = (SpotifyPlayerContext)null;
            var retries = 0;
            while (++retries < 10 &&
                   (currentlyPlayingTrack = await GetPlayerContext()).Item.Id == previousTrack.Item.Id &&
                   currentlyPlayingTrack.ProgressInMs >= previousTrack.ProgressInMs)
            {
                Thread.Sleep(150);
            }
            return currentlyPlayingTrack;
        }

        /// <param name="requestedShuffleState">Nullable bool depending on whether user specifies state or not.</param>
        public async Task ShuffleToggle(bool? requestedShuffleState)
        {
            var shuffleState = requestedShuffleState ?? !(await GetPlayerContext()).ShuffleState;

            var response = await _httpClient.PutAsync($"https://api.spotify.com/v1/me/player/shuffle?state={shuffleState}", null);

            response.EnsureSpotifySuccess();
        }

        public async Task QueueTrack(string trackUri)
        {
            var response = await _httpClient.PostAsync($"https://api.spotify.com/v1/me/player/queue?uri={trackUri}", null);

            response.EnsureSpotifySuccess();
        }
        
        private void Dispose(bool disposing)
        {
            if (!disposing) return;

            _httpClient?.Dispose();
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}