using LinqToDB.Mapping;
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
        public override void Configure(EntityMappingBuilder<MeasureWeight> builder)
        {
            builder.HasTableName(nameof(MeasureWeight));

            builder.Property(weight => weight.Name).HasLength(100);
            builder.Property(weight => weight.SystemKeyword).HasLength(100);
            builder.HasColumn(weight => weight.Name).IsColumnRequired();
            builder.HasColumn(weight => weight.SystemKeyword).IsColumnRequired();
            builder.Property(weight => weight.Ratio).HasDbType("decimal(18, 8)");

            builder.Property(measureweight => measureweight.DisplayOrder);
        }

        #endregion
    }
}