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
using Nop.Services.Media;
using Nop.Core.Domain.Media;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Helpers;

namespace Nop.Web.Components
{
    public class CustomerInfoViewComponent : NopViewComponent
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
        private readonly ICustomerModelFactory _customerModelFactory;
        private readonly IPictureService _pictureService;
        private readonly MediaSettings _mediaSettings;
        private readonly ICountryService _countryService;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;

        public CustomerInfoViewComponent(IShoppingCartModelFactory shoppingCartModelFactory,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IOrderModelFactory orderModelFactory,
            IOrderService orderService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext,
            IProductService productService,
            IRewardPointService rewardPointService,
            IPictureService pictureService,
            MediaSettings mediaSettings,
            ICountryService countryService,
            ILocalizationService localizationService,
            IDateTimeHelper dateTimeHelper,
            ICustomerModelFactory customerModelFactory)
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
            _customerModelFactory = customerModelFactory;
            _pictureService = pictureService;
            _mediaSettings = mediaSettings;
            _countryService = countryService;
            _localizationService = localizationService;
            _localizationService = localizationService;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync(int orderId = 0)
        {

            var customer = await _workContext.GetCurrentCustomerAsync();
            var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;

            //future implementation: check if subscription date is valid or not based on the subscription type 3, 6 or 1 year.

            var subscriptionProductId = await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.SubscriptionId, storeId);

            //var model = new SubscriptionModel
            //{
            //    SubscriptionId = subscriptionProductId,
            //    SubscriptionDate = await _genericAttributeService.GetAttributeAsync<DateTime>(customer, NopCustomerDefaults.SubscriptionDate, storeId),
            //    SubscriptionExpiryDate = await _genericAttributeService.GetAttributeAsync<DateTime>(customer, NopCustomerDefaults.SubscriptionExpiryDate, storeId),
            //    AllottedCreditCount = await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.SubscriptionAllottedCount, storeId),
            //    UsedCreditCount = await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.SubscriptionUsedCreditCount, storeId),
            //    IsPaidCustomer = await _customerService.IsInCustomerRoleAsync(customer, "PaidCustomer"),
            //    CustomerProfileTypeId = customer.CustomerProfileTypeId
            //};

            //get subscription product name
            var product = await _productService.GetProductByIdAsync(subscriptionProductId);

            //if (product != null)
            //    model.SubscriptionProduct = product.Name;

            var modelNew = new Models.Customer.CustomerInfoModel
            {
                FirstName = "",
                LastName = "",

                //SubscriptionId = subscriptionProductId,

                //SubscriptionDate = await _rewardPointService.GetSubscriptionStartDateAsync(customer.Id, storeId),
                //SubscriptionExpiryDate = await _rewardPointService.GetSubscriptionExpiryDateAsync(customer.Id, storeId),

                //AllottedCreditCount = await _rewardPointService.GetSubscriptionAlottedCreditCountAsync(customer.Id, storeId),
                //BalanceCreditCount = await _rewardPointService.GetRewardPointsBalanceAsync(customer.Id, storeId),
                //UsedCreditCount = await _rewardPointService.GetSubscriptionUsedCreditCountAsync(customer.Id, storeId),

                //IsPaidCustomer = await _customerService.IsInCustomerRoleAsync(customer, "PaidCustomer"),
                //CustomerProfileTypeId = customer.CustomerProfileTypeId
            };

            var model = new Models.Customer.CustomerInfoModel();
            model = await _customerModelFactory.PrepareCustomerInfoModelAsync(model, customer, false);

            model.CustomerAvatarUrl = await _pictureService.GetPictureUrlAsync(
                    await _genericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute),
                    _mediaSettings.AvatarPictureSize,
                    false);

            var country = await _countryService.GetCountryByIdAsync(model.CountryId);

            if (country != null)
            {
                model.Country = await _localizationService.GetLocalizedAsync(country, x => x.Name);
            }

            //model.JoinedDate = (await _dateTimeHelper.ConvertToUserTimeAsync(customer.CreatedOnUtc, DateTimeKind.Utc)).ToString("f");
            //model.LastActivityDateUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(customer.LastActivityDateUtc, DateTimeKind.Utc)).ToString("f");

            return View(model);

        }
    }
}
