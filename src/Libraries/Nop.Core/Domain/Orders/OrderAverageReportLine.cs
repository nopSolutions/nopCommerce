
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents an order average report line
    /// </summary>
    public partial class OrderAverageReportLine
    {
        /// <summary>
        /// Gets or sets the sum total
        /// </summary>
        public decimal SumOrders { get; set; }

        /// <summary>
        /// Gets or sets the count
        /// </summary>
        public int CountOrders { get; set; }
    }
}
