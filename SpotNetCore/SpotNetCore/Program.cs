using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SpotNetCore.Implementation;

namespace SpotNetCore
{
    class Program
    {
        private readonly IServiceProvider _serviceProvider;

        public Program(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .UseUrls("http://localhost:5001/")
                .Configure(c =>
                    c.Run(a =>
                        {
                            Console.WriteLine("writing response");
                            return a.Response.WriteAsync("Hello world");
                        }
                    )
                )
                .Build();
            
            host.Start();
            
            Console.WriteLine("Host setup finished, continuing with program.");
            Console.ReadLine();
        }

        private async Task<string> Authenticate()
        {
            using (var httpClient = new HttpClient())
            {
                var codeDetails = _serviceProvider.GetService<AuthorisationService>().Authorise();
                var response = await httpClient.GetAsync(codeDetails.AuthorisationUri);
                return response.StatusCode.ToString();
            }
        }
    }
}