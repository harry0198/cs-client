using System.Net.WebSockets;
using System.Threading.Tasks;

namespace CsClient.Connection
{
    public interface IWebSocket
    {
        Task ConnectAsync(string endpoint, string jwt);
        Task SendAsync(string message);
        Task<(WebSocketReceiveResult, string)> ReceiveAsync();
        Task CloseAsync();
        WebSocketState State { get; }
    }
}
