using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SpotNetCore.Implementation;
using SpotNetCore.Models;

namespace SpotNetCore.Services
{
    public class PlayerService
    {
        private readonly HttpClient _httpClient;

        private readonly SpotifyHttpClient _spotifyHttpClient;
        public PlayerService(SpotifyHttpClient spotifyHttpClient)
        {
            _spotifyHttpClient = spotifyHttpClient;
        }
        
        public async Task PlayCurrentTrack()
        {
            var response = await _spotifyHttpClient.Player.Play();
            response.EnsureSpotifySuccess();
        }

        public async Task PauseCurrentTrack()
        {
            var response = await _spotifyHttpClient.Player.Pause();
            response.EnsureSpotifySuccess();
        }

        public async Task<SpotifyPlayerContext> NextTrack()
        {
            var lastPlayingTrack = await GetPlayerContext();

            var response = await _spotifyHttpClient.Player.Next();
            response.EnsureSpotifySuccess();

            return response.StatusCode == HttpStatusCode.Forbidden
                ? lastPlayingTrack
                : await WaitForNextTrack(lastPlayingTrack);
        }
        
        public async Task<SpotifyPlayerContext> GetPlayerContext()
        {
            return await _spotifyHttpClient.Player.Player();
        }

        public async Task<SpotifyPlayerContext> PreviousTrack()
        {
            var lastPlayingTrack = await GetPlayerContext();

            var response = await _spotifyHttpClient.Player.Previous();
            response.EnsureSpotifySuccess();

            return response.StatusCode == HttpStatusCode.Forbidden
                ? lastPlayingTrack
                : await WaitForNextTrack(lastPlayingTrack);
        }

        public async Task RestartTrack()
        {
            var response = await _spotifyHttpClient.Player.Seek(0);
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

            var response = await _spotifyHttpClient.Player.Shuffle(shuffleState);

            response.EnsureSpotifySuccess();
        }

        public async Task QueueTrack(string trackUri)
        {
            var response = await _spotifyHttpClient.Player.Queue(trackUri);

            response.EnsureSpotifySuccess();
        }
    }
}