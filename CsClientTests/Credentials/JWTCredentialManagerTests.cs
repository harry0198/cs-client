using cs_client.Credentials;
using cs_client.DTO;
using cs_client.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CsClientTests.Credentials
{
    /// <summary>
    /// Tests for the <see cref="JWTCredentialManager"/> service business logic functions correctly.
    /// </summary>
    [TestClass]
    public class JWTCredentialManagerTests
    {
        /// <summary>
        /// Tests the <see cref="JWTCredentialManager.GetCredentials"/> function returns credentials if they exist
        /// in the repository.
        /// </summary>
        [TestMethod]
        public void GetCredentials_CredentialsExist_ReturnCredentials()
        {
            // Arrange
            string expectedUsername = "myUser";
            string expectedPwd = "password";
            ICredentialRepository repo = new MockMemoryCredentialRepository();
            JWTCredentialManager credManager = new JWTCredentialManager(repo);
            repo.SaveCredential(Constants.UsernameCredentialKey, expectedUsername);
            repo.SaveCredential(Constants.PasswordCredentialKey, expectedPwd);

            // Act
            UserCredentials creds = credManager.GetCredentials();

            // Assert
            Assert.IsNotNull(creds);
            Assert.AreEqual(expectedUsername, creds.Username);
            Assert.AreEqual(expectedPwd, creds.Password);
        }

        /// <summary>
        /// Tests that the <see cref="JWTCredentialManager.GetCredentials"/> function returns null if credentials are missing from the
        /// repository.
        /// </summary>
        [TestMethod]
        public void GetCredentials_MissingCredentials_ReturnsNull()
        {
            // Arrange
            ICredentialRepository repo = new MockMemoryCredentialRepository();
            JWTCredentialManager credManager = new JWTCredentialManager(repo);

            // Act
            UserCredentials creds = credManager.GetCredentials();

            // Assert
            Assert.IsNull(creds);
        }

        /// <summary>
        /// Tests that the <see cref="JWTCredentialManager.GetCredentials"/> function returns null if the username exists but the password
        /// is missing from the repository.
        /// </summary>
        [TestMethod]
        public void GetCredentials_UsernameExistsPasswordMissing_ReturnsNull()
        {
            // Arrange
            ICredentialRepository repo = new MockMemoryCredentialRepository();
            JWTCredentialManager credManager = new JWTCredentialManager(repo);
            repo.SaveCredential(Constants.UsernameCredentialKey, "user");

            // Act
            UserCredentials creds = credManager.GetCredentials();

            // Assert
            Assert.IsNull(creds);
        }


        /// <summary>
        /// Tests that the <see cref="JWTCredentialManager.GetCredentials"/> function returns null if the username is missing but the password
        /// exists in the repository.
        /// </summary>
        [TestMethod]
        public void GetCredentials_UsernameMissingPasswordExists_ReturnsNull()
        {
            // Arrange
            ICredentialRepository repo = new MockMemoryCredentialRepository();
            JWTCredentialManager credManager = new JWTCredentialManager(repo);
            repo.SaveCredential(Constants.PasswordCredentialKey, "pass");

            // Act
            UserCredentials creds = credManager.GetCredentials();

            // Assert
            Assert.IsNull(creds);
        }

        /// <summary>
        /// Tests that the <see cref="JWTCredentialManager.SaveCredentials(UserCredentials)"/> function posts to the repository
        /// correctly to save the data.
        /// </summary>
        [TestMethod]
        public void SaveCredentials_Saves()
        {
            // Arrange
            ICredentialRepository repo = new MockMemoryCredentialRepository();
            JWTCredentialManager credManager = new JWTCredentialManager(repo);
            string expectedUsername = "user";
            string expectedPassword = "pass";
            UserCredentials creds = new UserCredentials(expectedUsername, expectedPassword);

            // Act
            bool saved = credManager.SaveCredentials(creds);

            // Actual
            string actualUsername = repo.GetCredential(Constants.UsernameCredentialKey);
            string actualPassword = repo.GetCredential(Constants.PasswordCredentialKey);

            // Assert
            Assert.IsTrue(saved);
            Assert.AreEqual(expectedUsername, actualUsername);
            Assert.AreEqual(expectedPassword, actualPassword);
        }
    }
}
