using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Models.Checkout;
using Nop.Web.Models.Common;
using Nop.Web.Factories;

namespace Nop.Plugin.Misc.AbcCore.Factories
{
    // always provide "US" as the selectedCountryId, ignore what comes
    // from `selectedCountryId`
    public partial class AbcCheckoutModelFactory : CheckoutModelFactory, ICheckoutModelFactory
    {
        private const int US_COUNTRY_ID = 1;

        public AbcCheckoutModelFactory(AddressSettings addressSettings,
            CommonSettings commonSettings,
            IAddressModelFactory addressModelFactory,
            IAddressService addressService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IOrderProcessingService orderProcessingService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IPickupPluginManager pickupPluginManager,
            IPriceFormatter priceFormatter,
            IRewardPointService rewardPointService,
            IShippingPluginManager shippingPluginManager,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            ITaxService taxService,
            IWorkContext workContext,
            OrderSettings orderSettings,
            PaymentSettings paymentSettings,
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings
        ) : base(
            addressSettings, commonSettings, addressModelFactory,
            addressService, countryService, currencyService, customerService,
            genericAttributeService, localizationService, orderProcessingService,
            orderTotalCalculationService, paymentPluginManager, paymentService,
            pickupPluginManager, priceFormatter, rewardPointService,
            shippingPluginManager, shippingService, shoppingCartService,
            stateProvinceService, storeContext, storeMappingService, taxService,
            workContext, orderSettings, paymentSettings, rewardPointsSettings,
            shippingSettings
        )
            {}

        public override async Task<CheckoutBillingAddressModel> PrepareBillingAddressModelAsync(
            IList<ShoppingCartItem> cart,
            int? selectedCountryId = null,
            bool prePopulateNewAddressWithCustomerFields = false,
            string overrideAttributesXml = "")
        {
            
            var model = await base.PrepareBillingAddressModelAsync(cart,
                US_COUNTRY_ID,
                prePopulateNewAddressWithCustomerFields,
                overrideAttributesXml);
            return model;
        }

        public override async Task<CheckoutShippingAddressModel> PrepareShippingAddressModelAsync(
            IList<ShoppingCartItem> cart,
            int? selectedCountryId = null,
            bool prePopulateNewAddressWithCustomerFields = false,
            string overrideAttributesXml = "")
        {
            var model = await base.PrepareShippingAddressModelAsync(cart,
                US_COUNTRY_ID,
                prePopulateNewAddressWithCustomerFields,
                overrideAttributesXml);
            return model;
        }
    }
}
