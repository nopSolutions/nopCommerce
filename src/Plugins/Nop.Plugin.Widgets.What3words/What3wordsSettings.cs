using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.What3words
{
    /// <summary>
    /// Represents plugin settings
    /// </summary>
    public class What3wordsSettings : ISettings
    {
        /// <summary>
        /// Gets or sets the API key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the plugin is enabled
        /// </summary>
        public bool Enabled { get; set; }
    }
}