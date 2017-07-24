using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ivvy.Extensions.Setup;
using ExampleExtension.Accounts;
using ExampleExtension.Venues;

namespace ExampleExtension.Controllers
{
    /// <summary>
    /// The setup endpoint of an extension is called when an iVvy client attempts
    /// to add the extension to a venue in their account.
    /// </summary>
    [Route("[controller]")]
    public class VenueSetupController : BaseController
    {
        private IVenueServices VenueServices { get; set; }

        public VenueSetupController(
            IOptions<ExtensionSettings> settings,
            ILogger<SetupController> logger,
            IAccountServices accountServices,
            IVenueServices venueServices
        ) : base(settings, logger, accountServices)
        {
            VenueServices = venueServices;
        }

        [HttpPost]
        public async Task<SetupResponse> Post([FromBody] VenueSetupRequest request)
        {
            Venue venue = await VenueServices.SetupVenue(request);
            if (venue == null) {
                return new SetupResponse(false);
            }
            else {
                return new SetupResponse(true);
            }
        }
    }
}