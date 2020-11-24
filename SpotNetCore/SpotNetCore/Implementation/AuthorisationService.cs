using System;
using Microsoft.Extensions.Logging;
using SpotNetCore.Models;

namespace SpotNetCore.Implementation
{
    public class AuthorisationService
    {
        private readonly ILogger<AuthorisationService> _logger;

        public AuthorisationService(ILogger<AuthorisationService> logger)
        {
            _logger = logger;
        }

        public AuthorisationCodeDetails Authorise()
        {
            var details = new AuthorisationCodeDetails();

            details.AuthorisationUri = BuildAuthorisationUri("test", "test", details.CodeChallenge, "S256", "something something");
            
            return details;
        }

        private static string BuildAuthorisationUri(string clientId, string redirectUri, string codeChallenge, string state, string scopes)
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
                   + "&code_challenge=" + codeChallenge + "&state=" + state + "&scope=" + scopes);
        }
    }
}