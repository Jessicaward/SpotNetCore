using System.Collections.Generic;

namespace SpotNetCore.Models
{
    public class SpotifyArtist
    {
        //For more information on this type, visit https://developer.spotify.com/documentation/web-api/reference/object-model/#artist-object-simplified
        public IEnumerable<string> Genres { get; set; }
        public string Href { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public int Popularity { get; set; }
        public string Type { get; set; }
        public string Uri { get; set; }
    }
}