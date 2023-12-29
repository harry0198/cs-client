using CsClient.Credentials;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;

namespace CsClientTests.Credentials
{
    [TestClass]
    public class WindowsSIDAccountHelperTests
    {
        /// <summary>
        /// Tests the <see cref="WindowsSIDAccountHelper.IsValidUserAccount(SecurityIdentifier, out AccountType)"/> 
        /// with various SIDs and checks if the results are as expected.
        /// </summary>
        /// <param name="sid">The Security Identifier to test.</param>
        /// <param name="expectedIsUser">The expected result of whether it is a user account.</param>
        /// <param name="expectedAccountType">The expected account type.</param>
        [TestMethod]
        [DataRow("S-1-1-0", false, AccountType.UNKNOWN)] // Example SID for 'World'
        [DataRow("S-1-5-18", false, AccountType.UNKNOWN)]  // Example SID for 'Local System'
                                                        // Add more DataRows here for other test cases
        public void IsUserAccount_Parameterized(string sid, bool expectedIsUser, AccountType expectedAccountType)
        {
            // Arrange
            SecurityIdentifier securityIdentifier = new SecurityIdentifier(sid);

            // Act
            bool isUser = WindowsSIDAccountHelper.IsValidUserAccount(securityIdentifier, out AccountType accountType);

            // Assert
            Assert.AreEqual(expectedIsUser, isUser);
            Assert.AreEqual(expectedAccountType, accountType);
        }

        /// <summary>
        /// Tests the <see cref="WindowsSIDAccountHelper.IsValidUserAccount(SecurityIdentifier, out AccountType)"/>
        /// with the current account. This account will be a user and this test is invariable to the Local / Domain of
        /// the currently logged in user.
        /// </summary>
        [TestMethod]
        public void IsUserAccount_ThisAccountLocal()
        {
            // Arrange
            List<AccountType> expectedAccountType = new List<AccountType>() { AccountType.DOMAIN, AccountType.LOCAL };
            SecurityIdentifier sid = WindowsIdentity.GetCurrent().User;

            // Act
            bool isUser = WindowsSIDAccountHelper.IsValidUserAccount(sid, out AccountType accountType);

            // Assert
            Assert.IsTrue(isUser);
            Assert.IsTrue(expectedAccountType.Contains(accountType));
        }
    }
}
