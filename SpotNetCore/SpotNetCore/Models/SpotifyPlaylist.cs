using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SpotNetCore.Models
{
    public class SpotifyPlaylist
    {
        [JsonPropertyName("collaborative")]
        public bool Collaborative { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        [JsonPropertyName("href")]
        public string Href { get; set; }
        
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        public IEnumerable<SpotifyTrack> Tracks { get; set; }
    }
}