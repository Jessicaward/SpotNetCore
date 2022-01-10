using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using SpotNetCore.Implementation;
using SpotNetCore.Models;
using SpotNetCore.Services;

namespace SpotNetCore
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection()
                .AddSingleton(sp =>
                {
                    var options = new JsonSerializerOptions()
                    {
                        WriteIndented = true
                    };
                    options.Converters.Add(new SpotifyTokensJsonConverter(sp.GetRequiredService<IDataProtectionProvider>()));
                    
                    var appSettings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText("appsettings.json"),options);
                    appSettings!.SpotifyTokens ??= new();
                    appSettings.PropertyChanged += (_, _) =>
                    {
                        File.WriteAllText("appsettings.json", JsonSerializer.Serialize(appSettings, options));
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
                .AddSingleton<PlaylistService>();
            
            serviceCollection
                .AddHttpClient<SpotifyHttpClient>(httpClient => { httpClient.BaseAddress = new Uri("https://api.spotify.com/"); })
                .AddHttpMessageHandler(provider => provider.GetRequiredService<SpotifyHttpClientHandler>());
            serviceCollection
                .AddDataProtection();
            
            Terminal.Startup();
            
            var serviceProvider = serviceCollection.BuildServiceProvider();

            await serviceProvider.GetService<AuthenticationManager>().Authenticate();
            
            //This is the main command handler. It will essentially handle everything apart from auth-related code.
            //API consumption is initiated here, but will eventually be executed elsewhere.
            await serviceProvider.GetService<CommandHandler>().HandleCommands();
        }
    }
}