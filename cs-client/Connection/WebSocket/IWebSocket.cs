using System.Net.WebSockets;
using System.Threading.Tasks;

namespace CsClient.Connection.WebSocket
{
    public interface IWebSocket
    {
        /// <summary>
        /// Connects asynchronously to the websocket.
        /// </summary>
        /// <param name="endpoint">URL to connectto.</param>
        /// <param name="jwt">Json web token to authorize with.</param>
        /// <returns>Task of connecting.</returns>
        /// <exception cref="WebSocketException">If the connection could not be made.</exception>
        Task ConnectAsync(string endpoint, string jwt);
        Task SendAsync(string message);
        Task<(WebSocketReceiveResult, string)> ReceiveAsync();
        Task CloseAsync();
        WebSocketState State { get; }
    }
}
