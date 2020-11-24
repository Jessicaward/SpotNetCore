using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SpotNetCore.Implementation;

namespace SpotNetCore
{
    class Program
    {
        public static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<AuthorisationService>()
                .AddLogging(logging =>
                {
                    logging.AddConsole();
                })
                .BuildServiceProvider();

            var codeDetails = serviceProvider.GetService<AuthorisationService>().Authorise();
            
            Console.WriteLine(codeDetails.CodeChallenge);
            Console.WriteLine(codeDetails.CodeVerifier);
            Console.WriteLine(codeDetails.AuthorisationUri);
            
            Console.ReadLine();
        }
    }
}