using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SpotNetCore.Models;

namespace SpotNetCore.Implementation
{
    public class AuthenticationManager : IDisposable
    {
        private readonly IConfigurationRoot _config;
        public static bool IsAuthenticated;
        private static string _codeVerifier;
        private static int _checkRefreshTimeInSeconds; 
        public SpotifyAccessToken Token;
        private readonly HttpClient _httpClient;

        public AuthenticationManager(IConfigurationRoot config)
        {
            _config = config;
            _codeVerifier = AuthorisationCodeDetails.CreateCodeVerifier();
            _checkRefreshTimeInSeconds = 30;
            _httpClient = new HttpClient();
        }

        ~AuthenticationManager()
        {
            Dispose(false);
        }

        public bool IsTokenAboutToExpire() => Token.ExpiresAt <= DateTime.Now.AddSeconds(20);

        private async Task GetAuthToken()
        {
            await Task.Run(() =>
            {
                WebHost.CreateDefaultBuilder(null)
                    .Configure(y =>
                    {
                        y.UseRouting();
                        y.UseEndpoints(endpoints =>
                        {
                            endpoints.MapGet("/", async context =>
                            {
                                await context.Response.CompleteAsync();
                                
                                var response = await _httpClient.PostAsync("https://accounts.spotify.com/api/token",
                                    new FormUrlEncodedContent(new Dictionary<string, string>
                                    {
                                        {"code", context.Request.Query["code"].ToString()},
                                        {"client_id", _config.GetSection("clientId").Value},
                                        {"grant_type", "authorization_code"},
                                        {"redirect_uri", "http://localhost:5000/"},
                                        {"code_verifier", _codeVerifier}
                                    }));
                                
                                response.EnsureSuccessStatusCode();

                                Token = JsonSerializer.Deserialize<SpotifyAccessToken>(await response.Content.ReadAsStringAsync());
                                
                                Token.ExpiresAt = DateTime.Now.AddSeconds(Token.ExpiresInSeconds);
                                IsAuthenticated = true;
                            });
                        });
                    }).Build().RunAsync();
            });
        }
        
        public async Task RequestRefreshedAccessToken()
        {
            var response = await _httpClient.PostAsync("https://accounts.spotify.com/api/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"grant_type", "refresh_token"},
                    {"refresh_token", Token.RefreshToken},
                    {"client_id", _config.GetSection("clientId").Value}
                }));

            response.EnsureSuccessStatusCode();

            Token = JsonSerializer.Deserialize<SpotifyAccessToken>(
                await response.Content.ReadAsStringAsync());

            if (Token != null)
            {
                Token.ExpiresAt = DateTime.Now.AddSeconds(Token.ExpiresInSeconds);
            }
        }
        
        public async Task Authenticate()
        {
            await GetAuthToken();
            
            Terminal.WriteLine("Enter this into your browser to authorise this application to use Spotify on your behalf");
            Terminal.WriteLine(GetAuthorisationUrl(_codeVerifier));
            Terminal.ReadLine();
        }

        private string GetAuthorisationUrl(string codeVerifier)
        {
            var details = new AuthorisationCodeDetails(codeVerifier, "http://localhost:5000/");
            var scopes = _config.GetSection("requiredScopes").Get<List<string>>();
            details.AuthorisationUri = BuildAuthorisationUri(_config.GetSection("clientId").Value, details.RedirectUri, details.CodeChallenge, "fh82hfosdf8h", string.Join(' ', scopes));
            
            return details.AuthorisationUri;
        }

        private string BuildAuthorisationUri(string clientId, string redirectUri, string codeChallenge, string state, string scopes)
        {
            return new UriBuilder()
            {
                Scheme = "https",
                Host = "accounts.spotify.com",
                Path = "authorize",
                Query = BuildAuthorisationQuery(clientId, redirectUri, codeChallenge, state, scopes)
            }.Uri.ToString();
        }

        private static string BuildAuthorisationQuery(string clientId, string redirectUri, string codeChallenge, string state, string scopes)
        {
            return "?client_id=" + clientId + "&response_type=code" 
                   + "&redirect_uri=" + redirectUri + "&code_challenge_method=S256"
                   + "&code_challenge=" + codeChallenge + "&state=" + state + "&scope=" + Uri.EscapeUriString(scopes);
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