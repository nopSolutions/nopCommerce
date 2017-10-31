using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Directory;

namespace Nop.Data.Mapping.Directory
{
    /// <summary>
    /// Mapping class
    /// </summary>
    public partial class MeasureDimensionMap : NopEntityTypeConfiguration<MeasureDimension>
    {
        public override void Configure(EntityTypeBuilder<MeasureDimension> builder)
        {
            base.Configure(builder);
            builder.ToTable("MeasureDimension");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Name).IsRequired().HasMaxLength(100);
            builder.Property(m => m.SystemKeyword).IsRequired().HasMaxLength(100);
            builder.Property(m => m.Ratio);
        }
        public MeasureDimensionMap()
        {
            
        }
    }
}