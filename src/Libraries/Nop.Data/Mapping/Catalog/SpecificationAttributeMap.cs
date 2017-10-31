using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class SpecificationAttributeMap : NopEntityTypeConfiguration<SpecificationAttribute>
    {
        public override void Configure(EntityTypeBuilder<SpecificationAttribute> builder)
        {
            base.Configure(builder);
            builder.ToTable("SpecificationAttribute");
            builder.HasKey(sa => sa.Id);
            builder.Property(sa => sa.Name).IsRequired();
        }
    }
}