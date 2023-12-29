using System.DirectoryServices.AccountManagement;

namespace CsClient.Credentials
{
#nullable enable
    /// <summary>
    /// Contains signatures for helper functions within fetching the user account.
    /// </summary>
    public interface IAccountHelper
    {
        /// <summary>
        /// Checks if the account is a valid user account.
        /// Checks across both local and domain contexts.
        /// </summary>
        /// <param name="sid">Security Identifier to check.</param>
        /// <returns>True if is valid user account. If multiple accounts exist with same SID, returns false.</returns>
        AccountDetails? GetUserAccount(string sid);

        /// <summary>
        /// Gets the given account if is a valid user account. If the account type is not a user 
        /// e.g. group. This returns null. If there are multiple matches with the same SID, returns null.
        /// </summary>
        /// <param name="sid">Security Identifier to Check.</param>
        /// <param name="context">Context to check. E.g Local or Domain.</param>
        /// <returns>User account if valid user account. If not e.g. group. or, multiple accounts with same SID exist, Returns null</returns>
        AccountDetails? GetValidUserAccount(string sid, AccountType context);
    }
#nullable restore
}
