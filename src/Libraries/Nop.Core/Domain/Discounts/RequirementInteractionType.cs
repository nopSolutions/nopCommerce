namespace Nop.Core.Domain.Discounts
{
    /// <summary>
    /// Represents an interaction type with subsequent discount requirement
    /// </summary>
    public enum RequirementInteractionType
    {
        /// <summary>
        /// Both (current and subsequent) requirements must be met
        /// </summary>
        And = 0,

        /// <summary>
        /// At least one of requirements must be met (current or subsequent)
        /// </summary>
        Or = 2
    }
}