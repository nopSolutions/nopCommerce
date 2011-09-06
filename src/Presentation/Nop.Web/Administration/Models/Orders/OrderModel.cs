using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Admin.Models.Common;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Orders
{
    public class OrderModel : BaseNopEntityModel
    {
        public OrderModel()
        {
            TaxRates = new List<TaxRate>();
            GiftCards = new List<GiftCard>();
            Items = new List<OrderProductVariantModel>();
            OrderNotes = new List<OrderNote>();
        }

        //identifiers
        [NopResourceDisplayName("Admin.Orders.Fields.ID")]
        public override int Id { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.OrderGuid")]
        public Guid OrderGuid { get; set; }

        //customer info
        [NopResourceDisplayName("Admin.Orders.Fields.Customer")]
        public int CustomerId { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CustomerEmail")]
        public string CustomerEmail { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CustomerIP")]
        public string CustomerIp { get; set; }

        [NopResourceDisplayName("Admin.Orders.Fields.Affiliate")]
        public int? AffiliateId { get; set; }

        //totals
        public bool AllowCustomersToSelectTaxDisplayType { get; set; }
        public TaxDisplayType TaxDisplayType { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.OrderSubtotalInclTax")]
        public string OrderSubtotalInclTax { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.OrderSubtotalExclTax")]
        public string OrderSubtotalExclTax { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.OrderSubTotalDiscountInclTax")]
        public string OrderSubTotalDiscountInclTax { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.OrderSubTotalDiscountExclTax")]
        public string OrderSubTotalDiscountExclTax { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.OrderShippingInclTax")]
        public string OrderShippingInclTax { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.OrderShippingExclTax")]
        public string OrderShippingExclTax { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.PaymentMethodAdditionalFeeInclTax")]
        public string PaymentMethodAdditionalFeeInclTax { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.PaymentMethodAdditionalFeeExclTax")]
        public string PaymentMethodAdditionalFeeExclTax { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Tax")]
        public string Tax { get; set; }
        public IList<TaxRate> TaxRates { get; set; }
        public bool DisplayTax { get; set; }
        public bool DisplayTaxRates { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.OrderTotalDiscount")]
        public string OrderTotalDiscount { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.RedeemedRewardPoints")]
        public int RedeemedRewardPoints { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.RedeemedRewardPoints")]
        public string RedeemedRewardPointsAmount { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.OrderTotal")]
        public string OrderTotal { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.RefundedAmount")]
        public string RefundedAmount { get; set; }

        //edit totals
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.OrderSubtotal")]
        public decimal OrderSubtotalInclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.OrderSubtotal")]
        public decimal OrderSubtotalExclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.OrderSubTotalDiscount")]
        public decimal OrderSubTotalDiscountInclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.OrderSubTotalDiscount")]
        public decimal OrderSubTotalDiscountExclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.OrderShipping")]
        public decimal OrderShippingInclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.OrderShipping")]
        public decimal OrderShippingExclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.PaymentMethodAdditionalFee")]
        public decimal PaymentMethodAdditionalFeeInclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.PaymentMethodAdditionalFee")]
        public decimal PaymentMethodAdditionalFeeExclTaxValue { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.Tax")]
        public decimal TaxValue { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.TaxRates")]
        public string TaxRatesValue { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.OrderTotalDiscount")]
        public decimal OrderTotalDiscountValue { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Edit.OrderTotal")]
        public decimal OrderTotalValue { get; set; }

        //orderstatus
        [NopResourceDisplayName("Admin.Orders.Fields.OrderStatus")]
        public string OrderStatus { get; set; }

        //payment info
        [NopResourceDisplayName("Admin.Orders.Fields.PaymentStatus")]
        public string PaymentStatus { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.PaymentMethod")]
        public string PaymentMethod { get; set; }

        //credit card info
        public bool AllowStoringCreditCardNumber { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CardType")]
        [AllowHtml]
        public string CardType { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CardName")]
        [AllowHtml]
        public string CardName { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CardNumber")]
        [AllowHtml]
        public string CardNumber { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CardCVV2")]
        [AllowHtml]
        public string CardCvv2 { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CardExpirationMonth")]
        [AllowHtml]
        public string CardExpirationMonth { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CardExpirationYear")]
        [AllowHtml]
        public string CardExpirationYear { get; set; }

        //misc payment info
        public bool DisplayPurchaseOrderNumber { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.PurchaseOrderNumber")]
        public string PurchaseOrderNumber { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.AuthorizationTransactionID")]
        public string AuthorizationTransactionId { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CaptureTransactionID")]
        public string CaptureTransactionId { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.SubscriptionTransactionID")]
        public string SubscriptionTransactionId { get; set; }

        //shipping info
        public bool IsShippable { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.ShippingStatus")]
        public string ShippingStatus { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.ShippingAddress")]
        public AddressModel ShippingAddress { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.OrderWeight")]
        public decimal OrderWeight { get; set; }
        public string BaseWeightIn { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.ShippingMethod")]
        public string ShippingMethod { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.ShippedDate")]
        public string ShippedDate { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.DeliveryDate")]
        public string DeliveryDate { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.TrackingNumber")]
        public string TrackingNumber { get; set; }
        public string ShippingAddressGoogleMapsUrl { get; set; }

        //billing info
        [NopResourceDisplayName("Admin.Orders.Fields.BillingAddress")]
        public AddressModel BillingAddress { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.VatNumber")]
        public string VatNumber { get; set; }
        
        //gift cards
        public IList<GiftCard> GiftCards { get; set; }

        //items
        public bool HasDownloadableProducts { get; set; }
        public IList<OrderProductVariantModel> Items { get; set; }

        //creation date
        [NopResourceDisplayName("Admin.Orders.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        //checkout attributes
        public string CheckoutAttributeInfo { get; set; }


        //order notes
        public IList<OrderNote> OrderNotes { get; set; }
        [NopResourceDisplayName("Admin.Orders.OrderNotes.Fields.AddOrderNoteDisplayToCustomer")]
        public bool AddOrderNoteDisplayToCustomer { get; set; }
        [NopResourceDisplayName("Admin.Orders.OrderNotes.Fields.AddOrderNoteMessage")]
        [AllowHtml]
        public string AddOrderNoteMessage { get; set; }

        public bool DisplayPdfInvoice { get; set; }
        public bool DisplayPdfPackagingSlip { get; set; }


        //refund info
        [NopResourceDisplayName("Admin.Orders.Fields.PartialRefund.AmountToRefund")]
        public decimal AmountToRefund { get; set; }
        public decimal MaxAmountToRefund { get; set; }
        public string PrimaryStoreCurrencyCode { get; set; }

        //workflow info
        public bool CanCancelOrder { get; set; }
        public bool CanCapture { get; set; }
        public bool CanMarkOrderAsPaid { get; set; }
        public bool CanRefund { get; set; }
        public bool CanRefundOffline { get; set; }
        public bool CanPartiallyRefund { get; set; }
        public bool CanPartiallyRefundOffline { get; set; }
        public bool CanVoid { get; set; }
        public bool CanVoidOffline { get; set; }
        public bool CanShip { get; set; }
        public bool CanDeliver { get; set; }

        #region Nested Classes

        public class OrderProductVariantModel : BaseNopEntityModel
        {
            public OrderProductVariantModel()
            {
                ReturnRequestIds = new List<int>();
            }
            public int ProductVariantId { get; set; }

            public string FullProductName { get; set; }

            public string UnitPriceInclTax { get; set; }
            public string UnitPriceExclTax { get; set; }
            public decimal UnitPriceInclTaxValue { get; set; }
            public decimal UnitPriceExclTaxValue { get; set; }

            public int Quantity { get; set; }

            public string DiscountInclTax { get; set; }
            public string DiscountExclTax { get; set; }
            public decimal DiscountInclTaxValue { get; set; }
            public decimal DiscountExclTaxValue { get; set; }

            public string SubTotalInclTax { get; set; }
            public string SubTotalExclTax { get; set; }
            public decimal SubTotalInclTaxValue { get; set; }
            public decimal SubTotalExclTaxValue { get; set; }

            public string AttributeInfo { get; set; }
            public string RecurringInfo { get; set; }
            public IList<int> ReturnRequestIds { get; set; }

            public bool IsDownload { get; set; }
            public int DownloadCount { get; set; }
            public DownloadActivationType DownloadActivationType { get; set; }
            public bool IsDownloadActivated { get; set; }
            public int? LicenseDownloadId { get; set; }
        }

        public class TaxRate : BaseNopModel
        {
            public string Rate { get; set; }
            public string Value { get; set; }
        }

        public class GiftCard : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Orders.Fields.GiftCardInfo")]
            public string CouponCode { get; set; }
            public string Amount { get; set; }
        }

        public class OrderNote : BaseNopEntityModel
        {
            public int OrderId { get; set; }
            [NopResourceDisplayName("Admin.Orders.OrderNotes.Fields.DisplayToCustomer")]
            public bool DisplayToCustomer { get; set; }
            [NopResourceDisplayName("Admin.Orders.OrderNotes.Fields.Note")]
            public string Note { get; set; }
            [NopResourceDisplayName("Admin.Orders.OrderNotes.Fields.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }

        public class UploadLicenseModel : BaseNopModel
        {
            public int OrderId { get; set; }

            public int OrderProductVariantId { get; set; }

            [UIHint("Download")]
            public int LicenseDownloadId { get; set; }

        }


        #endregion
    }
}