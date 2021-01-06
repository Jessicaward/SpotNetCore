using System.Text.Json.Serialization;

namespace SpotNetCore.Models
{
    public class SpotifyContext
    {
        //For more information on this context visit: https://developer.spotify.com/documentation/web-api/reference/player/get-the-users-currently-playing-track/
        [JsonPropertyName("uri")]
        public string Uri { get; set; }
        
        [JsonPropertyName("href")]
        public string Href { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}