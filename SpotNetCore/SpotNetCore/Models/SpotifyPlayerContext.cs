using System.Text.Json.Serialization;

namespace SpotNetCore.Models
{
    public class SpotifyPlayerContext
    {
        [JsonPropertyName("device")]
        public SpotifyDevice Device { get; set; }
        
        [JsonPropertyName("repeat_state")]
        public string RepeatState { get; set; }

        [JsonPropertyName("shuffle_state")]
        public bool ShuffleState { get; set; }

        [JsonPropertyName("context")]
        public SpotifyContext Context { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("progress_ms")]
        public int ProgressInMs { get; set; }

        [JsonPropertyName("is_playing")]
        public bool IsPlaying { get; set; }

        [JsonPropertyName("item")]
        public SpotifyTrack Item { get; set; }

        [JsonPropertyName("currently_playing_type")]
        public string CurrentlyPlayingType { get; set; }
    }
}