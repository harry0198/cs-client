using cs_client.DTO;
using cs_client.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace cs_client.Credentials
{
    /// <summary>
    ///  Repository class for user credential information.
    ///  This class stores credentials using the data protection api (DPAPI).
    ///  Recommended to set the KeyStore entropy environment variable
    ///  for security purposes.
    /// </summary>
    /// <see cref="ICredentialRepository"/>
    public class DPAPICredentialRepository : ICredentialRepository
    {
        private readonly Utils.Environment _environment;
        private readonly ILogger _logger;
        public DPAPICredentialRepository(Utils.Environment environment)
        {
            this._environment = environment;

            ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            this._logger = factory.CreateLogger<DPAPICredentialRepository>();
        }
        public string GetCredential(string credentialKey)
        {
            string keyStoreFile = GetKeyStoreFile(credentialKey);
            using (FileStream fileStream = new FileStream(keyStoreFile, FileMode.OpenOrCreate))
            {
                // Decrypt data from file.
                byte[] entropy = GetEntropy();
                byte[] decryptedBytes = DecryptDataFromStream(entropy, DataProtectionScope.CurrentUser, fileStream);

                // Close Stream
                fileStream.Close();

                // Guard against null returns.
                if (decryptedBytes == null) return null;


                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }

        public bool SaveCredential(string credentialKey, string credentialValue)
        {
            byte[] toEncrypt = UnicodeEncoding.UTF8.GetBytes(credentialValue);
            string keyStoreFile = GetKeyStoreFile(credentialKey);
            FileStream fileStream = new FileStream(keyStoreFile, FileMode.OpenOrCreate);
            
            // Encrypt data to file.
            byte[] entropy = GetEntropy();
            EncryptDataToStream(toEncrypt, entropy, DataProtectionScope.CurrentUser, fileStream);

            // Close stream
            fileStream.Close();

            return true;
        }

        private string GetKeyStoreFile(string credentialKey)
        {
            return $"{Constants.KeyStoreFilePrefix}{credentialKey}{Constants.KeyStoreFileSuffix}";
        }

        private byte[] GetEntropy()
        {
            // Create empty byte array length 16 to hold random entropy.
            byte[] entropyArray = new byte[16];

            // Fetch entropy from the environment and get bytes.
            string entropy = this._environment.GetEntropy();
            byte[] entropyBytes = UnicodeEncoding.UTF8.GetBytes(entropy);

            // Copy the environment entropy bytes to the empty array and cap it at 16 bytes.
            Array.Copy(entropyBytes, 0, entropyArray, 0, 16);

            return entropyArray;
        }

        private byte[] DecryptDataFromStream(byte[] entropy, DataProtectionScope scope, Stream stream)
        {
            // Guard against stream issues.
            if (!stream.CanRead) return null;

            // Typically encrypted data is between 1-100 bytes so this is a good starting point for memory savings.
            byte[] buffer = new byte[25]; // Initial buffer size
            byte[] allBytes = null;

            // The length of the data in stream is of an unknown size. Here, we read in data chunks at a time and grow the buffer incrementally.
            using (MemoryStream memoryStream = new MemoryStream())
            {
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);
                }

                allBytes = memoryStream.ToArray();
            }

            if (allBytes == null) return null;

            try
            {
                return ProtectedData.Unprotect(allBytes, entropy, scope);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Exception occured while trying to unprotect a credential keystore.");
                return null;
            }
        }

        private static void EncryptDataToStream(byte[] buffer, byte[] entropy, DataProtectionScope scope, Stream stream)
        {
            byte[] encryptedData = ProtectedData.Protect(buffer, entropy, scope);

            if (stream.CanWrite && encryptedData != null)
            {
                stream.Write(encryptedData, 0, encryptedData.Length);
            }
        }
    }
}
