using Amazon.DynamoDBv2.DataModel;

namespace ExampleExtension.Venues
{
    [DynamoDBTable("Venue")]
    public sealed class Venue
    {
        /// <summary>
        /// The unique key of a venue. Made up of the region and id.
        /// </summary>
        [DynamoDBHashKey]
        public string Pk { get; set; }

        /// <summary>
        /// The iVvy application region.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// The unique id of the venue in the application region.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The unique id of the account in the application region
        /// to which the venue belongs.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// The unique setup key assigned to the extension for the venue.
        /// </summary>
        public string SetupKey { get; set; }
    }
}