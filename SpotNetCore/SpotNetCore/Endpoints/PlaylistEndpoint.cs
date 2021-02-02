using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SpotNetCore.Models;

namespace SpotNetCore.Endpoints
{
    public class PlaylistEndpoint
    {
        private readonly HttpClient _httpClient;

        public PlaylistEndpoint(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<IEnumerable<SpotifyPlaylistTrack>> GetPlaylistTracks(string playlistId)
        {
            return _httpClient.GetFromSpotifyJsonAsync<IEnumerable<SpotifyPlaylistTrack>>($"/v1/playlists/{playlistId}/tracks");
        }
    }
}