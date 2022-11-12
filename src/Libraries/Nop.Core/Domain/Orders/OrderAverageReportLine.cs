<<<<<<< HEAD
namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents an order average report line
    /// </summary>
    public partial class OrderAverageReportLine
    {
        /// <summary>
        /// Gets or sets the count
        /// </summary>
        public int CountOrders { get; set; }

        /// <summary>
        /// Gets or sets the shipping summary (excluding tax)
        /// </summary>
        public decimal SumShippingExclTax { get; set; }

        /// <summary>
        /// Gets or sets the payment fee summary (excluding tax)
        /// </summary>
        public decimal OrderPaymentFeeExclTaxSum { get; set; }

        /// <summary>
        /// Gets or sets the tax summary
        /// </summary>
        public decimal SumTax { get; set; }

        /// <summary>
        /// Gets or sets the order total summary
        /// </summary>
        public decimal SumOrders { get; set; }

        /// <summary>
        /// Gets or sets the refunded amount summary
        /// </summary>
        public decimal SumRefundedAmount { get; set; }
    }
}
=======
namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents an order average report line
    /// </summary>
    public partial class OrderAverageReportLine
    {
        /// <summary>
        /// Gets or sets the count
        /// </summary>
        public int CountOrders { get; set; }

        /// <summary>
        /// Gets or sets the shipping summary (excluding tax)
        /// </summary>
        public decimal SumShippingExclTax { get; set; }

        /// <summary>
        /// Gets or sets the payment fee summary (excluding tax)
        /// </summary>
        public decimal OrderPaymentFeeExclTaxSum { get; set; }

        /// <summary>
        /// Gets or sets the tax summary
        /// </summary>
        public decimal SumTax { get; set; }

        /// <summary>
        /// Gets or sets the order total summary
        /// </summary>
        public decimal SumOrders { get; set; }

        /// <summary>
        /// Gets or sets the refunded amount summary
        /// </summary>
        public decimal SumRefundedAmount { get; set; }
    }
}
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
