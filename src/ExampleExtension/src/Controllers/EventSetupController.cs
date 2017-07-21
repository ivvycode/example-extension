using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ivvy.Extensions.Setup;
using ExampleExtension.Accounts;
using ExampleExtension.Events;

namespace ExampleExtension.Controllers
{
    /// <summary>
    /// The setup endpoint of an extension is called when an iVvy client attempts
    /// to add the extension to an event in their account.
    /// </summary>
    [Route("[controller]")]
    public class EventSetupController : BaseController
    {
        private IEventServices EventServices { get; set; }

        public EventSetupController(
            IOptions<ExtensionSettings> settings,
            ILogger<SetupController> logger,
            IAccountServices accountServices,
            IEventServices eventServices
        ) : base(settings, logger, accountServices) {
            EventServices = eventServices;
        }

        [HttpPost]
        public async Task<SetupResponse> Post([FromBody] EventSetupRequest request)
        {
            Event iVvyEvent = await EventServices.SetupEvent(request);
            if (iVvyEvent == null) {
                return new SetupResponse(false);
            }
            else {
                return new SetupResponse(true);
            }
        }
    }
}