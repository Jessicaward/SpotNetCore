using System;
using System.Text.Json;
using System.Net.Http;
using System.Text;
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
            
            await GetAuthToken();
            var authorisationUrl = AuthorisationHelper.GetAuthorisationUrl(_codeVerifier);
            
            Console.WriteLine("Enter this into your browser to authorise this application to use Spotify on your behalf");
            Console.WriteLine(authorisationUrl);
            Console.ReadLine();
            Console.WriteLine(Token.AccessToken);
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
                                        new StringContent(JsonSerializer.Serialize(new SpotifyAuthorisationCode()
                                    {
                                        Code = context.Request.Query["code"],
                                        ClientId = "33bea7a309d24a08a71ff9c8f48be287",
                                        GrantType = "authorization_code",
                                        RedirectUri = "https://localhost:5001/",
                                        CodeVerifier = _codeVerifier
                                    }), Encoding.UTF8, "application/json"));
                                    
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