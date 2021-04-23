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
    public class PlaylistService : IDisposable
    {
        private readonly HttpClient _httpClient;
        
        public PlaylistService(AuthenticationManager authenticationManager)
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

        ~PlaylistService()
        {
            Dispose(false);
        }

        public async Task<IEnumerable<SpotifyTrack>> GetTracksInPlaylist(string id)
        {
            var tracks = Enumerable.Empty<SpotifyTrack>();

            var offset = 0;
            var total = Int32.MaxValue;
            while (offset < total)
            {
                var response = await _httpClient.GetAsync($"https://api.spotify.com/v1/playlists/{id}/tracks?market=GB&offset={offset}");

                response.EnsureSpotifySuccess();

                var responsePackage = await JsonSerializerExtensions.DeserializeAnonymousTypeAsync(await response.Content.ReadAsStreamAsync(),
                    new
                    {
                        items = default(IEnumerable<SpotifyPlaylistTrack>),
                        offset = default(int),
                        total = default(int)
                    });

                var responseTracks = responsePackage.items.Select(x => x.Track);
                offset = responsePackage.offset + responseTracks.Count();
                total = responsePackage.total;

                tracks = tracks.Union(responseTracks);
            }

            if (tracks.IsNullOrEmpty())
            {
                throw new NoSearchResultException();
            }

            return tracks;
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