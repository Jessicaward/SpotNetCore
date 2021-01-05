using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AuthenticationManager = SpotNetCore.Implementation.AuthenticationManager;

namespace SpotNetCore.Controllers
{
    public class PlayController
    {
        public async Task<bool> PlayCurrentTrack()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(AuthenticationManager.Token.TokenType,
                        AuthenticationManager.Token.AccessToken);
                
                var response = await httpClient.PutAsync("https://api.spotify.com/v1/me/player/play", null);
                                    
                response.EnsureSuccessStatusCode();

                return response.StatusCode == HttpStatusCode.NoContent;
            }
        }
    }
}