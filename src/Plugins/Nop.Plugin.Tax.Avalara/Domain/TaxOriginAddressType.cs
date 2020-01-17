
namespace Nop.Plugin.Tax.Avalara.Domain
{
    /// <summary>
    /// Tax origin address type enumeration
    /// </summary>
    public enum TaxOriginAddressType
    {
        /// <summary>
        /// Tax origin based on the shipping origin address
        /// </summary>
        ShippingOrigin = 0,

        /// <summary>
        /// Tax origin based on the default tax address
        /// </summary>
        DefaultTaxAddress = 1,
    }
}