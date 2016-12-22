using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework;

namespace Nop.Web.Models.ShoppingCart
{
    public partial class OrderTotalsModel : BaseNopModel
    {
        public OrderTotalsModel()
        {
            TaxRates = new List<TaxRate>();
            GiftCards = new List<GiftCard>();
        }
        public bool IsEditable { get; set; }

        public string SubTotal { get; set; }

        public string SubTotalDiscount { get; set; }

        public string Shipping { get; set; }
        public bool RequiresShipping { get; set; }
        public string SelectedShippingMethod { get; set; }
        public bool HideShippingTotal { get; set; }

        public string PaymentMethodAdditionalFee { get; set; }

        public string Tax { get; set; }
        public IList<TaxRate> TaxRates { get; set; }
        public bool DisplayTax { get; set; }
        public bool DisplayTaxRates { get; set; }


        public bool includingTax { get; set; } //MF 22.11.16
        public IList<GiftCard> GiftCards { get; set; }

        public string OrderTotalDiscount { get; set; }
        public int RedeemedRewardPoints { get; set; }
        public string RedeemedRewardPointsAmount { get; set; }

        public int WillEarnRewardPoints { get; set; }

        public string OrderTotal { get; set; }
        public string OrderAmount { get; set; } //MF 09.12.16
        public string OrderAmountIncl { get; set; } //MF 09.12.16

        #region Nested classes

        public partial class TaxRate: BaseNopModel
        {
            [NopResourceDisplayName("Order.TaxRateLine.VatRate")]
            public string Rate { get; set; }
            [NopResourceDisplayName("Order.TaxRateLine.Amount")]
            public string Amount { get; set; } // includes subtotal, shipping and payment fee
            [NopResourceDisplayName("Order.TaxRateLine.DiscountAmount")]
            public string DiscountAmount { get; set; }
            [NopResourceDisplayName("Order.TaxRateLine.BaseAmount")]
            public string BaseAmount { get; set; }
            [NopResourceDisplayName("Order.TaxRateLine.VatAmount")]
            public string VatAmount { get; set; }
        }

        public partial class GiftCard : BaseNopEntityModel
        {
            public string CouponCode { get; set; }
            public string Amount { get; set; }
            public string Remaining { get; set; }
        }
        #endregion
    }
}