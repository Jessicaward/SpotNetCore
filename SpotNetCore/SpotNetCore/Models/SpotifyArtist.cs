using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SpotNetCore.Models
{
    public class SpotifyArtist
    {
        //For more information on this type, visit https://developer.spotify.com/documentation/web-api/reference/object-model/#artist-object-simplified
        [JsonPropertyName("genres")]
        public IEnumerable<string> Genres { get; set; }
        
        [JsonPropertyName("href")]
        public string Href { get; set; }
        
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("popularity")]
        public int Popularity { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        //Implementation to support queueing and playing of many options.
        public IEnumerable<SpotifyTrack> Tracks { get; set; }
    }
}