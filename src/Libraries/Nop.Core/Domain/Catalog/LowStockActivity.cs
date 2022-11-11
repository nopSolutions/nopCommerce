namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a low stock activity
    /// </summary>
    public enum LowStockActivity
    {
        /// <summary>
        /// Nothing
        /// </summary>
        Nothing = 0,

        /// <summary>
        /// Disable buy button
        /// </summary>
        DisableBuyButton = 1,

        /// <summary>
        /// Unpublish
        /// </summary>
        Unpublish = 2,
    }
}
