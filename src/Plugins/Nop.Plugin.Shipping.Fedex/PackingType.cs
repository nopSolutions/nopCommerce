namespace Nop.Plugin.Shipping.Fedex
{
    /// <summary>
    /// Represents a packing type
    /// </summary>
    public enum PackingType
    {
        /// <summary>
        /// Pack by dimensions
        /// </summary>
        PackByDimensions = 0,
        /// <summary>
        /// Pack by one item per package
        /// </summary>
        PackByOneItemPerPackage = 1,
        /// <summary>
        /// Pack by volume
        /// </summary>
        PackByVolume = 2
    }
}
