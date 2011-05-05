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

namespace Nop.Web.Controllers
{
    public class ShoppingCartController : BaseNopController
    {
		#region Fields

        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IShoppingCartService _shoppingCartService;

        #endregion

		#region Constructors

        public ShoppingCartController(IProductService productService, IWorkContext workContext,
            IShoppingCartService shoppingCartService)
        {
            this._productService = productService;
            this._workContext = workContext;
            this._shoppingCartService = shoppingCartService;
        }

		#endregion Constructors 
        
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
            return Content("Shopping cart");
        }

        public ActionResult Wishlist()
        {
            return Content("Shopping cart");
        }
		#endregion
    }
}
