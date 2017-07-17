using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using ExampleExtension.Cryptography;
using Ivvy.Extensions;
using Ivvy.Extensions.Setup;
using Ivvy.Extensions.Unsetup;
using Ivvy.Extensions.Configure;

namespace ExampleExtension.Accounts
{
    /// <summary>
    /// Implementation of the required extension services that uses DynamoDB.
    /// </summary>
    public sealed class AccountServices : IAccountServices
    {
        private IDynamoDBContext Context { get; set; }

        private ExtensionSettings Settings { get; set; }

        private ILogger Logger { get; set; }

        public AccountServices(
            IAmazonDynamoDB dynamoDbClient,
            IOptions<ExtensionSettings> settings,
            ILogger<AccountServices> logger)
        {
            Context = new DynamoDBContext(dynamoDbClient);
            Settings = settings.Value;
            Logger = logger;
        }

        /// <summary>
        /// Registers an iVvy client account with the extension.
        /// </summary>
        public async Task<Account> SetupAccount(SetupRequest request)
        {
            // We must verify the setup request in order to obtain
            // the account details including the api credentials.
            // It also confirms the request originated from iVVy.

            Extension ext = new Extension(
                request.IvvySetupVerifyUrl,
                request.IvvySetupConfigureUrl,
                request.IvvyEventSetupVerifyUrl,
                request.IvvyEventSetupConfigureUrl
            );
            ResultOrError<VerifySetupResponse> verifyResult =
                await ext.VerifySetupAsync(request.AccountId, request.SetupKey);
            if (verifyResult.Success) {
                VerifySetupResponse verifyResponse = verifyResult.Result;
                Account account = new Account {
                    Id = verifyResponse.AccountId,
                    SetupKey = request.SetupKey,
                    ApiKey = verifyResponse.ApiKey,
                    ApiSecret = verifyResponse.ApiSecret,
                    IvvyApiEndPoint = request.IvvyApiEndPoint,
                    IvvySetupVerifyUrl = request.IvvySetupVerifyUrl,
                    IvvySetupConfigureUrl = request.IvvySetupConfigureUrl,
                    IvvyEventSetupVerifyUrl = request.IvvyEventSetupVerifyUrl,
                    IvvyEventSetupConfigureUrl = request.IvvyEventSetupConfigureUrl
                };
                await AddAccountAsync(account);
                return account;
            }
            else {
                Logger.LogError($"Setup Error: {verifyResult.ErrorMessage}");
                return null;
            }
        }

        /// <summary>
        /// Unregisters an iVvy client account with the extension.
        /// </summary>
        public async Task<bool> UnsetupAccount(UnsetupRequest request)
        {
            // Always verify the account exists before handling the request.
            // This confirms the request originated from iVvy.
            // The following code will delete the account details from DynamoDB
            // if it exists. The extension must handle the situation where
            // a client continuously adds/removes the extension to/from their account.

            Account account = await FindAccountAsync(request.AccountId, request.SetupKey);
            if (account == null) {
                return false;
            }
            else {
                await DeleteAccountAsync(account);                
                return true;
            }
        }

        /// <summary>
        /// Used by the extension to notify iVvy that it has been
        /// successfully configured for a client account.
        /// </summary>
        public async Task NotifyAccountConfigured(Account account)
        {
            // Verify the required account data.
            if (account.Id == null || account.Id.Trim() == "") {
                throw new ArgumentException("account does not have an Id value");
            }
            if (account.SetupKey == null || account.SetupKey.Trim() == "") {
                throw new ArgumentException("account does not have a SetupKey value");
            }
            Extension extension = new Extension(
                account.IvvySetupVerifyUrl,
                account.IvvySetupConfigureUrl,
                account.IvvyEventSetupVerifyUrl,
                account.IvvyEventSetupConfigureUrl        
            );
            ResultOrError<VerifyConfigureResponse> verifyResult = 
                await extension.ConfigureAsync(account.Id, account.SetupKey);
            if (!verifyResult.Success) {
                Logger.LogError("Unknown configure error");
            }
        }

        /// <summary>
        /// Looks up a registered client account by its unique
        /// id and setup key.
        /// </summary>
        public async Task<Account> FindAccountAsync(string id, string setupKey)
        {
            Account account = await Context.LoadAsync<Account>(id);
            if (account != null && account.SetupKey != setupKey) {
                account = null;
            }
            return account;
        }

        /// <summary>
        /// Adds the details of an iVvy client account to DynamoDB.
        /// </summary>
        public async Task AddAccountAsync(Account account)
        {
            // Verify the required account data.
            if (account.Id == null || account.Id.Trim() == "") {
                throw new ArgumentException("account does not have an Id value");
            }
            if (account.SetupKey == null || account.SetupKey.Trim() == "") {
                throw new ArgumentException("account does not have a SetupKey value");
            }
            if (account.ApiKey == null || account.ApiKey.Trim() == "") {
                throw new ArgumentException("account does not have an ApiKey value");
            }
            if (account.ApiSecret == null || account.ApiSecret.Trim() == "") {
                throw new ArgumentException("account does not have an ApiSecret value");
            }

            // Encrypt the api credentials.
            account.ApiKey = StringCipher.Encrypt(account.ApiKey, Settings.CipherPassphrase);
            account.ApiSecret = StringCipher.Encrypt(account.ApiSecret, Settings.CipherPassphrase);

            // Save the new account.
            await Context.SaveAsync<Account>(account);
        }

        /// <summary>
        /// Deletes the details of an iVvy client account from DynamoDB.
        /// </summary>
        public async Task DeleteAccountAsync(Account account)
        {
            // Verify the required account data.
            if (account.Id == null || account.Id.Trim() == "") {
                throw new ArgumentException("account does not have an Id value");
            }

            // Delete the account.
            await Context.DeleteAsync<Account>(account.Id);
        }

        /// <summary>
        /// Returns the iVvy api credentials for a specific account.
        /// The first value in the tuple is the api key.
        /// The second value in the tuple is the api secret.
        /// </summary>
        public Tuple<string, string> GetIvvyApiCredentials(Account account)
        {
            return new Tuple<string, string>(
                StringCipher.Decrypt(account.ApiKey, Settings.CipherPassphrase),
                StringCipher.Decrypt(account.ApiSecret, Settings.CipherPassphrase)
            );
        }
    }
}