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

namespace Nop.Web.Factories
{
    public partial class CheckoutModelFactory : ICheckoutModelFactory
    {
        #region Fields

        protected AddressSettings AddressSettings { get; }
        protected CommonSettings CommonSettings { get; }
        protected IAddressModelFactory AddressModelFactory { get; }
        protected IAddressService AddressService { get; }
        protected ICountryService CountryService { get; }
        protected ICurrencyService CurrencyService { get; }
        protected ICustomerService CustomerService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IOrderProcessingService OrderProcessingService { get; }
        protected IOrderTotalCalculationService OrderTotalCalculationService { get; }
        protected IPaymentPluginManager PaymentPluginManager { get; }
        protected IPaymentService PaymentService { get; }
        protected IPickupPluginManager PickupPluginManager { get; }
        protected IPriceFormatter PriceFormatter { get; }
        protected IRewardPointService RewardPointService { get; }
        protected IShippingPluginManager ShippingPluginManager { get; }
        protected IShippingService ShippingService { get; }
        protected IShoppingCartService ShoppingCartService { get; }
        protected IStateProvinceService StateProvinceService { get; }
        protected IStoreContext StoreContext { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected ITaxService TaxService { get; }
        protected IWorkContext WorkContext { get; }
        protected OrderSettings OrderSettings { get; }
        protected PaymentSettings PaymentSettings { get; }
        protected RewardPointsSettings RewardPointsSettings { get; }
        protected ShippingSettings ShippingSettings { get; }

        #endregion

        #region Ctor

        public CheckoutModelFactory(AddressSettings addressSettings,
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
            ShippingSettings shippingSettings)
        {
            AddressSettings = addressSettings;
            CommonSettings = commonSettings;
            AddressModelFactory = addressModelFactory;
            AddressService = addressService;
            CountryService = countryService;
            CurrencyService = currencyService;
            CustomerService = customerService;
            GenericAttributeService = genericAttributeService;
            LocalizationService = localizationService;
            OrderProcessingService = orderProcessingService;
            OrderTotalCalculationService = orderTotalCalculationService;
            PaymentPluginManager = paymentPluginManager;
            PaymentService = paymentService;
            PickupPluginManager = pickupPluginManager;
            PriceFormatter = priceFormatter;
            RewardPointService = rewardPointService;
            ShippingPluginManager = shippingPluginManager;
            ShippingService = shippingService;
            ShoppingCartService = shoppingCartService;
            StateProvinceService = stateProvinceService;
            StoreContext = storeContext;
            StoreMappingService = storeMappingService;
            TaxService = taxService;
            WorkContext = workContext;
            OrderSettings = orderSettings;
            PaymentSettings = paymentSettings;
            RewardPointsSettings = rewardPointsSettings;
            ShippingSettings = shippingSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepares the checkout pickup points model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the checkout pickup points model
        /// </returns>
        protected virtual async Task<CheckoutPickupPointsModel> PrepareCheckoutPickupPointsModelAsync(IList<ShoppingCartItem> cart)
        {
            var model = new CheckoutPickupPointsModel
            {
                AllowPickupInStore = ShippingSettings.AllowPickupInStore
            };

            if (!model.AllowPickupInStore) 
                return model;

            model.DisplayPickupPointsOnMap = ShippingSettings.DisplayPickupPointsOnMap;
            model.GoogleMapsApiKey = ShippingSettings.GoogleMapsApiKey;
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var pickupPointProviders = await PickupPluginManager.LoadActivePluginsAsync(customer, store.Id);
            if (pickupPointProviders.Any())
            {
                var languageId = (await WorkContext.GetWorkingLanguageAsync()).Id;
                var pickupPointsResponse = await ShippingService.GetPickupPointsAsync(customer.BillingAddressId ?? 0,
                    customer, storeId: store.Id);
                if (pickupPointsResponse.Success)
                    model.PickupPoints = await pickupPointsResponse.PickupPoints.SelectAwait(async point =>
                    {
                        var country = await CountryService.GetCountryByTwoLetterIsoCodeAsync(point.CountryCode);
                        var state = await StateProvinceService.GetStateProvinceByAbbreviationAsync(point.StateAbbreviation, country?.Id);

                        var pickupPointModel = new CheckoutPickupPointModel
                        {
                            Id = point.Id,
                            Name = point.Name,
                            Description = point.Description,
                            ProviderSystemName = point.ProviderSystemName,
                            Address = point.Address,
                            City = point.City,
                            County = point.County,
                            StateName = state != null ? await LocalizationService.GetLocalizedAsync(state, x => x.Name, languageId) : string.Empty,
                            CountryName = country != null ? await LocalizationService.GetLocalizedAsync(country, x => x.Name, languageId) : string.Empty,
                            ZipPostalCode = point.ZipPostalCode,
                            Latitude = point.Latitude,
                            Longitude = point.Longitude,
                            OpeningHours = point.OpeningHours
                        };

                        var cart = await ShoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
                        var amount = await OrderTotalCalculationService.IsFreeShippingAsync(cart) ? 0 : point.PickupFee;
                        var currentCurrency = await WorkContext.GetWorkingCurrencyAsync();

                        if (amount > 0)
                        {
                            (amount, _) = await TaxService.GetShippingPriceAsync(amount, customer);
                            amount = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(amount, currentCurrency);
                            pickupPointModel.PickupFee = await PriceFormatter.FormatShippingPriceAsync(amount, true);
                        }

                        //adjust rate
                        var (shippingTotal, _) = await OrderTotalCalculationService.AdjustShippingRateAsync(point.PickupFee, cart, true);
                        var (rateBase, _) = await TaxService.GetShippingPriceAsync(shippingTotal, customer);
                        var rate = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(rateBase, currentCurrency);
                        pickupPointModel.PickupFee = await PriceFormatter.FormatShippingPriceAsync(rate, true);

                        return pickupPointModel;
                    }).ToListAsync();
                else
                    foreach (var error in pickupPointsResponse.Errors)
                        model.Warnings.Add(error);
            }

            //only available pickup points
            var shippingProviders = await ShippingPluginManager.LoadActivePluginsAsync(customer, store.Id);
            if (!shippingProviders.Any())
            {
                if (!pickupPointProviders.Any())
                {
                    model.Warnings.Add(await LocalizationService.GetResourceAsync("Checkout.ShippingIsNotAllowed"));
                    model.Warnings.Add(await LocalizationService.GetResourceAsync("Checkout.PickupPoints.NotAvailable"));
                }
                model.PickupInStoreOnly = true;
                model.PickupInStore = true;
                return model;
            }

            return model;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare billing address model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="selectedCountryId">Selected country identifier</param>
        /// <param name="prePopulateNewAddressWithCustomerFields">Pre populate new address with customer fields</param>
        /// <param name="overrideAttributesXml">Override attributes xml</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the billing address model
        /// </returns>
        public virtual async Task<CheckoutBillingAddressModel> PrepareBillingAddressModelAsync(IList<ShoppingCartItem> cart,
            int? selectedCountryId = null,
            bool prePopulateNewAddressWithCustomerFields = false,
            string overrideAttributesXml = "")
        {
            var model = new CheckoutBillingAddressModel
            {
                ShipToSameAddressAllowed = ShippingSettings.ShipToSameAddress && await ShoppingCartService.ShoppingCartRequiresShippingAsync(cart),
                //allow customers to enter (choose) a shipping address if "Disable Billing address step" setting is enabled
                ShipToSameAddress = !OrderSettings.DisableBillingAddressCheckoutStep
            };

            //existing addresses
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var addresses = await (await CustomerService.GetAddressesByCustomerIdAsync(customer.Id))
                .WhereAwait(async a => !a.CountryId.HasValue || await CountryService.GetCountryByAddressAsync(a) is Country country &&
                    (//published
                    country.Published &&
                    //allow billing
                    country.AllowsBilling &&
                    //enabled for the current store
                    await StoreMappingService.AuthorizeAsync(country)))
                .ToListAsync();
            foreach (var address in addresses)
            {
                var addressModel = new AddressModel();
                await AddressModelFactory.PrepareAddressModelAsync(addressModel,
                    address: address,
                    excludeProperties: false,
                    addressSettings: AddressSettings);

                if (await AddressService.IsAddressValidAsync(address))
                {
                    model.ExistingAddresses.Add(addressModel);
                }
                else
                {
                    model.InvalidExistingAddresses.Add(addressModel);
                }
            }

            //new address
            model.BillingNewAddress.CountryId = selectedCountryId;
            await AddressModelFactory.PrepareAddressModelAsync(model.BillingNewAddress,
                address: null,
                excludeProperties: false,
                addressSettings: AddressSettings,
                loadCountries: async () => await CountryService.GetAllCountriesForBillingAsync((await WorkContext.GetWorkingLanguageAsync()).Id),
                prePopulateWithCustomerFields: prePopulateNewAddressWithCustomerFields,
                customer: customer,
                overrideAttributesXml: overrideAttributesXml);
            return model;
        }

        /// <summary>
        /// Prepare shipping address model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="selectedCountryId">Selected country identifier</param>
        /// <param name="prePopulateNewAddressWithCustomerFields">Pre populate new address with customer fields</param>
        /// <param name="overrideAttributesXml">Override attributes xml</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping address model
        /// </returns>
        public virtual async Task<CheckoutShippingAddressModel> PrepareShippingAddressModelAsync(IList<ShoppingCartItem> cart, 
            int? selectedCountryId = null, bool prePopulateNewAddressWithCustomerFields = false, string overrideAttributesXml = "")
        {
            var model = new CheckoutShippingAddressModel
            {
                DisplayPickupInStore = !OrderSettings.DisplayPickupInStoreOnShippingMethodPage
            };

            if (!OrderSettings.DisplayPickupInStoreOnShippingMethodPage)
                model.PickupPointsModel = await PrepareCheckoutPickupPointsModelAsync(cart);

            //existing addresses
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var addresses = await (await CustomerService.GetAddressesByCustomerIdAsync(customer.Id))
                .WhereAwait(async a => !a.CountryId.HasValue || await CountryService.GetCountryByAddressAsync(a) is Country country &&
                    (//published
                    country.Published &&
                    //allow shipping
                    country.AllowsShipping &&
                    //enabled for the current store
                    await StoreMappingService.AuthorizeAsync(country)))
                .ToListAsync();
            foreach (var address in addresses)
            {
                var addressModel = new AddressModel();
                await AddressModelFactory.PrepareAddressModelAsync(addressModel,
                    address: address,
                    excludeProperties: false,
                    addressSettings: AddressSettings);

                if (await AddressService.IsAddressValidAsync(address))
                {
                    model.ExistingAddresses.Add(addressModel);
                }
                else
                {
                    model.InvalidExistingAddresses.Add(addressModel);
                }
            }

            //new address
            model.ShippingNewAddress.CountryId = selectedCountryId;
            await AddressModelFactory.PrepareAddressModelAsync(model.ShippingNewAddress,
                address: null,
                excludeProperties: false,
                addressSettings: AddressSettings,
                loadCountries: async () => await CountryService.GetAllCountriesForShippingAsync((await WorkContext.GetWorkingLanguageAsync()).Id),
                prePopulateWithCustomerFields: prePopulateNewAddressWithCustomerFields,
                customer: customer,
                overrideAttributesXml: overrideAttributesXml);

            return model;
        }

        /// <summary>
        /// Prepare shipping method model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="shippingAddress">Shipping address</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping method model
        /// </returns>
        public virtual async Task<CheckoutShippingMethodModel> PrepareShippingMethodModelAsync(IList<ShoppingCartItem> cart, Address shippingAddress)
        {
            var model = new CheckoutShippingMethodModel
            {
                DisplayPickupInStore = OrderSettings.DisplayPickupInStoreOnShippingMethodPage
            };

            if (OrderSettings.DisplayPickupInStoreOnShippingMethodPage)
                model.PickupPointsModel = await PrepareCheckoutPickupPointsModelAsync(cart);

            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var getShippingOptionResponse = await ShippingService.GetShippingOptionsAsync(cart, shippingAddress, customer, storeId: store.Id);
            if (getShippingOptionResponse.Success)
            {
                //performance optimization. cache returned shipping options.
                //we'll use them later (after a customer has selected an option).
                await GenericAttributeService.SaveAttributeAsync(customer,
                                                       NopCustomerDefaults.OfferedShippingOptionsAttribute,
                                                       getShippingOptionResponse.ShippingOptions,
                                                       store.Id);

                foreach (var shippingOption in getShippingOptionResponse.ShippingOptions)
                {
                    var soModel = new CheckoutShippingMethodModel.ShippingMethodModel
                    {
                        Name = shippingOption.Name,
                        Description = shippingOption.Description,
                        DisplayOrder = shippingOption.DisplayOrder ?? 0,
                        ShippingRateComputationMethodSystemName = shippingOption.ShippingRateComputationMethodSystemName,
                        ShippingOption = shippingOption,
                    };

                    //adjust rate
                    var (shippingTotal, _) = await OrderTotalCalculationService.AdjustShippingRateAsync(shippingOption.Rate, cart, shippingOption.IsPickupInStore);

                    var (rateBase, _) = await TaxService.GetShippingPriceAsync(shippingTotal, customer);
                    var rate = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(rateBase, await WorkContext.GetWorkingCurrencyAsync());
                    soModel.Fee = await PriceFormatter.FormatShippingPriceAsync(rate, true);
                    soModel.Rate = rate;
                    model.ShippingMethods.Add(soModel);
                }

                //sort shipping methods
                if (model.ShippingMethods.Count > 1)
                {
                    model.ShippingMethods = (ShippingSettings.ShippingSorting switch
                    {
                        ShippingSortingEnum.ShippingCost => model.ShippingMethods.OrderBy(option => option.Rate),
                        _ => model.ShippingMethods.OrderBy(option => option.DisplayOrder)
                    }).ToList();
                }

                //find a selected (previously) shipping method
                var selectedShippingOption = await GenericAttributeService.GetAttributeAsync<ShippingOption>(customer,
                        NopCustomerDefaults.SelectedShippingOptionAttribute, store.Id);
                if (selectedShippingOption != null)
                {
                    var shippingOptionToSelect = model.ShippingMethods.ToList()
                        .Find(so =>
                           !string.IsNullOrEmpty(so.Name) &&
                           so.Name.Equals(selectedShippingOption.Name, StringComparison.InvariantCultureIgnoreCase) &&
                           !string.IsNullOrEmpty(so.ShippingRateComputationMethodSystemName) &&
                           so.ShippingRateComputationMethodSystemName.Equals(selectedShippingOption.ShippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase));
                    if (shippingOptionToSelect != null)
                    {
                        shippingOptionToSelect.Selected = true;
                    }
                }
                //if no option has been selected, let's do it for the first one
                if (model.ShippingMethods.FirstOrDefault(so => so.Selected) == null)
                {
                    var shippingOptionToSelect = model.ShippingMethods.FirstOrDefault();
                    if (shippingOptionToSelect != null)
                    {
                        shippingOptionToSelect.Selected = true;
                    }
                }

                //notify about shipping from multiple locations
                if (ShippingSettings.NotifyCustomerAboutShippingFromMultipleLocations)
                {
                    model.NotifyCustomerAboutShippingFromMultipleLocations = getShippingOptionResponse.ShippingFromMultipleLocations;
                }
            }
            else
            {
                foreach (var error in getShippingOptionResponse.Errors)
                    model.Warnings.Add(error);
            }

            return model;
        }

        /// <summary>
        /// Prepare payment method model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="filterByCountryId">Filter by country identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the payment method model
        /// </returns>
        public virtual async Task<CheckoutPaymentMethodModel> PreparePaymentMethodModelAsync(IList<ShoppingCartItem> cart, int filterByCountryId)
        {
            var model = new CheckoutPaymentMethodModel();
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var store = await StoreContext.GetCurrentStoreAsync();
            var currentCurrency = await WorkContext.GetWorkingCurrencyAsync();
            //reward points
            if (RewardPointsSettings.Enabled && !await ShoppingCartService.ShoppingCartIsRecurringAsync(cart))
            {
                var rewardPointsBalance = await RewardPointService.GetRewardPointsBalanceAsync(customer.Id, store.Id);
                rewardPointsBalance = RewardPointService.GetReducedPointsBalance(rewardPointsBalance);

                var rewardPointsAmountBase = await OrderTotalCalculationService.ConvertRewardPointsToAmountAsync(rewardPointsBalance);
                var rewardPointsAmount = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(rewardPointsAmountBase, currentCurrency);
                if (rewardPointsAmount > decimal.Zero &&
                    OrderTotalCalculationService.CheckMinimumRewardPointsToUseRequirement(rewardPointsBalance))
                {
                    model.DisplayRewardPoints = true;
                    model.RewardPointsAmount = await PriceFormatter.FormatPriceAsync(rewardPointsAmount, true, false);
                    model.RewardPointsBalance = rewardPointsBalance;

                    //are points enough to pay for entire order? like if this option (to use them) was selected
                    model.RewardPointsEnoughToPayForOrder = !await OrderProcessingService.IsPaymentWorkflowRequiredAsync(cart, true);
                }
            }

            //filter by country
            var paymentMethods = await (await PaymentPluginManager
                .LoadActivePluginsAsync(customer, store.Id, filterByCountryId))
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Standard || pm.PaymentMethodType == PaymentMethodType.Redirection)
                .WhereAwait(async pm => !await pm.HidePaymentMethodAsync(cart))
                .ToListAsync();
            foreach (var pm in paymentMethods)
            {
                if (await ShoppingCartService.ShoppingCartIsRecurringAsync(cart) && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                    continue;

                var pmModel = new CheckoutPaymentMethodModel.PaymentMethodModel
                {
                    Name = await LocalizationService.GetLocalizedFriendlyNameAsync(pm, (await WorkContext.GetWorkingLanguageAsync()).Id),
                    Description = PaymentSettings.ShowPaymentMethodDescriptions ? await pm.GetPaymentMethodDescriptionAsync() : string.Empty,
                    PaymentMethodSystemName = pm.PluginDescriptor.SystemName,
                    LogoUrl = await PaymentPluginManager.GetPluginLogoUrlAsync(pm)
                };
                //payment method additional fee
                var paymentMethodAdditionalFee = await PaymentService.GetAdditionalHandlingFeeAsync(cart, pm.PluginDescriptor.SystemName);
                var (rateBase, _) = await TaxService.GetPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFee, customer);
                var rate = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(rateBase, currentCurrency);
                if (rate > decimal.Zero)
                    pmModel.Fee = await PriceFormatter.FormatPaymentMethodAdditionalFeeAsync(rate, true);

                model.PaymentMethods.Add(pmModel);
            }

            //find a selected (previously) payment method
            var selectedPaymentMethodSystemName = await GenericAttributeService.GetAttributeAsync<string>(customer,
                NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
            if (!string.IsNullOrEmpty(selectedPaymentMethodSystemName))
            {
                var paymentMethodToSelect = model.PaymentMethods.ToList()
                    .Find(pm => pm.PaymentMethodSystemName.Equals(selectedPaymentMethodSystemName, StringComparison.InvariantCultureIgnoreCase));
                if (paymentMethodToSelect != null)
                    paymentMethodToSelect.Selected = true;
            }
            //if no option has been selected, let's do it for the first one
            if (model.PaymentMethods.FirstOrDefault(so => so.Selected) == null)
            {
                var paymentMethodToSelect = model.PaymentMethods.FirstOrDefault();
                if (paymentMethodToSelect != null)
                    paymentMethodToSelect.Selected = true;
            }

            return model;
        }

        /// <summary>
        /// Prepare payment info model
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the payment info model
        /// </returns>
        public virtual Task<CheckoutPaymentInfoModel> PreparePaymentInfoModelAsync(IPaymentMethod paymentMethod)
        {
            return Task.FromResult(new CheckoutPaymentInfoModel
            {
                PaymentViewComponentName = paymentMethod.GetPublicViewComponentName(),
                DisplayOrderTotals = OrderSettings.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab
            });
        }

        /// <summary>
        /// Prepare confirm order model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the confirm order model
        /// </returns>
        public virtual async Task<CheckoutConfirmModel> PrepareConfirmOrderModelAsync(IList<ShoppingCartItem> cart)
        {
            var model = new CheckoutConfirmModel
            {
                //terms of service
                TermsOfServiceOnOrderConfirmPage = OrderSettings.TermsOfServiceOnOrderConfirmPage,
                TermsOfServicePopup = CommonSettings.PopupForTermsOfServiceLinks
            };
            //min order amount validation
            var minOrderTotalAmountOk = await OrderProcessingService.ValidateMinOrderTotalAmountAsync(cart);
            if (!minOrderTotalAmountOk)
            {
                var minOrderTotalAmount = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(OrderSettings.MinOrderTotalAmount, await WorkContext.GetWorkingCurrencyAsync());
                model.MinOrderTotalWarning = string.Format(await LocalizationService.GetResourceAsync("Checkout.MinOrderTotalAmount"), await PriceFormatter.FormatPriceAsync(minOrderTotalAmount, true, false));
            }
            return model;
        }

        /// <summary>
        /// Prepare checkout completed model
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the checkout completed model
        /// </returns>
        public virtual Task<CheckoutCompletedModel> PrepareCheckoutCompletedModelAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var model = new CheckoutCompletedModel
            {
                OrderId = order.Id,
                OnePageCheckoutEnabled = OrderSettings.OnePageCheckoutEnabled,
                CustomOrderNumber = order.CustomOrderNumber
            };

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare checkout progress model
        /// </summary>
        /// <param name="step">Step</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the checkout progress model
        /// </returns>
        public virtual Task<CheckoutProgressModel> PrepareCheckoutProgressModelAsync(CheckoutProgressStep step)
        {
            var model = new CheckoutProgressModel { CheckoutProgressStep = step };
            
            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare one page checkout model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the one page checkout model
        /// </returns>
        public virtual async Task<OnePageCheckoutModel> PrepareOnePageCheckoutModelAsync(IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            var customer = await WorkContext.GetCurrentCustomerAsync();

            var model = new OnePageCheckoutModel
            {
                ShippingRequired = await ShoppingCartService.ShoppingCartRequiresShippingAsync(cart),
                DisableBillingAddressCheckoutStep = OrderSettings.DisableBillingAddressCheckoutStep && (await CustomerService.GetAddressesByCustomerIdAsync(customer.Id)).Any(),
                BillingAddress = await PrepareBillingAddressModelAsync(cart, prePopulateNewAddressWithCustomerFields: true)
            };
            return model;
        }

        #endregion
    }
}
