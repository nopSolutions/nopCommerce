using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.ShoppingCart
{
    public partial class ShoppingCartModel : BaseNopModel
    {
        public ShoppingCartModel()
        {
            Items = new List<ShoppingCartItemModel>();
            Warnings = new List<string>();
            DiscountBox = new DiscountBoxModel();
            GiftCardBox = new GiftCardBoxModel();
            CheckoutAttributes = new List<CheckoutAttributeModel>();
            OrderReviewData = new OrderReviewDataModel();

            ButtonPaymentMethodViewComponentNames = new List<string>();
        }

        public bool OnePageCheckoutEnabled { get; set; }

        public bool ShowSku { get; set; }
        public bool ShowProductImages { get; set; }
        public bool IsEditable { get; set; }
        public IList<ShoppingCartItemModel> Items { get; set; }
        
        public IList<CheckoutAttributeModel> CheckoutAttributes { get; set; }

        public IList<string> Warnings { get; set; }
        public string MinOrderSubtotalWarning { get; set; }
        public bool DisplayTaxShippingInfo { get; set; }
        public bool TermsOfServiceOnShoppingCartPage { get; set; }
        public bool TermsOfServiceOnOrderConfirmPage { get; set; }
        public bool TermsOfServicePopup { get; set; }
        public DiscountBoxModel DiscountBox { get; set; }
        public GiftCardBoxModel GiftCardBox { get; set; }
        public OrderReviewDataModel OrderReviewData { get; set; }

        public IList<string> ButtonPaymentMethodViewComponentNames { get; set; }

        public bool HideCheckoutButton { get; set; }
        public bool ShowVendorName { get; set; }

        #region Nested Classes

        public partial class ShoppingCartItemModel : BaseNopEntityModel
        {
            public ShoppingCartItemModel()
            {
                Picture = new PictureModel();
                AllowedQuantities = new List<SelectListItem>();
                Warnings = new List<string>();
            }

            public string Sku { get; set; }

            public string VendorName { get; set; }

            public PictureModel Picture {get;set;}

            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public string ProductSeName { get; set; }

            public string UnitPrice { get; set; }

            public string SubTotal { get; set; }

            public string Discount { get; set; }
            public int? MaximumDiscountedQty { get; set; }

            public int Quantity { get; set; }
            public List<SelectListItem> AllowedQuantities { get; set; }
            
            public string AttributeInfo { get; set; }

            public string RecurringInfo { get; set; }

            public string RentalInfo { get; set; }

            public bool AllowItemEditing { get; set; }

            public bool DisableRemoval { get; set; }

            public IList<string> Warnings { get; set; }
        }

        public partial class CheckoutAttributeModel : BaseNopEntityModel
        {
            public CheckoutAttributeModel()
            {
                AllowedFileExtensions = new List<string>();
                Values = new List<CheckoutAttributeValueModel>();
            }

            public string Name { get; set; }

            public string DefaultValue { get; set; }

            public string TextPrompt { get; set; }

            public bool IsRequired { get; set; }

            /// <summary>
            /// Selected day value for datepicker
            /// </summary>
            public int? SelectedDay { get; set; }
            /// <summary>
            /// Selected month value for datepicker
            /// </summary>
            public int? SelectedMonth { get; set; }
            /// <summary>
            /// Selected year value for datepicker
            /// </summary>
            public int? SelectedYear { get; set; }

            /// <summary>
            /// Allowed file extensions for customer uploaded files
            /// </summary>
            public IList<string> AllowedFileExtensions { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<CheckoutAttributeValueModel> Values { get; set; }
        }

        public partial class CheckoutAttributeValueModel : BaseNopEntityModel
        {
            public string Name { get; set; }

            public string ColorSquaresRgb { get; set; }

            public string PriceAdjustment { get; set; }

            public bool IsPreSelected { get; set; }
        }

        public partial class DiscountBoxModel: BaseNopModel
        {
            public DiscountBoxModel()
            {
                AppliedDiscountsWithCodes = new List<DiscountInfoModel>();
                Messages = new List<string>();
            }

            public List<DiscountInfoModel> AppliedDiscountsWithCodes { get; set; }
            public bool Display { get; set; }
            public List<string> Messages { get; set; }
            public bool IsApplied { get; set; }

            public class DiscountInfoModel : BaseNopEntityModel
            {
                public string CouponCode { get; set; }
            }
        }

        public partial class GiftCardBoxModel : BaseNopModel
        {
            public bool Display { get; set; }
            public string Message { get; set; }
            public bool IsApplied { get; set; }
        }

        public partial class OrderReviewDataModel : BaseNopModel
        {
            public OrderReviewDataModel()
            {
                BillingAddress = new AddressModel();
                ShippingAddress = new AddressModel();
                PickupAddress = new AddressModel();
                CustomValues= new Dictionary<string, object>();
            }
            public bool Display { get; set; }

            public AddressModel BillingAddress { get; set; }

            public bool IsShippable { get; set; }
            public AddressModel ShippingAddress { get; set; }
            public bool SelectedPickupInStore { get; set; }
            public AddressModel PickupAddress { get; set; }
            public string ShippingMethod { get; set; }

            public string PaymentMethod { get; set; }

            public Dictionary<string, object> CustomValues { get; set; }
        }

        #endregion
    }
}