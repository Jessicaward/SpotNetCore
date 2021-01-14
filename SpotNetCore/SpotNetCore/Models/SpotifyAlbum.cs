using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;

namespace SpotNetCore.Models
{
    public class SpotifyAlbum
    {
        //For more information on this type, visit https://developer.spotify.com/documentation/web-api/reference/object-model/#album-object-simplified
        [JsonPropertyName("album_type")]
        public string AlbumType { get; set; }
        
        [JsonPropertyName("artists")]
        public IEnumerable<SpotifyArtist> Artists { get; set; }
        
        [JsonPropertyName("href")]
        public string Href { get; set; }
        
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; }
        
        [JsonPropertyName("release_date_precision")]
        public string ReleaseDatePrecision { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        public IEnumerable<SpotifyTrack> Tracks { get; set; }
    }
}