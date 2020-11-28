using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Net.Http;
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
            _codeVerifier = AuthorisationCodeDetails.CreateCodeVerifier();
            
            await RunTheThing();
            var authorisationUrl = AuthorisationHelper.GetAuthorisationUrl(_codeVerifier);
            
            Console.WriteLine("Enter this into your browser to authorise this application to use Spotify on your behalf");
            Console.WriteLine(authorisationUrl);
            Console.ReadLine();
            Console.WriteLine(Token.AccessToken);
        }

        public static async Task RunTheThing()
        {
            Console.WriteLine("Running the thing");
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
                                    Console.WriteLine("sending response");
                                    var response = await httpClient.PostAsync("https://accounts.spotify.com/api/token",
                                        new StringContent(JsonSerializer.Serialize(new SpotifyAuthorisationCode()
                                    {
                                        Code = context.Request.Query["code"],
                                        ClientId = "33bea7a309d24a08a71ff9c8f48be287",
                                        GrantType = "authorization_code",
                                        RedirectUri = "https://localhost:5001/",
                                        CodeVerifier = _codeVerifier
                                    })));
                                    
                                    Console.WriteLine(response.StatusCode);
                                    
                                    response.EnsureSuccessStatusCode();

                                    Token = JsonSerializer.Deserialize<SpotifyAccessToken>(
                                        await response.Content.ReadAsStringAsync());
                                    
                                    Token.ExpiresAt = DateTime.Now.AddSeconds(Token.ExpiresInSeconds);
                                    
                                    Console.WriteLine(Token.AccessToken);
                                    Console.WriteLine(Token.ExpiresAt);
                                }
                            });
                        });
                    }).Build().RunAsync();
            });
        }
    }
}