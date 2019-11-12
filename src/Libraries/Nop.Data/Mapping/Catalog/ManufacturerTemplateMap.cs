using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a manufacturer template mapping configuration
    /// </summary>
    public partial class ManufacturerTemplateMap : NopEntityTypeConfiguration<ManufacturerTemplate>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ManufacturerTemplate> builder)
        {
            builder.HasTableName(nameof(ManufacturerTemplate));

            builder.Property(template => template.Name).HasLength(400);
            builder.Property(template => template.ViewPath).HasLength(400);
            builder.HasColumn(template => template.Name).IsColumnRequired();
            builder.HasColumn(template => template.ViewPath).IsColumnRequired();
            builder.Property(manufacturertemplate => manufacturertemplate.DisplayOrder);
        }

        #endregion
    }
}