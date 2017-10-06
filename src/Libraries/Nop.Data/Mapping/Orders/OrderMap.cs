using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Orders;

namespace Nop.Data.Mapping.Orders
{
    public partial class OrderMap : NopEntityTypeConfiguration<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            base.Configure(builder);
            builder.ToTable("Order");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.CurrencyRate);
            builder.Property(o => o.OrderSubtotalInclTax);
            builder.Property(o => o.OrderSubtotalExclTax);
            builder.Property(o => o.OrderSubTotalDiscountInclTax);
            builder.Property(o => o.OrderSubTotalDiscountExclTax);
            builder.Property(o => o.OrderShippingInclTax);
            builder.Property(o => o.OrderShippingExclTax);
            builder.Property(o => o.PaymentMethodAdditionalFeeInclTax);
            builder.Property(o => o.PaymentMethodAdditionalFeeExclTax);
            builder.Property(o => o.OrderTax);
            builder.Property(o => o.OrderDiscount);
            builder.Property(o => o.OrderTotal);
            builder.Property(o => o.RefundedAmount);
            builder.Property(o => o.CustomOrderNumber).IsRequired();

            builder.Ignore(o => o.OrderStatus);
            builder.Ignore(o => o.PaymentStatus);
            builder.Ignore(o => o.ShippingStatus);
            builder.Ignore(o => o.CustomerTaxDisplayType);
            builder.Ignore(o => o.TaxRatesDictionary);

            builder.HasOne(o => o.Customer)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey(o => o.CustomerId);

            //code below is commented because it causes some issues on big databases - https://www.nopcommerce.com/boards/t/11126/bug-version-20-command-confirm-takes-several-minutes-using-big-databases.aspx
            //builder.HasRequired(o => o.BillingAddress).WithOptional().Map(x => x.MapKey("BillingAddressId")).WillCascadeOnDelete(false);
            //builder.HasOptional(o => o.ShippingAddress).WithOptionalDependent().Map(x => x.MapKey("ShippingAddressId")).WillCascadeOnDelete(false);
            builder.HasOne(o => o.BillingAddress)
                .WithMany()
                .IsRequired(true)
                .HasForeignKey(o => o.BillingAddressId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(o => o.ShippingAddress)
                .WithMany()
                .HasForeignKey(o => o.ShippingAddressId)
                .OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(o => o.PickupAddress)
                .WithMany()
                .HasForeignKey(o => o.PickupAddressId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}