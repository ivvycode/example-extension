using System;
using System.Threading.Tasks;
using Ivvy.Extensions.Setup;
using Ivvy.Extensions.Unsetup;

namespace ExampleExtension.Events
{
    /// <summary>
    /// This interface defines the common requirements of how an extension
    /// manages the integration with a specific iVvy event.
    /// </summary>
    public interface IEventServices
    {
        /// <summary>
        /// Registers an iVvy client event with the extension.
        /// </summary>
        Task<Event> SetupEvent(EventSetupRequest request);

        /// <summary>
        /// Unregisters an iVvy client event with the extension.
        /// </summary>
        Task<bool> UnsetupEvent(EventUnsetupRequest request);

        /// <summary>
        /// Used by the extension to notify iVvy that it has been
        /// successfully configured for a client event.
        /// </summary>
        Task NotifyEventConfigured(Event iVvyEvent);

        /// <summary>
        /// Looks up a registered client event by its unique
        /// id and setup key.
        /// </summary>
        Task<Event> FindEventAsync(string region, string id, string setupKey);

        /// <summary>
        /// Adds the details of an iVvy client event to a permanent datastore.
        /// </summary>
        Task AddEventAsync(Event iVvyEvent);

        /// <summary>
        /// Deletes the details of an iVvy client event from a permanent datastore.
        /// </summary>
        Task DeleteEventAsync(Event iVvyEvent);
    }
}