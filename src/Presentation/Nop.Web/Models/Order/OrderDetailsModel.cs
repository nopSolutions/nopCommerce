using System;
using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Order
{
    public partial record OrderDetailsModel : BaseNopEntityModel
    {
        public OrderDetailsModel()
        {
            TaxRates = new List<TaxRate>();
            GiftCards = new List<GiftCard>();
            Items = new List<OrderItemModel>();
            OrderNotes = new List<OrderNote>();
            Shipments = new List<ShipmentBriefModel>();

            BillingAddress = new AddressModel();
            ShippingAddress = new AddressModel();
            PickupAddress = new AddressModel();

            CustomValues = new Dictionary<string, object>();
        }

        public bool PrintMode { get; set; }
        public bool PdfInvoiceDisabled { get; set; }

        public string CustomOrderNumber { get; set; }

        public DateTime CreatedOn { get; set; }

        public string OrderStatus { get; set; }

        public bool IsReOrderAllowed { get; set; }

        public bool IsReturnRequestAllowed { get; set; }
        
        public bool IsShippable { get; set; }
        public bool PickupInStore { get; set; }
        public AddressModel PickupAddress { get; set; }
        public string ShippingStatus { get; set; }
        public AddressModel ShippingAddress { get; set; }
        public string ShippingMethod { get; set; }
        public IList<ShipmentBriefModel> Shipments { get; set; }

        public AddressModel BillingAddress { get; set; }

        public string VatNumber { get; set; }

        public string PaymentMethod { get; set; }
        public string PaymentMethodStatus { get; set; }
        public bool CanRePostProcessPayment { get; set; }
        public Dictionary<string, object> CustomValues { get; set; }

        public string OrderSubtotal { get; set; }
        public decimal OrderSubtotalValue { get; set; }
        public string OrderSubTotalDiscount { get; set; }
        public decimal OrderSubTotalDiscountValue { get; set; }
        public string OrderShipping { get; set; }
        public decimal OrderShippingValue { get; set; }
        public string PaymentMethodAdditionalFee { get; set; }
        public decimal PaymentMethodAdditionalFeeValue { get; set; }
        public string CheckoutAttributeInfo { get; set; }

        public bool PricesIncludeTax { get; set; }
        public bool DisplayTaxShippingInfo { get; set; }
        public string Tax { get; set; }
        public IList<TaxRate> TaxRates { get; set; }
        public bool DisplayTax { get; set; }
        public bool DisplayTaxRates { get; set; }

        public string OrderTotalDiscount { get; set; }
        public decimal OrderTotalDiscountValue { get; set; }
        public int RedeemedRewardPoints { get; set; }
        public string RedeemedRewardPointsAmount { get; set; }
        public string OrderTotal { get; set; }
        public decimal OrderTotalValue { get; set; }
        
        public IList<GiftCard> GiftCards { get; set; }

        public bool ShowSku { get; set; }
        public IList<OrderItemModel> Items { get; set; }
        
        public IList<OrderNote> OrderNotes { get; set; }

        public bool ShowVendorName { get; set; }
        public bool ShowProductThumbnail { get; set; }


        #region Nested Classes

        public partial record OrderItemModel : BaseNopEntityModel
        {
            public OrderItemModel()
            {
                Picture = new PictureModel();
            }

            public Guid OrderItemGuid { get; set; }
            public string Sku { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string ProductSeName { get; set; }
            public string UnitPrice { get; set; }
            public decimal UnitPriceValue { get; set; }
            public string SubTotal { get; set; }
            public decimal SubTotalValue { get; set; }
            public int Quantity { get; set; }
            public PictureModel Picture { get; set; }
            public string AttributeInfo { get; set; }
            public string RentalInfo { get; set; }

            public string VendorName { get; set; }

            //downloadable product properties
            public int DownloadId { get; set; }
            public int LicenseId { get; set; }
        }

        public partial record TaxRate : BaseNopModel
        {
            public string Rate { get; set; }
            public string Value { get; set; }
        }

        public partial record GiftCard : BaseNopModel
        {
            public string CouponCode { get; set; }
            public string Amount { get; set; }
        }

        public partial record OrderNote : BaseNopEntityModel
        {
            public bool HasDownload { get; set; }
            public string Note { get; set; }
            public DateTime CreatedOn { get; set; }
        }

        public partial record ShipmentBriefModel : BaseNopEntityModel
        {
            public string TrackingNumber { get; set; }
            public DateTime? ShippedDate { get; set; }
            public DateTime? ReadyForPickupDate { get; set; }
            public DateTime? DeliveryDate { get; set; }
        }

		#endregion
    }
}