using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Tax.Events;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Tax service
    /// </summary>
    public partial class TaxService : ITaxService
    {
        #region Fields

        protected AddressSettings AddressSettings { get; }
        protected CustomerSettings CustomerSettings { get; }
        protected IAddressService AddressService { get; }
        protected ICountryService CountryService { get; }
        protected ICustomerService CustomerService { get; }
        protected IEventPublisher EventPublisher { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected IGeoLookupService GeoLookupService { get; }
        protected ILogger Logger { get; }
        protected IStateProvinceService StateProvinceService { get; }
        protected IStoreContext StoreContext { get; }
        protected ITaxPluginManager TaxPluginManager { get; }
        protected IWebHelper WebHelper { get; }
        protected IWorkContext WorkContext { get; }
        protected ShippingSettings ShippingSettings { get; }
        protected TaxSettings TaxSettings { get; }

        #endregion

        #region Ctor

        public TaxService(AddressSettings addressSettings,
            CustomerSettings customerSettings,
            IAddressService addressService,
            ICountryService countryService,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            IGeoLookupService geoLookupService,
            ILogger logger,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            ITaxPluginManager taxPluginManager,
            IWebHelper webHelper,
            IWorkContext workContext,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings)
        {
            AddressSettings = addressSettings;
            CustomerSettings = customerSettings;
            AddressService = addressService;
            CountryService = countryService;
            CustomerService = customerService;
            EventPublisher = eventPublisher;
            GenericAttributeService = genericAttributeService;
            GeoLookupService = geoLookupService;
            Logger = logger;
            StateProvinceService = stateProvinceService;
            StoreContext = storeContext;
            TaxPluginManager = taxPluginManager;
            WebHelper = webHelper;
            WorkContext = workContext;
            ShippingSettings = shippingSettings;
            TaxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get a value indicating whether a customer is consumer (a person, not a company) located in Europe Union
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        protected virtual async Task<bool> IsEuConsumerAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            Country country = null;

            //get country from billing address
            if (AddressSettings.CountryEnabled && await CustomerService.GetCustomerShippingAddressAsync(customer) is Address billingAddress)
                country = await CountryService.GetCountryByAddressAsync(billingAddress);

            //get country specified during registration?
            if (country == null && CustomerSettings.CountryEnabled)
            {
                var countryId = await GenericAttributeService.GetAttributeAsync<Customer, int>(customer.Id, NopCustomerDefaults.CountryIdAttribute);
                country = await CountryService.GetCountryByIdAsync(countryId);
            }

            //get country by IP address
            if (country == null)
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var countryIsoCode = GeoLookupService.LookupCountryIsoCode(ipAddress);
                country = await CountryService.GetCountryByTwoLetterIsoCodeAsync(countryIsoCode);
            }

            //we cannot detect country
            if (country == null)
                return false;

            //outside EU
            if (!country.SubjectToVat)
                return false;

            //company (business) or consumer?
            var customerVatStatus = (VatNumberStatus)await GenericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.VatNumberStatusIdAttribute);
            if (customerVatStatus == VatNumberStatus.Valid)
                return false;

            //consumer
            return true;
        }

        /// <summary>
        /// Gets a default tax address
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the address
        /// </returns>
        protected virtual async Task<Address> LoadDefaultTaxAddressAsync()
        {
            var addressId = TaxSettings.DefaultTaxAddressId;

            return await AddressService.GetAddressByIdAsync(addressId);
        }

        /// <summary>
        /// Gets or sets a pickup point address for tax calculation
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the address
        /// </returns>
        protected virtual async Task<Address> LoadPickupPointTaxAddressAsync(PickupPoint pickupPoint)
        {
            if (pickupPoint == null)
                throw new ArgumentNullException(nameof(pickupPoint));

            var country = await CountryService.GetCountryByTwoLetterIsoCodeAsync(pickupPoint.CountryCode);
            var state = await StateProvinceService.GetStateProvinceByAbbreviationAsync(pickupPoint.StateAbbreviation, country?.Id);

            return new Address
            {
                CountryId = country?.Id ?? 0,
                StateProvinceId = state?.Id ?? 0,
                County = pickupPoint.County,
                City = pickupPoint.City,
                Address1 = pickupPoint.Address,
                ZipPostalCode = pickupPoint.ZipPostalCode
            };
        }

        /// <summary>
        /// Prepare request to get tax rate
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="customer">Customer</param>
        /// <param name="price">Price</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the package for tax calculation
        /// </returns>
        protected virtual async Task<TaxRateRequest> PrepareTaxRateRequestAsync(Product product, int taxCategoryId, Customer customer, decimal price)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var store = await StoreContext.GetCurrentStoreAsync();
            var taxRateRequest = new TaxRateRequest
            {
                Customer = customer,
                Product = product,
                Price = price,
                TaxCategoryId = taxCategoryId > 0 ? taxCategoryId : product?.TaxCategoryId ?? 0,
                CurrentStoreId = store.Id
            };

            var basedOn = TaxSettings.TaxBasedOn;

            //new EU VAT rules starting January 1st 2015
            //find more info at http://ec.europa.eu/taxation_customs/taxation/vat/how_vat_works/telecom/index_en.htm#new_rules
            var overriddenBasedOn =
                //EU VAT enabled?
                TaxSettings.EuVatEnabled &&
                //telecommunications, broadcasting and electronic services?
                product != null && product.IsTelecommunicationsOrBroadcastingOrElectronicServices &&
                //January 1st 2015 passed? Yes, not required anymore
                //DateTime.UtcNow > new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc) &&
                //Europe Union consumer?
                await IsEuConsumerAsync(customer);
            if (overriddenBasedOn)
            {
                //We must charge VAT in the EU country where the customer belongs (not where the business is based)
                basedOn = TaxBasedOn.BillingAddress;
            }

            //tax is based on pickup point address
            if (!overriddenBasedOn && TaxSettings.TaxBasedOnPickupPointAddress && ShippingSettings.AllowPickupInStore)
            {
                var pickupPoint = await GenericAttributeService.GetAttributeAsync<PickupPoint>(customer,
                    NopCustomerDefaults.SelectedPickupPointAttribute, store.Id);
                if (pickupPoint != null)
                {
                    taxRateRequest.Address = await LoadPickupPointTaxAddressAsync(pickupPoint);
                    return taxRateRequest;
                }
            }

            if (basedOn == TaxBasedOn.BillingAddress && customer.BillingAddressId == null ||
                basedOn == TaxBasedOn.ShippingAddress && customer.ShippingAddressId == null)
                basedOn = TaxBasedOn.DefaultAddress;

            switch (basedOn)
            {
                case TaxBasedOn.BillingAddress:
                    var billingAddress = await CustomerService.GetCustomerBillingAddressAsync(customer);
                    taxRateRequest.Address = billingAddress;
                    break;
                case TaxBasedOn.ShippingAddress:
                    var shippingAddress = await CustomerService.GetCustomerShippingAddressAsync(customer);
                    taxRateRequest.Address = shippingAddress;
                    break;
                case TaxBasedOn.DefaultAddress:
                default:
                    taxRateRequest.Address = await LoadDefaultTaxAddressAsync();
                    break;
            }

            return taxRateRequest;
        }

        /// <summary>
        /// Calculated price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="percent">Percent</param>
        /// <param name="increase">Increase</param>
        /// <returns>New price</returns>
        protected virtual decimal CalculatePrice(decimal price, decimal percent, bool increase)
        {
            if (percent == decimal.Zero)
                return price;

            decimal result;
            if (increase)
                result = price * (1 + percent / 100);
            else
                result = price - price / (100 + percent) * percent;

            return result;
        }

        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="customer">Customer</param>
        /// <param name="price">Price (taxable value)</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the calculated tax rate. A value indicating whether a request is taxable
        /// </returns>
        protected virtual async Task<(decimal taxRate, bool isTaxable)> GetTaxRateAsync(Product product, int taxCategoryId,
            Customer customer, decimal price)
        {
            var taxRate = decimal.Zero;

            //active tax provider
            var store = await StoreContext.GetCurrentStoreAsync();
            var activeTaxProvider = await TaxPluginManager.LoadPrimaryPluginAsync(customer, store.Id);
            if (activeTaxProvider == null)
                return (taxRate, true);

            //tax request
            var taxRateRequest = await PrepareTaxRateRequestAsync(product, taxCategoryId, customer, price);

            var isTaxable = !await IsTaxExemptAsync(product, taxRateRequest.Customer);

            //tax exempt

            //make EU VAT exempt validation (the European Union Value Added Tax)
            if (isTaxable &&
                TaxSettings.EuVatEnabled &&
                await IsVatExemptAsync(taxRateRequest.Address, taxRateRequest.Customer))
                //VAT is not chargeable
                isTaxable = false;

            //get tax rate
            var taxRateResult = await activeTaxProvider.GetTaxRateAsync(taxRateRequest);

            //tax rate is calculated, now consumers can adjust it
            await EventPublisher.PublishAsync(new TaxRateCalculatedEvent(taxRateResult));

            if (taxRateResult.Success)
            {
                //ensure that tax is equal or greater than zero
                if (taxRateResult.TaxRate < decimal.Zero)
                    taxRateResult.TaxRate = decimal.Zero;

                taxRate = taxRateResult.TaxRate;
            }
            else if (TaxSettings.LogErrors)
                foreach (var error in taxRateResult.Errors)
                    await Logger.ErrorAsync($"{activeTaxProvider.PluginDescriptor.FriendlyName} - {error}", null, customer);

            return (taxRate, isTaxable);
        }

        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="twoLetterIsoCode">Two letter ISO code of a country</param>
        /// <param name="vatNumber">VAT number</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vAT Number status. Name (if received). Address (if received)
        /// </returns>
        protected virtual async Task<(VatNumberStatus vatNumberStatus, string name, string address)> GetVatNumberStatusAsync(string twoLetterIsoCode, string vatNumber)
        {
            var name = string.Empty;
            var address = string.Empty;

            if (string.IsNullOrEmpty(twoLetterIsoCode))
                return (VatNumberStatus.Empty, name, address);

            if (string.IsNullOrEmpty(vatNumber))
                return (VatNumberStatus.Empty, name, address);

            if (TaxSettings.EuVatAssumeValid)
                return (VatNumberStatus.Valid, name, address);

            if (!TaxSettings.EuVatUseWebService)
                return (VatNumberStatus.Unknown, name, address);

            var rez = await DoVatCheckAsync(twoLetterIsoCode, vatNumber);

            return (rez.vatNumberStatus, rez.name, rez.address);
        }

        /// <summary>
        /// Performs a basic check of a VAT number for validity
        /// </summary>
        /// <param name="twoLetterIsoCode">Two letter ISO code of a country</param>
        /// <param name="vatNumber">VAT number</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vAT number status. Company name. Address. Exception
        /// </returns>
        protected virtual async Task<(VatNumberStatus vatNumberStatus, string name, string address, Exception exception)> DoVatCheckAsync(string twoLetterIsoCode, string vatNumber)
        {
            if (vatNumber == null)
                vatNumber = string.Empty;
            vatNumber = vatNumber.Trim().Replace(" ", string.Empty);

            if (twoLetterIsoCode == null)
                twoLetterIsoCode = string.Empty;
            if (!string.IsNullOrEmpty(twoLetterIsoCode))
                //The service returns INVALID_INPUT for country codes that are not uppercase.
                twoLetterIsoCode = twoLetterIsoCode.ToUpperInvariant();

            string name;
            string address;

            try
            {
                var s = new EuropaCheckVatService.checkVatPortTypeClient();
                var result = await s.checkVatAsync(new EuropaCheckVatService.checkVatRequest
                {
                    vatNumber = vatNumber,
                    countryCode = twoLetterIsoCode
                });

                var valid = result.valid;
                name = result.name;
                address = result.address;

                return (valid ? VatNumberStatus.Valid : VatNumberStatus.Invalid, name, address, null);
            }
            catch (Exception ex)
            {
                name = address = string.Empty;
                var exception = ex;

                return (VatNumberStatus.Unknown, name, address, exception);
            }
        }

        /// <summary>
        /// Gets a value indicating whether EU VAT exempt (the European Union Value Added Tax)
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        protected virtual async Task<bool> IsVatExemptAsync(Address address, Customer customer)
        {
            if (!TaxSettings.EuVatEnabled)
                return false;

            if (customer == null || address == null)
                return false;

            var country = await CountryService.GetCountryByIdAsync(address.CountryId ?? 0);
            if (country == null)
                return false;

            if (!country.SubjectToVat)
                // VAT not chargeable if shipping outside VAT zone
                return true;

            // VAT not chargeable if address, customer and config meet our VAT exemption requirements:
            // returns true if this customer is VAT exempt because they are shipping within the EU but outside our shop country, they have supplied a validated VAT number, and the shop is configured to allow VAT exemption
            var customerVatStatus = (VatNumberStatus)await GenericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.VatNumberStatusIdAttribute);

            return country.Id != TaxSettings.EuVatShopCountryId &&
                   customerVatStatus == VatNumberStatus.Valid &&
                   TaxSettings.EuVatAllowVatExemption;
        }

        #endregion

        #region Methods

        #region Product price

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="price">Price</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price. Tax rate
        /// </returns>
        public virtual async Task<(decimal price, decimal taxRate)> GetProductPriceAsync(Product product, decimal price)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();

            return await GetProductPriceAsync(product, price, customer);
        }

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="price">Price</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price. Tax rate
        /// </returns>
        public virtual async Task<(decimal price, decimal taxRate)> GetProductPriceAsync(Product product, decimal price,
            Customer customer)
        {
            var includingTax = await WorkContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;
            return await GetProductPriceAsync(product, price, includingTax, customer);
        }

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price. Tax rate
        /// </returns>
        public virtual async Task<(decimal price, decimal taxRate)> GetProductPriceAsync(Product product, decimal price,
            bool includingTax, Customer customer)
        {
            var priceIncludesTax = TaxSettings.PricesIncludeTax;
            var taxCategoryId = 0;
            return await GetProductPriceAsync(product, taxCategoryId, price, includingTax, customer, priceIncludesTax);
        }

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <param name="priceIncludesTax">A value indicating whether price already includes tax</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price. Tax rate
        /// </returns>
        public virtual async Task<(decimal price, decimal taxRate)> GetProductPriceAsync(Product product, int taxCategoryId,
            decimal price, bool includingTax, Customer customer,
            bool priceIncludesTax)
        {
            var taxRate = decimal.Zero;

            //no need to calculate tax rate if passed "price" is 0
            if (price == decimal.Zero) 
                return (price, taxRate);

            bool isTaxable;

            (taxRate, isTaxable) = await GetTaxRateAsync(product, taxCategoryId, customer, price);

            if (priceIncludesTax)
            {
                //"price" already includes tax
                if (includingTax)
                {
                    //we should calculate price WITH tax
                    if (!isTaxable)
                    {
                        //but our request is not taxable
                        //hence we should calculate price WITHOUT tax
                        price = CalculatePrice(price, taxRate, false);
                    }
                }
                else
                {
                    //we should calculate price WITHOUT tax
                    price = CalculatePrice(price, taxRate, false);
                }
            }
            else
            {
                //"price" doesn't include tax
                if (includingTax)
                {
                    //we should calculate price WITH tax
                    //do it only when price is taxable
                    if (isTaxable)
                    {
                        price = CalculatePrice(price, taxRate, true);
                    }
                }
            }

            if (!isTaxable)
            {
                //we return 0% tax rate in case a request is not taxable
                taxRate = decimal.Zero;
            }

            //allowed to support negative price adjustments
            //if (price < decimal.Zero)
            //    price = decimal.Zero;

            return (price, taxRate);
        }

        /// <summary>
        /// Gets a value indicating whether a product is tax exempt
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a value indicating whether a product is tax exempt
        /// </returns>
        public virtual async Task<bool> IsTaxExemptAsync(Product product, Customer customer)
        {
            if (customer != null)
            {
                if (customer.IsTaxExempt)
                    return true;

                if ((await CustomerService.GetCustomerRolesAsync(customer)).Any(cr => cr.TaxExempt))
                    return true;
            }

            if (product == null)
                return false;

            if (product.IsTaxExempt)
                return true;

            return false;
        }

        #endregion

        #region Shipping price

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price. Tax rate
        /// </returns>
        public virtual async Task<(decimal price, decimal taxRate)> GetShippingPriceAsync(decimal price, Customer customer)
        {
            var includingTax = await WorkContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;

            return await GetShippingPriceAsync(price, includingTax, customer);
        }

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price. Tax rate
        /// </returns>
        public virtual async Task<(decimal price, decimal taxRate)> GetShippingPriceAsync(decimal price, bool includingTax, Customer customer)
        {
            var taxRate = decimal.Zero;

            if (!TaxSettings.ShippingIsTaxable)
            {
                return (price, taxRate);
            }

            var taxClassId = TaxSettings.ShippingTaxClassId;
            var priceIncludesTax = TaxSettings.ShippingPriceIncludesTax;

            return await GetProductPriceAsync(null, taxClassId, price, includingTax, customer, priceIncludesTax);
        }

        #endregion

        #region Payment additional fee

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price. Tax rate
        /// </returns>
        public virtual async Task<(decimal price, decimal taxRate)> GetPaymentMethodAdditionalFeeAsync(decimal price, Customer customer)
        {
            var includingTax = await WorkContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;
            
            return await GetPaymentMethodAdditionalFeeAsync(price, includingTax, customer);
        }

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price. Tax rate
        /// </returns>
        public virtual async Task<(decimal price, decimal taxRate)> GetPaymentMethodAdditionalFeeAsync(decimal price, bool includingTax, Customer customer)
        {
            var taxRate = decimal.Zero;

            if (!TaxSettings.PaymentMethodAdditionalFeeIsTaxable)
            {
                return (price, taxRate);
            }

            var taxClassId = TaxSettings.PaymentMethodAdditionalFeeTaxClassId;
            var priceIncludesTax = TaxSettings.PaymentMethodAdditionalFeeIncludesTax;
            return await GetProductPriceAsync(null, taxClassId, price, includingTax, customer, priceIncludesTax);
        }

        #endregion

        #region Checkout attribute price

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="ca">Checkout attribute</param>
        /// <param name="cav">Checkout attribute value</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price. Tax rate
        /// </returns>
        public virtual async Task<(decimal price, decimal taxRate)> GetCheckoutAttributePriceAsync(CheckoutAttribute ca, CheckoutAttributeValue cav)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();

            return await GetCheckoutAttributePriceAsync(ca, cav, customer);
        }

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="ca">Checkout attribute</param>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price. Tax rate
        /// </returns>
        public virtual async Task<(decimal price, decimal taxRate)> GetCheckoutAttributePriceAsync(CheckoutAttribute ca, CheckoutAttributeValue cav, Customer customer)
        {
            var includingTax = await WorkContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax;

            return await GetCheckoutAttributePriceAsync(ca, cav, includingTax, customer);
        }

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="ca">Checkout attribute</param>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price. Tax rate
        /// </returns>
        public virtual async Task<(decimal price, decimal taxRate)> GetCheckoutAttributePriceAsync(CheckoutAttribute ca, CheckoutAttributeValue cav,
            bool includingTax, Customer customer)
        {
            if (cav == null)
                throw new ArgumentNullException(nameof(cav));

            var taxRate = decimal.Zero;

            var price = cav.PriceAdjustment;
            if (ca.IsTaxExempt) 
                return (price, taxRate);

            var priceIncludesTax = TaxSettings.PricesIncludeTax;
            var taxClassId = ca.TaxCategoryId;

            return await GetProductPriceAsync(null, taxClassId, price, includingTax, customer, priceIncludesTax);
        }

        #endregion

        #region VAT
        
        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="fullVatNumber">Two letter ISO code of a country and VAT number (e.g. GB 111 1111 111)</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the vAT Number status. Name (if received). Address (if received)
        /// </returns>
        public virtual async Task<(VatNumberStatus vatNumberStatus, string name, string address)> GetVatNumberStatusAsync(string fullVatNumber)
        {
            var name = string.Empty;
            var address = string.Empty;

            if (string.IsNullOrWhiteSpace(fullVatNumber))
                return (VatNumberStatus.Empty, name, address);
            fullVatNumber = fullVatNumber.Trim();

            //GB 111 1111 111 or GB 1111111111
            //more advanced regex - http://codeigniter.com/wiki/European_Vat_Checker
            var r = new Regex(@"^(\w{2})(.*)");
            var match = r.Match(fullVatNumber);
            if (!match.Success)
                return (VatNumberStatus.Invalid, name, address); 

            var twoLetterIsoCode = match.Groups[1].Value;
            var vatNumber = match.Groups[2].Value;

            return await GetVatNumberStatusAsync(twoLetterIsoCode, vatNumber);
        }

        #endregion

        #region Tax total

        /// <summary>
        /// Get tax total for the passed shopping cart
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<TaxTotalResult> GetTaxTotalAsync(IList<ShoppingCartItem> cart, bool usePaymentMethodAdditionalFee = true)
        {
            var customer = await CustomerService.GetShoppingCartCustomerAsync(cart);
            var store = await StoreContext.GetCurrentStoreAsync();
            var activeTaxProvider = await TaxPluginManager.LoadPrimaryPluginAsync(customer, store.Id);
            if (activeTaxProvider == null)
                return null;

            //get result by using primary tax provider
            var taxTotalRequest = new TaxTotalRequest
            {
                ShoppingCart = cart,
                Customer = customer,
                StoreId = store.Id,
                UsePaymentMethodAdditionalFee = usePaymentMethodAdditionalFee
            };
            var taxTotalResult = await activeTaxProvider.GetTaxTotalAsync(taxTotalRequest);

            //tax total is calculated, now consumers can adjust it
            await EventPublisher.PublishAsync(new TaxTotalCalculatedEvent(taxTotalRequest, taxTotalResult));

            //error logging
            if (taxTotalResult != null && !taxTotalResult.Success && TaxSettings.LogErrors)
            {
                foreach (var error in taxTotalResult.Errors)
                {
                    await Logger.ErrorAsync($"{activeTaxProvider.PluginDescriptor.FriendlyName} - {error}", null, customer);
                }
            }

            return taxTotalResult;
        }

        #endregion

        #endregion
    }
}