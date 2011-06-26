using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents a gift card
    /// </summary>
    public partial class GiftCard : BaseEntity
    {
        private ICollection<GiftCardUsageHistory> _giftCardUsageHistory;
        
        /// <summary>
        /// Gets or sets the associated order product variant identifier
        /// </summary>
        public virtual int? PurchasedWithOrderProductVariantId { get; set; }

        /// <summary>
        /// Gets or sets the gift card type identifier
        /// </summary>
        public virtual int GiftCardTypeId { get; set; }

        /// <summary>
        /// Gets or sets the amount
        /// </summary>
        public virtual decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gift card is activated
        /// </summary>
        public virtual bool IsGiftCardActivated { get; set; }

        /// <summary>
        /// Gets or sets a gift card coupon code
        /// </summary>
        public virtual string GiftCardCouponCode { get; set; }

        /// <summary>
        /// Gets or sets a recipient name
        /// </summary>
        public virtual string RecipientName { get; set; }

        /// <summary>
        /// Gets or sets a recipient email
        /// </summary>
        public virtual string RecipientEmail { get; set; }

        /// <summary>
        /// Gets or sets a sender name
        /// </summary>
        public virtual string SenderName { get; set; }

        /// <summary>
        /// Gets or sets a sender email
        /// </summary>
        public virtual string SenderEmail { get; set; }

        /// <summary>
        /// Gets or sets a message
        /// </summary>
        public virtual string Message { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether recipient is notified
        /// </summary>
        public virtual bool IsRecipientNotified { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }
        
        /// <summary>
        /// Gets or sets the gift card usage history
        /// </summary>
        public virtual ICollection<GiftCardUsageHistory> GiftCardUsageHistory
        {
            get { return _giftCardUsageHistory ?? (_giftCardUsageHistory = new List<GiftCardUsageHistory>()); }
            protected set { _giftCardUsageHistory = value; }
        }
        
        /// <summary>
        /// Gets or sets the gift card type
        /// </summary>
        public virtual GiftCardType GiftCardType
        {
            get
            {
                return (GiftCardType)this.GiftCardTypeId;
            }
            set
            {
                this.GiftCardTypeId = (int)value;
            }
        }
        
        /// <summary>
        /// Gets or sets the associated order product variant
        /// </summary>
        public virtual OrderProductVariant PurchasedWithOrderProductVariant { get; set; }

        #region Methods

        /// <summary>
        /// Gets a gift card remaining amount
        /// </summary>
        /// <returns>Gift card remaining amount</returns>
        public virtual decimal GetGiftCardRemainingAmount()
        {
            decimal result = this.Amount;

            foreach (var gcuh in this.GiftCardUsageHistory)
                result -= gcuh.UsedValue;

            if (result < decimal.Zero)
                result = decimal.Zero;

            return result;
        }

        /// <summary>
        /// Is gift card valid
        /// </summary>
        /// <returns>Result</returns>
        public virtual bool IsGiftCardValid()
        {
            if (!this.IsGiftCardActivated)
                return false;

            decimal remainingAmount = GetGiftCardRemainingAmount();
            if (remainingAmount > decimal.Zero)
                return true;

            return false;
        }

        #endregion
    }
}
