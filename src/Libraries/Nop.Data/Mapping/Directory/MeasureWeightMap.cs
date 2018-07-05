using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Directory;

namespace Nop.Data.Mapping.Directory
{
    /// <summary>
    /// Represents a measure weight mapping configuration
    /// </summary>
    public partial class MeasureWeightMap : NopEntityTypeConfiguration<MeasureWeight>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<MeasureWeight> builder)
        {
            builder.ToTable(nameof(MeasureWeight));
            builder.HasKey(weight => weight.Id);

            builder.Property(weight => weight.Name).HasMaxLength(100).IsRequired();
            builder.Property(weight => weight.SystemKeyword).HasMaxLength(100).IsRequired();
            builder.Property(weight => weight.Ratio).HasColumnType("decimal(18, 8)");

            base.Configure(builder);
        }

        #endregion
    }
}