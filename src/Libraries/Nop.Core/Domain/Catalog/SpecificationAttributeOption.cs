using System.Collections.Generic;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a specification attribute option
    /// </summary>
    public partial class SpecificationAttributeOption : BaseEntity, ILocalizedEntity
    {
        private ICollection<ProductSpecificationAttribute> _productSpecificationAttributes;

        /// <summary>
        /// Gets or sets the specification attribute identifier
        /// </summary>
        public int SpecificationAttributeId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
        
        /// <summary>
        /// Gets or sets the specification attribute
        /// </summary>
        public virtual SpecificationAttribute SpecificationAttribute { get; set; }

        /// <summary>
        /// Gets or sets the product specification attribute
        /// </summary>
        public virtual ICollection<ProductSpecificationAttribute> ProductSpecificationAttributes
        {
            get { return _productSpecificationAttributes ?? (_productSpecificationAttributes = new List<ProductSpecificationAttribute>()); }
            protected set { _productSpecificationAttributes = value; }
        }
    }
}
