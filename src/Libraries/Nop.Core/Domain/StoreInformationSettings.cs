using Nop.Core.Configuration;

namespace Nop.Core.Domain
{
    public class StoreInformationSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a store name
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// Gets or sets a store URL
        /// </summary>
        public string StoreUrl { get; set; }

    }
}
