using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SpotNetCore.Implementation
{
    internal class SpotifyCallbackListener : IDisposable
    {
        private readonly int _port;
        private readonly TcpListener _tcpListener;

        public SpotifyCallbackListener(int port)
        {
            if (port < 1 || port == 80)
            {
                throw new ArgumentOutOfRangeException(nameof(port),"Expected a valid port number, > 0, not 80");
            }

            _port = port;
            _tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), _port);
        }

        public async Task ListenToSingleRequestAndRespondAsync(Func<Uri, Task<string>> responseProducer, CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => _tcpListener.Stop());
            _tcpListener.Start();

            TcpClient tcpClient = null;
            try
            {
                tcpClient = await AcceptTcpClientAsync(cancellationToken)
                    .ConfigureAwait(false);

                await ExtractUriAndRespondAsync(tcpClient, responseProducer, cancellationToken).ConfigureAwait(false);

            }
            finally
            {
                tcpClient?.Close();
            }
        }
        
        private async Task<TcpClient> AcceptTcpClientAsync(CancellationToken token)
        {
            try
            {
                return await _tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);
            }
            catch (Exception ex) when (token.IsCancellationRequested)
            {
                throw new OperationCanceledException("Cancellation was requested while awaiting TCP client connection.", ex);
            }
        }

        private async Task ExtractUriAndRespondAsync(
            TcpClient tcpClient,
            Func<Uri, Task<string>> responseProducer,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var httpRequest = await GetTcpResponseAsync(tcpClient, cancellationToken).ConfigureAwait(false);
            var uri = ExtractUriFromHttpRequest(httpRequest);

            // write an "OK, please close the browser message" 
            await WriteResponseAsync(await responseProducer(uri), tcpClient.GetStream(), cancellationToken)
                .ConfigureAwait(false);
        }


        private Uri ExtractUriFromHttpRequest(string httpRequest)
        {
            const string regexp = @"GET \/\?(.*) HTTP";
            var r1 = new Regex(regexp);
            var match = r1.Match(httpRequest);
            if (!match.Success)
            {
                throw new InvalidOperationException("Not a GET query");
            }

            var getQuery = match.Groups[1].Value;
            var uriBuilder = new UriBuilder
            {
                Query = getQuery, 
                Port = _port
            };

            return uriBuilder.Uri;
        }

        private static async Task<string> GetTcpResponseAsync(TcpClient client, CancellationToken cancellationToken)
        {
            var networkStream = client.GetStream();

            var readBuffer = new byte[1024];
            var stringBuilder = new StringBuilder();

            // Incoming message may be larger than the buffer size. 
            do
            {
                var numberOfBytesRead = await networkStream.ReadAsync(readBuffer.AsMemory(0, readBuffer.Length), cancellationToken)
                    .ConfigureAwait(false);

                var s = Encoding.ASCII.GetString(readBuffer, 0, numberOfBytesRead);
                stringBuilder.Append(s);

            }
            while (networkStream.DataAvailable);

            return stringBuilder.ToString();
        }

        private async Task WriteResponseAsync(string message, Stream stream, CancellationToken cancellationToken)
        {
            var fullResponse = $"HTTP/1.1 200 OK\r\n\r\n{message}";
            var response = Encoding.ASCII.GetBytes(fullResponse);
            await stream.WriteAsync(response.AsMemory(0, response.Length), cancellationToken).ConfigureAwait(false);
            await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _tcpListener?.Stop();
        }
    }
}