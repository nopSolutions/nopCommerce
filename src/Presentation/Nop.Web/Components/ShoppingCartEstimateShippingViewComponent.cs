using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    /// <summary>
    /// Represents a estimate shipping view component on shopping cart page
    /// </summary>
    public partial class ShoppingCartEstimateShippingViewComponent : NopViewComponent
    {
        protected readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        protected readonly IShoppingCartService _shoppingCartService;
        protected readonly IStoreContext _storeContext;
        protected readonly IWorkContext _workContext;
        protected readonly ShippingSettings _shippingSettings;

        public ShoppingCartEstimateShippingViewComponent(IShoppingCartModelFactory shoppingCartModelFactory,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IWorkContext workContext,
            ShippingSettings shippingSettings)
        {
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _workContext = workContext;
            _shippingSettings = shippingSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool? prepareAndDisplayOrderReviewData)
        {
            if (!_shippingSettings.EstimateShippingCartPageEnabled)
                return Content(string.Empty);

            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, store.Id);

            var model = await _shoppingCartModelFactory.PrepareEstimateShippingModelAsync(cart);
            if (!model.Enabled)
                return Content(string.Empty);

            return View(model);
        }
    }
}
