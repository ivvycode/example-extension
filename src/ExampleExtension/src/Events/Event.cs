using Amazon.DynamoDBv2.DataModel;

namespace ExampleExtension.Events
{
    [DynamoDBTable("Event")]
    public sealed class Event
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
        /// The unique id of the event in the application region.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The unique id of the account in the application region
        /// to which the event belongs.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// The unique setup key assigned to the extension for the event.
        /// </summary>
        public string SetupKey { get; set; }
    }
}