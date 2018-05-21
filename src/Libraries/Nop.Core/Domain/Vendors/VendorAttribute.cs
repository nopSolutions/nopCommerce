using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Vendors
{
    /// <summary>
    /// Represents a vendor attribute
    /// </summary>
    public partial class VendorAttribute : BaseEntity, ILocalizedEntity
    {
        private ICollection<VendorAttributeValue> _vendorAttributeValues;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the attribute is required
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the attribute control type identifier
        /// </summary>
        public int AttributeControlTypeId { get; set; }

        /// <summary>
        /// Gets the attribute control type
        /// </summary>
        public AttributeControlType AttributeControlType
        {
            get => (AttributeControlType)AttributeControlTypeId;
            set => AttributeControlTypeId = (int)value;
        }

        /// <summary>
        /// Gets the vendor attribute values
        /// </summary>
        public virtual ICollection<VendorAttributeValue> VendorAttributeValues
        {
            get => _vendorAttributeValues ?? (_vendorAttributeValues = new List<VendorAttributeValue>());
            protected set => _vendorAttributeValues = value;
        }
    }
}