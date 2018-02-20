using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ivvy.Extensions.Unsetup;
using ExampleExtension.Accounts;
using ExampleExtension.Events;

namespace ExampleExtension.Controllers
{
    /// <summary>
    /// The unsetup endpoint of an extension is called when an iVvy client
    /// removes the extension from their event. The extension implementation
    /// must handle the situation of iVvy clients continuously adding/removing
    /// the extension to/from their event.
    /// </summary>
    [Route("[controller]")]
    public class EventUnsetupController : BaseController
    {
        private IEventServices EventServices { get; set; }

        public EventUnsetupController(
            IOptions<ExtensionSettings> settings,
            ILogger<SetupController> logger,
            IAccountServices accountServices,
            IEventServices eventServices
        ) : base(settings, logger, accountServices)
        {
            EventServices = eventServices;
        }

        [HttpPost]
        public async Task<UnsetupResponse> Post([FromBody] EventUnsetupRequest request)
        {
            bool success = await EventServices.UnsetupEvent(request);
            return new UnsetupResponse(success);
        }
    }
}