using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    /// <summary>
    /// Represents an order mapping configuration
    /// </summary>
    public partial class OrderMap : NopEntityTypeConfiguration<Order>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable(nameof(Order));
            builder.HasKey(order => order.Id);

            builder.Property(order => order.CurrencyRate).HasColumnType("decimal(18, 8)");
            builder.Property(order => order.OrderSubtotalInclTax).HasColumnType("decimal(18, 4)");
            builder.Property(order => order.OrderSubtotalExclTax).HasColumnType("decimal(18, 4)");
            builder.Property(order => order.OrderSubTotalDiscountInclTax).HasColumnType("decimal(18, 4)");
            builder.Property(order => order.OrderSubTotalDiscountExclTax).HasColumnType("decimal(18, 4)");
            builder.Property(order => order.OrderShippingInclTax).HasColumnType("decimal(18, 4)");
            builder.Property(order => order.OrderShippingExclTax).HasColumnType("decimal(18, 4)");
            builder.Property(order => order.PaymentMethodAdditionalFeeInclTax).HasColumnType("decimal(18, 4)");
            builder.Property(order => order.PaymentMethodAdditionalFeeExclTax).HasColumnType("decimal(18, 4)");
            builder.Property(order => order.OrderTax).HasColumnType("decimal(18, 4)");
            builder.Property(order => order.OrderDiscount).HasColumnType("decimal(18, 4)");
            builder.Property(order => order.OrderTotal).HasColumnType("decimal(18, 4)");
            builder.Property(order => order.RefundedAmount).HasColumnType("decimal(18, 4)");
            builder.Property(order => order.CustomOrderNumber).IsRequired();

            builder.HasOne(order => order.Customer)
                .WithMany()
                .HasForeignKey(order => order.CustomerId)
                .IsRequired();

            builder.HasOne(order => order.BillingAddress)
                .WithMany()
                .HasForeignKey(order => order.BillingAddressId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(order => order.ShippingAddress)
                .WithMany()
                .HasForeignKey(order => order.ShippingAddressId);

            builder.HasOne(order => order.PickupAddress)
                .WithMany()
                .HasForeignKey(order => order.PickupAddressId);

            builder.Ignore(order => order.OrderStatus);
            builder.Ignore(order => order.PaymentStatus);
            builder.Ignore(order => order.ShippingStatus);
            builder.Ignore(order => order.CustomerTaxDisplayType);

            base.Configure(builder);
        }

        #endregion
    }
}