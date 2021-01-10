using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using SpotNetCore.Implementation;
using SpotNetCore.Models;

namespace SpotNetCore.Services
{
    public class SearchService : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationManager _authenticationManager;

        public SearchService(AuthenticationManager authenticationManager)
        {
            _httpClient = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue(_authenticationManager.Token.TokenType,
                        _authenticationManager.Token.AccessToken)
                }
            };
            _authenticationManager = authenticationManager;
        }

        ~SearchService()
        {
            Dispose(false);
        }

        public async Task<IEnumerable<SpotifyTrack>> SearchForTrack(string query)
        {
            var response = await _httpClient.GetAsync($"https://api.spotify.com/v1/search?q={query}&type=track");

            response.EnsureSpotifySuccess();

            return JsonSerializer.Deserialize<IEnumerable<SpotifyTrack>>(await response.Content.ReadAsStringAsync());
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