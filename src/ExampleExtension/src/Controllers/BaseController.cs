using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ExampleExtension.Accounts;

namespace ExampleExtension.Controllers
{
    /// <summary>
    /// The base controller of the MVC application.
    /// It encapsulates the common dependencies required by the extension.
    /// </summary>
    public class BaseController : Controller
    {
        protected ExtensionSettings Settings { get; set; }

        protected ILogger Logger { get; set; }

        protected IAccountServices AccountServices { get; set; }

        public BaseController(
            IOptions<ExtensionSettings> settings,
            ILogger<SetupController> logger,
            IAccountServices accountServices)
        {
            Settings = settings.Value;
            Logger = logger;
            AccountServices = accountServices;
        }
    }
}