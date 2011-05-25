namespace Nop.Core.Domain.Discounts
{
    /// <summary>
    /// Represents a discount type
    /// </summary>
    public enum DiscountType
    {
        /// <summary>
        /// Assigned to order total 
        /// </summary>
        AssignedToOrderTotal = 1,
        /// <summary>
        /// Assigned to product variants (SKUs)
        /// </summary>
        AssignedToSkus = 2,
        /// <summary>
        /// Assigned to product variants (SKUs) mapped to a certain category
        /// </summary>
        AssignedToCategories = 5,
        /// <summary>
        /// Assigned to shipping
        /// </summary>
        AssignedToShipping = 10,
        /// <summary>
        /// Assigned to order subtotal
        /// </summary>
        AssignedToOrderSubTotal = 20,
    }
}
