using CsClient.Credentials;
using System.Collections.Generic;

namespace CsClientTests.Mocks
{
    /// <summary>
    /// Provides an in-memory credential repository for testing purposes.
    /// <see cref="ICredentialRepository"/>
    /// </summary>
    public class MockMemoryCredentialRepository : ICredentialRepository
    {
        private readonly Dictionary<string, string> _memoryKeyStore = new Dictionary<string, string>();

        /// <inheritdoc />
        public string GetCredential(string credentialKey)
        {
            return _memoryKeyStore.TryGetValue(credentialKey, out var cred) ? cred : null;
        }


        /// <inheritdoc />
        public bool SaveCredential(string credentialKey, string credentialValue)
        {
            _memoryKeyStore.Add(credentialKey, credentialValue);
            return true;
        }
    }
}
