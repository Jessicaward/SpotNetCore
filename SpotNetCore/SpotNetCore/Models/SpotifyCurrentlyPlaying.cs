using System;
using System.Text.Json.Serialization;

namespace SpotNetCore.Models
{
    public class SpotifyCurrentlyPlaying
    {
        [JsonPropertyName("context")]
        public SpotifyContext Context { get; set; }
        
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }
        
        [JsonPropertyName("progress_ms")]
        public int ProgressMs { get; set; }
        
        [JsonPropertyName("is_playing")]
        public bool IsPlaying { get; set; }
        
        [JsonPropertyName("item")]
        public SpotifyTrack Item { get; set; }
        
        [JsonPropertyName("currently_playing_type")]
        public string CurrentlyPlayingType { get; set; }
    }
}