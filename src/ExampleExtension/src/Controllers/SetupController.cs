using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ivvy.Extensions.Setup;
using ExampleExtension.Accounts;

namespace ExampleExtension.Controllers
{
    /// <summary>
    /// The setup endpoint of an extension is called when an iVvy client attempts
    /// to add the extension to their account.
    /// </summary>
    [Route("[controller]")]
    public class SetupController : BaseController
    {
        public SetupController(
            IOptions<ExtensionSettings> settings,
            ILogger<SetupController> logger,
            IAccountServices accountServices
        ) : base(settings, logger, accountServices) {}

        [HttpPost]
        public async Task<SetupResponse> Post([FromBody] SetupRequest request)
        {
            Account account = await AccountServices.SetupAccount(request);
            if (account == null) {
                return new SetupResponse(false);
            }
            else {
                return new SetupResponse(true);
            }
        }
    }
}