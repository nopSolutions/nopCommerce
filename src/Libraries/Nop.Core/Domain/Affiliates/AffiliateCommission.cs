namespace Nop.Core.Domain.Affiliates;

/// <summary>
/// Represents an affiliate commission
/// </summary>
public partial class AffiliateCommission : BaseEntity
{
    /// <summary>
    /// Gets or sets the affiliate identifier
    /// </summary>
    public int AffiliateId { get; set; }

    /// <summary>
    /// Gets or sets the total commission amount
    /// </summary>
    public decimal TotalCommissionAmount { get; set; }

    /// <summary>
    /// Gets or sets the commission status identifier
    /// </summary>
    public int CommissionStatusId { get; set; }

    /// <summary>
    /// Gets or sets the date and time of commission payment
    /// </summary>
    public DateTime? PaidOn { get; set; }

    /// <summary>
    /// Gets or sets the date and time of commission creation
    /// </summary>
    public DateTime CreateOn { get; set; }

    /// <summary>
    /// Gets or sets the admin comment
    /// </summary>
    public string AdminComment { get; set; }

    /// <summary>
    /// Gets or sets the commission status
    /// </summary>
    public CommissionStatus CommissionStatus
    {
        get => (CommissionStatus)CommissionStatusId;
        set => CommissionStatusId = (int)value;
    }
}