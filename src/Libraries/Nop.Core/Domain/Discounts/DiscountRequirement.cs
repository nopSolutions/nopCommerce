namespace Nop.Core.Domain.Discounts;

/// <summary>
/// Represents a discount requirement
/// </summary>
public partial class DiscountRequirement : BaseEntity
{
    /// <summary>
    /// Gets or sets the discount identifier
    /// </summary>
    public int DiscountId { get; set; }

    /// <summary>
    /// Gets or sets the discount requirement rule system name
    /// </summary>
    public string DiscountRequirementRuleSystemName { get; set; }

    /// <summary>
    /// Gets or sets the parent requirement identifier
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// Gets or sets an interaction type identifier (has a value for the group and null for the child requirements)
    /// </summary>
    public int? InteractionTypeId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this requirement has any child requirements
    /// </summary>
    public bool IsGroup { get; set; }

    /// <summary>
    /// Gets or sets an interaction type
    /// </summary>
    public RequirementGroupInteractionType? InteractionType
    {
        get => (RequirementGroupInteractionType?)InteractionTypeId;
        set => InteractionTypeId = (int?)value;
    }
}