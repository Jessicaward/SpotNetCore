using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SpotNetCore.Models;

namespace SpotNetCore.Implementation
{
    public class AuthorisationManager
    {
        private static string _codeVerifier;
        public static SpotifyAccessToken Token;

        public AuthorisationManager()
        {
            _codeVerifier = AuthorisationCodeDetails.CreateCodeVerifier();
        }

        public async Task<SpotifyAccessToken> Authenticate()
        {
            await GetAuthToken();
            
            var authorisationUrl = AuthorisationManager.GetAuthorisationUrl(_codeVerifier);
            
            Terminal.WriteLine("Enter this into your browser to authorise this application to use Spotify on your behalf");
            Terminal.WriteLine(authorisationUrl);
            Terminal.ReadLine();

            while (Token == null)
            {
                //todo: rewrite. maybe we can pass token back from GetAuthToken? If we can get around void return.
                Thread.Sleep(250);
            }

            return Token;
        }
        
        public static string GetAuthorisationUrl(string codeVerifier)
        {
            var details = new AuthorisationCodeDetails(codeVerifier, "https://localhost:5001/");
            details.AuthorisationUri = BuildAuthorisationUri("33bea7a309d24a08a71ff9c8f48be287", details.RedirectUri, details.CodeChallenge, "fh82hfosdf8h", "user-follow-modify");
            
            return details.AuthorisationUri;
        }

        private static string BuildAuthorisationUri(string clientId, string redirectUri, string codeChallenge, string state, string scopes)
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
        
        private static async Task GetAuthToken()
        {
            Task.Run(() =>
            {
                WebHost.CreateDefaultBuilder(null)
                    .Configure(y =>
                    {
                        y.UseRouting();
                        y.UseEndpoints(endpoints =>
                        {
                            endpoints.MapGet("/", async context =>
                            {
                                await context.Response.WriteAsync("");

                                using (var httpClient = new HttpClient())
                                {
                                    var response = await httpClient.PostAsync("https://accounts.spotify.com/api/token",
                                        new FormUrlEncodedContent(new Dictionary<string, string>
                                        {
                                            {"code", context.Request.Query["code"].ToString()},
                                            {"client_id", "33bea7a309d24a08a71ff9c8f48be287"},
                                            {"grant_type", "authorization_code"},
                                            {"redirect_uri", "https://localhost:5001/"},
                                            {"code_verifier", _codeVerifier}
                                        }));
                                    
                                    response.EnsureSuccessStatusCode();

                                    Token = JsonSerializer.Deserialize<SpotifyAccessToken>(
                                        await response.Content.ReadAsStringAsync());
                                    
                                    Token.ExpiresAt = DateTime.Now.AddSeconds(Token.ExpiresInSeconds);
                                }
                            });
                        });
                    }).Build().RunAsync();
            });
        }
    }
}