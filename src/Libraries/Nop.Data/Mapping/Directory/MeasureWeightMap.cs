using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Directory;

namespace Nop.Data.Mapping.Directory
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class MeasureWeightMap : NopEntityTypeConfiguration<MeasureWeight>
    {
        public override void Configure(EntityTypeBuilder<MeasureWeight> builder)
        {
            base.Configure(builder);
            builder.ToTable("MeasureWeight");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Name).IsRequired().HasMaxLength(100);
            builder.Property(m => m.SystemKeyword).IsRequired().HasMaxLength(100);
            builder.Property(m => m.Ratio);
        }
    }
}