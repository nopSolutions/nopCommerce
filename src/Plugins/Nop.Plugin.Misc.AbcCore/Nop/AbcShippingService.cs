using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Shipping;

namespace Nop.Plugin.Misc.AbcCore.Nop
{
    public class AbcShippingService : ShippingService
    {
        private readonly ILocalizationService _localizationService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IShippingPluginManager _shippingPluginManager;
        private readonly ShippingSettings _shippingSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        public AbcShippingService(IAddressService addressService,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICountryService countryService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILogger logger,
            IPickupPluginManager pickupPluginManager,
            IPriceCalculationService priceCalculationService,
            IProductAttributeParser productAttributeParser,
            IProductService productService,
            IRepository<ShippingMethod> shippingMethodRepository,
            IRepository<ShippingMethodCountryMapping> shippingMethodCountryMappingRepository,
            IRepository<Warehouse> warehouseRepository,
            IShippingPluginManager shippingPluginManager,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            ShippingSettings shippingSettings,
            ShoppingCartSettings shoppingCartSettings
        ) : base(
            addressService,
            checkoutAttributeParser,
            countryService,
            customerService,
            genericAttributeService,
            localizationService,
            logger,
            pickupPluginManager,
            priceCalculationService,
            productAttributeParser,
            productService,
            shippingMethodRepository,
            shippingMethodCountryMappingRepository,
            warehouseRepository,
            shippingPluginManager,
            stateProvinceService,
            storeContext,
            shippingSettings,
            shoppingCartSettings
        )
        {
            _localizationService = localizationService;
            _priceCalculationService = priceCalculationService;
            _shippingPluginManager = shippingPluginManager;
            _shippingSettings = shippingSettings;
            _shoppingCartSettings = shoppingCartSettings;
        }

        // From Nop.Services.Shipping.ShippingService.GetShippingOptionsAsync()
        public override async Task<GetShippingOptionResponse> GetShippingOptionsAsync(IList<ShoppingCartItem> cart,
            Address shippingAddress, Customer customer = null, string allowedShippingRateComputationMethodSystemName = "",
            int storeId = 0)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            var result = new GetShippingOptionResponse();

            //create a package
            var (shippingOptionRequests, shippingFromMultipleLocations) = await CreateShippingOptionRequestsAsync(cart, shippingAddress, storeId);
            result.ShippingFromMultipleLocations = shippingFromMultipleLocations;

            var shippingRateComputationMethods = await _shippingPluginManager
                .LoadActivePluginsAsync(customer, storeId, allowedShippingRateComputationMethodSystemName);
            if (!shippingRateComputationMethods.Any())
                return result;

            //request shipping options from each shipping rate computation methods
            foreach (var srcm in shippingRateComputationMethods)
            {
                //request shipping options (separately for each package-request)
                IList<ShippingOption> srcmShippingOptions = null;
                foreach (var shippingOptionRequest in shippingOptionRequests)
                {
                    var getShippingOptionResponse = await srcm.GetShippingOptionsAsync(shippingOptionRequest);

                    if (getShippingOptionResponse.Success)
                    {
                        //success
                        if (srcmShippingOptions == null)
                        {
                            //first shipping option request
                            srcmShippingOptions = getShippingOptionResponse.ShippingOptions;
                        }
                        else
                        {
                            //get shipping options which already exist for prior requested packages for this scrm (i.e. common options)
                            srcmShippingOptions = srcmShippingOptions
                                .Where(existingso => getShippingOptionResponse.ShippingOptions.Any(newso => newso.Name == existingso.Name))
                                .ToList();

                            //and sum the rates
                            foreach (var existingso in srcmShippingOptions)
                            {
                                existingso.Rate += getShippingOptionResponse
                                    .ShippingOptions
                                    .First(newso => newso.Name == existingso.Name)
                                    .Rate;
                            }
                        }
                    }
                    else
                    {
                        //errors
                        foreach (var error in getShippingOptionResponse.Errors)
                        {
                            result.AddError(error);
                            // ABC: Don't log the warnings, since they're user-caused.
                            //await _logger.WarningAsync($"Shipping ({srcm.PluginDescriptor.FriendlyName}). {error}");
                        }
                        //clear the shipping options in this case
                        srcmShippingOptions = new List<ShippingOption>();
                        break;
                    }
                }

                //add this scrm's options to the result
                if (srcmShippingOptions == null)
                    continue;

                foreach (var so in srcmShippingOptions)
                {
                    //set system name if not set yet
                    if (string.IsNullOrEmpty(so.ShippingRateComputationMethodSystemName))
                        so.ShippingRateComputationMethodSystemName = srcm.PluginDescriptor.SystemName;
                    if (_shoppingCartSettings.RoundPricesDuringCalculation)
                        so.Rate = await _priceCalculationService.RoundPriceAsync(so.Rate);
                    result.ShippingOptions.Add(so);
                }
            }

            if (_shippingSettings.ReturnValidOptionsIfThereAreAny)
            {
                //return valid options if there are any (no matter of the errors returned by other shipping rate computation methods).
                if (result.ShippingOptions.Any() && result.Errors.Any())
                    result.Errors.Clear();
            }

            //no shipping options loaded
            if (!result.ShippingOptions.Any() && !result.Errors.Any())
                result.Errors.Add(await _localizationService.GetResourceAsync("Checkout.ShippingOptionCouldNotBeLoaded"));

            return result;
        }
    }
}