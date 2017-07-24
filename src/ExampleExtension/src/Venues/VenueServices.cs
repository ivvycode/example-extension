using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using ExampleExtension.Cryptography;
using ExampleExtension.Accounts;
using Ivvy.Extensions;
using Ivvy.Extensions.Setup;
using Ivvy.Extensions.Configure;

namespace ExampleExtension.Venues
{
    /// <summary>
    /// Implementation of the required venue extension services that uses DynamoDB.
    /// </summary>
    public sealed class VenueServices : IVenueServices
    {
        private IAccountServices AccountServices { get; set; }

        private IDynamoDBContext Context { get; set; }

        private ExtensionSettings Settings { get; set; }

        private ILogger Logger { get; set; }

        public VenueServices(
            IAccountServices accountServices,
            IAmazonDynamoDB dynamoDbClient,
            IOptions<ExtensionSettings> settings,
            ILogger<VenueServices> logger)
        {
            AccountServices = accountServices;
            Context = new DynamoDBContext(dynamoDbClient);
            Settings = settings.Value;
            Logger = logger;
        }

        /// <summary>
        /// Registers an iVvy client venue with the extension.
        /// </summary>
        public async Task<Venue> SetupVenue(VenueSetupRequest request)
        {
            // We must verify the setup request originated from iVVy.

            Account account = await AccountServices.FindAccountAsync(request.Region, request.AccountId);
            if (account == null) {
                Logger.LogError("Setup Error: Account does not exist");
                return null;
            }
            Extension ext = new Extension(
                account.IvvySetupVerifyUrl,
                account.IvvySetupConfigureUrl,
                account.IvvyEventSetupVerifyUrl,
                account.IvvyEventSetupConfigureUrl,
                account.IvvyVenueSetupVerifyUrl,
                account.IvvyVenueSetupConfigureUrl
            );
            ResultOrError<VenueVerifySetupResponse> verifyResult =
                await ext.VenueVerifySetupAsync(request.AccountId, request.VenueId, request.SetupKey);
            if (verifyResult.Success) {
                VenueVerifySetupResponse verifyResponse = verifyResult.Result;
                Venue venue = new Venue {
                    Pk = $"{request.Region}:{verifyResponse.VenueId}",
                    Region = request.Region,
                    Id = verifyResponse.VenueId,
                    AccountId = verifyResponse.AccountId,
                    SetupKey = request.SetupKey
                };
                await AddVenueAsync(venue);
                return venue;
            }
            else {
                Logger.LogError($"Setup Error: {verifyResult.ErrorMessage}");
                return null;
            }
        }

        /// <summary>
        /// Used by the extension to notify iVvy that it has been
        /// successfully configured for a client venue.
        /// </summary>
        public async Task NotifyVenueConfigured(Venue venue)
        {
            // Verify the required venue data.
            if (venue.Id == null || venue.Id.Trim() == "") {
                throw new ArgumentException("venue does not have a Id value");
            }
            if (venue.AccountId == null || venue.AccountId.Trim() == "") {
                throw new ArgumentException("venue does not have a AccountId value");
            }
            if (venue.SetupKey == null || venue.SetupKey.Trim() == "") {
                throw new ArgumentException("venue does not have a SetupKey value");
            }

            Account account = await AccountServices.FindAccountAsync(venue.Region, venue.AccountId);
            if (account == null) {
                Logger.LogError("Configure Error: Account does not exist");
                return;
            }

            Extension ext = new Extension(
                account.IvvySetupVerifyUrl,
                account.IvvySetupConfigureUrl,
                account.IvvyEventSetupVerifyUrl,
                account.IvvyEventSetupConfigureUrl,
                account.IvvyVenueSetupVerifyUrl,
                account.IvvyVenueSetupConfigureUrl
            );
            ResultOrError<VerifyConfigureResponse> verifyResult =
                await ext.VenueConfigureAsync(venue.AccountId, venue.Id, venue.SetupKey);
            if (!verifyResult.Success) {
                Logger.LogError("Unknown venue configure error");
            }
        }

        /// <summary>
        /// Looks up a registered client venue by its unique id and setup key.
        /// </summary>
        public async Task<Venue> FindVenueAsync(string region, string id, string setupKey)
        {
            Venue venue = await Context.LoadAsync<Venue>($"{region}:{id}");
            if (venue != null && venue.SetupKey != setupKey) {
                venue = null;
            }
            return venue;
        }

        /// <summary>
        /// Adds the details of an iVvy client venue to DynamoDB.
        /// </summary>
        public async Task AddVenueAsync(Venue venue)
        {
            // Verify the required venue data.
            if (venue.Pk == null || venue.Pk.Trim() == "") {
                throw new ArgumentException("venue does not have a Pk value");
            }
            if (venue.Id == null || venue.Id.Trim() == "") {
                throw new ArgumentException("venue does not have a Id value");
            }
            if (venue.AccountId == null || venue.AccountId.Trim() == "") {
                throw new ArgumentException("venue does not have a AccountId value");
            }
            if (venue.SetupKey == null || venue.SetupKey.Trim() == "") {
                throw new ArgumentException("venue does not have a SetupKey value");
            }

            // Save the venue details.
            await Context.SaveAsync<Venue>(venue);
        }
    }
}