using LinqToDB;
using LinqToDB.Mapping;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    /// <summary>
    /// Represents an order item mapping configuration
    /// </summary>
    public partial class OrderItemMap : NopEntityTypeConfiguration<OrderItem>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<OrderItem> builder)
        {
            builder.HasTableName(nameof(OrderItem));

            builder.Property(orderItem => orderItem.UnitPriceInclTax).HasDataType(DataType.Decimal).HasPrecision(18).HasScale(4);
            builder.Property(orderItem => orderItem.UnitPriceExclTax).HasDataType(DataType.Decimal).HasPrecision(18).HasScale(4);
            builder.Property(orderItem => orderItem.PriceInclTax).HasDataType(DataType.Decimal).HasPrecision(18).HasScale(4);
            builder.Property(orderItem => orderItem.PriceExclTax).HasDataType(DataType.Decimal).HasPrecision(18).HasScale(4);
            builder.Property(orderItem => orderItem.DiscountAmountInclTax).HasDataType(DataType.Decimal).HasPrecision(18).HasScale(4);
            builder.Property(orderItem => orderItem.DiscountAmountExclTax).HasDataType(DataType.Decimal).HasPrecision(18).HasScale(4);
            builder.Property(orderItem => orderItem.OriginalProductCost).HasDataType(DataType.Decimal).HasPrecision(18).HasScale(4);
            builder.Property(orderItem => orderItem.ItemWeight).HasDataType(DataType.Decimal).HasPrecision(18).HasScale(4);

            builder.Property(orderItem => orderItem.OrderItemGuid);
            builder.Property(orderItem => orderItem.OrderId);
            builder.Property(orderItem => orderItem.ProductId);
            builder.Property(orderItem => orderItem.Quantity);
            builder.Property(orderItem => orderItem.AttributeDescription);
            builder.Property(orderItem => orderItem.AttributesXml);
            builder.Property(orderItem => orderItem.DownloadCount);
            builder.Property(orderItem => orderItem.IsDownloadActivated);
            builder.Property(orderItem => orderItem.LicenseDownloadId);
            builder.Property(orderItem => orderItem.RentalStartDateUtc);
            builder.Property(orderItem => orderItem.RentalEndDateUtc);
        }

        #endregion
    }
}