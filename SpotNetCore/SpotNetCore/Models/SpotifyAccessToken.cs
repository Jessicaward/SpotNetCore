namespace SpotNetCore.Models
{
    public class SpotifyAccessToken
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public string Scope { get; set; }
        public int ExpiresInSeconds { get; set; }
        public string RefreshToken { get; set; }
    }
}