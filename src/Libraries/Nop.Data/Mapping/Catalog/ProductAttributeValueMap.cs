using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a product attribute value mapping configuration
    /// </summary>
    public partial class ProductAttributeValueMap : NopEntityTypeConfiguration<ProductAttributeValue>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ProductAttributeValue> builder)
        {
            builder.HasTableName(nameof(ProductAttributeValue));

            builder.Property(value => value.Name).HasLength(400);
            builder.HasColumn(value => value.Name).IsColumnRequired();
            builder.Property(value => value.ColorSquaresRgb).HasLength(100);
            builder.Property(value => value.PriceAdjustment).HasDbType("decimal(18, 4)");
            builder.Property(value => value.WeightAdjustment).HasDbType("decimal(18, 4)");
            builder.Property(value => value.Cost).HasDbType("decimal(18, 4)");

            builder.Property(productattributevalue => productattributevalue.ProductAttributeMappingId);
            builder.Property(productattributevalue => productattributevalue.AttributeValueTypeId);
            builder.Property(productattributevalue => productattributevalue.AssociatedProductId);
            builder.Property(productattributevalue => productattributevalue.ImageSquaresPictureId);
            builder.Property(productattributevalue => productattributevalue.PriceAdjustmentUsePercentage);
            builder.Property(productattributevalue => productattributevalue.CustomerEntersQty);
            builder.Property(productattributevalue => productattributevalue.Quantity);
            builder.Property(productattributevalue => productattributevalue.IsPreSelected);
            builder.Property(productattributevalue => productattributevalue.DisplayOrder);
            builder.Property(productattributevalue => productattributevalue.PictureId);
            builder.Property(productattributevalue => productattributevalue.AttributeValueType);

            builder.Ignore(value => value.AttributeValueType);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(value => value.ProductAttributeMapping)
            //    .WithMany(productAttributeMapping => productAttributeMapping.ProductAttributeValues)
            //    .HasForeignKey(value => value.ProductAttributeMappingId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}