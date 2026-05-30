namespace Nop.Core.Domain.Discounts;

public abstract partial class DiscountMapping : BaseEntity
{
    /// <summary>
    /// Gets the entity identifier
    /// </summary>
    public new int Id { get; }

    /// <summary>
    /// Gets or sets the discount identifier
    /// </summary>
    public int DiscountId { get; set; }

    /// <summary>
    /// Gets or sets the entity identifier
    /// </summary>
    public abstract int EntityId { get; set; }
}