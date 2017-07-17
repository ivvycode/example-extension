using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ivvy;
using Ivvy.Test;
using ExampleExtension.Accounts;

namespace ExampleExtension.Controllers
{
    /// <summary>
    /// An example configuration endpoint of an extension.
    /// It demonstrates how to use the Ivvy.API library and how an
    /// extension must report to iVvy that it has been successfully configured.
    /// </summary>
    [Route("[controller]")]
    public class ConfigureController : BaseController
    {
        public ConfigureController(
            IOptions<ExtensionSettings> settings,
            ILogger<SetupController> logger,
            IAccountServices accountServices
        ) : base(settings, logger, accountServices) {}

        [HttpGet]
        public async Task<string> Get([FromQuery] string accountId, [FromQuery] string setupKey)
        {
            // An extension should always verify the account has been setup first.
            Account account = await AccountServices.FindAccountAsync(accountId, setupKey);
            if (account == null) {
                return "ACCOUNT_NOT_FOUND";
            }

            // This notifies iVvy that the extension has been configured.
            // An extension can only be used in iVvy once it is configured.
            await AccountServices.NotifyAccountConfigured(account);

            // The following code demonstrates how to call the iVvy api.
            Tuple<string, string> credentials = AccountServices.GetIvvyApiCredentials(account);
            Api api = new Api(
                credentials.Item1,
                credentials.Item2,
                Settings.IvvyApiVersion,
                (account.IvvyApiEndPoint == null) ? Settings.IvvyApiBaseUrl : account.IvvyApiEndPoint
            );
            Ivvy.ResultOrError<Pong> apiResult = await api.PingAsync();
            if (apiResult.IsSuccess()) {
                Pong pong = apiResult.Result;
                string ack = pong.Ack.ToString();
                return ack;
            }
            else {
                return $"Error: {apiResult.ErrorMessage}";
            }
        }
    }
}