using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CsClient.Connection.WebSocket
{
    /// <summary>
    /// Implementation of a WebSocket with additional validations.
    /// </summary>
    public class WebSocketDecorator : IWebSocket
    {
        private readonly ClientWebSocket _webSocket;
        private const int DefaultBufferSize = 1024;
        private const int MaxReceiveFrameSize = 64 * 1024; // Is the max STOMp supported size in the server.
        private const int MaxSendFrameSize = 512 * 1024;

        /// <summary>
        /// Class constructor, initializes the websocket isntance.
        /// </summary>
        public WebSocketDecorator()
        {
            _webSocket = new ClientWebSocket();
        }

        /// <summary>
        /// Gets the current WebSocket State. E.g Connecting, OPEN, CLOSED etc.
        /// </summary>
        public WebSocketState State => _webSocket.State;

        /// <inheritdoc/>
        public async Task ConnectAsync(string endpoint, string jwt)
        {
            Uri uri = new Uri(endpoint);
            _webSocket.Options.SetRequestHeader("Authorization", $"Bearer {jwt}");

            await _webSocket.ConnectAsync(uri, CancellationToken.None);
        }

        /// <summary>
        /// Fetches results from the WebSocket.
        /// </summary>
        /// <returns><see cref="WebSocketReceiveResult"/> of message.</returns>
        /// <exception cref="WebSocketException">If the received message exceeds the bounds supported.</exception>
        public async Task<(WebSocketReceiveResult, string)> ReceiveAsync()
        {
            // Need to allocate memory for received bytes.
            int bufferSize = DefaultBufferSize;
            byte[] buffer = new byte[bufferSize];
            int offset = 0;

            // Receive bytes.
            WebSocketReceiveResult result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            // Keep reading data from websocket until end of the message is received.
            while (!result.EndOfMessage)
            {
                // Iteratively increase the buffer size.
                int newSize = buffer.Length + bufferSize;

                // Assert maximum is not exceeded.
                if (newSize > MaxReceiveFrameSize)
                {
                    throw new WebSocketException($"Maximum frame size {newSize} bytes exceeds maximum {MaxReceiveFrameSize} bytes");
                }

                byte[] newBuffer = new byte[newSize];

                // Copy old buffer & data into new buffer
                Array.Copy(buffer, 0, newBuffer, 0, offset);
                buffer = newBuffer;

                // Update the free bytes in buffer to give websocket receive func.
                int free = buffer.Length - offset;

                // Read next bytes with offset.
                result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer, offset, free), CancellationToken.None);
            }

            string content = Encoding.UTF8.GetString(buffer, 0, result.Count);
            return (result, content);
        }

        /// <summary>
        /// Sends an asynchronous message to the WebSocket.
        /// </summary>
        /// <param name="message">Message to send.</param>
        /// <returns>Task on completion.</returns>
        /// <exception cref="WebSocketException">If the message size exceeds maximum bounds.</exception>

        public async Task SendAsync(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            ArraySegment<byte> buffer = new ArraySegment<byte>(messageBytes);
            await _webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        /// <inheritdoc/>
        public async Task CloseAsync()
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnecting", CancellationToken.None);
        }
    }
}
