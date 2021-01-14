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
    public class SearchService : IDisposable
    {
        private readonly HttpClient _httpClient;

        public SearchService(AuthenticationManager authenticationManager)
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

        ~SearchService()
        {
            Dispose(false);
        }
 
        public async Task<IEnumerable<SpotifyTrack>> SearchForTrack(string query)
        {
            var response = await _httpClient.GetAsync($"https://api.spotify.com/v1/search?q={query}&type=track");

            response.EnsureSpotifySuccess();
            
            return (await JsonSerializerExtensions.DeserializeAnonymousTypeAsync(await response.Content.ReadAsStreamAsync(), 
                new
                {
                    tracks = new
                    {
                        items = default(IEnumerable<SpotifyTrack>)
                    }
                }))?.tracks?.items;
        }

        public async Task<SpotifyAlbum> SearchForAlbum(string query)
        {
            var metadataResponse = await _httpClient.GetAsync($"https://api.spotify.com/v1/search?q={query}&type=album");

            metadataResponse.EnsureSpotifySuccess();
            
            var album = (await JsonSerializerExtensions.DeserializeAnonymousTypeAsync(await metadataResponse.Content.ReadAsStreamAsync(),
                new
                {
                    albums = new
                    {
                        items = default(IEnumerable<SpotifyAlbum>)
                    } 
                })).albums.items.FirstOrDefault();

            if (album == null)
            {
                throw new NoResponseException();
            }

            var albumResponse = await _httpClient.GetAsync($"https://api.spotify.com/v1/albums/{album.Id}/tracks");

            albumResponse.EnsureSpotifySuccess();

            album.Tracks = (await JsonSerializerExtensions.DeserializeAnonymousTypeAsync(
                await albumResponse.Content.ReadAsStreamAsync(),
                new
                {
                    items = default(IEnumerable<SpotifyTrack>)
                })).items;
            
            return album;
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