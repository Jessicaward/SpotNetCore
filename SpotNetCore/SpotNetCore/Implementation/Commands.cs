using System.Collections.Generic;
using System.Linq;

namespace SpotNetCore.Implementation
{
    public static class Commands
    {
        public static Dictionary<string, string> GetCommandsWithDescriptionList()
        {
            return new Dictionary<string, string>
            {
                {"play", "play the current track."},
                {"pause", "pause the current track."},
                {"current", "shows the current track."},
                {"next", "play the next track."},
                {"previous", "play the previous track."},
                {"restart", "restart the current track."},
                {"shuffle", "play random tracks in the current playlist."},
                {"shuffle on", "enables the shuffle mode."},
                {"shuffle off", "disables the shuffle mode."},
                {"shuffle false", "disables the shuffle mode."},
                {"shuffle true", "enables the shuffle mode."},
                {"help", "show this help menu."},
                {"exit", "closes the application."},
                {"close", "closes the application."},
                {"quit", "closes the application."}
            };
        }

        public static List<string> GetCommandsList()
        {
            var listOfCommands = GetCommandsWithDescriptionList().Keys.ToList();
            
            listOfCommands.Sort();
            
            return listOfCommands;
        }
    }
}