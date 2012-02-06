namespace Nop.Plugin.Shipping.Fedex
{
    /// <summary>
    /// Represents a packing type
    /// </summary>
    public enum PackingType : int
    {
        /// <summary>
        /// Pack by dimensions
        /// </summary>
        PackByDimensions = 1,
        /// <summary>
        /// Pack by one item per package
        /// </summary>
        PackByOneItemPerPackage = 2,
        /// <summary>
        /// Pack by volume
        /// </summary>
        PackByVolume = 3
    }
}
