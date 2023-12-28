namespace CsClient.Connection
{
    /// <summary>
    /// Constants file for the connection package.
    /// Includes constants for websocket endpoints
    /// </summary>
    public class ConnectionConstants
    {
        /// <summary>
        /// Endpoint for the reporting request channel from the server.
        /// This endpoint will receive the commands to send data to the server.
        /// </summary>
        public const string ReportingRequestChannel = "/channel/request";

        /// <summary>
        /// Command to send to a websocket to begin a SUBSCRIBE.
        /// </summary>
        public const string SubscribeCommand = "SUBSCRIBE";
    }
}
