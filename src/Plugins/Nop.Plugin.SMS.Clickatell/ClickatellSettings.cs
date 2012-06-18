
using Nop.Core.Configuration;

namespace Nop.Plugin.SMS.Clickatell
{
    public class ClickatellSettings : ISettings
    {
        /// <summary>
        /// Gets or sets the value indicting whether this SMS provider is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the Clickatell phone number
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the Clickatell API ID
        /// </summary>
        public string ApiId { get; set; }

        /// <summary>
        /// Gets or sets the Clickatell username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the Clickatell password
        /// </summary>
        public string Password { get; set; }
    }
}