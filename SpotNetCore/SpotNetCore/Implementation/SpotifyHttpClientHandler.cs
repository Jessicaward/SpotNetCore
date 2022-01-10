using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using SpotNetCore.Models;

namespace SpotNetCore.Implementation
{
    public class SpotifyHttpClientHandler : DelegatingHandler
    {
        private readonly AuthenticationManager _authenticationManager;

        private readonly AppSettings _appSettings;
        public SpotifyHttpClientHandler(AuthenticationManager authenticationManager, AppSettings appSettings)
        {
            _authenticationManager = authenticationManager;
            _appSettings = appSettings;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!_authenticationManager.IsTokenAboutToExpire())
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(_authenticationManager.Token.TokenType, _authenticationManager.Token.AccessToken);
            }

            var uriBuilder = new UriBuilder(request.RequestUri!);
            var query = QueryHelpers.AddQueryString(request.RequestUri.Query, "market", _appSettings.Market?.ToUpper());
            uriBuilder.Query = query;
            
            request.RequestUri = uriBuilder.Uri;
            return await base.SendAsync(request, cancellationToken);
        }
    }
}