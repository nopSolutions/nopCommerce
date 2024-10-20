namespace Nop.Core.Domain.Shipping;

/// <summary>
/// Represents a shipping method-state/provicne mapping class
/// </summary>
public partial class ShippingMethodStateProvinceMapping : BaseEntity
{
    /// <summary>
    /// Gets or sets the shipping method identifier
    /// </summary>
    public int ShippingMethodId { get; set; }

    /// <summary>
    /// Gets or sets the country identifier
    /// </summary>
    public int CountryId { get; set; }

    /// <summary>
    /// Gets or sets the state province identifier
    /// </summary>
    public int StateProvinceId { get; set; }
}