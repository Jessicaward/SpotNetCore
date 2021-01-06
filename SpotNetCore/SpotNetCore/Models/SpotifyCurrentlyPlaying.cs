namespace SpotNetCore.Models
{
    public class SpotifyCurrentlyPlaying
    {
        public SpotifyContext Context { get; set; }
        public int Timestamp { get; set; }
        public int ProgressMs { get; set; }
        public bool IsPlaying { get; set; }
        public SpotifyTrack Item { get; set; }
        public string CurrentlyPlayingType { get; set; }
    }
}