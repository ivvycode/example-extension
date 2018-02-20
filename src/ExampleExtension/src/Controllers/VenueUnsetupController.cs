using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ivvy.Extensions.Unsetup;
using ExampleExtension.Accounts;
using ExampleExtension.Venues;

namespace ExampleExtension.Controllers
{
    /// <summary>
    /// The unsetup endpoint of an extension is called when an iVvy client
    /// removes the extension from their venue. The extension implementation
    /// must handle the situation of iVvy clients continuously adding/removing
    /// the extension to/from their venue.
    /// </summary>
    [Route("[controller]")]
    public class VenueUnsetupController : BaseController
    {
        private IVenueServices VenueServices { get; set; }

        public VenueUnsetupController(
            IOptions<ExtensionSettings> settings,
            ILogger<SetupController> logger,
            IAccountServices accountServices,
            IVenueServices venueServices
        ) : base(settings, logger, accountServices)
        {
            VenueServices = venueServices;
        }

        [HttpPost]
        public async Task<UnsetupResponse> Post([FromBody] VenueUnsetupRequest request)
        {
            bool success = await VenueServices.UnsetupVenue(request);
            return new UnsetupResponse(success);
        }
    }
}