using LinqToDB.Mapping;
using Nop.Core.Domain.Directory;
using Nop.Data.Extensions;

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

            builder.Property(weight => weight.Name).HasLength(100).IsNullable(false);
            builder.Property(weight => weight.SystemKeyword).HasLength(100).IsNullable(false);
            builder.Property(weight => weight.Ratio).HasDecimal(18, 8);
            builder.Property(measureweight => measureweight.DisplayOrder);
        }

        #endregion
    }
}