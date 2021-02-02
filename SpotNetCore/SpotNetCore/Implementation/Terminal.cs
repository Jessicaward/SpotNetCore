using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SpotNetCore.Models;

namespace SpotNetCore.Implementation
{
    /// <summary>
    /// Terminal is supposed to act as a wrapper around Console. To provide colour, state etc.
    /// </summary>
    public class Terminal
    {
        private const ConsoleColor DefaultConsoleColor = ConsoleColor.White;
        
        public static void Startup()
        {
            WriteDarkGreen(@"

███████ ██████   ██████  ████████ ███    ██ ███████ ████████  ██████  ██████  ██████  ███████ 
██      ██   ██ ██    ██    ██    ████   ██ ██         ██    ██      ██    ██ ██   ██ ██      
███████ ██████  ██    ██    ██    ██ ██  ██ █████      ██    ██      ██    ██ ██████  █████   
     ██ ██      ██    ██    ██    ██  ██ ██ ██         ██    ██      ██    ██ ██   ██ ██      
███████ ██       ██████     ██    ██   ████ ███████    ██     ██████  ██████  ██   ██ ███████ 

~ https://github.com/Jessicaward/SpotNetCore ~
");
        }

        public static void WriteCurrentSong(SpotifyPlayerContext currentTrack)
        {
            if (currentTrack?.Item?.Album == null || currentTrack.Item.Artists.IsNullOrEmpty())
            {
                WriteYellow("Nothing is playing right now.");
                return;
            }
            
            WriteYellow($"Now Playing: {currentTrack.Item?.Name} - {currentTrack.Item?.Album?.Name} - {currentTrack.Item?.Artists.FirstOrDefault()?.Name}");
        }

        public static void WriteRed(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ForegroundColor = DefaultConsoleColor;
        }

        public static void WriteBlue(string text)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(text);
            Console.ForegroundColor = DefaultConsoleColor;
        }
        
        public static void WriteDarkBlue(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(text);
            Console.ForegroundColor = DefaultConsoleColor;
        }
        
        public static void WriteGreen(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(text);
            Console.ForegroundColor = DefaultConsoleColor;
        }
        
        public static void WriteMagenta(string text)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(text);
            Console.ForegroundColor = DefaultConsoleColor;
        }
        
        public static void WriteDarkGreen(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(text);
            Console.ForegroundColor = DefaultConsoleColor;
        }
        
        public static void WriteDarkRed(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(text);
            Console.ForegroundColor = DefaultConsoleColor;
        }
        
        public static void WriteYellow(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(text);
            Console.ForegroundColor = DefaultConsoleColor;
        }

        public static void WriteWhite(string text)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(text);
            Console.ForegroundColor = DefaultConsoleColor;
        }

        public static void WriteLine(string text)
        {
            WriteWhite(text);
            Console.ForegroundColor = DefaultConsoleColor;
        }

        public static string ReadLine()
        {
            Console.ForegroundColor = DefaultConsoleColor;
            return Console.ReadLine();
        }

        public static void SetCursorPosition(int left, int top = 0)
        {
            Console.SetCursorPosition(AppConstants.Prompt.Length + left, top);
        }

        public static void Clear()
        {
            Console.ForegroundColor = DefaultConsoleColor;
            Console.Clear();
        }
    }
}