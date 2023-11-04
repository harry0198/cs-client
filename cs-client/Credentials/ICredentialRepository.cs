using cs_client.DTO;
using System;

namespace cs_client.Credentials
{
    /// <summary>
    /// Interface for handling the access portion to credentials.
    /// Contains signatures to save and get credentials from a provider.
    /// </summary>
    public interface ICredentialRepository
    {

        /// <summary>
        /// Save credentials for retrieval later.
        /// </summary>
        /// <param name="credentialKey">Key to store credential value under.</param>
        /// <param name="credentialValue">Value to store.</param>
        /// <returns>True if credentials were successfully stored. False otherwise.</returns>
        Boolean SaveCredential(string credentialKey, string credentialValue);

        /// <summary>
        /// Retrieves the user credentials for this app.
        /// </summary>
        /// <returns>Credential value if exists.</returns>
        string GetCredential(string credentialKey);
    }
}
