using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SpotNetCore.Implementation;
using SpotNetCore.Models;

namespace SpotNetCore
{
    class Program
    {
        private readonly IServiceProvider _serviceProvider;
        private static string _codeVerifier;
        public static SpotifyAccessToken Token;
        
        public Program(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public static async Task Main(string[] args)
        {
            Terminal.Startup();
            Console.ReadLine();
            
            _codeVerifier = AuthorisationCodeDetails.CreateCodeVerifier();
            
            await GetAuthToken();
            var authorisationUrl = AuthorisationHelper.GetAuthorisationUrl(_codeVerifier);
            
            Terminal.WriteLine("Enter this into your browser to authorise this application to use Spotify on your behalf");
            Terminal.WriteLine(authorisationUrl);
            Console.Clear();
        }
        
        public static async Task GetAuthToken()
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