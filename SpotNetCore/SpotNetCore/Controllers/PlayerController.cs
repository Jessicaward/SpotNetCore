using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AuthenticationManager = SpotNetCore.Implementation.AuthenticationManager;

namespace SpotNetCore.Controllers
{
    public class PlayerController
    {
        public async Task<bool> PlayCurrentTrack()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(AuthenticationManager.Token.TokenType,
                        AuthenticationManager.Token.AccessToken);

                return (await httpClient.PutAsync("https://api.spotify.com/v1/me/player/play", null)).StatusCode == HttpStatusCode.NoContent;
            }
        }

        public async Task<bool> PauseCurrentTrack()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(AuthenticationManager.Token.TokenType,
                        AuthenticationManager.Token.AccessToken);
                return (await httpClient.PutAsync("https://api.spotify.com/v1/me/player/pause", null)).StatusCode == HttpStatusCode.NoContent;
            }
        }
    }
}