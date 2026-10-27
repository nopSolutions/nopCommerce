namespace Nop.Core.Domain.PriceLists;

/// <summary>
/// Represents a price list
/// </summary>
public partial class PriceList : BaseEntity
{
    #region Properties

    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the price list is active
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the price list becomes active
    /// </summary>
    public DateTime? StartDateUtc { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the price list becomes inactive
    /// </summary>
    public DateTime? EndDateUtc { get; set; }

    /// <summary>
    ///  Gets or sets the price calculation type identifier
    /// </summary>
    public int PriceCalculationTypeId { get; set; }

    /// <summary>
    /// Gets or sets the price calculation value (e.g. percentage or fixed amount)
    /// </summary>
    public decimal PriceCalculationValue { get; set; }

    /// <summary>
    /// Gets or sets the priority of the price list (lower value means higher priority)
    /// </summary>
    public int Priority { get; set; }

    #endregion

    #region Custom properties

    /// <summary>
    /// Gets or sets the price calculation type
    /// </summary>
    public PriceCalculationTypeEnum PriceCalculationType
    {
        get => (PriceCalculationTypeEnum)PriceCalculationTypeId;
        set => PriceCalculationTypeId = (int)value;
    }

    #endregion
}
