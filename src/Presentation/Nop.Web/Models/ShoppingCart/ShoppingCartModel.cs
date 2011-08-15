using System.Collections.Generic;
using System.Web.Routing;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.ShoppingCart
{
    public class ShoppingCartModel : BaseNopModel
    {
        public ShoppingCartModel()
        {
            Items = new List<ShoppingCartItemModel>();
            Warnings = new List<string>();
            EstimateShipping = new EstimateShippingModel();
            CheckoutAttributes = new List<CheckoutAttributeModel>();

            ButtonPaymentMethodActionNames = new List<string>();
            ButtonPaymentMethodControllerNames = new List<string>();
            ButtonPaymentMethodRouteValues = new List<RouteValueDictionary>();
        }

        public bool ShowSku { get; set; }

        public bool ShowProductImages { get; set; }

        public bool IsEditable { get; set; }

        public IList<ShoppingCartItemModel> Items { get; set; }

        public IList<string> Warnings { get; set; }

        public string CheckoutAttributeInfo { get; set; }

        public string MinOrderSubtotalWarning { get; set; }

        public bool TermsOfServiceEnabled { get; set; }

        public bool ShowDiscountBox { get; set; }
        public string DiscountMessage { get; set; }

        public bool ShowGiftCardBox { get; set; }
        public string GiftCardMessage { get; set; }

        public EstimateShippingModel EstimateShipping { get; set; }

        public IList<CheckoutAttributeModel> CheckoutAttributes { get; set; }

        public IList<string> ButtonPaymentMethodActionNames { get; set; }
        public IList<string> ButtonPaymentMethodControllerNames { get; set; }
        public IList<RouteValueDictionary> ButtonPaymentMethodRouteValues { get; set; }

		#region Nested Classes

        public class ShoppingCartItemModel : BaseNopEntityModel
        {
            public ShoppingCartItemModel()
            {
                Picture = new PictureModel();
                Warnings = new List<string>();
            }
            public string Sku { get; set; }

            public PictureModel Picture {get;set;}

            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public string ProductSeName { get; set; }

            public string UnitPrice { get; set; }

            public string SubTotal { get; set; }

            public string Discount { get; set; }

            public int Quantity { get; set; }
            
            public string AttributeInfo { get; set; }

            public string RecurringInfo { get; set; }

            public IList<string> Warnings { get; set; }

        }

        public class CheckoutAttributeModel : BaseNopEntityModel
        {
            public CheckoutAttributeModel()
            {
                Values = new List<CheckoutAttributeValueModel>();
            }

            public string Name { get; set; }

            public string DefaultValue { get; set; }

            public string TextPrompt { get; set; }

            public bool IsRequired { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<CheckoutAttributeValueModel> Values { get; set; }
        }

        public class CheckoutAttributeValueModel : BaseNopEntityModel
        {
            public string Name { get; set; }

            public string PriceAdjustment { get; set; }

            public bool IsPreSelected { get; set; }
        }
		#endregion
    }
}