using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Tax;
using Nop.Web.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Web.Controllers
{
    public class ShoppingCartController : BaseNopController
    {
		#region Fields

        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;

        private readonly MediaSettings _mediaSetting;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly CatalogSettings _catalogSettings;

        #endregion

		#region Constructors

        public ShoppingCartController(IProductService productService, IWorkContext workContext,
            IShoppingCartService shoppingCartService, IPictureService pictureService,
            ILocalizationService localizationService, IProductAttributeFormatter productAttributeFormatter,
            ITaxService taxService, ICurrencyService currencyService,
            IPriceCalculationService priceCalculationService, IPriceFormatter priceFormatter,
            MediaSettings mediaSetting, ShoppingCartSettings shoppingCartSettings,
            CatalogSettings catalogSettings)
        {
            this._productService = productService;
            this._workContext = workContext;
            this._shoppingCartService = shoppingCartService;
            this._pictureService = pictureService;
            this._localizationService = localizationService;
            this._productAttributeFormatter = productAttributeFormatter;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;

            this._mediaSetting = mediaSetting;
            this._shoppingCartSettings = shoppingCartSettings;
            this._catalogSettings = catalogSettings;
        }

		#endregion Constructors 

        #region Utilities

        [NonAction]
        private ShoppingCartModel PrepareShoppingCartModel(ShoppingCartModel model, IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (model == null)
                throw new ArgumentNullException("model");

            model.IsEditable = true;
            model.ShowProductImages = _shoppingCartSettings.ShowProductImagesOnShoppingCart;
            model.ShowSku = _catalogSettings.ShowProductSku;

            foreach (var sci in cart)
            {
                var cartItemModel = new ShoppingCartModel.ShoppingCartItemModel()
                {
                    Id = sci.Id,
                    Sku = sci.ProductVariant.Sku,
                    ProductId = sci.ProductVariant.ProductId,
                    ProductSeName = sci.ProductVariant.Product.GetSeName(),
                    Quantity = sci.Quantity,
                    AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.ProductVariant, sci.AttributesXml),
                };
                
                //recurring info
                if (sci.ProductVariant.IsRecurring)
                    cartItemModel.RecurringInfo = string.Format(_localizationService.GetResource("ShoppingCart.RecurringPeriod"), sci.ProductVariant.RecurringCycleLength, sci.ProductVariant.RecurringCyclePeriod.GetLocalizedEnum(_localizationService, _workContext));

                //unit prices
                if (sci.ProductVariant.CallForPrice)
                {
                    cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    decimal taxRate = decimal.Zero;
                    decimal shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(sci.ProductVariant, _priceCalculationService.GetUnitPrice(sci, true), out taxRate);
                    decimal shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
                }
                //subtotal, discount
                if (sci.ProductVariant.CallForPrice)
                {
                    cartItemModel.SubTotal = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    //sub total
                    decimal taxRate = decimal.Zero;
                    decimal shoppingCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(sci.ProductVariant, _priceCalculationService.GetSubTotal(sci, true), out taxRate);
                    decimal shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.SubTotal = _priceFormatter.FormatPrice(shoppingCartItemSubTotalWithDiscount);

                    //display an applied discount amount
                    decimal shoppingCartItemSubTotalWithoutDiscountBase = _taxService.GetProductPrice(sci.ProductVariant, _priceCalculationService.GetSubTotal(sci, false), out taxRate);
                    decimal shoppingCartItemDiscountBase = shoppingCartItemSubTotalWithoutDiscountBase - shoppingCartItemSubTotalWithDiscountBase;
                    if (shoppingCartItemDiscountBase > decimal.Zero)
                    {
                        decimal shoppingCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemDiscountBase, _workContext.WorkingCurrency);
                        cartItemModel.Discount = _priceFormatter.FormatPrice(shoppingCartItemDiscount);
                    }
                }

                //product name
                if (!String.IsNullOrEmpty(sci.ProductVariant.GetLocalized(x=>x.Name)))
                    cartItemModel.ProductName = string.Format("{0} ({1})",sci.ProductVariant.Product.GetLocalized(x=>x.Name), sci.ProductVariant.GetLocalized(x=>x.Name));
                else
                    cartItemModel.ProductName = sci.ProductVariant.Product.GetLocalized(x=>x.Name);
                
                //picture
                if (_shoppingCartSettings.ShowProductImagesOnShoppingCart)
                {
                    var picture = _pictureService.GetPictureById(sci.ProductVariant.PictureId);
                    if (picture == null)
                    {
                        picture = _pictureService.GetPicturesByProductId(sci.ProductVariant.Product.Id, 1).FirstOrDefault();
                    }
                    cartItemModel.Picture = new PictureModel()
                    {
                        ImageUrl = _pictureService.GetPictureUrl(picture, _mediaSetting.CartThumbPictureSize, true),
                        Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), cartItemModel.ProductName),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), cartItemModel.ProductName),

                    };
                }

                model.Items.Add(cartItemModel);
            }

            return model;
        }
        #endregion

        #region Methods

        public ActionResult AddProductToCart(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                return RedirectToAction("Index", "Home");

            int productVariantId = 0;
            if (_shoppingCartService.DirectAddToCartAllowed(productId, out productVariantId))
            {
                var productVariant = _productService.GetProductVariantById(productVariantId);
                var addToCartWarnings = _shoppingCartService.AddToCart(_workContext.CurrentCustomer,
                    productVariant, ShoppingCartType.ShoppingCart,
                    string.Empty, decimal.Zero, 1);
                if (addToCartWarnings.Count == 0)
                    return RedirectToRoute("ShoppingCart");
                else
                    return RedirectToRoute("Product", new { productId = product.Id, SeName = product.GetSeName() });
            }
            else
                return RedirectToRoute("Product", new { productId = product.Id, SeName = product.GetSeName() });
        }

        public ActionResult Cart()
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();
            var model = PrepareShoppingCartModel(new ShoppingCartModel(), cart);
            return View(model);
        }

        public ActionResult Wishlist()
        {
            return Content("UNDONE Wishlist");
        }

		#endregion
    }
}
