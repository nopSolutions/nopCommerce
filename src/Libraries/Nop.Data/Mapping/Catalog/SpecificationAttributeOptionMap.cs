using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a specification attribute option mapping configuration
    /// </summary>
    public partial class SpecificationAttributeOptionMap : NopEntityTypeConfiguration<SpecificationAttributeOption>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<SpecificationAttributeOption> builder)
        {
            builder.HasTableName(nameof(SpecificationAttributeOption));

            builder.Property(option => option.Name).IsNullable(false);
            builder.Property(option => option.ColorSquaresRgb).HasLength(100);

            builder.Property(specificationattributeoption => specificationattributeoption.SpecificationAttributeId);
            builder.Property(specificationattributeoption => specificationattributeoption.DisplayOrder);
        }

        #endregion
    }
}