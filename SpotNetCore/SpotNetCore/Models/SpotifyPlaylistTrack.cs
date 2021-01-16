using System.Text.Json.Serialization;

namespace SpotNetCore.Models
{
    public class SpotifyPlaylistTrack
    {
        [JsonPropertyName("track")]
        public SpotifyTrack Track { get; set; }
    }
}