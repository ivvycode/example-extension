using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ivvy;
using Ivvy.Test;
using ExampleExtension.Accounts;
using ExampleExtension.Events;

namespace ExampleExtension.Controllers
{
    /// <summary>
    /// An example configuration endpoint of an extension.
    /// It demonstrates how to use the Ivvy.API library and how an
    /// extension must report to iVvy that it has been successfully configured.
    /// </summary>
    [Route("[controller]")]
    public class EventConfigureController : BaseController
    {
        private IEventServices EventServices { get; set; }

        public EventConfigureController(
            IOptions<ExtensionSettings> settings,
            ILogger<SetupController> logger,
            IAccountServices accountServices,
            IEventServices eventServices
        ) : base(settings, logger, accountServices) {
            EventServices = eventServices;
        }

        [HttpGet]
        public async Task<string> Get(
                [FromQuery] string region,
                [FromQuery] string accountId,
                [FromQuery] string eventId,
                [FromQuery] string setupKey)
        {
            // An extension should always verify the event has been setup first.
            Event iVvyEvent = await EventServices.FindEventAsync(region, eventId, setupKey);
            if (iVvyEvent == null) {
                return "EVENT_NOT_FOUND";
            }

            // We need the account for the api credentials.
            Account account = await AccountServices.FindAccountAsync(region, accountId);
            if (account == null) {
                return "ACCOUNT_NOT_FOUND";
            }

            // The following code demonstrates how to call the iVvy api.
            Tuple<string, string> credentials = AccountServices.GetIvvyApiCredentials(account);
            Api api = new Api(
                credentials.Item1,
                credentials.Item2,
                Settings.IvvyApiVersion,
                (account.IvvyApiEndPoint == null) ? Settings.IvvyApiBaseUrl : account.IvvyApiEndPoint
            );
            Ivvy.ResultOrError<Ivvy.Event.Event> apiResult = await api.GetEventAsync(Int32.Parse(iVvyEvent.Id));
            if (apiResult.IsSuccess()) {
                // This notifies iVvy that the extension has been configured.
                // An extension can only be used in iVvy once it is configured.
                await EventServices.NotifyEventConfigured(iVvyEvent);

                Ivvy.Event.Event ev = apiResult.Result;
                return $"Code: {ev.Code}, Title: {ev.Title}";
            }
            else {
                return $"Error: {apiResult.ErrorMessage}";
            }
        }
    }
}