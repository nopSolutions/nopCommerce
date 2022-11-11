
namespace Nop.Plugin.Shipping.FixedByWeightByTotal
{
    /// <summary>
    /// Represents constants of the "Fixed or by weight" shipping plugin
    /// </summary>
    public static class FixedByWeightByTotalDefaults
    {
        /// <summary>
        /// The key of the settings to save fixed rate of the shipping method
        /// </summary>
        public const string FixedRateSettingsKey = "ShippingRateComputationMethod.FixedByWeightByTotal.Rate.ShippingMethodId{0}";

        /// <summary>
        /// The key of the settings to save transit days of the shipping method
        /// </summary>
        public const string TransitDaysSettingsKey = "ShippingRateComputationMethod.FixedByWeightByTotal.TransitDays.ShippingMethodId{0}";
    }
}
