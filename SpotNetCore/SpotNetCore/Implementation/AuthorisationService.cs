using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SpotNetCore.Models;

namespace SpotNetCore.Implementation
{
    public class AuthorisationService
    {
        public async Task<SpotifyAccessToken> Authorise()
        {
            var spotifyAccessToken = (SpotifyAccessToken)null;
            var details = new AuthorisationCodeDetails
            {
                RedirectUri = "https://localhost:5001/"
            };
            
            //todo: move this info into config
            details.AuthorisationUri = BuildAuthorisationUri("33bea7a309d24a08a71ff9c8f48be287", details.RedirectUri, details.CodeChallenge, "fh82hfosdf8h", "user-follow-modify");
            
            WebHost.CreateDefaultBuilder(null)
                .Configure(y =>
                {
                    y.UseRouting();
                    y.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/", async context =>
                        {
                            await context.Response.WriteAsync("");
                            Console.WriteLine("Endpoint hit");
                            spotifyAccessToken = new SpotifyAccessToken();
                        });
                    });
                }).Build().RunAsync();

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(details.AuthorisationUri);
                Console.WriteLine("HTTP Request sent");
            }

            while (spotifyAccessToken == null)
            {
                //todo: there has to be a better way of doing this?
                await Task.Delay(250);
            }
            
            return spotifyAccessToken;
        }

        private static string BuildAuthorisationUri(string clientId, string redirectUri, string codeChallenge, string state, string scopes)
        {
            return HttpUtility.HtmlEncode(new UriBuilder()
            {
                Scheme = "https",
                Host = "accounts.spotify.com",
                Path = "authorize",
                Query = BuildAuthorisationQuery(clientId, redirectUri, codeChallenge, state, scopes)
            }.Uri.ToString());
        }

        private static string BuildAuthorisationQuery(string clientId, string redirectUri, string codeChallenge, string state, string scopes)
        {
            return Uri.EscapeUriString("?client_id=" + clientId + "&response_type=code" 
                   + "&redirect_uri=" + redirectUri + "&code_challenge_method=S256"
                   + "&code_challenge=" + codeChallenge + "&state=" + state + "&scope=" + Uri.EscapeUriString(scopes));
        }
    }
}