using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    public partial class OrderItemMap : NopEntityTypeConfiguration<OrderItem>
    {
        public override void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            base.Configure(builder);
            builder.ToTable("OrderItem");
            builder.HasKey(orderItem => orderItem.Id);

            builder.Property(orderItem => orderItem.UnitPriceInclTax);
            builder.Property(orderItem => orderItem.UnitPriceExclTax);
            builder.Property(orderItem => orderItem.PriceInclTax);
            builder.Property(orderItem => orderItem.PriceExclTax);
            builder.Property(orderItem => orderItem.DiscountAmountInclTax);
            builder.Property(orderItem => orderItem.DiscountAmountExclTax);
            builder.Property(orderItem => orderItem.OriginalProductCost);
            builder.Property(orderItem => orderItem.ItemWeight);


            builder.HasOne(orderItem => orderItem.Order)
                .WithMany(o => o.OrderItems)
                .IsRequired(true)
                .HasForeignKey(orderItem => orderItem.OrderId);

            builder.HasOne(orderItem => orderItem.Product)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey(orderItem => orderItem.ProductId);
        }
    }
}