using System.Collections.Generic;

namespace SpotNetCore.Models
{
    public class Command
    {
        public string command { get; set; }
        public IEnumerable<string> Parameters { get; set; }
    }
}