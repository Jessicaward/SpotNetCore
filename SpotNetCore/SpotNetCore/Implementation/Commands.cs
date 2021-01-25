using System.Collections.Generic;
using System.Linq;

namespace SpotNetCore.Implementation
{
    public static class Commands
    {
        public static Dictionary<string, string> GetCommandsWithDescriptionList()
        {
            var listOfCommands = new Dictionary<string, string>();

            listOfCommands.Add("play", "play the current track.");
            listOfCommands.Add("pause", "pause the current track.");
            listOfCommands.Add("current", "shows the current track.");
            listOfCommands.Add("next", "play the next track.");
            listOfCommands.Add("previous", "play the previous track.");
            listOfCommands.Add("restart", "restart the current track.");
            listOfCommands.Add("shuffle", "play random tracks in the current playlist.");
            listOfCommands.Add("shuffle on", "enables the shuffle mode.");
            listOfCommands.Add("shuffle off", "disables the shuffle mode.");
            listOfCommands.Add("shuffle false", "disables the shuffle mode.");
            listOfCommands.Add("shuffle true", "enables the shuffle mode.");
            listOfCommands.Add("help", "show this help menu.");
            listOfCommands.Add("exit", "closes the application.");
            listOfCommands.Add("close", "closes the application.");
            listOfCommands.Add("quit", "closes the application.");

            return listOfCommands;
        }

        public static List<string> GetCommandsList()
        {
            var listOfCommands = GetCommandsWithDescriptionList().Keys.ToList();

            listOfCommands.Sort();
            
            return listOfCommands;
        }
    }
}
