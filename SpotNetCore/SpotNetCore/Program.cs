using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SpotNetCore.Implementation;

namespace SpotNetCore
{
    class Program
    {
        private static IServiceProvider _serviceProvider;
        
        public static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<AuthorisationService>()
                .AddLogging(logging =>
                {
                    logging.AddConsole();
                })
                .BuildServiceProvider();

            _serviceProvider = serviceProvider;
        }

        private static async Task<string> Authenticate()
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