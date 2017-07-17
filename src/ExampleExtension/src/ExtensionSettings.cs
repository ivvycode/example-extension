namespace ExampleExtension
{
    /// <summary>
    /// This class encapsulates the common settings of an extension.
    /// </summary>
    public class ExtensionSettings
    {
        /// <summary>
        /// The iVvy api version.
        /// </summary>
        public string IvvyApiVersion { get; set; }

        /// <summary>
        /// The base url of the iVvy api.
        /// Useful for testing the extension in different environments.
        /// </summary>
        public string IvvyApiBaseUrl { get; set; }

        /// <summary>
        /// The secret used to encrypt/decrypt data.
        /// </summary>
        public string CipherPassphrase { get; set; }
    }
}