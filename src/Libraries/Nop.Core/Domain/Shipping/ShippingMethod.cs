using System.Collections.Generic;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Shipping
{
    /// <summary>
    /// Represents a shipping method (used by offline shipping rate computation methods)
    /// </summary>
    public partial class ShippingMethod : BaseEntity, ILocalizedEntity
    {
        private ICollection<ShippingMethodCountryMapping> _shippingMethodCountryMappings;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the shipping method-country mappings
        /// </summary>
        public virtual ICollection<ShippingMethodCountryMapping> ShippingMethodCountryMappings
        {
            get => _shippingMethodCountryMappings ?? (_shippingMethodCountryMappings = new List<ShippingMethodCountryMapping>());
            protected set => _shippingMethodCountryMappings = value;
        }
    }
}