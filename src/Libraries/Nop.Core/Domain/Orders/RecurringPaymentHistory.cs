namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents a recurring payment history
    /// </summary>
    public partial class RecurringPaymentHistory : BaseEntity
    {
        /// <summary>
        /// Gets or sets the recurring payment identifier
        /// </summary>
        public int RecurringPaymentId { get; set; }

        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
    }
}
