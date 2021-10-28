using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Authentication.External;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Models.Common;
using Nop.Web.Models.Customer;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the customer model factory
    /// </summary>
    public partial class CustomerModelFactory : ICustomerModelFactory
    {
        #region Fields

        protected AddressSettings AddressSettings { get; }
        protected CaptchaSettings CaptchaSettings { get; }
        protected CatalogSettings CatalogSettings { get; }
        protected CommonSettings CommonSettings { get; }
        protected CustomerSettings CustomerSettings { get; }
        protected DateTimeSettings DateTimeSettings { get; }
        protected ExternalAuthenticationSettings ExternalAuthenticationSettings { get; }
        protected ForumSettings ForumSettings { get; }
        protected GdprSettings GdprSettings { get; }
        protected IAddressModelFactory AddressModelFactory { get; }
        protected IAuthenticationPluginManager AuthenticationPluginManager { get; }
        protected ICountryService CountryService { get; }
        protected ICustomerAttributeParser CustomerAttributeParser { get; }
        protected ICustomerAttributeService CustomerAttributeService { get; }
        protected ICustomerService CustomerService { get; }
        protected IDateTimeHelper DateTimeHelper { get; }
        protected IExternalAuthenticationService ExternalAuthenticationService { get; }
        protected IGdprService GdprService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IMultiFactorAuthenticationPluginManager MultiFactorAuthenticationPluginManager { get; }
        protected INewsLetterSubscriptionService NewsLetterSubscriptionService { get; }
        protected IOrderService OrderService { get; }
        protected IPictureService PictureService { get; }
        protected IProductService ProductService { get; }
        protected IReturnRequestService ReturnRequestService { get; }
        protected IStateProvinceService StateProvinceService { get; }
        protected IStoreContext StoreContext { get; }
        protected IStoreMappingService StoreMappingService { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IWorkContext WorkContext { get; }
        protected MediaSettings MediaSettings { get; }
        protected OrderSettings OrderSettings { get; }
        protected RewardPointsSettings RewardPointsSettings { get; }
        protected SecuritySettings SecuritySettings { get; }
        protected TaxSettings TaxSettings { get; }
        protected VendorSettings VendorSettings { get; }

        #endregion

        #region Ctor

        public CustomerModelFactory(AddressSettings addressSettings,
            CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            CustomerSettings customerSettings,
            DateTimeSettings dateTimeSettings,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            ForumSettings forumSettings,
            GdprSettings gdprSettings,
            IAddressModelFactory addressModelFactory,
            IAuthenticationPluginManager authenticationPluginManager,
            ICountryService countryService,
            ICustomerAttributeParser customerAttributeParser,
            ICustomerAttributeService customerAttributeService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IExternalAuthenticationService externalAuthenticationService,
            IGdprService gdprService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IOrderService orderService,
            IPictureService pictureService,
            IProductService productService,
            IReturnRequestService returnRequestService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            RewardPointsSettings rewardPointsSettings,
            SecuritySettings securitySettings,
            TaxSettings taxSettings,
            VendorSettings vendorSettings)
        {
            AddressSettings = addressSettings;
            CaptchaSettings = captchaSettings;
            CatalogSettings = catalogSettings;
            CommonSettings = commonSettings;
            CustomerSettings = customerSettings;
            DateTimeSettings = dateTimeSettings;
            ExternalAuthenticationService = externalAuthenticationService;
            ExternalAuthenticationSettings = externalAuthenticationSettings;
            ForumSettings = forumSettings;
            GdprSettings = gdprSettings;
            AddressModelFactory = addressModelFactory;
            AuthenticationPluginManager = authenticationPluginManager;
            CountryService = countryService;
            CustomerAttributeParser = customerAttributeParser;
            CustomerAttributeService = customerAttributeService;
            CustomerService = customerService;
            DateTimeHelper = dateTimeHelper;
            GdprService = gdprService;
            GenericAttributeService = genericAttributeService;
            LocalizationService = localizationService;
            MultiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
            NewsLetterSubscriptionService = newsLetterSubscriptionService;
            OrderService = orderService;
            PictureService = pictureService;
            ProductService = productService;
            ReturnRequestService = returnRequestService;
            StateProvinceService = stateProvinceService;
            StoreContext = storeContext;
            StoreMappingService = storeMappingService;
            UrlRecordService = urlRecordService;
            WorkContext = workContext;
            MediaSettings = mediaSettings;
            OrderSettings = orderSettings;
            RewardPointsSettings = rewardPointsSettings;
            SecuritySettings = securitySettings;
            TaxSettings = taxSettings;
            VendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<GdprConsentModel> PrepareGdprConsentModelAsync(GdprConsent consent, bool accepted)
        {
            if (consent == null)
                throw new ArgumentNullException(nameof(consent));

            var requiredMessage = await LocalizationService.GetLocalizedAsync(consent, x => x.RequiredMessage);
            return new GdprConsentModel
            {
                Id = consent.Id,
                Message = await LocalizationService.GetLocalizedAsync(consent, x => x.Message),
                IsRequired = consent.IsRequired,
                RequiredMessage = !string.IsNullOrEmpty(requiredMessage) ? requiredMessage : $"'{consent.Message}' is required",
                Accepted = accepted
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the customer info model
        /// </summary>
        /// <param name="model">Customer info model</param>
        /// <param name="customer">Customer</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="overrideCustomCustomerAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer info model
        /// </returns>
        public virtual async Task<CustomerInfoModel> PrepareCustomerInfoModelAsync(CustomerInfoModel model, Customer customer,
            bool excludeProperties, string overrideCustomCustomerAttributesXml = "")
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            model.AllowCustomersToSetTimeZone = DateTimeSettings.AllowCustomersToSetTimeZone;
            foreach (var tzi in DateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == (await DateTimeHelper.GetCurrentTimeZoneAsync()).Id) });

            var store = await StoreContext.GetCurrentStoreAsync();
            if (!excludeProperties)
            {
                model.VatNumber = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.VatNumberAttribute);
                model.FirstName = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.FirstNameAttribute);
                model.LastName = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.LastNameAttribute);
                model.Gender = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.GenderAttribute);
                var dateOfBirth = await GenericAttributeService.GetAttributeAsync<DateTime?>(customer, NopCustomerDefaults.DateOfBirthAttribute);
                if (dateOfBirth.HasValue)
                {
                    var currentCalendar = CultureInfo.CurrentCulture.Calendar;

                    model.DateOfBirthDay = currentCalendar.GetDayOfMonth(dateOfBirth.Value);
                    model.DateOfBirthMonth = currentCalendar.GetMonth(dateOfBirth.Value);
                    model.DateOfBirthYear = currentCalendar.GetYear(dateOfBirth.Value);
                }
                model.Company = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CompanyAttribute);
                model.StreetAddress = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.StreetAddressAttribute);
                model.StreetAddress2 = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.StreetAddress2Attribute);
                model.ZipPostalCode = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.ZipPostalCodeAttribute);
                model.City = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CityAttribute);
                model.County = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CountyAttribute);
                model.CountryId = await GenericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.CountryIdAttribute);
                model.StateProvinceId = await GenericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.StateProvinceIdAttribute);
                model.Phone = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.PhoneAttribute);
                model.Fax = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.FaxAttribute);

                //newsletter
                var newsletter = await NewsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(customer.Email, store.Id);
                model.Newsletter = newsletter != null && newsletter.Active;

                model.Signature = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SignatureAttribute);

                model.Email = customer.Email;
                model.Username = customer.Username;
            }
            else
            {
                if (CustomerSettings.UsernamesEnabled && !CustomerSettings.AllowUsersToChangeUsernames)
                    model.Username = customer.Username;
            }

            if (CustomerSettings.UserRegistrationType == UserRegistrationType.EmailValidation)
                model.EmailToRevalidate = customer.EmailToRevalidate;

            var currentLanguage = await WorkContext.GetWorkingLanguageAsync();
            //countries and states
            if (CustomerSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem { Text = await LocalizationService.GetResourceAsync("Address.SelectCountry"), Value = "0" });
                foreach (var c in await CountryService.GetAllCountriesAsync(currentLanguage.Id))
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = await LocalizationService.GetLocalizedAsync(c, x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (CustomerSettings.StateProvinceEnabled)
                {
                    //states
                    var states = (await StateProvinceService.GetStateProvincesByCountryIdAsync(model.CountryId, currentLanguage.Id)).ToList();
                    if (states.Any())
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = await LocalizationService.GetResourceAsync("Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem { Text = await LocalizationService.GetLocalizedAsync(s, x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                        }
                    }
                    else
                    {
                        var anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);

                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = await LocalizationService.GetResourceAsync(anyCountrySelected ? "Address.Other" : "Address.SelectState"),
                            Value = "0"
                        });
                    }

                }
            }

            model.DisplayVatNumber = TaxSettings.EuVatEnabled;
            model.VatNumberStatusNote = await LocalizationService.GetLocalizedEnumAsync((VatNumberStatus)await GenericAttributeService
                .GetAttributeAsync<int>(customer, NopCustomerDefaults.VatNumberStatusIdAttribute));
            model.FirstNameEnabled = CustomerSettings.FirstNameEnabled;
            model.LastNameEnabled = CustomerSettings.LastNameEnabled;
            model.FirstNameRequired = CustomerSettings.FirstNameRequired;
            model.LastNameRequired = CustomerSettings.LastNameRequired;
            model.GenderEnabled = CustomerSettings.GenderEnabled;
            model.DateOfBirthEnabled = CustomerSettings.DateOfBirthEnabled;
            model.DateOfBirthRequired = CustomerSettings.DateOfBirthRequired;
            model.CompanyEnabled = CustomerSettings.CompanyEnabled;
            model.CompanyRequired = CustomerSettings.CompanyRequired;
            model.StreetAddressEnabled = CustomerSettings.StreetAddressEnabled;
            model.StreetAddressRequired = CustomerSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = CustomerSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = CustomerSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = CustomerSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = CustomerSettings.ZipPostalCodeRequired;
            model.CityEnabled = CustomerSettings.CityEnabled;
            model.CityRequired = CustomerSettings.CityRequired;
            model.CountyEnabled = CustomerSettings.CountyEnabled;
            model.CountyRequired = CustomerSettings.CountyRequired;
            model.CountryEnabled = CustomerSettings.CountryEnabled;
            model.CountryRequired = CustomerSettings.CountryRequired;
            model.StateProvinceEnabled = CustomerSettings.StateProvinceEnabled;
            model.StateProvinceRequired = CustomerSettings.StateProvinceRequired;
            model.PhoneEnabled = CustomerSettings.PhoneEnabled;
            model.PhoneRequired = CustomerSettings.PhoneRequired;
            model.FaxEnabled = CustomerSettings.FaxEnabled;
            model.FaxRequired = CustomerSettings.FaxRequired;
            model.NewsletterEnabled = CustomerSettings.NewsletterEnabled;
            model.UsernamesEnabled = CustomerSettings.UsernamesEnabled;
            model.AllowUsersToChangeUsernames = CustomerSettings.AllowUsersToChangeUsernames;
            model.CheckUsernameAvailabilityEnabled = CustomerSettings.CheckUsernameAvailabilityEnabled;
            model.SignatureEnabled = ForumSettings.ForumsEnabled && ForumSettings.SignaturesEnabled;

            //external authentication
            var currentCustomer = await WorkContext.GetCurrentCustomerAsync();
            model.AllowCustomersToRemoveAssociations = ExternalAuthenticationSettings.AllowCustomersToRemoveAssociations;
            model.NumberOfExternalAuthenticationProviders = (await AuthenticationPluginManager
                .LoadActivePluginsAsync(currentCustomer, store.Id))
                .Count;
            foreach (var record in await ExternalAuthenticationService.GetCustomerExternalAuthenticationRecordsAsync(customer))
            {
                var authMethod = await AuthenticationPluginManager
                    .LoadPluginBySystemNameAsync(record.ProviderSystemName, currentCustomer, store.Id);
                if (!AuthenticationPluginManager.IsPluginActive(authMethod))
                    continue;

                model.AssociatedExternalAuthRecords.Add(new CustomerInfoModel.AssociatedExternalAuthModel
                {
                    Id = record.Id,
                    Email = record.Email,
                    ExternalIdentifier = !string.IsNullOrEmpty(record.ExternalDisplayIdentifier)
                        ? record.ExternalDisplayIdentifier : record.ExternalIdentifier,
                    AuthMethodName = await LocalizationService.GetLocalizedFriendlyNameAsync(authMethod, currentLanguage.Id)
                });
            }

            //custom customer attributes
            var customAttributes = await PrepareCustomCustomerAttributesAsync(customer, overrideCustomCustomerAttributesXml);
            foreach (var attribute in customAttributes)
                model.CustomerAttributes.Add(attribute);

            //GDPR
            if (GdprSettings.GdprEnabled)
            {
                var consents = (await GdprService.GetAllConsentsAsync()).Where(consent => consent.DisplayOnCustomerInfoPage).ToList();
                foreach (var consent in consents)
                {
                    var accepted = await GdprService.IsConsentAcceptedAsync(consent.Id, currentCustomer.Id);
                    model.GdprConsents.Add(await PrepareGdprConsentModelAsync(consent, accepted.HasValue && accepted.Value));
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare the customer register model
        /// </summary>
        /// <param name="model">Customer register model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="overrideCustomCustomerAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
        /// <param name="setDefaultValues">Whether to populate model properties by default values</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer register model
        /// </returns>
        public virtual async Task<RegisterModel> PrepareRegisterModelAsync(RegisterModel model, bool excludeProperties,
            string overrideCustomCustomerAttributesXml = "", bool setDefaultValues = false)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.AllowCustomersToSetTimeZone = DateTimeSettings.AllowCustomersToSetTimeZone;
            foreach (var tzi in DateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == (await DateTimeHelper.GetCurrentTimeZoneAsync()).Id) });

            model.DisplayVatNumber = TaxSettings.EuVatEnabled;
            //form fields
            model.FirstNameEnabled = CustomerSettings.FirstNameEnabled;
            model.LastNameEnabled = CustomerSettings.LastNameEnabled;
            model.FirstNameRequired = CustomerSettings.FirstNameRequired;
            model.LastNameRequired = CustomerSettings.LastNameRequired;
            model.GenderEnabled = CustomerSettings.GenderEnabled;
            model.DateOfBirthEnabled = CustomerSettings.DateOfBirthEnabled;
            model.DateOfBirthRequired = CustomerSettings.DateOfBirthRequired;
            model.CompanyEnabled = CustomerSettings.CompanyEnabled;
            model.CompanyRequired = CustomerSettings.CompanyRequired;
            model.StreetAddressEnabled = CustomerSettings.StreetAddressEnabled;
            model.StreetAddressRequired = CustomerSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = CustomerSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = CustomerSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = CustomerSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = CustomerSettings.ZipPostalCodeRequired;
            model.CityEnabled = CustomerSettings.CityEnabled;
            model.CityRequired = CustomerSettings.CityRequired;
            model.CountyEnabled = CustomerSettings.CountyEnabled;
            model.CountyRequired = CustomerSettings.CountyRequired;
            model.CountryEnabled = CustomerSettings.CountryEnabled;
            model.CountryRequired = CustomerSettings.CountryRequired;
            model.StateProvinceEnabled = CustomerSettings.StateProvinceEnabled;
            model.StateProvinceRequired = CustomerSettings.StateProvinceRequired;
            model.PhoneEnabled = CustomerSettings.PhoneEnabled;
            model.PhoneRequired = CustomerSettings.PhoneRequired;
            model.FaxEnabled = CustomerSettings.FaxEnabled;
            model.FaxRequired = CustomerSettings.FaxRequired;
            model.NewsletterEnabled = CustomerSettings.NewsletterEnabled;
            model.AcceptPrivacyPolicyEnabled = CustomerSettings.AcceptPrivacyPolicyEnabled;
            model.AcceptPrivacyPolicyPopup = CommonSettings.PopupForTermsOfServiceLinks;
            model.UsernamesEnabled = CustomerSettings.UsernamesEnabled;
            model.CheckUsernameAvailabilityEnabled = CustomerSettings.CheckUsernameAvailabilityEnabled;
            model.HoneypotEnabled = SecuritySettings.HoneypotEnabled;
            model.DisplayCaptcha = CaptchaSettings.Enabled && CaptchaSettings.ShowOnRegistrationPage;
            model.EnteringEmailTwice = CustomerSettings.EnteringEmailTwice;
            if (setDefaultValues)
            {
                //enable newsletter by default
                model.Newsletter = CustomerSettings.NewsletterTickedByDefault;
            }

            //countries and states
            if (CustomerSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem { Text = await LocalizationService.GetResourceAsync("Address.SelectCountry"), Value = "0" });
                var currentLanguage = await WorkContext.GetWorkingLanguageAsync();
                foreach (var c in await CountryService.GetAllCountriesAsync(currentLanguage.Id))
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = await LocalizationService.GetLocalizedAsync(c, x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (CustomerSettings.StateProvinceEnabled)
                {
                    //states
                    var states = (await StateProvinceService.GetStateProvincesByCountryIdAsync(model.CountryId, currentLanguage.Id)).ToList();
                    if (states.Any())
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = await LocalizationService.GetResourceAsync("Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem { Text = await LocalizationService.GetLocalizedAsync(s, x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                        }
                    }
                    else
                    {
                        var anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);

                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = await LocalizationService.GetResourceAsync(anyCountrySelected ? "Address.Other" : "Address.SelectState"),
                            Value = "0"
                        });
                    }

                }
            }

            //custom customer attributes
            var customAttributes = await PrepareCustomCustomerAttributesAsync(await WorkContext.GetCurrentCustomerAsync(), overrideCustomCustomerAttributesXml);
            foreach (var attribute in customAttributes)
                model.CustomerAttributes.Add(attribute);

            //GDPR
            if (GdprSettings.GdprEnabled)
            {
                var consents = (await GdprService.GetAllConsentsAsync()).Where(consent => consent.DisplayDuringRegistration).ToList();
                foreach (var consent in consents)
                {
                    model.GdprConsents.Add(await PrepareGdprConsentModelAsync(consent, false));
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare the login model
        /// </summary>
        /// <param name="checkoutAsGuest">Whether to checkout as guest is enabled</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the login model
        /// </returns>
        public virtual Task<LoginModel> PrepareLoginModelAsync(bool? checkoutAsGuest)
        {
            var model = new LoginModel
            {
                UsernamesEnabled = CustomerSettings.UsernamesEnabled,
                RegistrationType = CustomerSettings.UserRegistrationType,
                CheckoutAsGuest = checkoutAsGuest.GetValueOrDefault(),
                DisplayCaptcha = CaptchaSettings.Enabled && CaptchaSettings.ShowOnLoginPage
            };

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the password recovery model
        /// </summary>
        /// <param name="model">Password recovery model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the password recovery model
        /// </returns>
        public virtual Task<PasswordRecoveryModel> PreparePasswordRecoveryModelAsync(PasswordRecoveryModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.DisplayCaptcha = CaptchaSettings.Enabled && CaptchaSettings.ShowOnForgotPasswordPage;

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the register result model
        /// </summary>
        /// <param name="resultId">Value of UserRegistrationType enum</param>
        /// <param name="returnUrl">URL to redirect</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the register result model
        /// </returns>
        public virtual async Task<RegisterResultModel> PrepareRegisterResultModelAsync(int resultId, string returnUrl)
        {
            var resultText = (UserRegistrationType)resultId switch
            {
                UserRegistrationType.Disabled => await LocalizationService.GetResourceAsync("Account.Register.Result.Disabled"),
                UserRegistrationType.Standard => await LocalizationService.GetResourceAsync("Account.Register.Result.Standard"),
                UserRegistrationType.AdminApproval => await LocalizationService.GetResourceAsync("Account.Register.Result.AdminApproval"),
                UserRegistrationType.EmailValidation => await LocalizationService.GetResourceAsync("Account.Register.Result.EmailValidation"),
                _ => null
            };

            var model = new RegisterResultModel
            {
                Result = resultText,
                ReturnUrl = returnUrl
            };

            return model;
        }

        /// <summary>
        /// Prepare the customer navigation model
        /// </summary>
        /// <param name="selectedTabId">Identifier of the selected tab</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer navigation model
        /// </returns>
        public virtual async Task<CustomerNavigationModel> PrepareCustomerNavigationModelAsync(int selectedTabId = 0)
        {
            var model = new CustomerNavigationModel();

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerInfo",
                Title = await LocalizationService.GetResourceAsync("Account.CustomerInfo"),
                Tab = CustomerNavigationEnum.Info,
                ItemClass = "customer-info"
            });

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerAddresses",
                Title = await LocalizationService.GetResourceAsync("Account.CustomerAddresses"),
                Tab = CustomerNavigationEnum.Addresses,
                ItemClass = "customer-addresses"
            });

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerOrders",
                Title = await LocalizationService.GetResourceAsync("Account.CustomerOrders"),
                Tab = CustomerNavigationEnum.Orders,
                ItemClass = "customer-orders"
            });

            var store = await StoreContext.GetCurrentStoreAsync();
            var customer = await WorkContext.GetCurrentCustomerAsync();

            if (OrderSettings.ReturnRequestsEnabled &&
                (await ReturnRequestService.SearchReturnRequestsAsync(store.Id,
                    customer.Id, pageIndex: 0, pageSize: 1)).Any())
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerReturnRequests",
                    Title = await LocalizationService.GetResourceAsync("Account.CustomerReturnRequests"),
                    Tab = CustomerNavigationEnum.ReturnRequests,
                    ItemClass = "return-requests"
                });
            }

            if (!CustomerSettings.HideDownloadableProductsTab)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerDownloadableProducts",
                    Title = await LocalizationService.GetResourceAsync("Account.DownloadableProducts"),
                    Tab = CustomerNavigationEnum.DownloadableProducts,
                    ItemClass = "downloadable-products"
                });
            }

            if (!CustomerSettings.HideBackInStockSubscriptionsTab)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerBackInStockSubscriptions",
                    Title = await LocalizationService.GetResourceAsync("Account.BackInStockSubscriptions"),
                    Tab = CustomerNavigationEnum.BackInStockSubscriptions,
                    ItemClass = "back-in-stock-subscriptions"
                });
            }

            if (RewardPointsSettings.Enabled)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerRewardPoints",
                    Title = await LocalizationService.GetResourceAsync("Account.RewardPoints"),
                    Tab = CustomerNavigationEnum.RewardPoints,
                    ItemClass = "reward-points"
                });
            }

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerChangePassword",
                Title = await LocalizationService.GetResourceAsync("Account.ChangePassword"),
                Tab = CustomerNavigationEnum.ChangePassword,
                ItemClass = "change-password"
            });

            if (CustomerSettings.AllowCustomersToUploadAvatars)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerAvatar",
                    Title = await LocalizationService.GetResourceAsync("Account.Avatar"),
                    Tab = CustomerNavigationEnum.Avatar,
                    ItemClass = "customer-avatar"
                });
            }

            if (ForumSettings.ForumsEnabled && ForumSettings.AllowCustomersToManageSubscriptions)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerForumSubscriptions",
                    Title = await LocalizationService.GetResourceAsync("Account.ForumSubscriptions"),
                    Tab = CustomerNavigationEnum.ForumSubscriptions,
                    ItemClass = "forum-subscriptions"
                });
            }
            if (CatalogSettings.ShowProductReviewsTabOnAccountPage)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerProductReviews",
                    Title = await LocalizationService.GetResourceAsync("Account.CustomerProductReviews"),
                    Tab = CustomerNavigationEnum.ProductReviews,
                    ItemClass = "customer-reviews"
                });
            }
            if (VendorSettings.AllowVendorsToEditInfo && await WorkContext.GetCurrentVendorAsync() != null)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerVendorInfo",
                    Title = await LocalizationService.GetResourceAsync("Account.VendorInfo"),
                    Tab = CustomerNavigationEnum.VendorInfo,
                    ItemClass = "customer-vendor-info"
                });
            }
            if (GdprSettings.GdprEnabled)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "GdprTools",
                    Title = await LocalizationService.GetResourceAsync("Account.Gdpr"),
                    Tab = CustomerNavigationEnum.GdprTools,
                    ItemClass = "customer-gdpr"
                });
            }

            if (CaptchaSettings.Enabled && CustomerSettings.AllowCustomersToCheckGiftCardBalance)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CheckGiftCardBalance",
                    Title = await LocalizationService.GetResourceAsync("CheckGiftCardBalance"),
                    Tab = CustomerNavigationEnum.CheckGiftCardBalance,
                    ItemClass = "customer-check-gift-card-balance"
                });
            }

            if (await MultiFactorAuthenticationPluginManager.HasActivePluginsAsync())
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "MultiFactorAuthenticationSettings",
                    Title = await LocalizationService.GetResourceAsync("PageTitle.MultiFactorAuthentication"),
                    Tab = CustomerNavigationEnum.MultiFactorAuthentication,
                    ItemClass = "customer-multiFactor-authentication"
                });
            }

            model.SelectedTab = (CustomerNavigationEnum)selectedTabId;

            return model;
        }

        /// <summary>
        /// Prepare the customer address list model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer address list model
        /// </returns>
        public virtual async Task<CustomerAddressListModel> PrepareCustomerAddressListModelAsync()
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();

            var addresses = await (await CustomerService.GetAddressesByCustomerIdAsync(customer.Id))
                //enabled for the current store
                .WhereAwait(async a => a.CountryId == null || await StoreMappingService.AuthorizeAsync(await CountryService.GetCountryByAddressAsync(a)))
                .ToListAsync();

            var model = new CustomerAddressListModel();
            foreach (var address in addresses)
            {
                var addressModel = new AddressModel();
                await AddressModelFactory.PrepareAddressModelAsync(addressModel,
                    address: address,
                    excludeProperties: false,
                    addressSettings: AddressSettings,
                    loadCountries: async () => await CountryService.GetAllCountriesAsync((await WorkContext.GetWorkingLanguageAsync()).Id));
                model.Addresses.Add(addressModel);
            }
            return model;
        }

        /// <summary>
        /// Prepare the customer downloadable products model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer downloadable products model
        /// </returns>
        public virtual async Task<CustomerDownloadableProductsModel> PrepareCustomerDownloadableProductsModelAsync()
        {
            var model = new CustomerDownloadableProductsModel();
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var items = await OrderService.GetDownloadableOrderItemsAsync(customer.Id);
            foreach (var item in items)
            {
                var order = await OrderService.GetOrderByIdAsync(item.OrderId);
                var product = await ProductService.GetProductByIdAsync(item.ProductId);

                var itemModel = new CustomerDownloadableProductsModel.DownloadableProductsModel
                {
                    OrderItemGuid = item.OrderItemGuid,
                    OrderId = order.Id,
                    CustomOrderNumber = order.CustomOrderNumber,
                    CreatedOn = await DateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc),
                    ProductName = await LocalizationService.GetLocalizedAsync(product, x => x.Name),
                    ProductSeName = await UrlRecordService.GetSeNameAsync(product),
                    ProductAttributes = item.AttributeDescription,
                    ProductId = item.ProductId
                };
                model.Items.Add(itemModel);

                if (await OrderService.IsDownloadAllowedAsync(item))
                    itemModel.DownloadId = product.DownloadId;

                if (await OrderService.IsLicenseDownloadAllowedAsync(item))
                    itemModel.LicenseId = item.LicenseDownloadId ?? 0;
            }

            return model;
        }

        /// <summary>
        /// Prepare the user agreement model
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user agreement model
        /// </returns>
        public virtual Task<UserAgreementModel> PrepareUserAgreementModelAsync(OrderItem orderItem, Product product)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var model = new UserAgreementModel
            {
                UserAgreementText = product.UserAgreementText,
                OrderItemGuid = orderItem.OrderItemGuid
            };

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the change password model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the change password model
        /// </returns>
        public virtual Task<ChangePasswordModel> PrepareChangePasswordModelAsync()
        {
            var model = new ChangePasswordModel();

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the customer avatar model
        /// </summary>
        /// <param name="model">Customer avatar model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer avatar model
        /// </returns>
        public virtual async Task<CustomerAvatarModel> PrepareCustomerAvatarModelAsync(CustomerAvatarModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.AvatarUrl = await PictureService.GetPictureUrlAsync(
                await GenericAttributeService.GetAttributeAsync<int>(await WorkContext.GetCurrentCustomerAsync(), NopCustomerDefaults.AvatarPictureIdAttribute),
                MediaSettings.AvatarPictureSize,
                false);

            return model;
        }

        /// <summary>
        /// Prepare the GDPR tools model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gDPR tools model
        /// </returns>
        public virtual Task<GdprToolsModel> PrepareGdprToolsModelAsync()
        {
            var model = new GdprToolsModel();

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the check gift card balance madel
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the check gift card balance madel
        /// </returns>
        public virtual Task<CheckGiftCardBalanceModel> PrepareCheckGiftCardBalanceModelAsync()
        {
            var model = new CheckGiftCardBalanceModel();

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the multi-factor authentication model
        /// </summary>
        /// <param name="model">Multi-factor authentication model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the multi-factor authentication model
        /// </returns>
        public virtual async Task<MultiFactorAuthenticationModel> PrepareMultiFactorAuthenticationModelAsync(MultiFactorAuthenticationModel model)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();

            model.IsEnabled = !string.IsNullOrEmpty(
                await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute));
            
            var store = await StoreContext.GetCurrentStoreAsync();
            var multiFactorAuthenticationProviders = (await MultiFactorAuthenticationPluginManager.LoadActivePluginsAsync(customer, store.Id)).ToList();
            foreach (var multiFactorAuthenticationProvider in multiFactorAuthenticationProviders)
            {
                var providerModel = new MultiFactorAuthenticationProviderModel();
                var sysName = multiFactorAuthenticationProvider.PluginDescriptor.SystemName;
                providerModel = await PrepareMultiFactorAuthenticationProviderModelAsync(providerModel, sysName);
                model.Providers.Add(providerModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare the multi-factor authentication provider model
        /// </summary>
        /// <param name="providerModel">Multi-factor authentication provider model</param>
        /// <param name="sysName">Multi-factor authentication provider system name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the multi-factor authentication model
        /// </returns>
        public virtual async Task<MultiFactorAuthenticationProviderModel> PrepareMultiFactorAuthenticationProviderModelAsync(MultiFactorAuthenticationProviderModel providerModel, string sysName, bool isLogin = false)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            var selectedProvider = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute);
            var store = await StoreContext.GetCurrentStoreAsync();

            var multiFactorAuthenticationProvider = (await MultiFactorAuthenticationPluginManager.LoadActivePluginsAsync(customer, store.Id))
                    .FirstOrDefault(provider => provider.PluginDescriptor.SystemName == sysName);

            if (multiFactorAuthenticationProvider != null)
            {
                providerModel.Name = await LocalizationService.GetLocalizedFriendlyNameAsync(multiFactorAuthenticationProvider, (await WorkContext.GetWorkingLanguageAsync()).Id);
                providerModel.SystemName = sysName;
                providerModel.Description = await multiFactorAuthenticationProvider.GetDescriptionAsync();
                providerModel.LogoUrl = await MultiFactorAuthenticationPluginManager.GetPluginLogoUrlAsync(multiFactorAuthenticationProvider);
                providerModel.ViewComponentName = isLogin ? multiFactorAuthenticationProvider.GetVerificationViewComponentName() : multiFactorAuthenticationProvider.GetPublicViewComponentName();
                providerModel.Selected = sysName == selectedProvider;
            }

            return providerModel;
        }

        /// <summary>
        /// Prepare the custom customer attribute models
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="overrideAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of the customer attribute model
        /// </returns>
        public virtual async Task<IList<CustomerAttributeModel>> PrepareCustomCustomerAttributesAsync(Customer customer, string overrideAttributesXml = "")
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var result = new List<CustomerAttributeModel>();

            var customerAttributes = await CustomerAttributeService.GetAllCustomerAttributesAsync();
            foreach (var attribute in customerAttributes)
            {
                var attributeModel = new CustomerAttributeModel
                {
                    Id = attribute.Id,
                    Name = await LocalizationService.GetLocalizedAsync(attribute, x => x.Name),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = await CustomerAttributeService.GetCustomerAttributeValuesAsync(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var valueModel = new CustomerAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = await LocalizationService.GetLocalizedAsync(attributeValue, x => x.Name),
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(valueModel);
                    }
                }

                //set already selected attributes
                var selectedAttributesXml = !string.IsNullOrEmpty(overrideAttributesXml) ?
                    overrideAttributesXml :
                    await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CustomCustomerAttributes);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                        {
                            if (!string.IsNullOrEmpty(selectedAttributesXml))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = await CustomerAttributeParser.ParseCustomerAttributeValuesAsync(selectedAttributesXml);
                                foreach (var attributeValue in selectedValues)
                                    foreach (var item in attributeModel.Values)
                                        if (attributeValue.Id == item.Id)
                                            item.IsPreSelected = true;
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //do nothing
                            //values are already pre-set
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            if (!string.IsNullOrEmpty(selectedAttributesXml))
                            {
                                var enteredText = CustomerAttributeParser.ParseValues(selectedAttributesXml, attribute.Id);
                                if (enteredText.Any())
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    default:
                        //not supported attribute control types
                        break;
                }

                result.Add(attributeModel);
            }

            return result;
        }

        #endregion
    }
}