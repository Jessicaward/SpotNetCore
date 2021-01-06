namespace SpotNetCore.Models
{
    public class SpotifyContext
    {
        //For more information on this context visit: https://developer.spotify.com/documentation/web-api/reference/player/get-the-users-currently-playing-track/
        public string Uri { get; set; }
        public string Href { get; set; }
        public string Type { get; set; }
    }
}