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
        private WindowsSIDAccountHelper _helper;
        [TestInitialize] 
        public void Init() 
        {
            this._helper = new WindowsSIDAccountHelper();
        }
        /// <summary>
        /// Tests the <see cref="WindowsSIDAccountHelper.IsValidUserAccount(SecurityIdentifier, out AccountType)"/> 
        /// with various SIDs and checks if the results are as expected.
        /// </summary>
        /// <param name="sid">The Security Identifier to test.</param>
        /// <param name="expectedIsNull">The expected result of whether it is null or not.</param>
        [TestMethod]
        [DataRow("S-1-1-0", true)] // Example SID for 'World'
        [DataRow("S-1-5-18", true)]  // Example SID for 'Local System'
        public void GetUserAccount_Parameterized(string sid, bool expectedIsNull)
        {
            // Act
            AccountDetails accountDetails = _helper.GetUserAccount(sid);

            // Assert
            Assert.AreEqual(expectedIsNull, accountDetails == null);
        }

        /// <summary>
        /// Tests the <see cref="WindowsSIDAccountHelper.IsValidUserAccount(SecurityIdentifier, out AccountType)"/>
        /// with the current account. This account will be a user and this test is invariable to the Local / Domain of
        /// the currently logged in user.
        /// </summary>
        [TestMethod]
        public void GetUserAccount_ThisAccountLocal()
        {
            // Arrange
            List<AccountType> expectedAccountType = new List<AccountType>() { AccountType.DOMAIN, AccountType.LOCAL };
            SecurityIdentifier sid = WindowsIdentity.GetCurrent().User;

            // Act
            AccountDetails accountDetail = _helper.GetUserAccount(sid.Value);

            // Assert
            Assert.IsNotNull(accountDetail);
            Assert.IsTrue(expectedAccountType.Contains(accountDetail.AccountType));
        }
    }
}
