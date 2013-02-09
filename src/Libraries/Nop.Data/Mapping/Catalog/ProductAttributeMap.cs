using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class ProductAttributeMap : EntityTypeConfiguration<ProductAttribute>
    {
        public ProductAttributeMap()
        {
            this.ToTable("ProductAttribute");
            this.HasKey(pa => pa.Id);
            this.Property(pa => pa.Name).IsRequired();
        }
    }
}