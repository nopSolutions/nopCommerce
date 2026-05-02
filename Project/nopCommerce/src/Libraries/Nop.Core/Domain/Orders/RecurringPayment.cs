using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;

namespace Nop.Core.Domain.Orders;

/// <summary>
/// Represents a recurring payment
/// </summary>
public partial class RecurringPayment : BaseEntity, ISoftDeletedEntity
{
    /// <summary>
    /// Gets or sets the cycle length
    /// </summary>
    public int CycleLength { get; set; }

    /// <summary>
    /// Gets or sets the cycle period identifier
    /// </summary>
    public int CyclePeriodId { get; set; }

    /// <summary>
    /// Gets or sets the total cycles
    /// </summary>
    public int TotalCycles { get; set; }

    /// <summary>
    /// Gets or sets the start date
    /// </summary>
    public DateTime StartDateUtc { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the payment is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the last payment failed
    /// </summary>
    public bool LastPaymentFailed { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity has been deleted
    /// </summary>
    public bool Deleted { get; set; }

    /// <summary>
    /// Gets or sets the initial order identifier
    /// </summary>
    public int InitialOrderId { get; set; }

    /// <summary>
    /// Gets or sets the date and time of payment creation
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the cycle period
    /// </summary>
    public RecurringProductCyclePeriod CyclePeriod
    {
        get => (RecurringProductCyclePeriod)CyclePeriodId;
        set => CyclePeriodId = (int)value;
    }
}