using CsClient.Data.DTO;
using System;

namespace CsClient.Credentials
{
    /// <summary>
    /// Interface representing a service for managing user credentials (login) and JWT.
    /// </summary>
    public interface ICredentialService
    {
        /// <summary>
        /// Retrieves user credentials consisting of a username and password.
        /// </summary>
        /// <returns>UserCredentials if available, or null.</returns>
        UserCredentials GetCredentials();

        /// <summary>
        /// Saves user credentials.
        /// </summary>
        /// <param name="userCredential">The user credentials to be saved.</param>
        /// <returns>True if the credentials were successfully saves, false otherwise.</returns>
        bool SaveCredentials(UserCredentials userCredential);
    }
}
