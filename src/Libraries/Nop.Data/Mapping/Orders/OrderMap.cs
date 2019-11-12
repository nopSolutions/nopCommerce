using LinqToDB.Mapping;
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
        public override void Configure(EntityMappingBuilder<Order> builder)
        {
            builder.HasTableName(nameof(Order));

            builder.Property(order => order.CurrencyRate).HasDbType("decimal(18, 8)");
            builder.Property(order => order.OrderSubtotalInclTax).HasDbType("decimal(18, 4)");
            builder.Property(order => order.OrderSubtotalExclTax).HasDbType("decimal(18, 4)");
            builder.Property(order => order.OrderSubTotalDiscountInclTax).HasDbType("decimal(18, 4)");
            builder.Property(order => order.OrderSubTotalDiscountExclTax).HasDbType("decimal(18, 4)");
            builder.Property(order => order.OrderShippingInclTax).HasDbType("decimal(18, 4)");
            builder.Property(order => order.OrderShippingExclTax).HasDbType("decimal(18, 4)");
            builder.Property(order => order.PaymentMethodAdditionalFeeInclTax).HasDbType("decimal(18, 4)");
            builder.Property(order => order.PaymentMethodAdditionalFeeExclTax).HasDbType("decimal(18, 4)");
            builder.Property(order => order.OrderTax).HasDbType("decimal(18, 4)");
            builder.Property(order => order.OrderDiscount).HasDbType("decimal(18, 4)");
            builder.Property(order => order.OrderTotal).HasDbType("decimal(18, 4)");
            builder.Property(order => order.RefundedAmount).HasDbType("decimal(18, 4)");
            builder.HasColumn(order => order.CustomOrderNumber).IsColumnRequired();

            builder.Property(order => order.OrderGuid);
            builder.Property(order => order.StoreId);
            builder.Property(order => order.CustomerId);
            builder.Property(order => order.BillingAddressId);
            builder.Property(order => order.ShippingAddressId);
            builder.Property(order => order.PickupAddressId);
            builder.Property(order => order.PickupInStore);
            builder.Property(order => order.OrderStatusId);
            builder.Property(order => order.ShippingStatusId);
            builder.Property(order => order.PaymentStatusId);
            builder.Property(order => order.PaymentMethodSystemName);
            builder.Property(order => order.CustomerCurrencyCode);
            builder.Property(order => order.CustomerTaxDisplayTypeId);
            builder.Property(order => order.VatNumber);
            builder.Property(order => order.TaxRates);
            builder.Property(order => order.RewardPointsHistoryEntryId);
            builder.Property(order => order.CheckoutAttributeDescription);
            builder.Property(order => order.CheckoutAttributesXml);
            builder.Property(order => order.CustomerLanguageId);
            builder.Property(order => order.AffiliateId);
            builder.Property(order => order.CustomerIp);
            builder.Property(order => order.AllowStoringCreditCardNumber);
            builder.Property(order => order.CardType);
            builder.Property(order => order.CardName);
            builder.Property(order => order.CardNumber);
            builder.Property(order => order.MaskedCreditCardNumber);
            builder.Property(order => order.CardCvv2);
            builder.Property(order => order.CardExpirationMonth);
            builder.Property(order => order.CardExpirationYear);
            builder.Property(order => order.AuthorizationTransactionId);
            builder.Property(order => order.AuthorizationTransactionCode);
            builder.Property(order => order.AuthorizationTransactionResult);
            builder.Property(order => order.CaptureTransactionId);
            builder.Property(order => order.CaptureTransactionResult);
            builder.Property(order => order.SubscriptionTransactionId);
            builder.Property(order => order.PaidDateUtc);
            builder.Property(order => order.ShippingMethod);
            builder.Property(order => order.ShippingRateComputationMethodSystemName);
            builder.Property(order => order.CustomValuesXml);
            builder.Property(order => order.Deleted);
            builder.Property(order => order.CreatedOnUtc);
            builder.Property(order => order.RedeemedRewardPointsEntryId);

            builder.Ignore(order => order.OrderStatus);
            builder.Ignore(order => order.PaymentStatus);
            builder.Ignore(order => order.ShippingStatus);
            builder.Ignore(order => order.CustomerTaxDisplayType);

            //TODO: 239 Try to add ForeignKey
            //builder.HasOne(order => order.Customer)
            //    .WithMany()
            //    .HasForeignKey(order => order.CustomerId)
            //    .IsColumnRequired();

            //builder.HasOne(order => order.BillingAddress)
            //    .WithMany()
            //    .HasForeignKey(order => order.BillingAddressId)
            //    .IsColumnRequired()
            //    .OnDelete(DeleteBehavior.Restrict);

            //builder.HasOne(order => order.ShippingAddress)
            //    .WithMany()
            //    .HasForeignKey(order => order.ShippingAddressId);

            //builder.HasOne(order => order.PickupAddress)
            //    .WithMany()
            //    .HasForeignKey(order => order.PickupAddressId);
        }

        #endregion
    }
}