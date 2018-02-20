using System;
using System.Threading.Tasks;
using Ivvy.Extensions.Setup;
using Ivvy.Extensions.Unsetup;

namespace ExampleExtension.Venues
{
    /// <summary>
    /// This interface defines the common requirements of how an extension
    /// manages the integration with a specific iVvy venue.
    /// </summary>
    public interface IVenueServices
    {
        /// <summary>
        /// Registers an iVvy client venue with the extension.
        /// </summary>
        Task<Venue> SetupVenue(VenueSetupRequest request);

        /// <summary>
        /// Unregisters an iVvy client venue with the extension.
        /// </summary>
        Task<bool> UnsetupVenue(VenueUnsetupRequest request);

        /// <summary>
        /// Used by the extension to notify iVvy that it has been
        /// successfully configured for a client venue.
        /// </summary>
        Task NotifyVenueConfigured(Venue venue);

        /// <summary>
        /// Looks up a registered client venue by its unique
        /// id and setup key.
        /// </summary>
        Task<Venue> FindVenueAsync(string region, string id, string setupKey);

        /// <summary>
        /// Adds the details of an iVvy client venue to a permanent datastore.
        /// </summary>
        Task AddVenueAsync(Venue venue);

        /// <summary>
        /// Deletes the details of an iVvy client venue from a permanent datastore.
        /// </summary>
        Task DeleteVenueAsync(Venue venue);
    }
}