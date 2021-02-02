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
    public class PlaylistService
    {
        private readonly SpotifyHttpClient _spotifyHttpClient;
        public PlaylistService(SpotifyHttpClient spotifyHttpClient)
        {
            _spotifyHttpClient = spotifyHttpClient;
        }

        public async Task<IEnumerable<SpotifyTrack>> GetTracksInPlaylist(string id)
        {
            var items = await _spotifyHttpClient.Playlist.GetPlaylistTracks(id);

            if (items == null || items.IsNullOrEmpty())
            {
                throw new NoSearchResultException();
            }

            return items.Select(x => x.Track);
        }
    }
}