namespace CsClient.Utils
{
    /// <summary>
    /// Class containing functions which require an environment context. E.g. Environment variables.
    /// </summary>
    public class Environment
    {
        /// <summary>
        /// Gets the entropy from the environment variable <see cref="Constants.EnvEntropy"/> or the default at <see cref="Constants.DefaultEntropy"/>.
        /// </summary>
        /// <returns>The entropy from the environment variable or default.</returns>
        public virtual string GetEntropy()
        {
            string entropy = System.Environment.GetEnvironmentVariable(Constants.EnvEntropy);
            return entropy == null || entropy.Length < 16 ? Constants.DefaultEntropy : entropy;
        }

        /// <summary>
        /// Gets the host from the environment variable <see cref="Constants.EnvHost"/> or the default at <see cref="Constants.DefaultHost"/>.
        /// </summary>
        /// <returns>The hostname and port from the environment variable or default./></returns>
        public virtual string GetHost()
        {
            string host = System.Environment.GetEnvironmentVariable(Constants.EnvHost);
            return host ?? Constants.DefaultHost;
        }

        /// <summary>
        /// Gets the web socket url from the environment variable <see cref="Constants.EnvWebSocketUrl"/> or the default at <see cref="Constants.DefaultWebSocketUrl"/>.
        /// </summary>
        /// <returns>The web socket url from the environment variable or default.</returns>
        public virtual string GetWebSocketUrl()
        {
            string wsurl = System.Environment.GetEnvironmentVariable(Constants.EnvWebSocketUrl);
            return wsurl ?? Constants.DefaultWebSocketUrl;
        }

        /// <summary>
        /// Fetches the computer machine name that this is running on.
        /// </summary>
        /// <returns></returns>
        public virtual string GetMachineName()
        {
            return System.Environment.MachineName;
        }
    }
}
