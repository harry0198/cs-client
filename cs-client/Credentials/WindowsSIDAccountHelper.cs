using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using NLog;

namespace CsClient.Credentials
{
    /// <summary>
    /// Contains helper functions for handling SID (security identifiers).
    /// </summary>
    public class WindowsSIDAccountHelper
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Checks if the account is a valid user account.
        /// Checks across both local and domain contexts.
        /// </summary>
        /// <param name="sid">Security Identifier to check.</param>
        /// <param name="accountType">output AccountType of user. Ie. Local, Domain etc.</param>
        /// <returns>True if is valid user account. If multiple accounts exist with same SID, returns false.</returns>
        public static bool IsValidUserAccount(SecurityIdentifier sid, out AccountType accountType)
        {
            logger.Info("Starting SID user account check");

            logger.Info("Checking for local account validity.");
            if (IsValidUserAccount(sid, ContextType.Machine))
            {
                accountType = AccountType.LOCAL;
                return true;
            }

            logger.Info("Checking for domain account validity.");
            if (IsValidUserAccount(sid, ContextType.Domain))
            {
                accountType = AccountType.DOMAIN;
                return true;
            }

            logger.Info("Account was not valid.");
            accountType = AccountType.UNKNOWN;
            return false;

        }

        /// <summary>
        /// Checks if the given account is a valid user account. If the account type is not a user 
        /// e.g. group. This returns false. If there are multiple matches with the same SID, returns false.
        /// </summary>
        /// <param name="sid">Security Identifier to Check.</param>
        /// <param name="context">Context to check. E.g Local or Domain.</param>
        /// <param name="accountName">User's account name or null.</param>
        /// <returns></returns>
        public static bool IsValidUserAccount(SecurityIdentifier sid, ContextType context, out string accountName)
        {
            try
            {
                using (PrincipalContext principalContext = new PrincipalContext(context))
                {
                    logger.Info("Searching for user account via SID: [" + sid.Value + " ] with context type: [" + context.ToString() + "]");

                    Principal principal = Principal.FindByIdentity(principalContext, IdentityType.Sid, sid.Value);


                    // If account type is a user and not a group or other.
                    if (principal != null && principal is UserPrincipal)
                    {
                        logger.Info("Found user");
                        accountName = principal.SamAccountName;
                        return true;
                    }


                    logger.Info("No user account found");
                    accountName = null;
                    return false;
                }
            }
            catch (MultipleMatchesException mme)
            {
                logger.Error("More than one user with this SID exists. Cannot check account. SID: " + sid.Value);
                logger.Error(mme.Message);
                accountName = null;
                return false;
            }
            catch (PrincipalServerDownException psde)
            {
                logger.Warn("Cannot connect to the domain server. Ignore if this machine is not part of a domain network.");
                logger.Warn(psde.Message);
                accountName = null;
                return false;
            }
        }
    }
}
