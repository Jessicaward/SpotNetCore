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
    public class ArtistService
    {
        private readonly SpotifyHttpClient _spotifyHttpClient;
        
        public ArtistService(SpotifyHttpClient spotifyHttpClient)
        {
            _spotifyHttpClient = spotifyHttpClient;
        }

        public async Task<IEnumerable<SpotifyTrack>> GetTopTracksForArtist(string id)
        {
            return await _spotifyHttpClient.Artists.GetArtistTopTracks(id);
        }
        
        public async Task<IEnumerable<SpotifyAlbum>> GetDiscographyForArtist(string id)
        {
            var result = await _spotifyHttpClient.Artists.GetArtistAlbums(id);

            return result.Reverse();
        }
    }
}