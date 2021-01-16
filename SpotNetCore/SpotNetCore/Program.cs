using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpotNetCore.Implementation;
using SpotNetCore.Services;

namespace SpotNetCore
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Terminal.Startup();
            Terminal.ReadLine();
            
            var serviceProvider = new ServiceCollection()
                .AddSingleton<AuthenticationManager>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<PlayerService>()
                .AddSingleton<SearchService>()
                .AddSingleton<ArtistService>()
                .AddSingleton<AlbumService>()
                .AddSingleton<PlaylistService>()
                .AddSingleton<IConfigurationRoot>(config => new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build())
                .AddHostedService<AuthenticationManager>() //Refresh token
                .BuildServiceProvider();

            await serviceProvider.GetService<AuthenticationManager>().Authenticate();
            
            Terminal.Clear();
            
            //This is the main command handler. It will essentially handle everything apart from auth-related code.
            //API consumption is initiated here, but will eventually be executed elsewhere.
            await serviceProvider.GetService<CommandHandler>().HandleCommands();
        }
    }
}