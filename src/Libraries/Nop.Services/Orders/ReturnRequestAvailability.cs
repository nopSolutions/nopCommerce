namespace Nop.Services.Orders
{
    /// <summary>
    /// Represents the return request availability
    /// </summary>
    public partial class ReturnRequestAvailability
    {
        #region Properties

        /// <summary>
        /// Gets the value indicating whether a return request is allowed
        /// </summary>
        public bool IsAllowed => ReturnableOrderItems?.Any(i => i.AvailableQuantityForReturn > 0) ?? false;

        /// <summary>
        /// Gets or sets the returnable order items
        /// </summary>
        public IList<ReturnableOrderItem> ReturnableOrderItems { get; set; }

        #endregion
    }
}
