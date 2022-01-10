using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SpotNetCore.Models
{
    public class SpotifySearchResult
    {
        
        [JsonPropertyName("tracks")]
        public SpotifySearchEntityResult<SpotifyTrack> Tracks { get; set; }

        [JsonPropertyName("albums")]
        public SpotifySearchEntityResult<SpotifyAlbum> Albums { get; set; }        
        
        [JsonPropertyName("artists")]
        public SpotifySearchEntityResult<SpotifyArtist> Artists { get; set; }        
        
        [JsonPropertyName("playlists")]
        public SpotifySearchEntityResult<SpotifyPlaylist> Playlists { get; set; }        
        
    }

    public class SpotifySearchEntityResult<T>
    {
        [JsonPropertyName("items")]
        public IEnumerable<T> Items { get; set; }
    }
}