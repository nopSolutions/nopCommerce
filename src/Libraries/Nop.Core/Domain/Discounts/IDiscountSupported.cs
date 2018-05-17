using System.Collections.Generic;

namespace Nop.Core.Domain.Discounts
{
    /// <summary>
    /// Represents an entity which supports discounts
    /// </summary>
    public partial interface IDiscountSupported
    {
        /// <summary>
        /// Gets or sets the collection of applied discounts
        /// </summary>
        IList<Discount> AppliedDiscounts { get; }
    }
}