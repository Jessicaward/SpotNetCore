using System;
using System.Linq;

namespace SpotNetCore.Implementation
{
    public class HelpManager
    {
        public static void DisplayHelp()
        {
            Terminal.WriteMagenta("---Help---");
            Terminal.WriteWhite("This is a full list of SpotNetCore commands:");

            var commandsWithDescription = Commands.GetCommandsWithDescriptionList();
            var commands = commandsWithDescription.Keys.ToList();

            foreach (var command in commands)
            {
                Console.WriteLine("{0}: {1}", command, commandsWithDescription[command]);
            }
        }
    }
}