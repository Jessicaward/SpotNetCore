using System;
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
            var authService = new AuthorisationService();
            var token = authService.Authorise().Result;
            Console.ReadLine();
        }
    }
}