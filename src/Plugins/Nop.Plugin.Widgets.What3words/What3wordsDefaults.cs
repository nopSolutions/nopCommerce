using Nop.Core;

namespace Nop.Plugin.Widgets.What3words
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public static class What3wordsDefaults
    {
        /// <summary>
        /// Gets the system name
        /// </summary>
        public static string SystemName => "Widgets.What3words";

        /// <summary>
        /// Gets the user agent used to request third-party services
        /// </summary>
        public static string UserAgent => $"nopCommerce-{NopVersion.CURRENT_VERSION}";

        /// <summary>
        /// Gets the configuration route name
        /// </summary>
        public static string ConfigurationRouteName => "Plugin.Widgets.What3words.Configure";

        /// <summary>
        /// Gets the name of autosuggest component
        /// </summary>
        public static string ComponentName => "w3w";

        /// <summary>
        /// Gets a key of the attribute to store words for address
        /// </summary>
        public static string AddressValueAttribute => "What3wordsValue";

        /// <summary>
        /// Gets a field prefix on the checkout billing address page
        /// </summary>
        public static string BillingAddressPrefix => "BillingNewAddress";

        /// <summary>
        /// Gets a field prefix on the checkout billing address page
        /// </summary>
        public static string ShippingAddressPrefix => "ShippingNewAddress";
    }
}