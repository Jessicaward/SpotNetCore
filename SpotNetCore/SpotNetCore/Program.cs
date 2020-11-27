using System;
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
        public static SpotifyAccessToken Token;
        
        public Program(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public static async Task Main(string[] args)
        {
            await RunTheThing();
            var authorisationUrl = AuthorisationHelper.GetAuthorisationUrl();
            
            Console.WriteLine("Enter this into your browser to authorise this application to use Spotify on your behalf");
            Console.WriteLine(authorisationUrl);
            Console.ReadLine();
            Console.WriteLine(Token.AccessToken);
        }

        public static async Task RunTheThing()
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
                                Console.WriteLine("Endpoint hit");
                                Token = new SpotifyAccessToken();
                            });
                        });
                    }).Build().RunAsync();
            });
        }
    }
}