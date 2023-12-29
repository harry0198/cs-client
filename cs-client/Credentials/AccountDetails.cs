namespace CsClient.Credentials
{
    /// <summary>
    /// Record for the windows account with details.
    /// </summary>
    /// <param name="sid">Security identifier.</param>
    /// <param name="accountType">Account type.</param>
    /// <param name="accountName">Account name.</param>
    public record AccountDetails(string SID, AccountType AccountType, string AccountName);
}
