using LinqToDB.Mapping;
using Nop.Core.Domain.Tax;

namespace Nop.Data.Mapping.Tax
{
    /// <summary>
    /// Represents a tax category mapping configuration
    /// </summary>
    public partial class TaxCategoryMap : NopEntityTypeConfiguration<TaxCategory>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<TaxCategory> builder)
        {
            builder.HasTableName(nameof(TaxCategory));

            builder.Property(taxCategory => taxCategory.Name).HasLength(400).IsNullable(false);
            builder.Property(taxcategory => taxcategory.DisplayOrder);
        }

        #endregion
    }
}