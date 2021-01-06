using System.Collections.Generic;

namespace SpotNetCore.Models
{
    public class SpotifyAlbum
    {
        //For more information on this type, visit https://developer.spotify.com/documentation/web-api/reference/object-model/#album-object-simplified
        public string AlbumType { get; set; }
        public IEnumerable<SpotifyArtist> Artists { get; set; }
        public string Href { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string ReleaseDate { get; set; }
        public string ReleaseDatePrecision { get; set; }
        public string Type { get; set; }
        public string Uri { get; set; }
    }
}