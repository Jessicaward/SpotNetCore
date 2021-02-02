using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotNetCore.Implementation;
using SpotNetCore.Models;

namespace SpotNetCore.Services
{
    public class SearchService
    {
        private readonly ArtistService _artistService;
        private readonly AlbumService _albumService;
        private readonly PlaylistService _playlistService;
        private readonly SpotifyHttpClient _spotifyHttpClient;

        public SearchService(SpotifyHttpClient spotifyHttpClient, ArtistService artistService, AlbumService albumService, PlaylistService playlistService)
        {
            _artistService = artistService;
            _albumService = albumService;
            _playlistService = playlistService;
            _spotifyHttpClient = spotifyHttpClient;
        }

        public async Task<IEnumerable<SpotifyTrack>> SearchForTrack(string query)
        {
            return await _spotifyHttpClient.Search.SearchTracks(query);
        }

        public async Task<IEnumerable<SpotifyAlbum>> SearchForAlbum(string query, int limit = 10)
        {
            var albums = await _spotifyHttpClient.Search.SearchAlbums(query, limit);

            var spotifyAlbums = new List<SpotifyAlbum>();
            foreach (var spotifyAlbum in albums)
            {
                spotifyAlbum.Tracks = await _albumService.GetTracksForAlbum(spotifyAlbum.Id);
                if (spotifyAlbum.Tracks != null && spotifyAlbum.Tracks.IsNullOrEmpty())
                    spotifyAlbums.Add(spotifyAlbum);
            }
            
            if (spotifyAlbums.Count == 0)
            {
                throw new NoSearchResultException();
            }
            
            return spotifyAlbums;
        }

        public async Task<IEnumerable<SpotifyArtist>> SearchForArtist(string query, ArtistOption option, int limit = 10)
        {
            var artists = await _spotifyHttpClient.Search.SearchArtists(query, limit);
            
            var spotifyArtists = new List<SpotifyArtist>();
            foreach (var artist in artists)
            {
                artist.Tracks = option switch
                {
                    ArtistOption.Discography => await _albumService.GetTracksFromAlbumCollection(await _artistService.GetDiscographyForArtist(artist.Id)),
                    ArtistOption.Popular     => await _artistService.GetTopTracksForArtist(artist.Id),
                    ArtistOption.Essential   => (await SearchForPlaylist($"This Is {artist.Name}")).SelectMany(spotifyPlaylist=>spotifyPlaylist.Tracks),
                    _                        => artist.Tracks
                };
                
                if (artist.Tracks != null && !artist.Tracks.IsNullOrEmpty())
                    spotifyArtists.Add(artist);
            }
            
            if (spotifyArtists.Count == 0)
            {
                throw new NoSearchResultException();
            }
            
            return spotifyArtists;
        }

        public async Task<IEnumerable<SpotifyPlaylist>> SearchForPlaylist(string query, int limit = 10)
        {
            var playlists = await _spotifyHttpClient.Search.SearchPlaylists(query, limit);
            
            var spotifyPlaylists = new List<SpotifyPlaylist>();
            foreach (var playlist in playlists)
            {
                playlist.Tracks = await _playlistService.GetTracksInPlaylist(playlist.Id);
                if (playlist.Tracks != null && !playlist.Tracks.IsNullOrEmpty())
                    spotifyPlaylists.Add(playlist);
            }
            
            if (spotifyPlaylists.Count == 0)
            {
                throw new NoSearchResultException();
            }
            
            return spotifyPlaylists;
        }
    }
}