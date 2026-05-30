namespace Nop.Core.Domain.Shipping;

/// <summary>
/// Represents a shipping method-country mapping class
/// </summary>
public partial class ShippingMethodCountryMapping : BaseEntity
{
    /// <summary>
    /// Gets or sets the shipping method identifier
    /// </summary>
    public int ShippingMethodId { get; set; }

    /// <summary>
    /// Gets or sets the country identifier
    /// </summary>
    public int CountryId { get; set; }
}