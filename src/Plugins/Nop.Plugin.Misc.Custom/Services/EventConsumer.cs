using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Orders;
using System.Linq;
using System.Threading.Tasks;
using Nop.Services.Affiliates;
using Nop.Web.Areas.Admin.Models.Affiliates;
using Nop.Core.Domain.Affiliates;
using Nop.Services.Localization;


namespace Nop.Plugin.Misc.Custom.Services
{
    /// <summary>
    /// Represents plugin event consumer
    /// </summary>
    public class EventConsumerCustom : IConsumer<OrderPaidEvent>, IConsumer<CustomerRegisteredEvent>
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerService _customerService;
        private readonly ILogger _logger;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IOrderService _orderService;
        private readonly IStoreContext _storeContext;
        private readonly IAffiliateService _affiliateService;
        private readonly IAddressService _addressService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public EventConsumerCustom(IGenericAttributeService genericAttributeService,
             ICustomerService customerService,
             ILogger logger,
             IStoreContext storeContext,
             ShoppingCartSettings shoppingCartSettings,
             IOrderService orderService,
             IAffiliateService affiliateService,
             IAddressService addressService,
             ILocalizationService localizationService,
             ICustomerActivityService customerActivityService)
        {
            _genericAttributeService = genericAttributeService;
            _customerService = customerService;
            _logger = logger;
            _customerActivityService = customerActivityService;
            _shoppingCartSettings = shoppingCartSettings;
            _orderService = orderService;
            _storeContext = storeContext;
            _affiliateService = affiliateService;
            _addressService = addressService;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        public async Task HandleEventAsync(OrderPaidEvent eventMessage)
        {
            await AddCustomerToPaidCustomerRole(eventMessage.Order.CustomerId);
            await AddCustomerGenericAttributes(eventMessage.Order);
        }

        public async Task HandleEventAsync(CustomerRegisteredEvent eventMessage)
        {
            var customer = eventMessage.Customer;
            var address = await _customerService.GetAddressesByCustomerIdAsync(customer.Id);

            var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
            var firstName = customer.FirstName;
            var lastName = customer.LastName;

            //affiliate.AddressId = address.Id;

            var affiliate = new Affiliate
            {
                Active = true,
                AdminComment = "Affiliate created for customer",
                FriendlyUrlName = ""
            };

            //validate friendly URL name
            var freindlyName = string.Format("referral-{0}-{1}", firstName, lastName);
            var friendlyUrlName = await _affiliateService.ValidateFriendlyUrlNameAsync(affiliate, freindlyName);

            affiliate.FriendlyUrlName = friendlyUrlName;
            //affiliate.AddressId = address.FirstOrDefault().Id;

            //await _affiliateService.InsertAffiliateAsync(affiliate);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewAffiliate",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewAffiliate"), affiliate.Id), affiliate);

        }

        public async Task HandleEventAsync(EntityInsertedEvent<Customer> eventMessage)
        {
            await Task.FromResult(0);
        }

        public async Task AddCustomerToPaidCustomerRole(int customerId)
        {
            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            var isCustomerInPaidCustomerRole = await _customerService.IsInCustomerRoleAsync(customer, NopCustomerDefaults.PaidCustomerRoleName, true);

            if (!isCustomerInPaidCustomerRole)
            {
                //add customer to paidcustomer role. CustomerRoleId= 9 - PaidCustomer
                await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customerId, CustomerRoleId = 9 });

                //customer activity
                await _customerActivityService.InsertActivityAsync(customer, "PublicStore.EditCustomerAvailabilityToTrue", "Customer Has Been Added To PaidCustomer Role ", customer);

            }
            else
                await _customerActivityService.InsertActivityAsync(customer, "PublicStore.EditCustomerAvailabilityToTrue", "Customer already having PaidCustomer Role.Paid again may be by mistake.", customer);

        }

        public async Task AddCustomerGenericAttributes(Order order)
        {
            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

            var activeOrderItems = await _orderService.GetOrderItemsAsync(order.Id);
            var customerSubscribedProductId = activeOrderItems.FirstOrDefault().ProductId;

            var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
            var allottedCount = 00;

            if (customerSubscribedProductId == _shoppingCartSettings.ThreeMonthSubscriptionProductId)
            {
                allottedCount = _shoppingCartSettings.ThreeMonthSubscriptionAllottedCount;
            }
            else if (customerSubscribedProductId == _shoppingCartSettings.SixMonthSubscriptionProductId)
            {
                allottedCount = _shoppingCartSettings.SixMonthSubscriptionAllottedCount;
            }
            else if (customerSubscribedProductId == _shoppingCartSettings.OneYearSubscriptionProductId)
            {
                allottedCount = _shoppingCartSettings.OneYearSubscriptionAllottedCount;
            }

            var SubscriptionId = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SubscriptionId, storeId);
            var SubscriptionAllottedCount = await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.SubscriptionAllottedCount, storeId);
            var SubscriptionDate = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SubscriptionDate, storeId);

            // carry forward previous credits
            allottedCount += SubscriptionAllottedCount;

            var oldSubscriptionInfo = string.Format("Old Subscription Info - Customer Email:{0} ; SubscriptionId: {1} ; Credits: {2} ; SubscriptionDate: {3}",
                                        customer.Email,
                                        SubscriptionId,
                                        SubscriptionAllottedCount,
                                        SubscriptionDate);

            //customer activity : Before updating the new subscription , save the old subscription details
            await _customerActivityService.InsertActivityAsync(customer, "PublicStore.EditCustomerAvailabilityToTrue", oldSubscriptionInfo, customer);

            //save SubscriptionId, credits , subscription date 
            await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SubscriptionId, customerSubscribedProductId, storeId);
            await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SubscriptionAllottedCount, allottedCount, storeId);
            await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SubscriptionDate, order.CreatedOnUtc, storeId);


            var newSubscriptionInfo = string.Format("New Subscription Info - Customer Email:{0} ; SubscriptionId: {1} ; Credits: {2} ; SubscriptionDate: {3}",
                                        customer.Email,
                                        customerSubscribedProductId,
                                        allottedCount,
                                        order.CreatedOnUtc.ToString());

            //customer activity
            await _customerActivityService.InsertActivityAsync(customer, "PublicStore.EditCustomerAvailabilityToTrue", newSubscriptionInfo, customer);

        }


        #endregion
    }
}