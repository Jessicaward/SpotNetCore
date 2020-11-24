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
        private static HttpListener _httpListener;
        
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

            Authenticate();

            Console.ReadLine();
        }

        private static async Task<string> Authenticate()
        {
            var nonBlockingHttpListener = new NonBlockingHttpListener(5);
            nonBlockingHttpListener.Start(12345);
            
            using (var httpClient = new HttpClient())
            {
                var codeDetails = _serviceProvider.GetService<AuthorisationService>().Authorise();
                var response = await httpClient.GetAsync(codeDetails.AuthorisationUri);
                return response.StatusCode.ToString();
            }
        }
    }
}