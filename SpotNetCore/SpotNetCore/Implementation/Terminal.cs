using System;
using System.Threading;
using System.Threading.Tasks;

namespace SpotNetCore.Implementation
{
    /// <summary>
    /// Terminal is suppoesed to act as a wrapper around Console. To provide colour, state etc.
    /// </summary>
    public class Terminal
    {
        public static void Startup()
        {
            WriteGreen(@"

███████ ██████   ██████  ████████ ███    ██ ███████ ████████  ██████  ██████  ██████  ███████ 
██      ██   ██ ██    ██    ██    ████   ██ ██         ██    ██      ██    ██ ██   ██ ██      
███████ ██████  ██    ██    ██    ██ ██  ██ █████      ██    ██      ██    ██ ██████  █████   
     ██ ██      ██    ██    ██    ██  ██ ██ ██         ██    ██      ██    ██ ██   ██ ██      
███████ ██       ██████     ██    ██   ████ ███████    ██     ██████  ██████  ██   ██ ███████ 

~ https://github.com/Jessicaward/SpotNetCore ~
");
        }
        
        public static void WriteRed(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
        }
        
        public static void WriteBlue(string text)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(text);
        }
        
        public static void WriteDarkBlue(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(text);
        }
        
        public static void WriteGreen(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(text);
        }
        
        public static void WriteMagenta(string text)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(text);
        }
        
        public static void WriteDarkGreen(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(text);
        }
        
        public static void WriteDarkRed(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(text);
        }
        
        public static void WriteYellow(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(text);
        }

        public static void WriteWhite(string text)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(text);
        }
    }
}