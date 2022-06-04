using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Orders;
using Nop.Services.Security;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.ShoppingCart;
using System;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using System.Collections.Generic;
using Nop.Core.Infrastructure;

namespace Nop.Web.Factories
{

    public partial interface IShoppingCartModelFactory
    {
        Task<WishlistModel> PrepareWhoShortListedMeModel(WishlistModel model, IList<ShoppingCartItem> cart, bool isEditable = true);

        Task<WishlistModel> PrepareWishlistModel(WishlistModel model, ShoppingCartType shoppingCartType, Customer customer);
    }

    public partial class ShoppingCartModelFactory
    {
        #region methods

        public virtual async Task<WishlistModel> PrepareWhoShortListedMeModel(WishlistModel model, IList<ShoppingCartItem> cart, bool isEditable = true)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!cart.Any())
                return model;

            //get customer table vendor ids (product ids)
            var customerIds = cart.Select(o => o.CustomerId).ToArray();
            var customers = await _customerService.GetCustomersByIdsAsync(customerIds);

            var productIds = customers.Select(o => o.VendorId).ToArray();

            var products = await _productService.GetProductsByIdsAsync(productIds);

            var _productModelFactory = EngineContext.Current.Resolve<IProductModelFactory>();
            model.Products = (await _productModelFactory.PrepareProductOverviewModelsAsync(products)).ToList();

            return model;
        }


        public virtual async Task<WishlistModel> PrepareWishlistModel(WishlistModel model, ShoppingCartType shoppingCartType, Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //dummy variables to prevent error in view
            model.EmailWishlistEnabled = _shoppingCartSettings.EmailWishlistEnabled;
            model.IsEditable = false;
            model.DisplayAddToCart = await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart);
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoWishlist;

            var productsCustom = await _productService.ProductsByShoppingCartTypeAsync(customerId: (await _workContext.GetCurrentCustomerAsync()).Id,
                                                                            shoppingCartType: shoppingCartType);

            //product overview model building
            var _productModelFactory = EngineContext.Current.Resolve<IProductModelFactory>();
            model.Products = (await _productModelFactory.PrepareProductOverviewModelsAsync(productsCustom)).ToList();

            return model;
        }

        #endregion
    }
}
