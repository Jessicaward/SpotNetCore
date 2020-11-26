using System;
using System.Net.Http;
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
        private readonly ILogger<AuthorisationService> _logger;

        public AuthorisationService(ILogger<AuthorisationService> logger)
        {
            _logger = logger;
        }

        public async Task<AuthorisationCodeDetails> Authorise()
        {
            var details = new AuthorisationCodeDetails
            {
                RedirectUri = "https://localhost:5001/"
            };
            details.AuthorisationUri = BuildAuthorisationUri("test", "test", details.CodeChallenge, "S256", "something something");
            
            await WebHost.CreateDefaultBuilder(null)
                .Configure(y =>
                {
                    y.UseRouting();
                    y.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/", async context =>
                        {
                            Console.WriteLine("endpoint received response");
                            await context.Response.WriteAsync("Hello world");
                        });
                    });
                }).Build().RunAsync();

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(details.AuthorisationUri);
                
            }
            
            return details;
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