namespace Nop.Services.Discounts;

/// <summary>
/// Represents a result of discount validation
/// </summary>
public partial class DiscountValidationResult : BaseNopResult
{
    /// <summary>
    /// Gets or sets a value indicating whether discount is valid
    /// </summary>
    public bool IsValid { get; set; }
}