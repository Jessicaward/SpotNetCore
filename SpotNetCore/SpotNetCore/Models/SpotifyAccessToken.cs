using System;
using System.Text.Json.Serialization;

namespace SpotNetCore.Models
{
    public class SpotifyAccessToken
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
        
        [JsonPropertyName("scope")]
        public string Scope { get; set; }
        
        [JsonPropertyName("expires_in")]
        public int ExpiresInSeconds { get; set; }
        
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}