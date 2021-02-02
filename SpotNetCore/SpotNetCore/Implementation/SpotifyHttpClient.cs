using System.Net.Http;
using SpotNetCore.Endpoints;

namespace SpotNetCore.Implementation
{
    public class SpotifyHttpClient 
    {
        public PlayerEndpoint Player { get; }
        public AlbumEndpoint Albums { get; }
        public ArtistEndpoint Artists { get; }
        public PlaylistEndpoint Playlist { get; }
        public SearchEndpoint Search { get; }
        
        private readonly HttpClient _httpClient;
        public SpotifyHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            Player = new PlayerEndpoint(httpClient);
            Albums = new AlbumEndpoint(httpClient);
            Artists = new ArtistEndpoint(httpClient);
            Playlist = new PlaylistEndpoint(httpClient);
            Search = new SearchEndpoint(httpClient);
        }
    }
}