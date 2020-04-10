namespace Nop.Core.Domain.Shipping
{
    /// <summary>
    /// Represents a shipping option
    /// </summary>
    public partial class ShippingOption
    {
        /// <summary>
        /// Gets or sets the system name of shipping rate computation method
        /// </summary>
        public string ShippingRateComputationMethodSystemName { get; set; }

        /// <summary>
        /// Gets or sets a shipping rate (without discounts, additional shipping charges, etc)
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// Gets or sets a shipping option name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a shipping option description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a transit days
        /// </summary>
        public int? TransitDays { get; set; }
    }
}
