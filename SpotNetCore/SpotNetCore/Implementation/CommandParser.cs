using System;
using SpotNetCore.Models;

namespace SpotNetCore.Implementation
{
    public class CommandParser
    {
        public async void HandleCommands()
        {
            var exit = false;
            while (!exit)
            {
                var command = ParseCommand(GetUserInput());
            }
        }

        private string GetUserInput()
        {
            return Console.ReadLine();
        }

        private Command ParseCommand(string input)
        {
            return new Command();
        }
    }
}