using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SpotNetCore.Models
{
    public class SpotifyTrack
    {
        //For more information on this type, visit https://developer.spotify.com/documentation/web-api/reference/object-model/#track-object-full
        [JsonPropertyName("album")]
        public SpotifyAlbum Album { get; set; }
        
        [JsonPropertyName("artists")]
        public IEnumerable<SpotifyArtist> Artists { get; set; }
        
        [JsonPropertyName("disc_number")]
        public int DiscNumber { get; set; }
        
        [JsonPropertyName("duration_ms")]
        public int DurationInMs { get; set; }
        
        [JsonPropertyName("explicit")]
        public bool Explicit { get; set; }
        
        [JsonPropertyName("href")]
        public string Href { get; set; }
        
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("is_playable")]
        public bool IsPlayable { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("popularity")]
        public int Popularity { get; set; }
        
        [JsonPropertyName("preview_url")]
        public string PreviewUrl { get; set; }
        
        [JsonPropertyName("track_number")]
        public int TrackNumber { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        [JsonPropertyName("uri")]
        public string Uri { get; set; }
        
        [JsonPropertyName("is_local")]
        public bool IsLocal { get; set; }
    }
}