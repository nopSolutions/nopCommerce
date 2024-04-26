using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
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

namespace Nop.Web.Factories;

public partial class CheckoutModelFactory : ICheckoutModelFactory
{
    #region Fields

    protected readonly AddressSettings _addressSettings;
    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CommonSettings _commonSettings;
    protected readonly IAddressModelFactory _addressModelFactory;
    protected readonly IAddressService _addressService;
    protected readonly ICountryService _countryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerService _customerService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IOrderProcessingService _orderProcessingService;
    protected readonly IOrderTotalCalculationService _orderTotalCalculationService;
    protected readonly IPaymentPluginManager _paymentPluginManager;
    protected readonly IPaymentService _paymentService;
    protected readonly IPickupPluginManager _pickupPluginManager;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IRewardPointService _rewardPointService;
    protected readonly IShippingPluginManager _shippingPluginManager;
    protected readonly IShippingService _shippingService;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly ITaxService _taxService;
    protected readonly IWorkContext _workContext;
    protected readonly OrderSettings _orderSettings;
    protected readonly PaymentSettings _paymentSettings;
    protected readonly RewardPointsSettings _rewardPointsSettings;
    protected readonly ShippingSettings _shippingSettings;
    protected readonly TaxSettings _taxSettings;

    #endregion

    #region Ctor

    public CheckoutModelFactory(AddressSettings addressSettings,
        CaptchaSettings captchaSettings,
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
        ShippingSettings shippingSettings,
        TaxSettings taxSettings)
    {
        _addressSettings = addressSettings;
        _captchaSettings = captchaSettings;
        _commonSettings = commonSettings;
        _addressModelFactory = addressModelFactory;
        _addressService = addressService;
        _countryService = countryService;
        _currencyService = currencyService;
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _orderProcessingService = orderProcessingService;
        _orderTotalCalculationService = orderTotalCalculationService;
        _paymentPluginManager = paymentPluginManager;
        _paymentService = paymentService;
        _pickupPluginManager = pickupPluginManager;
        _priceFormatter = priceFormatter;
        _rewardPointService = rewardPointService;
        _shippingPluginManager = shippingPluginManager;
        _shippingService = shippingService;
        _shoppingCartService = shoppingCartService;
        _stateProvinceService = stateProvinceService;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _taxService = taxService;
        _workContext = workContext;
        _orderSettings = orderSettings;
        _paymentSettings = paymentSettings;
        _rewardPointsSettings = rewardPointsSettings;
        _shippingSettings = shippingSettings;
        _taxSettings = taxSettings;
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
            AllowPickupInStore = _shippingSettings.AllowPickupInStore
        };

        if (!model.AllowPickupInStore)
            return model;

        model.DisplayPickupPointsOnMap = _shippingSettings.DisplayPickupPointsOnMap;
        model.GoogleMapsApiKey = _shippingSettings.GoogleMapsApiKey;
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var pickupPointProviders = await _pickupPluginManager.LoadActivePluginsAsync(customer, store.Id);
        if (pickupPointProviders.Any())
        {
            var languageId = (await _workContext.GetWorkingLanguageAsync()).Id;
            var address = customer.BillingAddressId.HasValue
                ? await _addressService.GetAddressByIdAsync(customer.BillingAddressId.Value)
                : null;
            var pickupPointsResponse = await _shippingService.GetPickupPointsAsync(cart, address,
                customer, storeId: store.Id);
            if (pickupPointsResponse.Success)
            {
                var selectedPickupPoint = await _genericAttributeService
                    .GetAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, store.Id);

                var selectedShippingOption = await _genericAttributeService
                    .GetAttributeAsync<ShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, store.Id);

                model.PickupInStore = selectedShippingOption is not null && selectedShippingOption.IsPickupInStore;

