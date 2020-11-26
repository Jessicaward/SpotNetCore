using System;

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
            Console.WriteLine("Authorising");
            Console.ReadLine();
        }
    }
}