using cs_client.DTO;
using cs_client.Utils;
using System;

namespace cs_client.Credentials
{
    /// <summary>
    /// An implementation of the <see cref="ICredentialService"/> interface for managing user credentials and JWT
    /// using a <see cref="ICredentialRepository"/>
    /// </summary>
    public class JWTCredentialManager : ICredentialService
    {
        private readonly ICredentialRepository _credentialRepository;

        /// <summary>
        /// Constructs a new <see cref="JWTCredentialManager"/> with the provided repository.
        /// </summary>
        /// <param name="credentialRepository">Repository to save and fetch credential data to / from.</param>
        public JWTCredentialManager(ICredentialRepository credentialRepository)
        {
            _credentialRepository = credentialRepository;
        }

        /// <summary>
        /// Gets or sets the value for the jwt token.
        /// </summary>
        public string Jwt {  get; set; }

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
            return this._credentialRepository.SaveCredential(userCredentials.Username, userCredentials.Password);
        }
    }
}
