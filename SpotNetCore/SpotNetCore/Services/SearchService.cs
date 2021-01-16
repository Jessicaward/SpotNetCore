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
                throw new NoSearchResultException();
            }

            var albumResponse = await _httpClient.GetAsync($"https://api.spotify.com/v1/albums/{album.Id}/tracks");

            albumResponse.EnsureSpotifySuccess();

            album.Tracks = (await JsonSerializerExtensions.DeserializeAnonymousTypeAsync(
                await albumResponse.Content.ReadAsStreamAsync(),
                new
                {
                    items = default(IEnumerable<SpotifyTrack>)
                })).items;

            if (album.Tracks.IsNullOrEmpty())
            {
                throw new NoSearchResultException();
            }
            
            return album;
        }

        public async Task<SpotifyArtist> SearchForArtist(string query, ArtistOption option)
        {
            var metadataResponse = await _httpClient.GetAsync($"https://api.spotify.com/v1/search?q={query}&type=artist");

            metadataResponse.EnsureSpotifySuccess();

            var artist = (await JsonSerializerExtensions.DeserializeAnonymousTypeAsync(
                await metadataResponse.Content.ReadAsStreamAsync(),
                    new
                    {
                        artists = new
                        {
                            items = default(IEnumerable<SpotifyArtist>)
                        }
                    })).artists.items.FirstOrDefault();

            if (artist == null)
            {
                throw new NoSearchResultException();
            }

            var tracks = new List<SpotifyTrack>();

            if (option == ArtistOption.Discography)
            {
                //todo: implement
            }

            if (option == ArtistOption.Popular)
            {
                //todo: implement
            }

            if (option == ArtistOption.Essential)
            {
                //todo: implement
            }

            if (tracks.IsNullOrEmpty())
            {
                throw new NoSearchResultException();
            }
            
            artist.Tracks = tracks;

            return artist;
        }

        public async Task<SpotifyPlaylist> SearchForPlaylist(string query)
        {
            //need to create spotify playlist type
            
            throw new NotImplementedException();
        }

        private async Task<SpotifyAlbum> GetDiscographyForArtist()
        {
            throw new NotImplementedException();
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