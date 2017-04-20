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
        /// The unique id of the account.
        /// </summary>
        [DynamoDBHashKey]
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
    }
}