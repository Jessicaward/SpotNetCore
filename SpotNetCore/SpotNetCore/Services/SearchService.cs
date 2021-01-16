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
        private readonly ArtistService _artistService;
        private readonly AlbumService _albumService;
        private readonly PlaylistService _playlistService;
        private readonly HttpClient _httpClient;

        public SearchService(AuthenticationManager authenticationManager, ArtistService artistService, AlbumService albumService, PlaylistService playlistService)
        {
            _artistService = artistService;
            _albumService = albumService;
            _playlistService = playlistService;
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

            if (option == ArtistOption.Discography)
            {
                artist.Tracks = await _albumService.GetTracksFromAlbumCollection(await _artistService.GetDiscographyForArtist(artist.Id));
            }

            if (option == ArtistOption.Popular)
            {
                artist.Tracks = await _artistService.GetTopTracksForArtist(artist.Id);
            }

            if (option == ArtistOption.Essential)
            {
                var playlist = await SearchForPlaylist($"This Is {artist.Name}");

                if (playlist == null)
                {
                    throw new NoSearchResultException();
                }

                artist.Tracks = (await _playlistService.GetTracksInPlaylist(playlist.Id));
            }

            if (artist.Tracks.IsNullOrEmpty())
            {
                throw new NoSearchResultException();
            }

            return artist;
        }

        public async Task<SpotifyPlaylist> SearchForPlaylist(string query)
        {
            var response = await _httpClient.GetAsync($"https://api.spotify.com/v1/search?q={query}&type=playlist");

            response.EnsureSpotifySuccess();

            return (await JsonSerializerExtensions.DeserializeAnonymousTypeAsync(
                await response.Content.ReadAsStreamAsync(),
                new
                {
                    playlists = new
                    {
                        items = default(IEnumerable<SpotifyPlaylist>)
                    }
                })).playlists.items.FirstOrDefault();
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