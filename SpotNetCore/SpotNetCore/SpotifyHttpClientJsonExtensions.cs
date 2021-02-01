using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SpotNetCore
{
    public static class SpotifyHttpClientJsonExtensions
    {
        public static async Task<TValue> GetFromSpotifyJsonAsync<TValue>(this HttpClient client, string requestUri, JsonSerializerOptions options = default,
            CancellationToken cancellationToken = default)
        {
            using var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            response.EnsureSpotifySuccess();
            return await response.Content!.ReadFromJsonAsync<TValue>(options, cancellationToken).ConfigureAwait(false);
        }
    }
}