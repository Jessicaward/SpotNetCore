using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SpotNetCore.Models;

namespace SpotNetCore.Endpoints
{
    public class ArtistEndpoint
    {
        private HttpClient _httpClient;

        public ArtistEndpoint(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<SpotifyTrack>> GetArtistTopTracks(string artistId)
        {
            return await _httpClient.GetFromSpotifyJsonAsync<IEnumerable<SpotifyTrack>>($"/v1/artists/{artistId}/top-tracks");
        }

        public async Task<IEnumerable<SpotifyAlbum>> GetArtistAlbums(string artistId)
        {
            return await _httpClient.GetFromSpotifyJsonAsync<IEnumerable<SpotifyAlbum>>($"/v1/artists/{artistId}/albums?include_groups=album");
        }
    }
}