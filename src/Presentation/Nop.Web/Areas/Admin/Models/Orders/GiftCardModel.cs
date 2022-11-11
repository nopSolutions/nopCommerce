using System;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a gift card model
    /// </summary>
    public partial record GiftCardModel: BaseNopEntityModel
    {
        #region Ctor

        public GiftCardModel()
        {
            GiftCardUsageHistorySearchModel = new GiftCardUsageHistorySearchModel();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.GiftCards.Fields.GiftCardType")]
        public int GiftCardTypeId { get; set; }

        [NopResourceDisplayName("Admin.GiftCards.Fields.OrderId")]
        public int? PurchasedWithOrderId { get; set; }

        [NopResourceDisplayName("Admin.GiftCards.Fields.CustomOrderNumber")]
        public string PurchasedWithOrderNumber { get; set; }

        [NopResourceDisplayName("Admin.GiftCards.Fields.Amount")]
        public decimal Amount { get; set; }

        [NopResourceDisplayName("Admin.GiftCards.Fields.Amount")]
        public string AmountStr { get; set; }

        [NopResourceDisplayName("Admin.GiftCards.Fields.RemainingAmount")]
        public string RemainingAmountStr { get; set; }

        [NopResourceDisplayName("Admin.GiftCards.Fields.IsGiftCardActivated")]
        public bool IsGiftCardActivated { get; set; }

        [NopResourceDisplayName("Admin.GiftCards.Fields.GiftCardCouponCode")]
        public string GiftCardCouponCode { get; set; }

        [NopResourceDisplayName("Admin.GiftCards.Fields.RecipientName")]
        public string RecipientName { get; set; }

        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Admin.GiftCards.Fields.RecipientEmail")]
        public string RecipientEmail { get; set; }

        [NopResourceDisplayName("Admin.GiftCards.Fields.SenderName")]
        public string SenderName { get; set; }

        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Admin.GiftCards.Fields.SenderEmail")]
        public string SenderEmail { get; set; }

        [NopResourceDisplayName("Admin.GiftCards.Fields.Message")]
        public string Message { get; set; }

        [NopResourceDisplayName("Admin.GiftCards.Fields.IsRecipientNotified")]
        public bool IsRecipientNotified { get; set; }

        [NopResourceDisplayName("Admin.GiftCards.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }

        public GiftCardUsageHistorySearchModel GiftCardUsageHistorySearchModel { get; set; }

        #endregion
    }
}