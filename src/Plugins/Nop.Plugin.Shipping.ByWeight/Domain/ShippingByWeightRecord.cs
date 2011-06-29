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
        /// Gets or sets a value indicating whether to use percentage
        /// </summary>
        public virtual bool UsePercentage { get; set; }

        /// <summary>
        /// Gets or sets the shipping charge percentage
        /// </summary>
        public virtual decimal ShippingChargePercentage { get; set; }

        /// <summary>
        /// Gets or sets the shipping charge amount
        /// </summary>
        public virtual decimal ShippingChargeAmount { get; set; }
    }
}