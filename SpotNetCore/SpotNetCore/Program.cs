using System;
using System.IO;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SpotNetCore.Implementation;
using SpotNetCore.Services;

namespace SpotNetCore
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection()
                .AddSingleton(_ =>
                {
                    var appSettings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText("appsettings.json"));
                    appSettings!.SpotifyTokens ??= new();
                    appSettings.PropertyChanged += (_, _) =>
                    {
                        File.WriteAllText("appsettings.json", JsonSerializer.Serialize(appSettings, new()
                        {
                            WriteIndented = true
                        }));
                    };
                    return appSettings;
                })
                .AddTransient<SpotifyHttpClientHandler>()
                .AddTransient<SpotifyCallbackListener>()
                .AddSingleton<AuthenticationManager>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<PlayerService>()
                .AddSingleton<SearchService>()
                .AddSingleton<ArtistService>()
                .AddSingleton<AlbumService>()
                .AddSingleton<PlaylistService>()
                .AddHttpClient<SpotifyHttpClient>(httpClient => { httpClient.BaseAddress = new Uri("https://api.spotify.com/"); })
                .AddHttpMessageHandler(provider => provider.GetRequiredService<SpotifyHttpClientHandler>())
                .Services;
            Terminal.Startup();
            
            var serviceProvider = serviceCollection.BuildServiceProvider();

            await serviceProvider.GetService<AuthenticationManager>().Authenticate();
            
            //This is the main command handler. It will essentially handle everything apart from auth-related code.
            //API consumption is initiated here, but will eventually be executed elsewhere.
            await serviceProvider.GetService<CommandHandler>().HandleCommands();
        }
    }
}