using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;
using Nop.Data.Data;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a product product attribute mapping configuration
    /// </summary>
    public partial class ProductAttributeMappingMap : NopEntityTypeConfiguration<ProductAttributeMapping>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ProductAttributeMapping> builder)
        {
            builder.HasTableName(NopMappingDefaults.ProductProductAttributeTable);

            builder.Property(productattributemapping => productattributemapping.ProductId);
            builder.Property(productattributemapping => productattributemapping.ProductAttributeId);
            builder.Property(productattributemapping => productattributemapping.TextPrompt);
            builder.Property(productattributemapping => productattributemapping.IsRequired);
            builder.Property(productattributemapping => productattributemapping.AttributeControlTypeId);
            builder.Property(productattributemapping => productattributemapping.DisplayOrder);
            builder.Property(productattributemapping => productattributemapping.ValidationMinLength);
            builder.Property(productattributemapping => productattributemapping.ValidationMaxLength);
            builder.Property(productattributemapping => productattributemapping.ValidationFileAllowedExtensions);
            builder.Property(productattributemapping => productattributemapping.ValidationFileMaximumSize);
            builder.Property(productattributemapping => productattributemapping.DefaultValue);
            builder.Property(productattributemapping => productattributemapping.ConditionAttributeXml);

            builder.Ignore(pam => pam.AttributeControlType);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(productAttributeMapping => productAttributeMapping.Product)
            //    .WithMany(product => product.ProductAttributeMappings)
            //    .HasForeignKey(productAttributeMapping => productAttributeMapping.ProductId)
            //    .IsColumnRequired();

            //builder.HasOne(productAttributeMapping => productAttributeMapping.ProductAttribute)
            //    .WithMany()
            //    .HasForeignKey(productAttributeMapping => productAttributeMapping.ProductAttributeId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}