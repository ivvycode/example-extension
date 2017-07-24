using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ivvy;
using Ivvy.Test;
using ExampleExtension.Accounts;
using ExampleExtension.Venues;

namespace ExampleExtension.Controllers
{
    /// <summary>
    /// An example configuration endpoint of an extension.
    /// It demonstrates how an extension must report to iVvy that it
    /// has been successfully configured.
    /// </summary>
    [Route("[controller]")]
    public class VenueConfigureController : BaseController
    {
        private IVenueServices VenueServices { get; set; }

        public VenueConfigureController(
            IOptions<ExtensionSettings> settings,
            ILogger<SetupController> logger,
            IAccountServices accountServices,
            IVenueServices venueServices
        ) : base(settings, logger, accountServices)
        {
            VenueServices = venueServices;
        }

        [HttpGet]
        public async Task<string> Get(
            [FromQuery] string region,
            [FromQuery] string accountId,
            [FromQuery] string venueId,
            [FromQuery] string setupKey)
        {
            // An extension should always verify the venue has been setup first.
            Venue venue = await VenueServices.FindVenueAsync(region, venueId, setupKey);
            if (venue == null) {
                return "VENUE_NOT_FOUND";
            }

            // This notifies iVvy that the extension has been configured.
            // An extension can only be used in iVvy once it is configured.
            await VenueServices.NotifyVenueConfigured(venue);

            return "HELLO WORLD";
        }
    }
}