using System;
using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Common;

namespace Nop.Web.Models.Order
{
    public class OrderDetailsModel : BaseNopEntityModel
    {
        public OrderDetailsModel()
        {
            TaxRates = new List<TaxRate>();
            GiftCards = new List<GiftCard>();
            Items = new List<OrderProductVariantModel>();
            OrderNotes = new List<OrderNote>();
        }

        public bool PrintMode { get; set; }
        public bool DisplayPdfInvoice { get; set; }

        public DateTime CreatedOn { get; set; }

        public string OrderStatus { get; set; }

        public bool IsReOrderAllowed { get; set; }

        public bool IsReturnRequestAllowed { get; set; }

        public bool IsShippable { get; set; }
        public string ShippingStatus { get; set; }
        public AddressModel ShippingAddress { get; set; }
        public decimal OrderWeight { get; set; }
        public string BaseWeightIn { get; set; }
        public string ShippingMethod { get; set; }
        public string ShippedDate { get; set; }
        public string DeliveryDate { get; set; }
        public string TrackingNumber { get; set; }

        public AddressModel BillingAddress { get; set; }

        public string VatNumber { get; set; }

        public string PaymentMethod { get; set; }
        public bool CanRePostProcessPayment { get; set; }

        public string OrderSubtotal { get; set; }
        public string OrderSubTotalDiscount { get; set; }
        public string OrderShipping { get; set; }
        public string PaymentMethodAdditionalFee { get; set; }
        public string CheckoutAttributeInfo { get; set; }
        public string Tax { get; set; }
        public IList<TaxRate> TaxRates { get; set; }
        public bool DisplayTax { get; set; }
        public bool DisplayTaxRates { get; set; }
        public string OrderTotalDiscount { get; set; }
        public int RedeemedRewardPoints { get; set; }
        public string RedeemedRewardPointsAmount { get; set; }
        public string OrderTotal { get; set; }
        
        public IList<GiftCard> GiftCards { get; set; }

        public bool ShowSku { get; set; }
        public IList<OrderProductVariantModel> Items { get; set; }
        
        public IList<OrderNote> OrderNotes { get; set; }

		#region Nested Classes

        public class OrderProductVariantModel : BaseNopEntityModel
        {
            public string Sku { get; set; }

            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public string ProductSeName { get; set; }

            public string UnitPrice { get; set; }

            public string SubTotal { get; set; }

            public int Quantity { get; set; }
            
            public string AttributeInfo { get; set; }
        }

        public class TaxRate : BaseNopModel
        {
            public string Rate { get; set; }
            public string Value { get; set; }
        }

        public class GiftCard : BaseNopModel
        {
            public string CouponCode { get; set; }
            public string Amount { get; set; }
        }

        public class OrderNote : BaseNopModel
        {
            public string Note { get; set; }
            public DateTime CreatedOn { get; set; }
        }
		#endregion
    }
}