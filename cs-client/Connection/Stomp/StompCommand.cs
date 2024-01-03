namespace CsClient.Connection.Stomp
{
    /// <summary>
    /// Utility class containing the commands a STOMP
    /// message can send.
    /// Contains CONNECT, SUBSCRIBE and SEND.
    /// </summary>
    public static class StompCommand
    {
        /// <summary>
        /// Constant for connecting via Stomp Protocol.
        /// </summary>
        public const string Connect = "CONNECT";

        /// <summary>
        /// Constants for subscribing to an endpoint via Stomp Protocol.
        /// </summary>
        public const string Subscribe = "SUBSCRIBE";

        /// <summary>
        /// Constant for sending a message via Stomp Protocol.
        /// </summary>
        public const string Send = "SEND";
    }
}
