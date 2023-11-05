using cs_client.DTO;
using cs_client.Utils;
using System;

namespace cs_client.Credentials
{
    /// <summary>
    /// An implementation of the <see cref="ICredentialService"/> interface for managing user credentials and JWT
    /// using a <see cref="ICredentialRepository"/>
    /// </summary>
    public class CredentialManager : ICredentialService
    {
        private readonly ICredentialRepository _credentialRepository;

        /// <summary>
        /// Constructs a new <see cref="CredentialManager"/> with the provided repository.
        /// </summary>
        /// <param name="credentialRepository">Repository to save and fetch credential data to / from.</param>
        public CredentialManager(ICredentialRepository credentialRepository)
        {
            _credentialRepository = credentialRepository;
        }

        /// <inheritdoc />
        public UserCredentials GetCredentials()
        {
            string username = _credentialRepository.GetCredential(Constants.UsernameCredentialKey);
            string password = _credentialRepository.GetCredential(Constants.PasswordCredentialKey);

            return username != null && password != null ?
                new UserCredentials(username, password) : null;
        }

        /// <inheritdoc />
        public bool SaveCredentials(UserCredentials userCredentials)
        {
            bool userSaved = this._credentialRepository.SaveCredential(Constants.UsernameCredentialKey, userCredentials.Username);
            bool passSaved = this._credentialRepository.SaveCredential(Constants.PasswordCredentialKey, userCredentials.Password);

            return userSaved && passSaved;
        }
    }
}
