using Amazon.DynamoDBv2.DataModel;

namespace ExampleExtension.Accounts
{
    /// <summary>
    /// The common details of an iVvy client account.
    /// Extensions can modify this to suit their specific requirements.
    /// </summary>
    [DynamoDBTable("Account")]
    public sealed class Account
    {
        /// <summary>
        /// The unique key of an account. Made up of the region and id.
        /// </summary>
        [DynamoDBHashKey]
        public string Pk { get; set; }

        /// <summary>
        /// The iVvy application region.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// The unique id of the account in the application region.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The unique setup key assigned to the extension for the account.
        /// </summary>
        public string SetupKey { get; set; }

        /// <summary>
        /// The api key assigned to the extension.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The api secret assigned to the extension.
        /// </summary>
        public string ApiSecret { get; set; }

        /// <summary>
        /// The iVvy api endpoint url.
        /// </summary>
        public string IvvyApiEndPoint { get; set; }

        /// <summary>
        /// The url to use to verify the iVvy extension setup request.
        /// </summary>
        public string IvvySetupVerifyUrl { get; set; }

        /// <summary>
        /// The url to use to inform iVvy the extension has been configured.
        /// </summary>
        public string IvvySetupConfigureUrl { get; set; }

        /// <summary>
        /// The url to use to verify the iVvy extension event setup request.
        /// </summary>
        public string IvvyEventSetupVerifyUrl { get; set; }

        /// <summary>
        /// The url to use to inform iVvy the extension has been configured in the event.
        /// </summary>
        public string IvvyEventSetupConfigureUrl { get; set; }
    }
}