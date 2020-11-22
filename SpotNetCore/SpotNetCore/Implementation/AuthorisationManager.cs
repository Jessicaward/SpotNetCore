using System;
using Microsoft.Extensions.Logging;

namespace SpotNetCore.Implementation
{
    public class AuthorisationManager
    {
        private readonly ILogger<AuthorisationManager> _logger;

        public AuthorisationManager(ILogger<AuthorisationManager> logger)
        {
            _logger = logger;
        }

        private string BuildAuthorisationUri(string clientId, string redirectUri, string codeChallenge, string state, string scopes)
        {
            return new UriBuilder()
            {
                Scheme = "https",
                Host = "accounts.spotify.com",
                Path = "authorize",
                Query = BuildAuthorisationQuery(clientId, redirectUri, codeChallenge, state, scopes)
            }.Uri.ToString();
        }

        private static string BuildAuthorisationQuery(string clientId, string redirectUri, string codeChallenge, string state, string scopes)
        {
            return Uri.EscapeUriString("?client_id=" + clientId + "&response_type=code" 
                   + "&redirect_uri=" + redirectUri + "&code_challenge_method=S256"
                   + "&code_challenge= " + codeChallenge + "&state=" + state + "&scope=" + scopes);
        }
    }
}