using CsClient.Credentials;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CsClient.Utils;
using System.IO;
using CsClientTests.Mocks;

namespace CsClientTests.Credentials
{
    /// <summary>
    /// Tests the <see cref="DPAPICredentialRepository"/> class implementation.
    /// Contains tests for saving credentials, getting credentials and ensuring correct return types
    /// when the keystore is missing or invalid.
    /// </summary>
    [TestClass]
    public class DPAPICredentialRepositoryTests : BaseTest
    {
        /// <summary>
        /// Before each test, delete all file that start with the keystore file prefix.
        /// We want a fresh run and not to overwrite / read existing files.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            var currentDir = System.Environment.CurrentDirectory;

            foreach (string file in Directory.GetFiles(currentDir))
            {
                if (file.StartsWith(Constants.KeyStoreFilePrefix))
                {
                    File.Delete(file);
                }
            }
        }

        /// <summary>
        /// Tests the <see cref="DPAPICredentialRepository.SaveCredential(string, string)"/> function.
        /// Makes sure that the function does save to the expected file.
        /// </summary>
        [TestMethod]
        public void SaveCredentials_DoesSave()
        {
            // Arrange
            var environment = new CsClient.Utils.Environment();
            var wcm = new DPAPICredentialRepository(environment);
            var credentialKey = "username";

            var expectedCredentialValue = "m-01";
            var expectedFile = $"{Constants.KeyStoreFilePrefix}{credentialKey}{Constants.KeyStoreFileSuffix}";

            // Act
            wcm.SaveCredential(credentialKey, expectedCredentialValue);

            // Assert
            Assert.IsTrue(File.Exists(expectedFile), "The keystore file should exist.");
        }

        /// <summary>
        /// Tests the <see cref="DPAPICredentialRepository.GetCredential(string)"/> function
        /// can decrypt a keystore file successfully.
        /// </summary>
        [TestMethod]
        public void GetCredentials_DoesFetch()
        {
            // Arrange
            // Set the entropy to the one the test data uses (in case the default changes)
            var environment = new MockEnvironment();
            environment.Entropy = "DefaultEntropy16";

            var wcm = new DPAPICredentialRepository(environment);
            var credentialKey = "username";

            var expectedCredentialValue = "myUser";
            System.Environment.CurrentDirectory = this.TestDir;

            // Act
            string actualCredentialValue = wcm.GetCredential(credentialKey);

            // Assert
            Assert.AreEqual(expectedCredentialValue, actualCredentialValue);
        }

        /// <summary>
        /// Tests that when no file / credential with the specified key exists that
        /// the <see cref="DPAPICredentialRepository.GetCredential(string)"/> function
        /// returns null.
        /// </summary>
        [TestMethod]
        public void GetCredential_NoCredential_Null()
        {
            // Arrange
            // Set the entropy to the one the test data uses (in case the default changes)
            var wcm = new DPAPICredentialRepository(new Environment());
            var credentialKey = "invalid_credential_key";

            System.Environment.CurrentDirectory = this.TestDir;

            // Act
            string actualCredentialValue = wcm.GetCredential(credentialKey);

            // Assert
            Assert.IsNull(actualCredentialValue);
        }

        /// <summary>
        /// Tests the repository can save and fetch credentials.
        /// </summary>
        [TestMethod]
        public void Integration_Test()
        {
            // Arrange
            var environment = new Environment();
            var wcm = new DPAPICredentialRepository(environment);
            var credentialKey = "username";

            var expectedCredentialValue = "m-01";

            // Act
            wcm.SaveCredential(credentialKey, expectedCredentialValue);
            string actualCredentialValue = wcm.GetCredential(credentialKey);

            // Assert
            Assert.AreEqual(expectedCredentialValue, actualCredentialValue);
        }

        /// <summary>
        /// Integration test for the <see cref="DPAPICredentialRepository.SaveCredential(string, string)"/> function
        /// saves credentials into an encrypted format. This test uses <see cref="DPAPICredentialRepository.GetCredential(string)"/>
        /// but sets it up to use a different key. GetCredential should then return null.
        /// </summary>
        [TestMethod]
        public void Integration_SaveCredentials_IsProtected()
        {
            // Arrange
            var wcm = new DPAPICredentialRepository(new Environment());
            var credentialKey = "username";

            var expectedCredentialValue = "m-01";
            var expectedFile = $"{Constants.KeyStoreFilePrefix}{credentialKey}{Constants.KeyStoreFileSuffix}";
            wcm.SaveCredential(credentialKey, expectedCredentialValue);

            var alteredEnvironment = new MockEnvironment();
            alteredEnvironment.Entropy = "DIFFERENT-9876789UIHJI";
            var alteredWcm = new DPAPICredentialRepository(alteredEnvironment);

            // Act
            string actualCredential = alteredWcm.GetCredential(credentialKey);


            // Assert
            Assert.IsTrue(actualCredential == null, "Credential was not protected properly.");
        }
    }
}