                model.PickupPoints = await pickupPointsResponse.PickupPoints.SelectAwait(async point =>
                {
                    var country = await _countryService.GetCountryByTwoLetterIsoCodeAsync(point.CountryCode);
                    var state = await _stateProvinceService.GetStateProvinceByAbbreviationAsync(point.StateAbbreviation, country?.Id);

                    var pickupPointModel = new CheckoutPickupPointModel
                    {
                        Id = point.Id,
                        Name = point.Name,
                        Description = point.Description,
                        ProviderSystemName = point.ProviderSystemName,
                        Address = point.Address,
                        City = point.City,
                        County = point.County,
                        StateName = state != null ? await _localizationService.GetLocalizedAsync(state, x => x.Name, languageId) : string.Empty,
                        CountryName = country != null ? await _localizationService.GetLocalizedAsync(country, x => x.Name, languageId) : string.Empty,
                        ZipPostalCode = point.ZipPostalCode,
                        Latitude = point.Latitude,
                        Longitude = point.Longitude,
                        OpeningHours = point.OpeningHours,
                        IsPreSelected = selectedPickupPoint is not null && selectedPickupPoint.Id == point.Id,
                    };
                    
                    var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
                    
                    //adjust rate
                    var (shippingTotal, _) = await _orderTotalCalculationService.AdjustShippingRateAsync(point.PickupFee, cart, true);
                    var (rateBase, _) = await _taxService.GetShippingPriceAsync(shippingTotal, customer);
                    var rate = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(rateBase, currentCurrency);
                    pickupPointModel.PickupFee = await _priceFormatter.FormatShippingPriceAsync(rate, true);

                    return pickupPointModel;
                }).ToListAsync();
            }
            else
                foreach (var error in pickupPointsResponse.Errors)
                    model.Warnings.Add(error);
        }

        //only available pickup points
        var shippingProviders = await _shippingPluginManager.LoadActivePluginsAsync(customer, store.Id);
        if (!shippingProviders.Any())
        {
            if (!pickupPointProviders.Any())
            {
                model.Warnings.Add(await _localizationService.GetResourceAsync("Checkout.ShippingIsNotAllowed"));
                model.Warnings.Add(await _localizationService.GetResourceAsync("Checkout.PickupPoints.NotAvailable"));
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
            ShipToSameAddressAllowed = _shippingSettings.ShipToSameAddress && await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart),
            //allow customers to enter (choose) a shipping address if "Disable Billing address step" setting is enabled
            ShipToSameAddress = !_orderSettings.DisableBillingAddressCheckoutStep
        };

        var customer = await _workContext.GetCurrentCustomerAsync();
        if (await _customerService.IsGuestAsync(customer) && _taxSettings.EuVatEnabled)
        {
            model.VatNumber = customer.VatNumber;
            model.EuVatEnabled = true;
            model.EuVatEnabledForGuests = _taxSettings.EuVatEnabledForGuests;
        }

        //existing addresses
        var addresses = await (await _customerService.GetAddressesByCustomerIdAsync(customer.Id))
            .WhereAwait(async a => !a.CountryId.HasValue || await _countryService.GetCountryByAddressAsync(a) is
                {
                    Published: true, 
                    AllowsBilling: true
                } country
                &&
                //enabled for the current store
                await _storeMappingService.AuthorizeAsync(country))
            .ToListAsync();
        foreach (var address in addresses)
        {
            var addressModel = new AddressModel();
            await _addressModelFactory.PrepareAddressModelAsync(addressModel,
                address: address,
                excludeProperties: false,
                addressSettings: _addressSettings);

            if (await _addressService.IsAddressValidAsync(address))
                model.ExistingAddresses.Add(addressModel);
            else
                model.InvalidExistingAddresses.Add(addressModel);
        }

        //new address
        model.BillingNewAddress.CountryId = selectedCountryId;
        await _addressModelFactory.PrepareAddressModelAsync(model.BillingNewAddress,
            address: null,
            excludeProperties: false,
            addressSettings: _addressSettings,
            loadCountries: async () => await _countryService.GetAllCountriesForBillingAsync((await _workContext.GetWorkingLanguageAsync()).Id),
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
            DisplayPickupInStore = !_orderSettings.DisplayPickupInStoreOnShippingMethodPage
        };

        if (!_orderSettings.DisplayPickupInStoreOnShippingMethodPage)
            model.PickupPointsModel = await PrepareCheckoutPickupPointsModelAsync(cart);

        //existing addresses
        var customer = await _workContext.GetCurrentCustomerAsync();
        var addresses = await (await _customerService.GetAddressesByCustomerIdAsync(customer.Id))
            .WhereAwait(async a => !a.CountryId.HasValue || await _countryService.GetCountryByAddressAsync(a) is
                {
                    Published: true, 
                    AllowsShipping: true
                } country
                &&
                //enabled for the current store
                await _storeMappingService.AuthorizeAsync(country))
            .ToListAsync();
        foreach (var address in addresses)
        {
            var addressModel = new AddressModel();
            await _addressModelFactory.PrepareAddressModelAsync(addressModel,
                address: address,
                excludeProperties: false,
                addressSettings: _addressSettings);

            if (await _addressService.IsAddressValidAsync(address))
                model.ExistingAddresses.Add(addressModel);
            else
                model.InvalidExistingAddresses.Add(addressModel);
        }

        //new address
        model.ShippingNewAddress.CountryId = selectedCountryId;
        await _addressModelFactory.PrepareAddressModelAsync(model.ShippingNewAddress,
            address: null,
            excludeProperties: false,
            addressSettings: _addressSettings,
            loadCountries: async () => await _countryService.GetAllCountriesForShippingAsync((await _workContext.GetWorkingLanguageAsync()).Id),
            prePopulateWithCustomerFields: prePopulateNewAddressWithCustomerFields,
            customer: customer,
            overrideAttributesXml: overrideAttributesXml);

        model.SelectedBillingAddress = customer.BillingAddressId ?? 0;

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
            DisplayPickupInStore = _orderSettings.DisplayPickupInStoreOnShippingMethodPage
        };

        if (_orderSettings.DisplayPickupInStoreOnShippingMethodPage)
            model.PickupPointsModel = await PrepareCheckoutPickupPointsModelAsync(cart);

        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var getShippingOptionResponse = await _shippingService.GetShippingOptionsAsync(cart, shippingAddress, customer, storeId: store.Id);
        if (getShippingOptionResponse.Success)
        {
            //performance optimization. cache returned shipping options.
            //we'll use them later (after a customer has selected an option).
            await _genericAttributeService.SaveAttributeAsync(customer,
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
                var (shippingTotal, _) = await _orderTotalCalculationService.AdjustShippingRateAsync(shippingOption.Rate, cart, shippingOption.IsPickupInStore);

                var (rateBase, _) = await _taxService.GetShippingPriceAsync(shippingTotal, customer);
                var rate = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(rateBase, await _workContext.GetWorkingCurrencyAsync());
                soModel.Fee = await _priceFormatter.FormatShippingPriceAsync(rate, true);
                soModel.Rate = rate;
                model.ShippingMethods.Add(soModel);
            }

            //sort shipping methods
            if (model.ShippingMethods.Count > 1)
                model.ShippingMethods = (_shippingSettings.ShippingSorting switch
                {
                    ShippingSortingEnum.ShippingCost => model.ShippingMethods.OrderBy(option => option.Rate),
                    _ => model.ShippingMethods.OrderBy(option => option.DisplayOrder)
                }).ToList();

            //find a selected (previously) shipping method
            var selectedShippingOption = await _genericAttributeService.GetAttributeAsync<ShippingOption>(customer,
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
                    shippingOptionToSelect.Selected = true;
            }
            //if no option has been selected, let's do it for the first one
            if (model.ShippingMethods.FirstOrDefault(so => so.Selected) == null)
            {
                var shippingOptionToSelect = model.ShippingMethods.FirstOrDefault();
                if (shippingOptionToSelect != null) 
                    shippingOptionToSelect.Selected = true;
            }

            //notify about shipping from multiple locations
            if (_shippingSettings.NotifyCustomerAboutShippingFromMultipleLocations) 
                model.NotifyCustomerAboutShippingFromMultipleLocations = getShippingOptionResponse.ShippingFromMultipleLocations;
        }
        else
            foreach (var error in getShippingOptionResponse.Errors)
                model.Warnings.Add(error);

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

        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();

        //reward points
        if (_rewardPointsSettings.Enabled && !await _shoppingCartService.ShoppingCartIsRecurringAsync(cart))
        {
            var shoppingCartTotal = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart, true, false);
            if (shoppingCartTotal.redeemedRewardPoints > 0)
            {
                model.DisplayRewardPoints = true;
                model.RewardPointsToUseAmount = await _priceFormatter.FormatPriceAsync(shoppingCartTotal.redeemedRewardPointsAmount, true, false);
                model.RewardPointsToUse = shoppingCartTotal.redeemedRewardPoints;
                model.RewardPointsBalance = await _rewardPointService.GetRewardPointsBalanceAsync(customer.Id, store.Id);

                //are points enough to pay for entire order? like if this option (to use them) was selected
                model.RewardPointsEnoughToPayForOrder = !await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart, true);
            }
        }

        //filter by country
        var paymentMethods = await (await _paymentPluginManager
                .LoadActivePluginsAsync(customer, store.Id, filterByCountryId))
            .Where(pm => pm.PaymentMethodType == PaymentMethodType.Standard || pm.PaymentMethodType == PaymentMethodType.Redirection)
            .WhereAwait(async pm => !await pm.HidePaymentMethodAsync(cart))
            .ToListAsync();
        foreach (var pm in paymentMethods)
        {
            if (await _shoppingCartService.ShoppingCartIsRecurringAsync(cart) && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                continue;

            var pmModel = new CheckoutPaymentMethodModel.PaymentMethodModel
            {
                Name = await _localizationService.GetLocalizedFriendlyNameAsync(pm, (await _workContext.GetWorkingLanguageAsync()).Id),
                Description = _paymentSettings.ShowPaymentMethodDescriptions ? await pm.GetPaymentMethodDescriptionAsync() : string.Empty,
                PaymentMethodSystemName = pm.PluginDescriptor.SystemName,
                LogoUrl = await _paymentPluginManager.GetPluginLogoUrlAsync(pm)
            };
            //payment method additional fee
            var paymentMethodAdditionalFee = await _paymentService.GetAdditionalHandlingFeeAsync(cart, pm.PluginDescriptor.SystemName);
            var (rateBase, _) = await _taxService.GetPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFee, customer);
            var rate = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(rateBase, await _workContext.GetWorkingCurrencyAsync());

            if (rate > decimal.Zero)
                pmModel.Fee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(rate, true);

            model.PaymentMethods.Add(pmModel);
        }

        //find a selected (previously) payment method
        var selectedPaymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer,
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
            PaymentViewComponent = paymentMethod.GetPublicViewComponent(),
            DisplayOrderTotals = _orderSettings.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab
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
            TermsOfServiceOnOrderConfirmPage = _orderSettings.TermsOfServiceOnOrderConfirmPage,
            TermsOfServicePopup = _commonSettings.PopupForTermsOfServiceLinks,
            DisplayCaptcha = await _customerService.IsGuestAsync(await _customerService.GetShoppingCartCustomerAsync(cart))
                             && _captchaSettings.Enabled && _captchaSettings.ShowOnCheckoutPageForGuests
        };
        //min order amount validation
        var minOrderTotalAmountOk = await _orderProcessingService.ValidateMinOrderTotalAmountAsync(cart);
        if (!minOrderTotalAmountOk)
        {
            var minOrderTotalAmount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(_orderSettings.MinOrderTotalAmount, await _workContext.GetWorkingCurrencyAsync());
            model.MinOrderTotalWarning = string.Format(await _localizationService.GetResourceAsync("Checkout.MinOrderTotalAmount"), await _priceFormatter.FormatPriceAsync(minOrderTotalAmount, true, false));
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
        ArgumentNullException.ThrowIfNull(order);

        var model = new CheckoutCompletedModel
        {
            OrderId = order.Id,
            OnePageCheckoutEnabled = _orderSettings.OnePageCheckoutEnabled,
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
        ArgumentNullException.ThrowIfNull(cart);

        var customer = await _workContext.GetCurrentCustomerAsync();

        var model = new OnePageCheckoutModel
        {
            ShippingRequired = await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart),
            DisableBillingAddressCheckoutStep = _orderSettings.DisableBillingAddressCheckoutStep && (await _customerService.GetAddressesByCustomerIdAsync(customer.Id)).Any(),
            BillingAddress = await PrepareBillingAddressModelAsync(cart, prePopulateNewAddressWithCustomerFields: true),
            DisplayCaptcha = await _customerService.IsGuestAsync(await _customerService.GetShoppingCartCustomerAsync(cart))
                             && _captchaSettings.Enabled && _captchaSettings.ShowOnCheckoutPageForGuests,
            IsReCaptchaV3 = _captchaSettings.CaptchaType == CaptchaType.ReCaptchaV3,
            ReCaptchaPublicKey = _captchaSettings.ReCaptchaPublicKey
        };
        return model;
    }

    #endregion
}