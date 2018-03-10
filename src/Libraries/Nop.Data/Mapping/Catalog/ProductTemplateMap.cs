using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class ProductTemplateMap : NopEntityTypeConfiguration<ProductTemplate>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ProductTemplateMap()
        {
            this.ToTable("ProductTemplate");
            this.HasKey(p => p.Id);
            this.Property(p => p.Name).IsRequired().HasMaxLength(400);
            this.Property(p => p.ViewPath).IsRequired().HasMaxLength(400);
        }
    }
}