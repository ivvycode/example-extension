using System;
using System.Threading.Tasks;
using Ivvy.Extensions.Setup;
using Ivvy.Extensions.Unsetup;

namespace ExampleExtension.Accounts
{
    /// <summary>
    /// This interface defines the common requirements of an extension.
    /// </summary>
    public interface IAccountServices
    {
        /// <summary>
        /// Registers an iVvy client account with the extension.
        /// </summary>
        Task<Account> SetupAccount(SetupRequest request);

        /// <summary>
        /// Unregisters an iVvy client account with the extension.
        /// </summary>
        Task<bool> UnsetupAccount(UnsetupRequest request);

        /// <summary>
        /// Used by the extension to notify iVvy that it has been
        /// successfully configured for a client account.
        /// </summary>
        Task NotifyAccountConfigured(Account account);

        /// <summary>
        /// Looks up a registered client account by its unique
        /// id and setup key.
        /// </summary>
        Task<Account> FindAccountAsync(string region, string id, string setupKey);

        /// <summary>
        /// Adds the details of an iVvy client account to a permanent datastore.
        /// </summary>
        Task AddAccountAsync(Account account);

        /// <summary>
        /// Deletes the details of an iVvy client account from a permanent datastore.
        /// </summary>
        Task DeleteAccountAsync(Account account);

        /// <summary>
        /// Returns the iVvy api credentials for a specific account.
        /// The first value in the tuple is the api key.
        /// The second value in the tuple is the api secret.
        /// </summary>
        Tuple<string, string> GetIvvyApiCredentials(Account account);
    }
}