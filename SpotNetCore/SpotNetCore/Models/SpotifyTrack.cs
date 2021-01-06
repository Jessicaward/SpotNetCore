using System.Collections.Generic;

namespace SpotNetCore.Models
{
    public class SpotifyTrack
    {
        //For more information on this type, visit https://developer.spotify.com/documentation/web-api/reference/object-model/#track-object-full
        public SpotifyAlbum Album { get; set; }
        public IEnumerable<SpotifyArtist> Artists { get; set; }
        public int DiscNumber { get; set; }
        public int DurationInMs { get; set; }
        public bool Explicit { get; set; }
        public string Href { get; set; }
        public string Id { get; set; }
        public bool IsPlayable { get; set; }
        public string Name { get; set; }
        public int Popularity { get; set; }
        public string PreviewUrl { get; set; }
        public int TrackNumber { get; set; }
        public string Type { get; set; }
        public string Uri { get; set; }
        public bool IsLocal { get; set; }
    }
}