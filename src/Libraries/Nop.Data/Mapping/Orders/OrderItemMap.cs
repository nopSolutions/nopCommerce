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

            builder.Property(orderItem => orderItem.UnitPriceInclTax).HasDbType("decimal(18, 4)");
            builder.Property(orderItem => orderItem.UnitPriceExclTax).HasDbType("decimal(18, 4)");
            builder.Property(orderItem => orderItem.PriceInclTax).HasDbType("decimal(18, 4)");
            builder.Property(orderItem => orderItem.PriceExclTax).HasDbType("decimal(18, 4)");
            builder.Property(orderItem => orderItem.DiscountAmountInclTax).HasDbType("decimal(18, 4)");
            builder.Property(orderItem => orderItem.DiscountAmountExclTax).HasDbType("decimal(18, 4)");
            builder.Property(orderItem => orderItem.OriginalProductCost).HasDbType("decimal(18, 4)");
            builder.Property(orderItem => orderItem.ItemWeight).HasDbType("decimal(18, 4)");

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

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(orderItem => orderItem.Order)
            //    .WithMany(order => order.OrderItems)
            //    .HasForeignKey(orderItem => orderItem.OrderId)
            //    .IsColumnRequired();

            //builder.HasOne(orderItem => orderItem.Product)
            //    .WithMany()
            //    .HasForeignKey(orderItem => orderItem.ProductId)
            //    .IsColumnRequired();
        }

        #endregion
    }
}