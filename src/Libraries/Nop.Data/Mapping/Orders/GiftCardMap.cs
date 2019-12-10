using LinqToDB.Mapping;
using Nop.Core.Domain.Orders;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Orders
{
    /// <summary>
    /// Represents a gift card mapping configuration
    /// </summary>
    public partial class GiftCardMap : NopEntityTypeConfiguration<GiftCard>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<GiftCard> builder)
        {
            builder.HasTableName(nameof(GiftCard));

            builder.Property(giftCard => giftCard.Amount).HasDecimal();

            builder.Property(giftCard => giftCard.PurchasedWithOrderItemId);
            builder.Property(giftCard => giftCard.GiftCardTypeId);
            builder.Property(giftCard => giftCard.IsGiftCardActivated);
            builder.Property(giftCard => giftCard.GiftCardCouponCode);
            builder.Property(giftCard => giftCard.RecipientName);
            builder.Property(giftCard => giftCard.RecipientEmail);
            builder.Property(giftCard => giftCard.SenderName);
            builder.Property(giftCard => giftCard.SenderEmail);
            builder.Property(giftCard => giftCard.Message);
            builder.Property(giftCard => giftCard.IsRecipientNotified);
            builder.Property(giftCard => giftCard.CreatedOnUtc);

            builder.Ignore(giftCard => giftCard.GiftCardType);
        }

        #endregion
    }
}