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
using Nop.Core.Domain.Payments;

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
        private readonly IRewardPointService _rewardPointService;


        public ActiveSubscriptionViewComponent(IShoppingCartModelFactory shoppingCartModelFactory,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IOrderModelFactory orderModelFactory,
            IOrderService orderService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext,
            IProductService productService,
            IRewardPointService rewardPointService)
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
            _rewardPointService = rewardPointService;
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
                SubscriptionDate = await _genericAttributeService.GetAttributeAsync<DateTime>(customer, NopCustomerDefaults.SubscriptionDate, storeId),
                SubscriptionExpiryDate = await _genericAttributeService.GetAttributeAsync<DateTime>(customer, NopCustomerDefaults.SubscriptionExpiryDate, storeId),
                AllottedCreditCount = await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.SubscriptionAllottedCount, storeId),
                UsedCreditCount = await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.SubscriptionUsedCreditCount, storeId),
                IsPaidCustomer = await _customerService.IsInCustomerRoleAsync(customer, "PaidCustomer"),
                CustomerProfileTypeId = customer.CustomerProfileTypeId
            };

            //New implemenation based on Reward Point system
            
            //get subscription product id from latest orders
            var activeOrder = (await _orderService.SearchOrdersAsync(customerId: customer.Id, psIds: new List<int> { (int)PaymentStatus.Paid }, pageSize: 1)).FirstOrDefault();

            string subscriptionPlan = "Free Subscription Plan";

            if (activeOrder != null)
            {
                var activeOrderItems = await _orderService.GetOrderItemsAsync(activeOrder.Id);
                var customerSubscribedProductId = activeOrderItems.FirstOrDefault().ProductId;

                //get subscription product name
                var product = await _productService.GetProductByIdAsync(customerSubscribedProductId);
                subscriptionPlan = product != null ? product.Name : "Free Subscription Plan";
            }

            var modelNew = new SubscriptionModel
            {
                //SubscriptionId = subscriptionProductId,
                SubscriptionProduct = subscriptionPlan,

                SubscriptionDate = await _rewardPointService.GetSubscriptionStartDateAsync(customer.Id, storeId),
                SubscriptionExpiryDate = await _rewardPointService.GetSubscriptionExpiryDateAsync(customer.Id, storeId),

                AllottedCreditCount = await _rewardPointService.GetSubscriptionAlottedCreditCountAsync(customer.Id, storeId),
                BalanceCreditCount = await _rewardPointService.GetRewardPointsBalanceAsync(customer.Id, storeId),
                UsedCreditCount = await _rewardPointService.GetSubscriptionUsedCreditCountAsync(customer.Id, storeId),

                IsPaidCustomer = await _customerService.IsInCustomerRoleAsync(customer, "PaidCustomer"),
                CustomerProfileTypeId = customer.CustomerProfileTypeId
            };

            return View(modelNew);

        }
    }
}
