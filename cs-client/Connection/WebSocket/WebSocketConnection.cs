using CsClient.Connection.Service;
using CsClient.Connection.Stomp;
using CsClient.Credentials;
using CsClient.Data;
using CsClient.Data.DTO;
using CsClient.Statistic;
using CsClient.Utils;
using NLog;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace CsClient.Connection.WebSocket
{
    public class WebSocketConnection
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly Utils.Environment _environment;
        private readonly IWebSocket _webSocket;
        private readonly StompMessageSerializer _serializer;

        /// <summary>
        /// Initializes a WebSocketConnection.
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="webSocket"></param>
        public WebSocketConnection(Utils.Environment environment, IWebSocket webSocket)
        {
            _environment = environment;
            _webSocket = webSocket;
            _serializer = new StompMessageSerializer();
        }

        /// <summary>
        /// Gets the current state of the WebSocket.
        /// </summary>
        public virtual WebSocketState State
        {
            get
            {
                return _webSocket.State;
            }
        }

        /// <summary>
        /// Receives async
        /// </summary>
        /// <returns>Task containing <see cref="WebSocketReceiveResult"/> and content body.</returns>
        public virtual async Task<(WebSocketReceiveResult, string)> ReceiveAsync()
        {
            logger.Info("Receive async channel monitor call.");
            var (result, content) = await _webSocket.ReceiveAsync();

            // Extract body from message before sending.
            StompMessage message = _serializer.Deserialize(content);
            string body = message.Body;

            return (result, body);
        }

        /// <summary>
        /// Closes the connection async.
        /// </summary>
        /// <returns>Task.</returns>
        public virtual async Task CloseConnectionAsync()
        {
            await _webSocket.CloseAsync();
        }


        /// <summary>
        /// Opens the connection to the websocket.
        /// </summary>
        /// <param name="jwt">Json Web Token to provide as authentication credentials.</param>
        /// <returns>Asynchronous Task of opening the connection.</returns>
        public virtual async Task OpenConnectionAsync(string jwt)
        {
            // Initialize handshake with server.
            await _webSocket.ConnectAsync(_environment.GetWebSocketUrl(), jwt);

            // Connect via STOMP.
            await SendConnectMessageAsync();
        }

        /// <summary>
        /// Sends the connect stomp message to the websocket.
        /// </summary>
        /// <returns>Task.</returns>
        public virtual async Task SendConnectMessageAsync()
        {
            var connect = new StompMessage(StompCommand.Connect);
            await _webSocket.SendAsync(_serializer.Serialize(connect));
        }

        /// <summary>
        /// Subscribes to the ping channel with the given machineId as connection.
        /// </summary>
        /// <param name="machineId">Id for websocket. This should be same as auth.</param>
        /// <returns>Task.</returns>
        /// <exception cref="ArgumentException">If machine id is null or empty.</exception>
        public virtual async Task SubscribeToPingChannelAsync(string machineId)
        {
            if (machineId == null || machineId.Length == 0)
            {
                throw new ArgumentException("MachineId cannot be empty or null");
            }

            var subscribe = new StompMessage(StompCommand.Subscribe);
            subscribe.SetId(machineId);
            subscribe.SetDestination(Constants.PingEndpoint);
            await _webSocket.SendAsync(_serializer.Serialize(subscribe));
        }

        /// <summary>
        /// Sends a message to the websocket at the given destination.
        /// </summary>
        /// <param name="message">Message to send.</param>
        /// <param name="destination">Destination for the message.</param>
        /// <returns>Task.</returns>
        /// <exception cref="ArgumentException">If either message or destination are null. Or if destination is empty.</exception>
        public virtual async Task SendMessage(string message, string destination)
        {
            if (message == null || destination == null || destination.Length == 0)
            {
                throw new ArgumentException("Parameters must not be null and destination must have a length.");
            }

            // Create stomp message.
            var publishStompMessage = new StompMessage(StompCommand.Send, message);
            publishStompMessage.SetPlainTextContentType();
            publishStompMessage.SetDestination(destination);
            await _webSocket.SendAsync(_serializer.Serialize(publishStompMessage));
        }

        public class Content
        {
            public string Subject { get; set; }
            public string Message { get; set; }
        }
    }
}
