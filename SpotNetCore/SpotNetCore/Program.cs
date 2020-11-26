using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            WebHost.CreateDefaultBuilder(args)
                .Configure(y =>
                {
                    y.UseRouting();
                    y.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/", async context =>
                        {
                            Console.WriteLine("endpoint received response");
                            await context.Response.WriteAsync("Hello world");
                        });
                    });
                }).Build().RunAsync();
            
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