using CsClient.Connection.WebSocket;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace CsClientTests.Mocks
{
    public class MockIWebSocket : IWebSocket
    {
        public Stack<string> Message { get; set; } = new Stack<string>();
        public bool RejectOpenRequest { get; set; } = false;
        public WebSocketState State { get; set; }
        public bool PreventConnect { get; set; } = false;


        public Task CloseAsync()
        {
            State = WebSocketState.Closed;
            return Task.CompletedTask;
        }

        public Task ConnectAsync(string endpoint, string jwt)
        {
            // If the mock is set to reject the open request, finish here.
            if (RejectOpenRequest)
            {
                throw new WebSocketException();
            }

            State = WebSocketState.Open;
            return Task.CompletedTask;
        }

        public async Task<(WebSocketReceiveResult, string)> ReceiveAsync()
        {
            DateTime startTime = DateTime.UtcNow;
            
            while (State == WebSocketState.Open)
            {
                if ((DateTime.UtcNow - startTime).TotalSeconds >= 5)
                {
                    break; // Exit the loop
                }

                // If message is null, keep monitoring.
                if (Message.Count == 0) continue;
                int dataCount = Encoding.UTF8.GetByteCount(Message.Peek());
                return (new WebSocketReceiveResult(dataCount, WebSocketMessageType.Text, true), Message.Pop());
            }

            throw new TimeoutException("No messages received in timeframe");
        }

        public Task SendAsync(string message)
        {
            Message.Push(message);
            return Task.CompletedTask;
        }
    }
}
