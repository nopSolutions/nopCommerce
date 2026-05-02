using Newtonsoft.Json;
using Nop.Core.Domain.Discounts;
using Newtonsoft.Json.Converters;

namespace Nop.Services.Installation.SampleData;

/// <summary>
/// Represents a sample discount
/// </summary>
public partial class SampleDiscount
{
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use percentage
    /// </summary>
    public bool UsePercentage { get; set; }

    /// <summary>
    /// Gets or sets the discount percentage
    /// </summary>
    public decimal DiscountPercentage { get; set; }

    /// <summary>
    /// Gets or sets the discount amount
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether discount requires coupon code
    /// </summary>
    public bool RequiresCouponCode { get; set; }

    /// <summary>
    /// Gets or sets the coupon code
    /// </summary>
    public string CouponCode { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the discount is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the discount type
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public DiscountType DiscountType { get; set; }

    /// <summary>
    /// Gets or sets the discount limitation
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public DiscountLimitationType DiscountLimitation { get; set; }
}
