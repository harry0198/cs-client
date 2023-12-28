namespace CsClient.Utils
{
    public static class Constants
    {
        /// <summary>
        /// Name of this application.
        /// </summary>
        public static readonly string ApplicationName = "Cs-Client";

        /// <summary>
        /// Prefix for the keystore file.
        /// </summary>
        public static readonly string KeyStoreFilePrefix = "CSCLIENT_";

        /// <summary>
        /// Suffix of the keystore file. .kst = keystore
        /// </summary>
        public static readonly string KeyStoreFileSuffix = ".kst";

        /// <summary>
        /// Environment variable to change the default entropy.
        /// </summary>
        public static readonly string EnvEntropy = "CSCLIENT_ENTROPY";

        /// <summary>
        /// Default entropy to encrypt credentials with.
        /// </summary>
        public static readonly string DefaultEntropy = "DefaultEntropy16";

        /// <summary>
        /// Key to store user username under.
        /// </summary>
        public static readonly string UsernameCredentialKey = "username";

        /// <summary>
        /// Key to store user password under.
        /// </summary>
        public static readonly string PasswordCredentialKey = "id";

        /// <summary>
        /// Environment variable to change the host with port.
        /// </summary>
        public static readonly string EnvHost = "CSCLIENT_HOST";

        /// <summary>
        /// Default host with port to connect with.
        /// </summary>
        public static readonly string DefaultHost = "localhost:8080";

        /// <summary>
        /// Default web socket url to connect with.
        /// </summary>
        public static readonly string DefaultWebSocketUrl = "ws://192.168.1.16:8080/handshake";

        /// <summary>
        /// Environment variable to change the web socket connection url.
        /// </summary>
        public static readonly string EnvWebSocketUrl = "CSCLIENT_WSURL";
    }
}
