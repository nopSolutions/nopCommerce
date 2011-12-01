using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.MailChimp.Data {
    public class MailChimpSettings : ISettings {
        /// <summary>
        /// Gets or sets the API key.
        /// </summary>
        /// <value>
        /// The API key.
        /// </value>
        public virtual string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the default list id.
        /// </summary>
        /// <value>
        /// The default list id.
        /// </value>
        public virtual string DefaultListId { get; set; }
    }
}