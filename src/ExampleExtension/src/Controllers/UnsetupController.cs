using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ivvy.Extensions.Unsetup;
using ExampleExtension.Accounts;

namespace ExampleExtension.Controllers
{
    /// <summary>
    /// The unsetup endpoint of an extension is called when an iVvy client
    /// removes the extension from their account. The extension implementation
    /// must handle the situation of iVvy clients continuously adding/removing
    /// the extension to/from their account.
    /// </summary>
    [Route("[controller]")]
    public class UnsetupController : BaseController
    {
        public UnsetupController(
            IOptions<ExtensionSettings> settings,
            ILogger<SetupController> logger,
            IAccountServices accountServices
        ) : base(settings, logger, accountServices) {}

        [HttpPost]
        public async Task<UnsetupResponse> Post([FromBody] UnsetupRequest request)
        {
            bool success = await AccountServices.UnsetupAccount(request);
            return new UnsetupResponse(success);
        }
    }
}