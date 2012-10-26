using Nop.Core;

namespace Nop.Plugin.Shipping.ByWeight.Domain
{
    /// <summary>
    /// Represents a shipping by weight record
    /// </summary>
    public partial class ShippingByWeightRecord : BaseEntity
    {
        /// <summary>
        /// Gets or sets the country identifier
        /// </summary>
        public virtual int CountryId { get; set; }

        /// <summary>
        /// Gets or sets the state/province identifier
        /// </summary>
        public virtual int StateProvinceId { get; set; }

        /// <summary>
        /// Gets or sets the zip
        /// </summary>
        public virtual string Zip { get; set; }

        /// <summary>
        /// Gets or sets the shipping method identifier
        /// </summary>
        public virtual int ShippingMethodId { get; set; }

        /// <summary>
        /// Gets or sets the "from" value
        /// </summary>
        public virtual decimal From { get; set; }

        /// <summary>
        /// Gets or sets the "to" value
        /// </summary>
        public virtual decimal To { get; set; }

        /// <summary>
        /// Gets or sets the additional fixed cost
        /// </summary>
        public virtual decimal AdditionalFixedCost { get; set; }

        /// <summary>
        /// Gets or sets the shipping charge percentage (of subtotal)
        /// </summary>
        public virtual decimal PercentageRateOfSubtotal { get; set; }

        /// <summary>
        /// Gets or sets the shipping charge amount (per weight unit)
        /// </summary>
        public virtual decimal RatePerWeightUnit { get; set; }

        /// <summary>
        /// Gets or sets the lower weight limit
        /// </summary>
        public virtual decimal LowerWeightLimit { get; set; }
    }
}