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
    public class ArtistService : IDisposable
    {
        private readonly HttpClient _httpClient;
        
        public ArtistService(AuthenticationManager authenticationManager)
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
        
        ~ArtistService()
        {
            Dispose(false);
        }

        public async Task<IEnumerable<SpotifyTrack>> GetTopTracksForArtist(string id)
        {
            var response = await _httpClient.GetAsync($"https://api.spotify.com/v1/artists/{id}/top-tracks?market=GB");

            response.EnsureSpotifySuccess();

            return (await JsonSerializerExtensions.DeserializeAnonymousTypeAsync(await response.Content.ReadAsStreamAsync(),
                new
                {
                    tracks = default(IEnumerable<SpotifyTrack>)
                })).tracks;
        }
        
        public async Task<IEnumerable<SpotifyAlbum>> GetDiscographyForArtist(string id)
        {
            var response = await _httpClient.GetAsync($"https://api.spotify.com/v1/artists/{id}/albums?market=GB&include_groups=album");

            response.EnsureSpotifySuccess();

            var albums =  (await JsonSerializerExtensions.DeserializeAnonymousTypeAsync(await response.Content.ReadAsStreamAsync(),
                new
                {
                    items = default(IEnumerable<SpotifyAlbum>)
                })).items;

            //Spotify returns the albums in date order descending. Discography should be played in ascending order.
            return albums.Reverse();
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