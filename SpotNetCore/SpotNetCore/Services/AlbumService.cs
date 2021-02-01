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
    public class AlbumService
    {
        private readonly SpotifyHttpClient _spotifyHttpClient;
        
        public AlbumService(SpotifyHttpClient spotifyHttpClient)
        {
            _spotifyHttpClient = spotifyHttpClient;
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
            return await _spotifyHttpClient.Albums.GetAlbumTracks(albumId);
        }
    }
}