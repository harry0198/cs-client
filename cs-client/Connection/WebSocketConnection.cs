using CsClient.Connection.Service;
using CsClient.Connection.Stomp;
using CsClient.Credentials;
using CsClient.Data;
using CsClient.Data.DTO;
using CsClient.Statistic;
using CsClient.Utils;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CsClient.Connection
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
        /// <param name="authenticationService">Authentication service provided</param>
        /// <param name="environment"></param>
        /// <param name="webSocket"></param>
        public WebSocketConnection(Utils.Environment environment, IWebSocket webSocket) 
        {
            this._environment = environment;
            this._webSocket = webSocket;
            this._serializer = new StompMessageSerializer();
        }

        /// <summary>
        /// Connects to the websocket asynchronously and recursively handles incoming subscripted 
        /// messages until close message is received.
        /// </summary>
        /// <param name="jwt">Json Web Token to connect with.</param>
        /// <param name="machineId">Machine id to provide the server with.</param>
        /// <param name="handler">Delegate to handle the WebSocket Received.</param>
        /// <returns>Async Task of connecting.</returns>
        public async Task ConnectAsync(string jwt, string machineId, WebSocketMessageHandler handler)
        {
            logger.ConditionalTrace("Entering Connect Async Method");
            try
            {
                await OpenConnectionAsync(jwt);
                await SubscribeToPingChannelAsync(machineId);

                logger.ConditionalTrace("Entering recursive check for WebSocket monitoring.");
                while (_webSocket.State == WebSocketState.Open)
                {
                    // Wait in background until a message is received.
                    var (result, content) = await _webSocket.ReceiveAsync();

                    logger.Info("Received channel");

                    // Exit before invoking.
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }

                    StompMessage message = _serializer.Deserialize(content);
                    string body = message.Body;

                    // Invoke handler
                    handler.Invoke(result, body);
                }
            }
            catch (Exception ex)
            {
                logger.Fatal(ex.Message);
                logger.ForExceptionEvent(ex);
            }
            finally
            {
                // If the WebSocket is still open, close it.
                if (_webSocket.State == WebSocketState.Open)
                {
                    await _webSocket.CloseAsync();
                }
            }
        }
        
        /// <summary>
        /// Opens the connection to the websocket.
        /// </summary>
        /// <param name="jwt">Json Web Token to provide as authentication credentials.</param>
        /// <returns>Asynchronous Task of opening the connection.</returns>
        public async Task OpenConnectionAsync(string jwt)
        {
            // Initialize handshake with server.
            await _webSocket.ConnectAsync(_environment.GetWebSocketUrl(), jwt);

            // Connect via STOMP.
            await SendConnectMessageAsync();
        }

        public async Task SendConnectMessageAsync()
        {
            var connect = new StompMessage(StompCommand.Connect);
            connect["accept-version"] = "1.2";
            connect["host"] = "";
            await _webSocket.SendAsync(_serializer.Serialize(connect));
        }

        public async Task SubscribeToPingChannelAsync(string machineId)
        {
            var subscribe = new StompMessage(StompCommand.Subscribe);
            subscribe["id"] = machineId;
            subscribe["destination"] = Constants.PingEndpoint;
            await _webSocket.SendAsync(_serializer.Serialize(subscribe));
        }

        public async Task SendMessage(string message, string destination)
        {
            try
            {
                // Create stomp message.
                var publishStompMessage = new StompMessage(StompCommand.Send, message);
                publishStompMessage.SetPlainTextContentType();
                publishStompMessage["destination"] = destination;
                await _webSocket.SendAsync(_serializer.Serialize(publishStompMessage));
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                logger.ForExceptionEvent(e);
            }
        }

        public class Content
        {
            public string Subject { get; set; }
            public string Message { get; set; }
        }
    }
}
