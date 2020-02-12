using LinqToDB.Mapping;
using Nop.Core.Domain.Directory;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Directory
{
    /// <summary>
    /// Represents a measure dimension mapping configuration
    /// </summary>
    public partial class MeasureDimensionMap : NopEntityTypeConfiguration<MeasureDimension>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<MeasureDimension> builder)
        {
            builder.HasTableName(nameof(MeasureDimension));

            builder.Property(dimension => dimension.Name).HasLength(100).IsNullable(false);
            builder.Property(dimension => dimension.SystemKeyword).HasLength(100).IsNullable(false);
            builder.Property(dimension => dimension.Ratio).HasDecimal(18, 8);
            builder.Property(measuredimension => measuredimension.DisplayOrder);
        }

        #endregion
    }
}