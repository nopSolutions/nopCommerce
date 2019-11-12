using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a manufacturer mapping configuration
    /// </summary>
    public partial class ManufacturerMap : NopEntityTypeConfiguration<Manufacturer>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<Manufacturer> builder)
        {
            builder.HasTableName(nameof(Manufacturer));

            builder.HasColumn(manufacturer => manufacturer.Name).IsColumnRequired();
            builder.Property(manufacturer => manufacturer.Name).HasLength(400);
            builder.Property(manufacturer => manufacturer.MetaKeywords).HasLength(400);
            builder.Property(manufacturer => manufacturer.MetaTitle).HasLength(400);
            builder.Property(manufacturer => manufacturer.PriceRanges).HasLength(400);
            builder.Property(manufacturer => manufacturer.PageSizeOptions).HasLength(200);

            builder.Property(manufacturer => manufacturer.Description);
            builder.Property(manufacturer => manufacturer.ManufacturerTemplateId);
            builder.Property(manufacturer => manufacturer.MetaDescription);
            builder.Property(manufacturer => manufacturer.PictureId);
            builder.Property(manufacturer => manufacturer.PageSize);
            builder.Property(manufacturer => manufacturer.AllowCustomersToSelectPageSize);
            builder.Property(manufacturer => manufacturer.SubjectToAcl);
            builder.Property(manufacturer => manufacturer.LimitedToStores);
            builder.Property(manufacturer => manufacturer.Published);
            builder.Property(manufacturer => manufacturer.Deleted);
            builder.Property(manufacturer => manufacturer.DisplayOrder);
            builder.Property(manufacturer => manufacturer.CreatedOnUtc);
            builder.Property(manufacturer => manufacturer.UpdatedOnUtc);
        }

        #endregion
    }
}