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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SpotNetCore.Models;

namespace SpotNetCore.Implementation
{
    public class AuthenticationManager : BackgroundService
    {
        private readonly IConfigurationRoot _config;
        public static bool IsAuthenticated;
        private static string _codeVerifier;
        private static int _checkRefreshTimeInSeconds; 
        public SpotifyAccessToken Token;

        public AuthenticationManager(IConfigurationRoot config)
        {
            _config = config;
            _codeVerifier = AuthorisationCodeDetails.CreateCodeVerifier();
            _checkRefreshTimeInSeconds = 30;
        }

        public bool IsTokenAboutToExpire() => Token.ExpiresAt <= DateTime.Now.AddSeconds(20);

        public static async Task<SpotifyAccessToken> RequestRefreshedAccessToken()
        {
            throw new NotImplementedException();
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
            var details = new AuthorisationCodeDetails(codeVerifier, "https://localhost:5001/");
            var scopes = _config.GetSection("requiredScopes").Get<List<string>>();
            details.AuthorisationUri = BuildAuthorisationUri("33bea7a309d24a08a71ff9c8f48be287", details.RedirectUri, details.CodeChallenge, "fh82hfosdf8h", string.Join(' ', scopes));
            
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
        
        private async Task GetAuthToken()
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

                                    Token = JsonSerializer.Deserialize<SpotifyAccessToken>(await response.Content.ReadAsStringAsync());
                                    
                                    Token.ExpiresAt = DateTime.Now.AddSeconds(Token.ExpiresInSeconds);
                                    IsAuthenticated = true;
                                }
                            });
                        });
                    }).Build().RunAsync();
            });
        }
        
        //Refresh Token task
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //todo: this doesn't run, why?
            while (!stoppingToken.IsCancellationRequested)
            {
                if (Token.ExpiresAt.AddSeconds(_checkRefreshTimeInSeconds) < DateTime.Now)
                {
                    //If refresh is not needed, wait for x amount of time.
                    await Task.Delay(_checkRefreshTimeInSeconds * 1000, stoppingToken);
                    continue;
                }
                
                //Refresh access token
                await Task.Run(async () =>
                {
                    using (var httpClient = new HttpClient())
                    {
                        var response = await httpClient.PostAsync("https://accounts.spotify.com/api/token",
                            new FormUrlEncodedContent(new Dictionary<string, string>
                            {
                                {"grant_type", "refresh_token"},
                                {"refresh_token", Token.RefreshToken},
                                {"client_id", "33bea7a309d24a08a71ff9c8f48be287"}
                            }), stoppingToken);

                        response.EnsureSuccessStatusCode();

                        Token = JsonSerializer.Deserialize<SpotifyAccessToken>(
                            await response.Content.ReadAsStringAsync());

                        Token.ExpiresAt = DateTime.Now.AddSeconds(Token.ExpiresInSeconds);
                    }
                }, stoppingToken);
            }
        }
    }
}