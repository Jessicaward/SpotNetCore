using System;
using System.Linq;
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

        private ParsedCommand ParseCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Input must contain command");
            }
            
            var split = input.Split(" ");
            return new ParsedCommand
            {
                Command = split[0],
                Parameters = split.Skip(1).Take(split.Length - 1)
            };
        } 
    }
}