using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SpotNetCore.Models;

namespace SpotNetCore.Endpoints
{
    public class AlbumEndpoint
    {
        private readonly HttpClient _httpClient;

        internal AlbumEndpoint(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<SpotifyTrack>> GetAlbumTracks(String albumId)
        {
            return await _httpClient.GetFromSpotifyJsonAsync<IEnumerable<SpotifyTrack>>($"/v1/albums/{albumId}/tracks");
        }
        
        public async Task<IEnumerable<SpotifyTrack>> GetAlbumTracks(SpotifyAlbum album)
        {
            return await GetAlbumTracks(album.Id);
        }
    }
}