namespace Nop.Plugin.Tax.Avalara.Domain
{
    /// <summary>
    /// Represents the tax origin address type enumeration
    /// </summary>
    public enum TaxOriginAddressType
    {
        /// <summary>
        /// Tax origin based on the shipping origin address
        /// </summary>
        ShippingOrigin = 1,

        /// <summary>
        /// Tax origin based on the default tax address
        /// </summary>
        DefaultTaxAddress = 2
    }
}