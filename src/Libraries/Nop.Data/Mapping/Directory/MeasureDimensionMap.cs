using LinqToDB.Mapping;
using Nop.Core.Domain.Directory;

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

            builder.Property(dimension => dimension.Name).HasLength(100);
            builder.Property(dimension => dimension.SystemKeyword).HasLength(100);
            builder.HasColumn(dimension => dimension.Name).IsColumnRequired();
            builder.HasColumn(dimension => dimension.SystemKeyword).IsColumnRequired();
            builder.Property(dimension => dimension.Ratio).HasDbType("decimal(18, 8)");
            builder.Property(measuredimension => measuredimension.DisplayOrder);
        }

        #endregion
    }
}