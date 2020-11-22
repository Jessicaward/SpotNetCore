using Microsoft.Extensions.DependencyInjection;

namespace SpotNetCore
{
    class Program
    {
        public static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                //.AddSingleton<SomethingService>()
                .BuildServiceProvider();
        }
    }
}