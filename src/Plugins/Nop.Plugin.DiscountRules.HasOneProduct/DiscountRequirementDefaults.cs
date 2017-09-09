
namespace Nop.Plugin.DiscountRules.HasOneProduct
{
    /// <summary>
    /// Represents constants for the discount requirement rule
    /// </summary>
    public static class DiscountRequirementDefaults
    {
        /// <summary>
        /// The system name of the discount requirement rule
        /// </summary>
        public const string SystemName = "DiscountRequirement.HasOneProduct";

        /// <summary>
        /// The key of the settings to save restricted product identifiers
        /// </summary>
        public const string SettingsKey = "DiscountRequirement.RestrictedProductIds-{0}";

        /// <summary>
        /// The HTML field prefix for discount requirements
        /// </summary>
        public const string HtmlFieldPrefix = "DiscountRulesHasOneProduct{0}";
    }
}
