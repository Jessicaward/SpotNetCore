using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SpotNetCore.Models;

namespace SpotNetCore.Endpoints
{
    public class SearchEndpoint
    {
        private HttpClient _httpClient;

        public SearchEndpoint(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private async Task<SpotifySearchResult> Search(string query, string type, int limit = 10, int offset = 0)
        {
            return await _httpClient.GetFromSpotifyJsonAsync<SpotifySearchResult>($"/v1/search?q={query}&type={type}&limit={limit}&offset={offset}");
        }
        
        public async Task<IEnumerable<SpotifyTrack>> SearchTracks(string query, int limit = 10, int offset = 0)
        {
            var data = await Search(query, "track", limit, offset);
            return data.Tracks.Items;
        } 
        
        public async Task<IEnumerable<SpotifyAlbum>> SearchAlbums(string query, int limit = 10, int offset = 0)
        {
            var data = await Search(query, "album", limit, offset);
            return data.Albums.Items;
        } 
        
        public async Task<IEnumerable<SpotifyPlaylist>> SearchPlaylists(string query, int limit = 10, int offset = 0)
        {
            var data = await Search(query, "playlist", limit, offset);
            return data.Playlists.Items;
        } 
        
        public async Task<IEnumerable<SpotifyArtist>> SearchArtists(string query, int limit = 10, int offset = 0)
        {
            var data = await Search(query, "artist", limit, offset);
            return data.Artists.Items;
        } 
    }
}