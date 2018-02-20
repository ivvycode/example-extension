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
using Ivvy.Extensions.Unsetup;
using Ivvy.Extensions.Configure;

namespace ExampleExtension.Events
{
    /// <summary>
    /// Implementation of the required event extension services that uses DynamoDB.
    /// </summary>
    public sealed class EventServices : IEventServices
    {
        private IAccountServices AccountServices { get; set; }

        private IDynamoDBContext Context { get; set; }

        private ExtensionSettings Settings { get; set; }

        private ILogger Logger { get; set; }

        public EventServices(
            IAccountServices accountServices,
            IAmazonDynamoDB dynamoDbClient,
            IOptions<ExtensionSettings> settings,
            ILogger<EventServices> logger)
        {
            AccountServices = accountServices;
            Context = new DynamoDBContext(dynamoDbClient);
            Settings = settings.Value;
            Logger = logger;
        }

        /// <summary>
        /// Registers an iVvy client event with the extension.
        /// </summary>
        public async Task<Event> SetupEvent(EventSetupRequest request)
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
            ResultOrError<EventVerifySetupResponse> verifyResult =
                await ext.EventVerifySetupAsync(request.AccountId, request.EventId, request.SetupKey);
            if (verifyResult.Success) {
                EventVerifySetupResponse verifyResponse = verifyResult.Result;
                Event iVvyEvent = new Event {
                    Pk = $"{request.Region}:{verifyResponse.EventId}",
                    Region = request.Region,
                    Id = verifyResponse.EventId,
                    AccountId = verifyResponse.AccountId,
                    SetupKey = request.SetupKey
                };
                await AddEventAsync(iVvyEvent);
                return iVvyEvent;
            }
            else {
                Logger.LogError($"Setup Error: {verifyResult.ErrorMessage}");
                return null;
            }
        }

        /// <summary>
        /// Unregisters an iVvy client event with the extension.
        /// </summary>
        public async Task<bool> UnsetupEvent(EventUnsetupRequest request)
        {
            // Always verify the event exists before handling the request.
            // This confirms the request originated from iVvy.
            // The following code will delete the event details from DynamoDB
            // if it exists. The extension must handle the situation where
            // a client continuously adds/removes the extension to/from their event.

            Event iVvyEvent = await FindEventAsync(
                request.Region, request.EventId, request.SetupKey
            );
            if (iVvyEvent == null) {
                return false;
            }
            else {
                await DeleteEventAsync(iVvyEvent);
                return true;
            }
        }

        /// <summary>
        /// Used by the extension to notify iVvy that it has been
        /// successfully configured for a client event.
        /// </summary>
        public async Task NotifyEventConfigured(Event iVvyEvent)
        {
            // Verify the required event data.
            if (iVvyEvent.Id == null || iVvyEvent.Id.Trim() == "") {
                throw new ArgumentException("iVvyEvent does not have a Id value");
            }
            if (iVvyEvent.AccountId == null || iVvyEvent.AccountId.Trim() == "") {
                throw new ArgumentException("iVvyEvent does not have a AccountId value");
            }
            if (iVvyEvent.SetupKey == null || iVvyEvent.SetupKey.Trim() == "") {
                throw new ArgumentException("iVvyEvent does not have a SetupKey value");
            }

            Account account = await AccountServices.FindAccountAsync(iVvyEvent.Region, iVvyEvent.AccountId);
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
                await ext.EventConfigureAsync(iVvyEvent.AccountId, iVvyEvent.Id, iVvyEvent.SetupKey);
            if (!verifyResult.Success) {
                Logger.LogError("Unknown event configure error");
            }
        }

        /// <summary>
        /// Looks up a registered client event by its unique id and setup key.
        /// </summary>
        public async Task<Event> FindEventAsync(string region, string id, string setupKey)
        {
            Event iVvyEvent = await Context.LoadAsync<Event>($"{region}:{id}");
            if (iVvyEvent != null && iVvyEvent.SetupKey != setupKey) {
                iVvyEvent = null;
            }
            return iVvyEvent;
        }

        /// <summary>
        /// Adds the details of an iVvy client event to DynamoDB.
        /// </summary>
        public async Task AddEventAsync(Event iVvyEvent)
        {
            // Verify the required event data.
            if (iVvyEvent.Pk == null || iVvyEvent.Pk.Trim() == "") {
                throw new ArgumentException("iVvyEvent does not have a Pk value");
            }
            if (iVvyEvent.Id == null || iVvyEvent.Id.Trim() == "") {
                throw new ArgumentException("iVvyEvent does not have a Id value");
            }
            if (iVvyEvent.AccountId == null || iVvyEvent.AccountId.Trim() == "") {
                throw new ArgumentException("iVvyEvent does not have a AccountId value");
            }
            if (iVvyEvent.SetupKey == null || iVvyEvent.SetupKey.Trim() == "") {
                throw new ArgumentException("iVvyEvent does not have a SetupKey value");
            }

            // Save the event details.
            await Context.SaveAsync<Event>(iVvyEvent);
        }

        /// <summary>
        /// Deletes the details of an iVvy client event from DynamoDB.
        /// </summary>
        public async Task DeleteEventAsync(Event iVvyEvent)
        {
            // Verify the required event data.
            if (iVvyEvent.Pk == null || iVvyEvent.Pk.Trim() == "") {
                throw new ArgumentException("iVvyEvent does not have a Pk value");
            }

            // Delete the event.
            await Context.DeleteAsync<Event>(iVvyEvent.Pk);
        }
    }
}