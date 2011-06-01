
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Orders
{

    /// <summary>
    /// Represents an order product variant report line
    /// </summary>
    public partial class OrderProductVariantReportLine
    {
        /// <summary>
        /// Gets or sets the product variant identifier
        /// </summary>
        public int ProductVariantId { get; set; }

        /// <summary>
        /// Gets or sets the price excluding tax
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public int TotalQuantity { get; set; }
    }
}
