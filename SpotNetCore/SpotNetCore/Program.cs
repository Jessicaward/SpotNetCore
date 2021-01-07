using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        
        public static async Task Main(string[] args)
        {
            Terminal.Startup();
            Terminal.ReadLine();
            
            var serviceProvider = new ServiceCollection()
                .AddSingleton<AuthenticationManager>()
                .AddLogging(logging =>
                {
                    logging.AddConsole();
                })
                .AddHostedService<AuthenticationManager>() //Refresh token
                .BuildServiceProvider();
            
            Terminal.Clear();
            
            //This is the main command handler. It will essentially handle everything apart from auth-related code.
            //API consumption is initiated here, but will eventually be executed elsewhere.
            await new CommandHandler().HandleCommands();
        }
    }
}