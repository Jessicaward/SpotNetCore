using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using SpotNetCore.Models;

namespace SpotNetCore.Implementation
{
    public class AuthenticationManager : IDisposable
    {
        public bool IsAuthenticated { get; private set; }
        public SpotifyAccessToken Token => _appSettings?.SpotifyTokens;

        private readonly AppSettings _appSettings;
        private static string _codeVerifier;
        private readonly HttpClient _httpClient;

        public AuthenticationManager(AppSettings settingsConfig)
        {
            _appSettings = settingsConfig;
            _codeVerifier = AuthorisationCodeDetails.CreateCodeVerifier();
            _httpClient = new HttpClient();
        }

        public bool IsTokenAboutToExpire() => _appSettings.SpotifyTokens.ExpiresAt <= DateTime.Now.AddSeconds(20);
        
        private async Task<bool> ListenForCallbackAndGetResult(CancellationToken token)
        {
            var success = false;
            using var listener = new SpotifyCallbackListener(5000);
            await listener.ListenToSingleRequestAndRespondAsync(async uri =>
            {
                var query = HttpUtility.ParseQueryString(HttpUtility.HtmlDecode(uri.Query));
                var response = await _httpClient.PostAsync("https://accounts.spotify.com/api/token",
                    new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        {"code", query["code"]},
                        {"client_id", _appSettings.ClientId},
                        {"grant_type", "authorization_code"},
                        {"redirect_uri", "http://localhost:5000/"},
                        {"code_verifier", _codeVerifier}
                    }), token);

                if (!response.IsSuccessStatusCode)
                {
                    return string.Format(AppConstants.ErrorHtml, response.StatusCode, await response.Content.ReadAsStringAsync(token));
                }
                _appSettings.SpotifyTokens = JsonSerializer.Deserialize<SpotifyAccessToken>(await response.Content.ReadAsStringAsync(token));
                
                _appSettings.SpotifyTokens!.ExpiresAt = DateTime.Now.AddSeconds(_appSettings.SpotifyTokens.ExpiresInSeconds);
                IsAuthenticated = true;
                success = true;
                return AppConstants.SuccessHtml;
            }, token);
            return success;
        }
        
        private async Task<bool> GetAuthToken()
        {
            using var listener = new SpotifyCallbackListener(5000);
            var cts = new CancellationTokenSource();
            var result = await ListenForCallbackAndGetResult(cts.Token);
            cts.Cancel();
            return result;
        }

        public async Task<bool> RequestRefreshedAccessToken()
        {
            var response = await _httpClient.PostAsync("https://accounts.spotify.com/api/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"grant_type", "refresh_token"},
                    {"refresh_token", _appSettings.SpotifyTokens.RefreshToken},
                    {"client_id", _appSettings.ClientId}
                }));

            if (!response.IsSuccessStatusCode)
                return false;

            _appSettings.SpotifyTokens = JsonSerializer.Deserialize<SpotifyAccessToken>(
                await response.Content.ReadAsStringAsync());

            if (_appSettings.SpotifyTokens != null)
            {
                _appSettings.SpotifyTokens.ExpiresAt = DateTime.Now.AddSeconds(_appSettings.SpotifyTokens.ExpiresInSeconds);
            }

            IsAuthenticated = true;
            return true;
        }
        
        public async Task Authenticate()
        {
            async Task RequestNewSession()
            {
                Terminal.WriteLine("Please authorise this application to use Spotify on your behalf");
                SpotifyUrlHelper.RunUrl(GetAuthorisationUrl(_codeVerifier));
                if (!await GetAuthToken())
                    throw new NotAuthenticatedException();
            }
            
            if (_appSettings.SpotifyTokens?.ExpiresAt != null)
            {
                if (_appSettings.SpotifyTokens?.ExpiresAt < DateTime.Now)
                {
                    if (!await RequestRefreshedAccessToken())
                    {
                        await RequestNewSession();
                    }
                }
                else if (await AreCachedCredentialsStillValid())
                {
                    IsAuthenticated = true;
                }
            }
            else
            {
                RequestNewSession();
            }
        }

        private async Task<bool> AreCachedCredentialsStillValid()
        {
            using var httpClient = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Authorization = new("Bearer", _appSettings.SpotifyTokens.AccessToken)
                }
            };
            var responseMessage = await httpClient.GetAsync("https://api.spotify.com/v1/me");
            return responseMessage.IsSuccessStatusCode;
        }

        private string GetAuthorisationUrl(string codeVerifier)
        {
            var details = new AuthorisationCodeDetails(codeVerifier, "http://localhost:5000/");
            var scopes = _appSettings.RequiredScopes;
            details.AuthorisationUri = BuildAuthorisationUri(
                _appSettings.ClientId,
                details.RedirectUri,
                details.CodeChallenge,
                "fh82hfosdf8h",
                string.Join("%20", scopes)
            );

            return details.AuthorisationUri;
        }

        private string BuildAuthorisationUri(string clientId, string redirectUri, string codeChallenge, string state, string scopes)
        {
            return new UriBuilder
            {
                Scheme = "https",
                Host = "accounts.spotify.com",
                Path = "authorize",
                Query = BuildAuthorisationQuery(clientId, redirectUri, codeChallenge, state, scopes)
            }.Uri.ToString();
        }

        private static string BuildAuthorisationQuery(string clientId, string redirectUri, string codeChallenge, string state, string scopes) =>
            $"?client_id={clientId}&response_type=code&redirect_uri={redirectUri}&code_challenge_method=S256&code_challenge={codeChallenge}&state={state}&scope={Uri.EscapeUriString(scopes)}";

        public void Dispose()
        {
            _httpClient?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}