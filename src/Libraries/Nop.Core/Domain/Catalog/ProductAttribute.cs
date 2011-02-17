
using System.Collections.Generic;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product attribute
    /// </summary>
    public partial class ProductAttribute : BaseEntity, ILocalizedEntity
    {
        public ProductAttribute() 
        {
            this.ProductVariantAttributes = new List<ProductVariantAttribute>();
        }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the product variant attributes
        /// </summary>
        public virtual ICollection<ProductVariantAttribute> ProductVariantAttributes { get; set; }
        
    }
}
