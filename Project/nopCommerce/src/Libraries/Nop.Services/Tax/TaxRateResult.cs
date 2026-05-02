namespace Nop.Services.Tax;

/// <summary>
/// Represents a result of tax rate calculation
/// </summary>
public partial class TaxRateResult : BaseNopResult
{
    /// <summary>
    /// Gets or sets a tax rate
    /// </summary>
    public decimal TaxRate { get; set; }
}