using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Nop.Admin.Models.Catalog;
using Nop.Admin.Models.Common;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Telerik.Web.Mvc;

namespace Nop.Admin.Models.Orders
{
    public partial class OrderModel : BaseNopEntityModel
    {
        public OrderModel()
        {
            TaxRates = new List<TaxRate>();
            GiftCards = new List<GiftCard>();
            Items = new List<OrderProductVariantModel>();
            UsedDiscounts = new List<UsedDiscountModel>();
        }

        public bool IsLoggedInAsVendor { get; set; }

        //identifiers
        [NopResourceDisplayName("Admin.Orders.Fields.ID")]
        public override int Id { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.OrderGuid")]
        public Guid OrderGuid { get; set; }

        //store
        [NopResourceDisplayName("Admin.Orders.Fields.Store")]
        public string StoreName { get; set; }

        //customer info
        [NopResourceDisplayName("Admin.Orders.Fields.Customer")]
        public int CustomerId { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.Customer")]
        public string CustomerInfo { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CustomerEmail")]
        public string CustomerEmail { get; set; }
        [NopResourceDisplayName("Admin.Orders.Fields.CustomerIP")]
        public string CustomerIp { get; set; }

        [NopResourceDisplayName("Admin.Orders.Fields.Affiliate")]
        public int? AffiliateId { get; set; }

        //Used discounts
        [NopResourceDisplayName("Admin.Orders.Fields.UsedDiscounts")]
        public IList<UsedDiscountModel> UsedDiscounts { get; set; }

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

        //associated recurring payment id
        [NopResourceDisplayName("Admin.Orders.Fields.RecurringPayment")]
        public int RecurringPaymentId { get; set; }

        //order status
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
        [NopResourceDisplayName("Admin.Orders.Fields.ShippingMethod")]
        public string ShippingMethod { get; set; }
        public string ShippingAddressGoogleMapsUrl { get; set; }
        public bool CanAddNewShipments { get; set; }

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
        [NopResourceDisplayName("Admin.Orders.OrderNotes.Fields.AddOrderNoteDisplayToCustomer")]
        public bool AddOrderNoteDisplayToCustomer { get; set; }
        [NopResourceDisplayName("Admin.Orders.OrderNotes.Fields.AddOrderNoteMessage")]
        [AllowHtml]
        public string AddOrderNoteMessage { get; set; }

        public bool DisplayPdfInvoice { get; set; }


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
        
        //aggergator proeprties
        public string aggregatorprofit { get; set; }
        public string aggregatortax { get; set; }
        public string aggregatortotal { get; set; }

        #region Nested Classes

        public partial class OrderProductVariantModel : BaseNopEntityModel
        {
            public OrderProductVariantModel()
            {
                ReturnRequestIds = new List<int>();
                PurchasedGiftCardIds = new List<int>();
            }
            public int ProductVariantId { get; set; }

            public string FullProductName { get; set; }
            public string VendorName { get; set; }
            public string Sku { get; set; }

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
            public IList<int> PurchasedGiftCardIds { get; set; }

            public bool IsDownload { get; set; }
            public int DownloadCount { get; set; }
            public DownloadActivationType DownloadActivationType { get; set; }
            public bool IsDownloadActivated { get; set; }
            public int? LicenseDownloadId { get; set; }
        }

        public partial class TaxRate : BaseNopModel
        {
            public string Rate { get; set; }
            public string Value { get; set; }
        }

        public partial class GiftCard : BaseNopModel
        {
            [NopResourceDisplayName("Admin.Orders.Fields.GiftCardInfo")]
            public string CouponCode { get; set; }
            public string Amount { get; set; }
        }

        public partial class OrderNote : BaseNopEntityModel
        {
            public int OrderId { get; set; }
            [NopResourceDisplayName("Admin.Orders.OrderNotes.Fields.DisplayToCustomer")]
            public bool DisplayToCustomer { get; set; }
            [NopResourceDisplayName("Admin.Orders.OrderNotes.Fields.Note")]
            public string Note { get; set; }
            [NopResourceDisplayName("Admin.Orders.OrderNotes.Fields.CreatedOn")]
            public DateTime CreatedOn { get; set; }
        }

        public partial class UploadLicenseModel : BaseNopModel
        {
            public int OrderId { get; set; }

            public int OrderProductVariantId { get; set; }

            [UIHint("Download")]
            public int LicenseDownloadId { get; set; }

        }

        public partial class AddOrderProductModel : BaseNopModel
        {
            public AddOrderProductModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
            }
            public GridModel<ProductVariantModel> ProductVariants { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchProductName")]
            [AllowHtml]
            public string SearchProductName { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchCategory")]
            public int SearchCategoryId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Products.List.SearchManufacturer")]
            public int SearchManufacturerId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableManufacturers { get; set; }

            public int OrderId { get; set; }

            #region Nested classes
            
            public partial class ProductVariantLineModel : BaseNopEntityModel
            {
                [NopResourceDisplayName("Admin.Orders.Products.AddNew.Name")]
                [AllowHtml]
                public string Name { get; set; }

                [NopResourceDisplayName("Admin.Orders.Products.AddNew.SKU")]
                [AllowHtml]
                public string Sku { get; set; }
            }

            public partial class ProductDetailsModel : BaseNopModel
            {
                public ProductDetailsModel()
                {
                    ProductVariantAttributes = new List<ProductVariantAttributeModel>();
                    GiftCard = new GiftCardModel();
                    Warnings = new List<string>();
                }

                public int ProductVariantId { get; set; }

                public int OrderId { get; set; }

                public string Name { get; set; }

                [NopResourceDisplayName("Admin.Orders.Products.AddNew.UnitPriceInclTax")]
                public decimal UnitPriceInclTax { get; set; }
                [NopResourceDisplayName("Admin.Orders.Products.AddNew.UnitPriceExclTax")]
                public decimal UnitPriceExclTax { get; set; }

                [NopResourceDisplayName("Admin.Orders.Products.AddNew.Quantity")]
                public int Quantity { get; set; }

                [NopResourceDisplayName("Admin.Orders.Products.AddNew.SubTotalInclTax")]
                public decimal SubTotalInclTax { get; set; }
                [NopResourceDisplayName("Admin.Orders.Products.AddNew.SubTotalExclTax")]
                public decimal SubTotalExclTax { get; set; }

                //product attrbiutes
                public IList<ProductVariantAttributeModel> ProductVariantAttributes { get; set; }
                //gift card info
                public GiftCardModel GiftCard { get; set; }

                public List<string> Warnings { get; set; }

            }

            public partial class ProductVariantAttributeModel : BaseNopEntityModel
            {
                public ProductVariantAttributeModel()
                {
                    Values = new List<ProductVariantAttributeValueModel>();
                }

                public int ProductAttributeId { get; set; }

                public string Name { get; set; }

                public string TextPrompt { get; set; }

                public bool IsRequired { get; set; }

                public AttributeControlType AttributeControlType { get; set; }

                public IList<ProductVariantAttributeValueModel> Values { get; set; }
            }

            public partial class ProductVariantAttributeValueModel : BaseNopEntityModel
            {
                public string Name { get; set; }

                public bool IsPreSelected { get; set; }
            }


            public partial class GiftCardModel : BaseNopModel
            {
                public bool IsGiftCard { get; set; }

                [NopResourceDisplayName("Products.GiftCard.RecipientName")]
                [AllowHtml]
                public string RecipientName { get; set; }
                [NopResourceDisplayName("Products.GiftCard.RecipientEmail")]
                [AllowHtml]
                public string RecipientEmail { get; set; }
                [NopResourceDisplayName("Products.GiftCard.SenderName")]
                [AllowHtml]
                public string SenderName { get; set; }
                [NopResourceDisplayName("Products.GiftCard.SenderEmail")]
                [AllowHtml]
                public string SenderEmail { get; set; }
                [NopResourceDisplayName("Products.GiftCard.Message")]
                [AllowHtml]
                public string Message { get; set; }

                public GiftCardType GiftCardType { get; set; }
            }
            #endregion
        }

        public partial class UsedDiscountModel:BaseNopModel
        {
            public int DiscountId { get; set; }
            public string DiscountName { get; set; }
        }

        #endregion
    }
}