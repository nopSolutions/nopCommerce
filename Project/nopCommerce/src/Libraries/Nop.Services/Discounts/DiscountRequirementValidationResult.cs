namespace Nop.Services.Discounts;

/// <summary>
/// Represents a result of discount requirement validation
/// </summary>
public partial class DiscountRequirementValidationResult
{
    /// <summary>
    /// Gets or sets a value indicating whether discount is valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets an error that a customer should see when entering a coupon code (in case if "IsValid" is set to "false")
    /// </summary>
    public string UserError { get; set; }
}