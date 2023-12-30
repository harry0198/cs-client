using CsClient.Connection.Stomp;
using CsClient.Credentials;
using CsClient.Statistic;
using CsClient.Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace CsClient.Connection.WebSocket
{
    public class WebSocketConnectionTask
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly WebSocketConnection _connection;

        public WebSocketConnectionTask(WebSocketConnection webSocketConnection)
        {
            _connection = webSocketConnection;
        }
        /// <summary>
        /// Connects to the websocket asynchronously and recursively handles incoming subscripted 
        /// messages until close message is received.
        /// </summary>
        /// <param name="machineId">Machine id to provide the server with.</param>
        /// <returns>Async Task of connecting.</returns>
        public async Task Run(string jwt, string machineId)
        {
            // Connect to WebSocket and provide subscription message.
            await _connection.ConnectAsync(jwt, machineId, ProcessSubscriptionMessageAsync);
        }

        /// <summary>
        /// Processes a Subscription message asynchronously.
        /// </summary>
        /// <param name="webSocketReceiveResult">The received result from the WebSocket <see cref="WebSocketReceiveResult"/>.</param>
        /// <param name="content">Body of the result.</param>
        /// <returns>Asynchronous task of processing the message.</returns>
        public async void ProcessSubscriptionMessageAsync(WebSocketReceiveResult webSocketReceiveResult, string content)
        {
            switch (webSocketReceiveResult.MessageType)
            {
                case WebSocketMessageType.Text:
                    if (content.Equals(Constants.EnergyPingMessage))
                    {
                        EnergyStatisticTask stat = new EnergyStatisticTask();
                        string samplePath = stat.NewSample();

                        EnergyStatisticsCsvProcessor csvProcessor = new EnergyStatisticsCsvProcessor(samplePath, new WindowsSIDAccountHelper());
                        string statistics = csvProcessor.ProcessCsv();

                        // Send message
                        await _connection.SendMessage(statistics, Constants.EnergyPublishEndpoint);
                    }
                    break;
                case WebSocketMessageType.Close:
                    logger.Info("Close Message Received");
                    break;
            }

        }
    }
}
