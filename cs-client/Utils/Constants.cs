namespace CsClient.Utils
{
    /// <summary>
    /// Constants for this service.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The ping message from the server to send usage stats.
        /// </summary>
        public const string UsagePingMessage = "REQUEST-USAGE";
        /// <summary>
        /// The ping message from the server to send energy stats.
        /// </summary>
        public const string EnergyPingMessage = "REQUEST-ENERGY";
        /// <summary>
        /// The websocket endpoint to publish usage statistics to.
        /// </summary>
        public const string UsagePublishEndpoint = "/publish/usage";
        /// <summary>
        /// The websocket endpoint to publish energy statistics to.
        /// </summary>
        public const string EnergyPublishEndpoint = "/publish/energy";
        /// <summary>
        /// The websocket subscription endpoint that pings the client (this app).
        /// </summary>
        public const string PingEndpoint = "/channel/request";
        /// <summary>
        /// Name of this application.
        /// </summary>
        public const string ApplicationName = "Cs-Client";

        /// <summary>
        /// Prefix for the keystore file.
        /// </summary>
        public const string KeyStoreFilePrefix = "CSCLIENT_";

        /// <summary>
        /// Suffix of the keystore file. .kst = keystore
        /// </summary>
        public const string KeyStoreFileSuffix = ".kst";

        /// <summary>
        /// Environment variable to change the default entropy.
        /// </summary>
        public const string EnvEntropy = "CSCLIENT_ENTROPY";

        /// <summary>
        /// Default entropy to encrypt credentials with.
        /// </summary>
        public const string DefaultEntropy = "DefaultEntropy16";

        /// <summary>
        /// Key to store user username under.
        /// </summary>
        public const string UsernameCredentialKey = "username";

        /// <summary>
        /// Key to store user password under.
        /// </summary>
        public const string PasswordCredentialKey = "id";

        /// <summary>
        /// Environment variable to change the host with port.
        /// </summary>
        public const string EnvHost = "CSCLIENT_HOST";

        /// <summary>
        /// Default host with port to connect with.
        /// </summary>
        public const string DefaultHost = "192.168.1.16:8080";

        /// <summary>
        /// Default web socket url to connect with.
        /// </summary>
        public const string DefaultWebSocketUrl = "ws://192.168.1.16:8080/handshake";

        /// <summary>
        /// Environment variable to change the web socket connection url.
        /// </summary>
        public const string EnvWebSocketUrl = "CSCLIENT_WSURL";
    }
}
