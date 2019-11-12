using LinqToDB.Mapping;
using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    /// <summary>
    /// Represents a product attribute combination mapping configuration
    /// </summary>
    public partial class ProductAttributeCombinationMap : NopEntityTypeConfiguration<ProductAttributeCombination>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ProductAttributeCombination> builder)
        {
            builder.HasTableName(nameof(ProductAttributeCombination));

            builder.Property(combination => combination.Sku).HasLength(400);
            builder.Property(combination => combination.ManufacturerPartNumber).HasLength(400);
            builder.Property(combination => combination.Gtin).HasLength(400);
            builder.Property(combination => combination.OverriddenPrice).HasDbType("decimal(18, 4)");

            builder.Property(productattributecombination => productattributecombination.ProductId);
            builder.Property(productattributecombination => productattributecombination.AttributesXml);
            builder.Property(productattributecombination => productattributecombination.StockQuantity);
            builder.Property(productattributecombination => productattributecombination.AllowOutOfStockOrders);
            builder.Property(productattributecombination => productattributecombination.NotifyAdminForQuantityBelow);
            builder.Property(productattributecombination => productattributecombination.PictureId);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(combination => combination.Product)
            //    .WithMany(product => product.ProductAttributeCombinations)
            //    .HasForeignKey(combination => combination.ProductId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}