using CsClient.Connection.Stomp;
using CsClient.Credentials;
using CsClient.Statistic;
using CsClient.Utils;
using Extend;
using NLog;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace CsClient.Connection.WebSocket
{
    /// <summary>
    /// Starts the WebSocket connection, actively listens to the subscribed channels and processes them.
    /// </summary>
    public class WebSocketConnectionTask
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly WebSocketConnection _connection;
        private readonly SystemStatistics _statistics;

        public WebSocketConnectionTask(WebSocketConnection webSocketConnection)
        {
            _connection = webSocketConnection;
            _statistics = new SystemStatistics();
        }

        /// <summary>
        /// Connects to the websocket asynchronously and recursively handles incoming subscripted 
        /// messages until close message is received.
        /// </summary>
        /// <param name="jwt">Json Web Token to connect with.</param>
        /// <param name="machineId">Machine id to provide the server with.</param>
        /// <returns>Async Task of connecting.</returns>
        public async Task Run(string jwt, string machineId)
        {
            logger.ConditionalTrace("Entering Connect Async Method");
            try
            {

                var t = _connection.State;
                await _connection.OpenConnectionAsync(jwt);
                await _connection.SubscribeToPingChannelAsync(machineId);

                logger.ConditionalTrace("Entering recursive check for WebSocket monitoring.");
                while (_connection.State == WebSocketState.Open)
                {
                    // Wait in background until a message is received.
                    var (result, content) = await _connection.ReceiveAsync();

                    // Exit before invoking.
                    var rs = result.MessageType;
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        switch (result.CloseStatus)
                        {
                            case WebSocketCloseStatus.NormalClosure:
                                break;// todo all other closure types. It crashes if server disconnects here for some reason.
                        }
                        break;
                    }


                    // Process message
                    ProcessSubscriptionMessageAsync(result, content);
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
                if (_connection.State == WebSocketState.Open)
                {
                    await _connection.CloseConnectionAsync();
                }
            }
        }

        /// <summary>
        /// Processes a Subscription message asynchronously.
        /// </summary>
        /// <param name="webSocketReceiveResult">The received result from the WebSocket <see cref="WebSocketReceiveResult"/>.</param>
        /// <param name="content">Body of the result.</param>
        /// <returns>Asynchronous task of processing the message.</returns>
        private async void ProcessSubscriptionMessageAsync(WebSocketReceiveResult webSocketReceiveResult, string content)
        {
            switch (webSocketReceiveResult.MessageType)
            {
                case WebSocketMessageType.Text:
                    logger.Debug("Identifying message");
                    if (content.Equals(Constants.EnergyPingMessage))
                    {
                        logger.Debug("Processing Energy Ping Message");
                        try
                        {
                            EnergyStatisticTask stat = new EnergyStatisticTask();
                            string samplePath = stat.NewSample();

                            EnergyStatisticsCsvProcessor csvProcessor = new EnergyStatisticsCsvProcessor(samplePath, new WindowsSIDAccountHelper());
                            string statistics = csvProcessor.ProcessCsv();

                            // Send message
                            await _connection.SendMessage(statistics, Constants.EnergyPublishEndpoint);
                        }
                        catch (NotSupportedException)
                        {
                            logger.Error("Did not send energy information - nothing to send.");
                        }
                        catch (FileNotFoundException)
                        {
                            logger.Error("Energy Statistic task exit successfully but no file in location was found. Does this program lack permissions?");
                        }
                    }
                    else if (content.Equals(Constants.UsagePingMessage))
                    {
                        logger.Debug("Processing Usage Ping Message");
                        DateTime timestamp = DateTime.Now;
                        Task<float> cpuTask = _statistics.GetCpuUsageAsync();
                        Task<float> memoryTask = _statistics.GetMemoryUsageAsync();
                        Task<float> diskTask = _statistics.GetDiskUsageAsync();
                        Task<float> networkTask = _statistics.GetNetworkUsageAsync();

                        // Wait for all tasks to complete
                        await Task.WhenAll(cpuTask, memoryTask, diskTask, networkTask);

                        // Generate the csv.
                        string usageCsv = UsageStatisticsCsvProcessor.GenerateCsv(cpuTask.Result, memoryTask.Result, diskTask.Result, networkTask.Result, timestamp);
                   
                        // Send Message
                        await _connection.SendMessage(usageCsv, Constants.UsagePublishEndpoint);
                    }
                    break;
                case WebSocketMessageType.Close:
                    logger.Info("Close Message Received");
                    break;
            }

        }
    }
}
