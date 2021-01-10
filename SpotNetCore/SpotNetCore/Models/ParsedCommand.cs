using System.Collections.Generic;

namespace SpotNetCore.Models
{
    public class ParsedCommand
    {
        public string Command { get; set; }
        public IEnumerable<ParsedParameter> Parameters { get; set; }
    }
}