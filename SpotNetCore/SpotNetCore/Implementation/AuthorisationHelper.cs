using System;
using System.Web;
using SpotNetCore.Models;

namespace SpotNetCore.Implementation
{
    public class AuthorisationHelper
    {
        public static string GetAuthorisationUrl()
        {
            var details = new AuthorisationCodeDetails
            {
                RedirectUri = "https://localhost:5001/"
            };
            
            details.AuthorisationUri = BuildAuthorisationUri("33bea7a309d24a08a71ff9c8f48be287", details.RedirectUri, details.CodeChallenge, "fh82hfosdf8h", "user-follow-modify");
            
            return details.AuthorisationUri;
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
            return "?client_id=" + clientId + "&response_type=code" 
                   + "&redirect_uri=" + redirectUri + "&code_challenge_method=S256"
                   + "&code_challenge=" + codeChallenge + "&state=" + state + "&scope=" + Uri.EscapeUriString(scopes);
        }
    }
}