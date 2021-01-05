using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SpotNetCore.Implementation;

namespace SpotNetCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<AuthorisationManager>()
                .AddLogging(logging =>
                {
                    logging.AddConsole();
                })
                .BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app)
        {
            
        }
    }
}