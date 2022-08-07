using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.ShoppingCart;
using System.Threading.Tasks;

namespace Nop.Web.Components
{
    public class ShortListViewComponent : NopViewComponent
    {
        private readonly IPermissionService _permissionService;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IShoppingCartService _shoppingCartService;

        public ShortListViewComponent(IPermissionService permissionService,
            IShoppingCartModelFactory shoppingCartModelFactory,
            ShoppingCartSettings shoppingCartSettings,
            IWorkContext workContext,
            IStoreContext storeContext,
            IShoppingCartService shoppingCartService)
        {
            _permissionService = permissionService;
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _shoppingCartSettings = shoppingCartSettings;
            _workContext = workContext;
            _storeContext = storeContext;
            _shoppingCartService = shoppingCartService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int actionTypeId)
        {
            var actionType = (ShoppingCartType)actionTypeId;

            if (!_shoppingCartSettings.MiniShoppingCartEnabled)
                return Content(string.Empty);

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart))
                return Content(string.Empty);

            var customer = await _workContext.GetCurrentCustomerAsync();
            var model = new WishlistModel();
            var currentStoreId = (await _storeContext.GetCurrentStoreAsync()).Id;
            var myProductId = customer.VendorId;

            switch (actionType)
            {
                case ShoppingCartType.Wishlist:
                    model = await _shoppingCartModelFactory.PrepareWishlistModel(model, ShoppingCartType.Wishlist, customer);
                    break;
                case ShoppingCartType.ShortListedMe:
                    model = await _shoppingCartModelFactory.PrepareWishlistModel(model, ShoppingCartType.ShortListedMe, customer);
                    break;
                case ShoppingCartType.InterestSent:
                    var interestSent = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.InterestSent, currentStoreId);
                    model = await _shoppingCartModelFactory.PrepareWishlistModelAsync(model, interestSent, true);
                    break;
                case ShoppingCartType.InterestReceived:
                    break;
                case ShoppingCartType.AcceptedByMe:
                    var acceptedByMe = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.AcceptedByMe, currentStoreId);
                    model = await _shoppingCartModelFactory.PrepareWishlistModelAsync(model, acceptedByMe, true);
                    break;
                case ShoppingCartType.AcceptedMe:
                    break;
                case ShoppingCartType.DeclinedByMe:
                    var declinedByMe = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.DeclinedByMe, currentStoreId);
                    model = await _shoppingCartModelFactory.PrepareWishlistModelAsync(model, declinedByMe, true);
                    break;
                case ShoppingCartType.DeclinedMe:
                    break;
                case ShoppingCartType.ViewedByMe:
                    var viewedByMe = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ViewedByMe, currentStoreId);
                    model = await _shoppingCartModelFactory.PrepareWishlistModelAsync(model, viewedByMe, true);
                    break;
                case ShoppingCartType.ViewedMe:
                    break;
                case ShoppingCartType.BlockedByMe:
                    var blockedByMe = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.BlockedByMe, currentStoreId);
                    model = await _shoppingCartModelFactory.PrepareWishlistModelAsync(model, blockedByMe, true);
                    break;
                case ShoppingCartType.BlockedMe:
                    break;
                default:
                    break;
            }

            return View(model);
        }
    }
}
