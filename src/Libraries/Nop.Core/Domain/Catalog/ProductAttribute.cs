
using System.Collections.Generic;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product attribute
    /// </summary>
    public partial class ProductAttribute : BaseEntity, ILocalizedEntity
    {
        private ICollection<ProductVariantAttribute> _productVariantAttributes;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets the product variant attributes
        /// </summary>
        public virtual ICollection<ProductVariantAttribute> ProductVariantAttributes
        {
            get { return _productVariantAttributes ?? (_productVariantAttributes = new List<ProductVariantAttribute>()); }
            protected set { _productVariantAttributes = value; }
        }
        
    }
}
