using LinqToDB.Mapping;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

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

            builder.Property(orderItem => orderItem.UnitPriceInclTax).HasDecimal();
            builder.Property(orderItem => orderItem.UnitPriceExclTax).HasDecimal();
            builder.Property(orderItem => orderItem.PriceInclTax).HasDecimal();
            builder.Property(orderItem => orderItem.PriceExclTax).HasDecimal();
            builder.Property(orderItem => orderItem.DiscountAmountInclTax).HasDecimal();
            builder.Property(orderItem => orderItem.DiscountAmountExclTax).HasDecimal();
            builder.Property(orderItem => orderItem.OriginalProductCost).HasDecimal();
            builder.Property(orderItem => orderItem.ItemWeight).HasDecimal();

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