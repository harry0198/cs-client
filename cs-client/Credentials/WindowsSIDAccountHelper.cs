using System.Security.Principal;
using System.DirectoryServices.AccountManagement;
using NLog;

namespace CsClient.Credentials
{
#nullable enable
    /// <summary>
    /// Contains helper functions for handling SID (security identifiers).
    /// </summary>
    public class WindowsSIDAccountHelper : IAccountHelper
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <inheritdoc/>
        public AccountDetails? GetUserAccount(string sid)
        {
            SecurityIdentifier securityIdentifier = new SecurityIdentifier(sid);
            return GetUserAccount(securityIdentifier);
        }
        
        /// <inheritdoc/>
        public AccountDetails? GetValidUserAccount(string sid, AccountType context)
        {
            SecurityIdentifier identifier = new SecurityIdentifier(sid);
            ContextType contextType;
            switch (context)
            {
                case AccountType.LOCAL:
                    contextType = ContextType.Machine;
                    break;
                case AccountType.DOMAIN:
                    contextType = ContextType.Domain;
                    break;
                default:
                    return null;
            }

            return GetValidUserAccount(identifier, contextType);
        }

        private AccountDetails? GetUserAccount(SecurityIdentifier sid)
        {
            logger.Info("Starting SID user account check");

            logger.Info("Checking for local account validity.");
            AccountDetails? localAccountDetails = GetValidUserAccount(sid, ContextType.Machine);
            if (localAccountDetails != null)
            {
                return localAccountDetails;
            }

            logger.Info("Checking for domain account validity.");
            AccountDetails? domainAccountDetails = GetValidUserAccount(sid, ContextType.Domain);
            if (domainAccountDetails != null)
            {
                return domainAccountDetails;
            }

            logger.Info("Account was not valid.");
            return null;

        }

        private AccountDetails? GetValidUserAccount(SecurityIdentifier sid, ContextType context)
        {
            try
            {
                using (PrincipalContext principalContext = new PrincipalContext(context))
                {
                    logger.Info("Searching for user account via SID: [" + sid.Value + " ] with context type: [" + context.ToString() + "]");

                    Principal principal = Principal.FindByIdentity(principalContext, IdentityType.Sid, sid.Value);


                    // If account type exists.
                    if (principal != null)
                    {
                        logger.Info("Found user");
                        AccountType accountType;
                        switch (context)
                        {
                            case ContextType.Machine:
                                accountType = AccountType.LOCAL;
                                break;
                            case ContextType.Domain:
                                accountType = AccountType.DOMAIN;
                                break;
                            default:
                                accountType = AccountType.UNKNOWN;
                                break;
                        }

                        return new AccountDetails(sid.Value, accountType, principal.SamAccountName);
                    }

                    logger.Info("No user account found");
                    return null;
                }
            }
            catch (MultipleMatchesException mme)
            {
                logger.Error("More than one user with this SID exists. Cannot check account. SID: " + sid.Value);
                logger.Error(mme.Message);
                return null;
            }
            catch (PrincipalServerDownException psde)
            {
                logger.Warn("Cannot connect to the domain server. Ignore if this machine is not part of a domain network.");
                logger.Warn(psde.Message);
                return null;
            }
        }
    }
#nullable restore
}
