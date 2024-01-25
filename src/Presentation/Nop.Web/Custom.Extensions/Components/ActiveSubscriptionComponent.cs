using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.ShoppingCart;
using Nop.Services.Common;
using Nop.Core.Domain.Customers;
using Nop.Web.Models.Customer;
using Nop.Services.Catalog;

namespace Nop.Web.Components
{
    public class ActiveSubscriptionViewComponent : NopViewComponent
    {
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IOrderModelFactory _orderModelFactory;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IProductService _productService;


        public ActiveSubscriptionViewComponent(IShoppingCartModelFactory shoppingCartModelFactory,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IOrderModelFactory orderModelFactory,
            IOrderService orderService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext,
            IProductService productService)
        {
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _workContext = workContext;
            _orderModelFactory = orderModelFactory;
            _orderService = orderService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _productService = productService;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync(int orderId = 0)
        {

            var customer = await _workContext.GetCurrentCustomerAsync();
            var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;

            //future implementation: check if subscription date is valid or not based on the subscription type 3, 6 or 1 year.

            var subscriptionProductId = await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.SubscriptionId, storeId);

            var model = new SubscriptionModel
            {
                SubscriptionId = subscriptionProductId,
                SubscriptionDate = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SubscriptionDate, storeId),
                AllottedCreditCount = await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.SubscriptionAllottedCount, storeId),
                UsedCreditCount = await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.SubscriptionUsedCreditCount, storeId),
                IsPaidCustomer = await _customerService.IsInCustomerRoleAsync(customer, "PaidCustomer")
            };

            //get subscription product name
            var product = await _productService.GetProductByIdAsync(subscriptionProductId);

            if (product != null)
                model.SubscriptionProduct = product.Name;

            return View(model);

        }
    }
}
