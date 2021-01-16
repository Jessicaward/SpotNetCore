using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SpotNetCore.Implementation;
using SpotNetCore.Models;

namespace SpotNetCore.Services
{
    public class AlbumService : IDisposable
    {
        private readonly HttpClient _httpClient;
        
        public AlbumService(AuthenticationManager authenticationManager)
        {
            _httpClient = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue(authenticationManager.Token.TokenType,
                        authenticationManager.Token.AccessToken)
                }
            };
        }

        ~AlbumService()
        {
            Dispose(false);
        }
        
        public async Task<IEnumerable<SpotifyTrack>> GetTracksFromAlbumCollection(IEnumerable<SpotifyAlbum> albums)
        {
            if (albums.IsNullOrEmpty())
            {
                return new List<SpotifyTrack>();
            }
            
            var tracks = new List<SpotifyTrack>();

            foreach (var album in albums)
            {
                var albumTracks = (await GetTracksForAlbum(album.Id)).ToList();

                if (albumTracks.IsNullOrEmpty())
                {
                    break;
                }

                tracks.AddRange(albumTracks);
            }
            
            return tracks;
        }

        public async Task<IEnumerable<SpotifyTrack>> GetTracksForAlbum(string albumId)
        {
            var response = await _httpClient.GetAsync($"https://api.spotify.com/v1/albums/{albumId}/tracks");

            response.EnsureSpotifySuccess();

            return (await JsonSerializerExtensions.DeserializeAnonymousTypeAsync(
                await response.Content.ReadAsStreamAsync(),
                new
                {
                    items = default(IEnumerable<SpotifyTrack>)
                })).items;
        }
        
        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            _httpClient?.Dispose();
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}