using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Core.Http;
using Nop.Core.Http.Extensions;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Tax;
using Nop.Web.Extensions;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;

namespace Nop.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class CustomerController : BasePublicController
    {
        #region Fields

        protected AddressSettings AddressSettings { get; }
        protected CaptchaSettings CaptchaSettings { get; }
        protected CustomerSettings CustomerSettings { get; }
        protected DateTimeSettings DateTimeSettings { get; }
        protected IDownloadService DownloadService { get; }
        protected ForumSettings ForumSettings { get; }
        protected GdprSettings GdprSettings { get; }
        protected IAddressAttributeParser AddressAttributeParser { get; }
        protected IAddressModelFactory AddressModelFactory { get; }
        protected IAddressService AddressService { get; }
        protected IAuthenticationService AuthenticationService { get; }
        protected ICountryService CountryService { get; }
        protected ICurrencyService CurrencyService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected ICustomerAttributeParser CustomerAttributeParser { get; }
        protected ICustomerAttributeService CustomerAttributeService { get; }
        protected ICustomerModelFactory CustomerModelFactory { get; }
        protected ICustomerRegistrationService CustomerRegistrationService { get; }
        protected ICustomerService CustomerService { get; }
        protected IEventPublisher EventPublisher { get; }
        protected IExportManager ExportManager { get; }
        protected IExternalAuthenticationService ExternalAuthenticationService { get; }
        protected IGdprService GdprService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected IGiftCardService GiftCardService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILogger Logger { get; }
        protected IMultiFactorAuthenticationPluginManager MultiFactorAuthenticationPluginManager { get; }
        protected INewsLetterSubscriptionService NewsLetterSubscriptionService { get; }
        protected INotificationService NotificationService { get; }
        protected IOrderService OrderService { get; }
        protected IPictureService PictureService { get; }
        protected IPriceFormatter PriceFormatter { get; }
        protected IProductService ProductService { get; }
        protected IStateProvinceService StateProvinceService { get; }
        protected IStoreContext StoreContext { get; }
        protected ITaxService TaxService { get; }
        protected IWorkContext WorkContext { get; }
        protected IWorkflowMessageService WorkflowMessageService { get; }
        protected LocalizationSettings LocalizationSettings { get; }
        protected MediaSettings MediaSettings { get; }
        protected MultiFactorAuthenticationSettings MultiFactorAuthenticationSettings { get; }
        protected StoreInformationSettings StoreInformationSettings { get; }
        protected TaxSettings TaxSettings { get; }

        #endregion

        #region Ctor

        public CustomerController(AddressSettings addressSettings,
            CaptchaSettings captchaSettings,
            CustomerSettings customerSettings,
            DateTimeSettings dateTimeSettings,
            IDownloadService downloadService,
            ForumSettings forumSettings,
            GdprSettings gdprSettings,
            IAddressAttributeParser addressAttributeParser,
            IAddressModelFactory addressModelFactory,
            IAddressService addressService,
            IAuthenticationService authenticationService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerActivityService customerActivityService,
            ICustomerAttributeParser customerAttributeParser,
            ICustomerAttributeService customerAttributeService,
            ICustomerModelFactory customerModelFactory,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            IExportManager exportManager,
            IExternalAuthenticationService externalAuthenticationService,
            IGdprService gdprService,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            ILocalizationService localizationService,
            ILogger logger,
            IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            INotificationService notificationService,
            IOrderService orderService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductService productService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            ITaxService taxService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            MediaSettings mediaSettings,
            MultiFactorAuthenticationSettings multiFactorAuthenticationSettings,
            StoreInformationSettings storeInformationSettings,
            TaxSettings taxSettings)
        {
            AddressSettings = addressSettings;
            CaptchaSettings = captchaSettings;
            CustomerSettings = customerSettings;
            DateTimeSettings = dateTimeSettings;
            DownloadService = downloadService;
            ForumSettings = forumSettings;
            GdprSettings = gdprSettings;
            AddressAttributeParser = addressAttributeParser;
            AddressModelFactory = addressModelFactory;
            AddressService = addressService;
            AuthenticationService = authenticationService;
            CountryService = countryService;
            CurrencyService = currencyService;
            CustomerActivityService = customerActivityService;
            CustomerAttributeParser = customerAttributeParser;
            CustomerAttributeService = customerAttributeService;
            CustomerModelFactory = customerModelFactory;
            CustomerRegistrationService = customerRegistrationService;
            CustomerService = customerService;
            EventPublisher = eventPublisher;
            ExportManager = exportManager;
            ExternalAuthenticationService = externalAuthenticationService;
            GdprService = gdprService;
            GenericAttributeService = genericAttributeService;
            GiftCardService = giftCardService;
            LocalizationService = localizationService;
            Logger = logger;
            MultiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
            NewsLetterSubscriptionService = newsLetterSubscriptionService;
            NotificationService = notificationService;
            OrderService = orderService;
            PictureService = pictureService;
            PriceFormatter = priceFormatter;
            ProductService = productService;
            StateProvinceService = stateProvinceService;
            StoreContext = storeContext;
            TaxService = taxService;
            WorkContext = workContext;
            WorkflowMessageService = workflowMessageService;
            LocalizationSettings = localizationSettings;
            MediaSettings = mediaSettings;
            MultiFactorAuthenticationSettings = multiFactorAuthenticationSettings;
            StoreInformationSettings = storeInformationSettings;
            TaxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        protected virtual void ValidateRequiredConsents(List<GdprConsent> consents, IFormCollection form)
        {
            foreach (var consent in consents)
            {
                var controlId = $"consent{consent.Id}";
                var cbConsent = form[controlId];
                if (StringValues.IsNullOrEmpty(cbConsent) || !cbConsent.ToString().Equals("on"))
                {
                    ModelState.AddModelError("", consent.RequiredMessage);
                }
            }
        }

        protected virtual async Task<string> ParseSelectedProviderAsync(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var store = await StoreContext.GetCurrentStoreAsync();

            var multiFactorAuthenticationProviders = await MultiFactorAuthenticationPluginManager.LoadActivePluginsAsync(await WorkContext.GetCurrentCustomerAsync(), store.Id);
            foreach (var provider in multiFactorAuthenticationProviders)
            {
                var controlId = $"provider_{provider.PluginDescriptor.SystemName}";

                var curProvider = form[controlId];
                if (!StringValues.IsNullOrEmpty(curProvider))
                {
                    var selectedProvider = curProvider.ToString();
                    if (!string.IsNullOrEmpty(selectedProvider))
                    {
                        return selectedProvider;
                    }
                }
            }
            return string.Empty;
        }

        protected virtual async Task<string> ParseCustomCustomerAttributesAsync(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = "";
            var attributes = await CustomerAttributeService.GetAllCustomerAttributesAsync();
            foreach (var attribute in attributes)
            {
                var controlId = $"{NopCustomerServicesDefaults.CustomerAttributePrefix}{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = CustomerAttributeParser.AddCustomerAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = CustomerAttributeParser.AddCustomerAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = await CustomerAttributeService.GetCustomerAttributeValuesAsync(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = CustomerAttributeParser.AddCustomerAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var enteredText = ctrlAttributes.ToString().Trim();
                                attributesXml = CustomerAttributeParser.AddCustomerAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.FileUpload:
                    //not supported customer attributes
                    default:
                        break;
                }
            }

            return attributesXml;
        }

        protected virtual async Task LogGdprAsync(Customer customer, CustomerInfoModel oldCustomerInfoModel,
            CustomerInfoModel newCustomerInfoModel, IFormCollection form)
        {
            try
            {
                //consents
                var consents = (await GdprService.GetAllConsentsAsync()).Where(consent => consent.DisplayOnCustomerInfoPage).ToList();
                foreach (var consent in consents)
                {
                    var previousConsentValue = await GdprService.IsConsentAcceptedAsync(consent.Id, customer.Id);
                    var controlId = $"consent{consent.Id}";
                    var cbConsent = form[controlId];
                    if (!StringValues.IsNullOrEmpty(cbConsent) && cbConsent.ToString().Equals("on"))
                    {
                        //agree
                        if (!previousConsentValue.HasValue || !previousConsentValue.Value)
                        {
                            await GdprService.InsertLogAsync(customer, consent.Id, GdprRequestType.ConsentAgree, consent.Message);
                        }
                    }
                    else
                    {
                        //disagree
                        if (!previousConsentValue.HasValue || previousConsentValue.Value)
                        {
                            await GdprService.InsertLogAsync(customer, consent.Id, GdprRequestType.ConsentDisagree, consent.Message);
                        }
                    }
                }

                //newsletter subscriptions
                if (GdprSettings.LogNewsletterConsent)
                {
                    if (oldCustomerInfoModel.Newsletter && !newCustomerInfoModel.Newsletter)
                        await GdprService.InsertLogAsync(customer, 0, GdprRequestType.ConsentDisagree, await LocalizationService.GetResourceAsync("Gdpr.Consent.Newsletter"));
                    if (!oldCustomerInfoModel.Newsletter && newCustomerInfoModel.Newsletter)
                        await GdprService.InsertLogAsync(customer, 0, GdprRequestType.ConsentAgree, await LocalizationService.GetResourceAsync("Gdpr.Consent.Newsletter"));
                }

                //user profile changes
                if (!GdprSettings.LogUserProfileChanges)
                    return;

                if (oldCustomerInfoModel.Gender != newCustomerInfoModel.Gender)
                    await GdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await LocalizationService.GetResourceAsync("Account.Fields.Gender")} = {newCustomerInfoModel.Gender}");

                if (oldCustomerInfoModel.FirstName != newCustomerInfoModel.FirstName)
                    await GdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await LocalizationService.GetResourceAsync("Account.Fields.FirstName")} = {newCustomerInfoModel.FirstName}");

                if (oldCustomerInfoModel.LastName != newCustomerInfoModel.LastName)
                    await GdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await LocalizationService.GetResourceAsync("Account.Fields.LastName")} = {newCustomerInfoModel.LastName}");

                if (oldCustomerInfoModel.ParseDateOfBirth() != newCustomerInfoModel.ParseDateOfBirth())
                    await GdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await LocalizationService.GetResourceAsync("Account.Fields.DateOfBirth")} = {newCustomerInfoModel.ParseDateOfBirth()}");

                if (oldCustomerInfoModel.Email != newCustomerInfoModel.Email)
                    await GdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await LocalizationService.GetResourceAsync("Account.Fields.Email")} = {newCustomerInfoModel.Email}");

                if (oldCustomerInfoModel.Company != newCustomerInfoModel.Company)
                    await GdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await LocalizationService.GetResourceAsync("Account.Fields.Company")} = {newCustomerInfoModel.Company}");

                if (oldCustomerInfoModel.StreetAddress != newCustomerInfoModel.StreetAddress)
                    await GdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await LocalizationService.GetResourceAsync("Account.Fields.StreetAddress")} = {newCustomerInfoModel.StreetAddress}");

                if (oldCustomerInfoModel.StreetAddress2 != newCustomerInfoModel.StreetAddress2)
                    await GdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await LocalizationService.GetResourceAsync("Account.Fields.StreetAddress2")} = {newCustomerInfoModel.StreetAddress2}");

                if (oldCustomerInfoModel.ZipPostalCode != newCustomerInfoModel.ZipPostalCode)
                    await GdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await LocalizationService.GetResourceAsync("Account.Fields.ZipPostalCode")} = {newCustomerInfoModel.ZipPostalCode}");

                if (oldCustomerInfoModel.City != newCustomerInfoModel.City)
                    await GdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await LocalizationService.GetResourceAsync("Account.Fields.City")} = {newCustomerInfoModel.City}");

                if (oldCustomerInfoModel.County != newCustomerInfoModel.County)
                    await GdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await LocalizationService.GetResourceAsync("Account.Fields.County")} = {newCustomerInfoModel.County}");

                if (oldCustomerInfoModel.CountryId != newCustomerInfoModel.CountryId)
                {
                    var countryName = (await CountryService.GetCountryByIdAsync(newCustomerInfoModel.CountryId))?.Name;
                    await GdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await LocalizationService.GetResourceAsync("Account.Fields.Country")} = {countryName}");
                }

                if (oldCustomerInfoModel.StateProvinceId != newCustomerInfoModel.StateProvinceId)
                {
                    var stateProvinceName = (await StateProvinceService.GetStateProvinceByIdAsync(newCustomerInfoModel.StateProvinceId))?.Name;
                    await GdprService.InsertLogAsync(customer, 0, GdprRequestType.ProfileChanged, $"{await LocalizationService.GetResourceAsync("Account.Fields.StateProvince")} = {stateProvinceName}");
                }
            }
            catch (Exception exception)
            {
                await Logger.ErrorAsync(exception.Message, exception, customer);
            }
        }

        #endregion

        #region Methods

        #region Login / logout

        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual async Task<IActionResult> Login(bool? checkoutAsGuest)
        {
            var model = await CustomerModelFactory.PrepareLoginModelAsync(checkoutAsGuest);
            var customer = await WorkContext.GetCurrentCustomerAsync();

            if (await CustomerService.IsRegisteredAsync(customer))
            {
                var fullName = await CustomerService.GetCustomerFullNameAsync(customer);
                var message = await LocalizationService.GetResourceAsync("Account.Login.AlreadyLogin");
                NotificationService.SuccessNotification(string.Format(message, fullName));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateCaptcha]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual async Task<IActionResult> Login(LoginModel model, string returnUrl, bool captchaValid)
        {
            //validate CAPTCHA
            if (CaptchaSettings.Enabled && CaptchaSettings.ShowOnLoginPage && !captchaValid)
            {
                ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                var customerUserName = model.Username?.Trim();
                var customerEmail = model.Email?.Trim();
                var userNameOrEmail = CustomerSettings.UsernamesEnabled ? customerUserName : customerEmail;

                var loginResult = await CustomerRegistrationService.ValidateCustomerAsync(userNameOrEmail, model.Password);
                switch (loginResult)
                {
                    case CustomerLoginResults.Successful:
                        {
                            var customer = CustomerSettings.UsernamesEnabled
                                ? await CustomerService.GetCustomerByUsernameAsync(customerUserName)
                                : await CustomerService.GetCustomerByEmailAsync(customerEmail);

                            return await CustomerRegistrationService.SignInCustomerAsync(customer, returnUrl, model.RememberMe);
                        }
                    case CustomerLoginResults.MultiFactorAuthenticationRequired:
                        {
                            var customerMultiFactorAuthenticationInfo = new CustomerMultiFactorAuthenticationInfo
                            {
                                UserName = userNameOrEmail,
                                RememberMe = model.RememberMe,
                                ReturnUrl = returnUrl
                            };
                            HttpContext.Session.Set(NopCustomerDefaults.CustomerMultiFactorAuthenticationInfo, customerMultiFactorAuthenticationInfo);
                            return RedirectToRoute("MultiFactorVerification");
                        }
                    case CustomerLoginResults.CustomerNotExist:
                        ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Account.Login.WrongCredentials.CustomerNotExist"));
                        break;
                    case CustomerLoginResults.Deleted:
                        ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Account.Login.WrongCredentials.Deleted"));
                        break;
                    case CustomerLoginResults.NotActive:
                        ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Account.Login.WrongCredentials.NotActive"));
                        break;
                    case CustomerLoginResults.NotRegistered:
                        ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Account.Login.WrongCredentials.NotRegistered"));
                        break;
                    case CustomerLoginResults.LockedOut:
                        ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Account.Login.WrongCredentials.LockedOut"));
                        break;
                    case CustomerLoginResults.WrongPassword:
                    default:
                        ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Account.Login.WrongCredentials"));
                        break;
                }
            }

            //If we got this far, something failed, redisplay form
            model = await CustomerModelFactory.PrepareLoginModelAsync(model.CheckoutAsGuest);
            return View(model);
        }

        /// <summary>
        /// The entry point for injecting a plugin component of type "MultiFactorAuth"
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user verification page for Multi-factor authentication. Served by an authentication provider.
        /// </returns>
        public virtual async Task<IActionResult> MultiFactorVerification()
        {
            if (!await MultiFactorAuthenticationPluginManager.HasActivePluginsAsync())
                return RedirectToRoute("Login");

            var customerMultiFactorAuthenticationInfo = HttpContext.Session.Get<CustomerMultiFactorAuthenticationInfo>(NopCustomerDefaults.CustomerMultiFactorAuthenticationInfo);
            var userName = customerMultiFactorAuthenticationInfo.UserName;
            if (string.IsNullOrEmpty(userName))
                return RedirectToRoute("HomePage");

            var customer = CustomerSettings.UsernamesEnabled ? await CustomerService.GetCustomerByUsernameAsync(userName) : await CustomerService.GetCustomerByEmailAsync(userName);
            if (customer == null)
                return RedirectToRoute("HomePage");

            var selectedProvider = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute);
            if (string.IsNullOrEmpty(selectedProvider))
                return RedirectToRoute("HomePage");

            var model = new MultiFactorAuthenticationProviderModel();
            model = await CustomerModelFactory.PrepareMultiFactorAuthenticationProviderModelAsync(model, selectedProvider, true);

            return View(model);
        }

        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual async Task<IActionResult> Logout()
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (WorkContext.OriginalCustomerIfImpersonated != null)
            {
                //activity log
                await CustomerActivityService.InsertActivityAsync(WorkContext.OriginalCustomerIfImpersonated, "Impersonation.Finished",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.Impersonation.Finished.StoreOwner"),
                        customer.Email, customer.Id),
                    customer);

                await CustomerActivityService.InsertActivityAsync("Impersonation.Finished",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.Impersonation.Finished.Customer"),
                        WorkContext.OriginalCustomerIfImpersonated.Email, WorkContext.OriginalCustomerIfImpersonated.Id),
                    WorkContext.OriginalCustomerIfImpersonated);

                //logout impersonated customer
                await GenericAttributeService
                    .SaveAttributeAsync<int?>(WorkContext.OriginalCustomerIfImpersonated, NopCustomerDefaults.ImpersonatedCustomerIdAttribute, null);

                //redirect back to customer details page (admin area)
                return RedirectToAction("Edit", "Customer", new { id = customer.Id, area = AreaNames.Admin });
            }

            //activity log
            await CustomerActivityService.InsertActivityAsync(customer, "PublicStore.Logout",
                await LocalizationService.GetResourceAsync("ActivityLog.PublicStore.Logout"), customer);

            //standard logout 
            await AuthenticationService.SignOutAsync();

            //raise logged out event       
            await EventPublisher.PublishAsync(new CustomerLoggedOutEvent(customer));

            //EU Cookie
            if (StoreInformationSettings.DisplayEuCookieLawWarning)
            {
                //the cookie law message should not pop up immediately after logout.
                //otherwise, the user will have to click it again...
                //and thus next visitor will not click it... so violation for that cookie law..
                //the only good solution in this case is to store a temporary variable
                //indicating that the EU cookie popup window should not be displayed on the next page open (after logout redirection to homepage)
                //but it'll be displayed for further page loads
                TempData[$"{NopCookieDefaults.Prefix}{NopCookieDefaults.IgnoreEuCookieLawWarning}"] = true;
            }

            return RedirectToRoute("Homepage");
        }

        #endregion

        #region Password recovery

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        public virtual async Task<IActionResult> PasswordRecovery()
        {
            var model = new PasswordRecoveryModel();
            model = await CustomerModelFactory.PreparePasswordRecoveryModelAsync(model);

            return View(model);
        }

        [ValidateCaptcha]
        [HttpPost, ActionName("PasswordRecovery")]
        [FormValueRequired("send-email")]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        public virtual async Task<IActionResult> PasswordRecoverySend(PasswordRecoveryModel model, bool captchaValid)
        {
            // validate CAPTCHA
            if (CaptchaSettings.Enabled && CaptchaSettings.ShowOnForgotPasswordPage && !captchaValid)
            {
                ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                var customer = await CustomerService.GetCustomerByEmailAsync(model.Email.Trim());
                if (customer != null && customer.Active && !customer.Deleted)
                {
                    //save token and current date
                    var passwordRecoveryToken = Guid.NewGuid();
                    await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute,
                        passwordRecoveryToken.ToString());
                    DateTime? generatedDateTime = DateTime.UtcNow;
                    await GenericAttributeService.SaveAttributeAsync(customer,
                        NopCustomerDefaults.PasswordRecoveryTokenDateGeneratedAttribute, generatedDateTime);

                    //send email
                    await WorkflowMessageService.SendCustomerPasswordRecoveryMessageAsync(customer,
                        (await WorkContext.GetWorkingLanguageAsync()).Id);

                    NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Account.PasswordRecovery.EmailHasBeenSent"));
                }
                else
                {
                    NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Account.PasswordRecovery.EmailNotFound"));
                }
            }

            model = await CustomerModelFactory.PreparePasswordRecoveryModelAsync(model);

            return View(model);
        }

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        public virtual async Task<IActionResult> PasswordRecoveryConfirm(string token, string email, Guid guid)
        {
            //For backward compatibility with previous versions where email was used as a parameter in the URL
            var customer = await CustomerService.GetCustomerByEmailAsync(email)
                ?? await CustomerService.GetCustomerByGuidAsync(guid);

            if (customer == null)
                return RedirectToRoute("Homepage");

            var model = new PasswordRecoveryConfirmModel { ReturnUrl = Url.RouteUrl("Homepage") };
            if (string.IsNullOrEmpty(await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute)))
            {
                model.DisablePasswordChanging = true;
                model.Result = await LocalizationService.GetResourceAsync("Account.PasswordRecovery.PasswordAlreadyHasBeenChanged");
                return View(model);
            }

            //validate token
            if (!await CustomerService.IsPasswordRecoveryTokenValidAsync(customer, token))
            {
                model.DisablePasswordChanging = true;
                model.Result = await LocalizationService.GetResourceAsync("Account.PasswordRecovery.WrongToken");
                return View(model);
            }

            //validate token expiration date
            if (await CustomerService.IsPasswordRecoveryLinkExpiredAsync(customer))
            {
                model.DisablePasswordChanging = true;
                model.Result = await LocalizationService.GetResourceAsync("Account.PasswordRecovery.LinkExpired");
                return View(model);
            }

            return View(model);
        }

        [HttpPost, ActionName("PasswordRecoveryConfirm")]
        [FormValueRequired("set-password")]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        public virtual async Task<IActionResult> PasswordRecoveryConfirmPOST(string token, string email, Guid guid, PasswordRecoveryConfirmModel model)
        {
            //For backward compatibility with previous versions where email was used as a parameter in the URL
            var customer = await CustomerService.GetCustomerByEmailAsync(email)
                ?? await CustomerService.GetCustomerByGuidAsync(guid);

            if (customer == null)
                return RedirectToRoute("Homepage");

            model.ReturnUrl = Url.RouteUrl("Homepage");

            //validate token
            if (!await CustomerService.IsPasswordRecoveryTokenValidAsync(customer, token))
            {
                model.DisablePasswordChanging = true;
                model.Result = await LocalizationService.GetResourceAsync("Account.PasswordRecovery.WrongToken");
                return View(model);
            }

            //validate token expiration date
            if (await CustomerService.IsPasswordRecoveryLinkExpiredAsync(customer))
            {
                model.DisablePasswordChanging = true;
                model.Result = await LocalizationService.GetResourceAsync("Account.PasswordRecovery.LinkExpired");
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            var response = await CustomerRegistrationService
                .ChangePasswordAsync(new ChangePasswordRequest(customer.Email, false, CustomerSettings.DefaultPasswordFormat, model.NewPassword));
            if (!response.Success)
            {
                model.Result = string.Join(';', response.Errors);
                return View(model);
            }

            await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute, "");

            //authenticate customer after changing password
            await CustomerRegistrationService.SignInCustomerAsync(customer, null, true);

            model.DisablePasswordChanging = true;
            model.Result = await LocalizationService.GetResourceAsync("Account.PasswordRecovery.PasswordHasBeenChanged");
            return View(model);
        }

        #endregion     

        #region Register

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual async Task<IActionResult> Register(string returnUrl)
        {
            //check whether registration is allowed
            if (CustomerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled, returnUrl });

            var model = new RegisterModel();
            model = await CustomerModelFactory.PrepareRegisterModelAsync(model, false, setDefaultValues: true);

            return View(model);
        }

        [HttpPost]
        [ValidateCaptcha]
        [ValidateHoneypot]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual async Task<IActionResult> Register(RegisterModel model, string returnUrl, bool captchaValid)
        {
            //check whether registration is allowed
            if (CustomerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled, returnUrl });

            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (await CustomerService.IsRegisteredAsync(customer))
            {
                //Already registered customer. 
                await AuthenticationService.SignOutAsync();

                //raise logged out event       
                await EventPublisher.PublishAsync(new CustomerLoggedOutEvent(customer));

                //Save a new record
                await WorkContext.SetCurrentCustomerAsync(await CustomerService.InsertGuestCustomerAsync());
            }

            var store = await StoreContext.GetCurrentStoreAsync();
            customer.RegisteredInStoreId = store.Id;

            var form = model.Form;

            //custom customer attributes
            var customerAttributesXml = await ParseCustomCustomerAttributesAsync(form);
            var customerAttributeWarnings = await CustomerAttributeParser.GetAttributeWarningsAsync(customerAttributesXml);
            foreach (var error in customerAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            //validate CAPTCHA
            if (CaptchaSettings.Enabled && CaptchaSettings.ShowOnRegistrationPage && !captchaValid)
            {
                ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            //GDPR
            if (GdprSettings.GdprEnabled)
            {
                var consents = (await GdprService
                    .GetAllConsentsAsync()).Where(consent => consent.DisplayDuringRegistration && consent.IsRequired).ToList();

                ValidateRequiredConsents(consents, form);
            }

            if (ModelState.IsValid)
            {
                var customerUserName = model.Username?.Trim();
                var customerEmail = model.Email?.Trim();

                var isApproved = CustomerSettings.UserRegistrationType == UserRegistrationType.Standard;
                var registrationRequest = new CustomerRegistrationRequest(customer,
                    customerEmail,
                    CustomerSettings.UsernamesEnabled ? customerUserName : customerEmail,
                    model.Password,
                    CustomerSettings.DefaultPasswordFormat,
                    store.Id,
                    isApproved);
                var registrationResult = await CustomerRegistrationService.RegisterCustomerAsync(registrationRequest);
                if (registrationResult.Success)
                {
                    //properties
                    if (DateTimeSettings.AllowCustomersToSetTimeZone)
                    {
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.TimeZoneIdAttribute, model.TimeZoneId);
                    }
                    //VAT number
                    if (TaxSettings.EuVatEnabled)
                    {
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.VatNumberAttribute, model.VatNumber);

                        var (vatNumberStatus, _, vatAddress) = await TaxService.GetVatNumberStatusAsync(model.VatNumber);
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.VatNumberStatusIdAttribute, (int)vatNumberStatus);
                        //send VAT number admin notification
                        if (!string.IsNullOrEmpty(model.VatNumber) && TaxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                            await WorkflowMessageService.SendNewVatSubmittedStoreOwnerNotificationAsync(customer, model.VatNumber, vatAddress, LocalizationSettings.DefaultAdminLanguageId);
                    }

                    //form fields
                    if (CustomerSettings.GenderEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.GenderAttribute, model.Gender);
                    if (CustomerSettings.FirstNameEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.FirstNameAttribute, model.FirstName);
                    if (CustomerSettings.LastNameEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.LastNameAttribute, model.LastName);
                    if (CustomerSettings.DateOfBirthEnabled)
                    {
                        var dateOfBirth = model.ParseDateOfBirth();
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.DateOfBirthAttribute, dateOfBirth);
                    }
                    if (CustomerSettings.CompanyEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.CompanyAttribute, model.Company);
                    if (CustomerSettings.StreetAddressEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.StreetAddressAttribute, model.StreetAddress);
                    if (CustomerSettings.StreetAddress2Enabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.StreetAddress2Attribute, model.StreetAddress2);
                    if (CustomerSettings.ZipPostalCodeEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.ZipPostalCodeAttribute, model.ZipPostalCode);
                    if (CustomerSettings.CityEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.CityAttribute, model.City);
                    if (CustomerSettings.CountyEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.CountyAttribute, model.County);
                    if (CustomerSettings.CountryEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.CountryIdAttribute, model.CountryId);
                    if (CustomerSettings.CountryEnabled && CustomerSettings.StateProvinceEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.StateProvinceIdAttribute,
                            model.StateProvinceId);
                    if (CustomerSettings.PhoneEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.PhoneAttribute, model.Phone);
                    if (CustomerSettings.FaxEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.FaxAttribute, model.Fax);

                    //newsletter
                    if (CustomerSettings.NewsletterEnabled)
                    {
                        var isNewsletterActive = CustomerSettings.UserRegistrationType != UserRegistrationType.EmailValidation;

                        //save newsletter value
                        var newsletter = await NewsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(customerEmail, store.Id);
                        if (newsletter != null)
                        {
                            if (model.Newsletter)
                            {
                                newsletter.Active = isNewsletterActive;
                                await NewsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(newsletter);

                                //GDPR
                                if (GdprSettings.GdprEnabled && GdprSettings.LogNewsletterConsent)
                                {
                                    await GdprService.InsertLogAsync(customer, 0, GdprRequestType.ConsentAgree, await LocalizationService.GetResourceAsync("Gdpr.Consent.Newsletter"));
                                }
                            }
                            //else
                            //{
                            //When registering, not checking the newsletter check box should not take an existing email address off of the subscription list.
                            //NewsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
                            //}
                        }
                        else
                        {
                            if (model.Newsletter)
                            {
                                await NewsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(new NewsLetterSubscription
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = customerEmail,
                                    Active = isNewsletterActive,
                                    StoreId = store.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });

                                //GDPR
                                if (GdprSettings.GdprEnabled && GdprSettings.LogNewsletterConsent)
                                {
                                    await GdprService.InsertLogAsync(customer, 0, GdprRequestType.ConsentAgree, await LocalizationService.GetResourceAsync("Gdpr.Consent.Newsletter"));
                                }
                            }
                        }
                    }

                    if (CustomerSettings.AcceptPrivacyPolicyEnabled)
                    {
                        //privacy policy is required
                        //GDPR
                        if (GdprSettings.GdprEnabled && GdprSettings.LogPrivacyPolicyConsent)
                        {
                            await GdprService.InsertLogAsync(customer, 0, GdprRequestType.ConsentAgree, await LocalizationService.GetResourceAsync("Gdpr.Consent.PrivacyPolicy"));
                        }
                    }

                    //GDPR
                    if (GdprSettings.GdprEnabled)
                    {
                        var consents = (await GdprService.GetAllConsentsAsync()).Where(consent => consent.DisplayDuringRegistration).ToList();
                        foreach (var consent in consents)
                        {
                            var controlId = $"consent{consent.Id}";
                            var cbConsent = form[controlId];
                            if (!StringValues.IsNullOrEmpty(cbConsent) && cbConsent.ToString().Equals("on"))
                            {
                                //agree
                                await GdprService.InsertLogAsync(customer, consent.Id, GdprRequestType.ConsentAgree, consent.Message);
                            }
                            else
                            {
                                //disagree
                                await GdprService.InsertLogAsync(customer, consent.Id, GdprRequestType.ConsentDisagree, consent.Message);
                            }
                        }
                    }

                    //save customer attributes
                    await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.CustomCustomerAttributes, customerAttributesXml);

                    //insert default address (if possible)
                    var defaultAddress = new Address
                    {
                        FirstName = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.FirstNameAttribute),
                        LastName = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.LastNameAttribute),
                        Email = customer.Email,
                        Company = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CompanyAttribute),
                        CountryId = await GenericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.CountryIdAttribute) > 0
                            ? (int?)await GenericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.CountryIdAttribute)
                            : null,
                        StateProvinceId = await GenericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.StateProvinceIdAttribute) > 0
                            ? (int?)await GenericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.StateProvinceIdAttribute)
                            : null,
                        County = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CountyAttribute),
                        City = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CityAttribute),
                        Address1 = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.StreetAddressAttribute),
                        Address2 = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.StreetAddress2Attribute),
                        ZipPostalCode = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.ZipPostalCodeAttribute),
                        PhoneNumber = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.PhoneAttribute),
                        FaxNumber = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.FaxAttribute),
                        CreatedOnUtc = customer.CreatedOnUtc
                    };
                    if (await AddressService.IsAddressValidAsync(defaultAddress))
                    {
                        //some validation
                        if (defaultAddress.CountryId == 0)
                            defaultAddress.CountryId = null;
                        if (defaultAddress.StateProvinceId == 0)
                            defaultAddress.StateProvinceId = null;
                        //set default address
                        //customer.Addresses.Add(defaultAddress);

                        await AddressService.InsertAddressAsync(defaultAddress);

                        await CustomerService.InsertCustomerAddressAsync(customer, defaultAddress);

                        customer.BillingAddressId = defaultAddress.Id;
                        customer.ShippingAddressId = defaultAddress.Id;

                        await CustomerService.UpdateCustomerAsync(customer);
                    }

                    //notifications
                    if (CustomerSettings.NotifyNewCustomerRegistration)
                        await WorkflowMessageService.SendCustomerRegisteredNotificationMessageAsync(customer,
                            LocalizationSettings.DefaultAdminLanguageId);

                    //raise event       
                    await EventPublisher.PublishAsync(new CustomerRegisteredEvent(customer));
                    var currentLanguage = await WorkContext.GetWorkingLanguageAsync();

                    switch (CustomerSettings.UserRegistrationType)
                    {
                        case UserRegistrationType.EmailValidation:
                            //email validation message
                            await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
                            await WorkflowMessageService.SendCustomerEmailValidationMessageAsync(customer, currentLanguage.Id);

                            //result
                            return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.EmailValidation, returnUrl });

                        case UserRegistrationType.AdminApproval:
                            return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.AdminApproval, returnUrl });

                        case UserRegistrationType.Standard:
                            //send customer welcome message
                            await WorkflowMessageService.SendCustomerWelcomeMessageAsync(customer, currentLanguage.Id);

                            //raise event       
                            await EventPublisher.PublishAsync(new CustomerActivatedEvent(customer));

                            returnUrl = Url.RouteUrl("RegisterResult", new { resultId = (int)UserRegistrationType.Standard, returnUrl });
                            return await CustomerRegistrationService.SignInCustomerAsync(customer, returnUrl, true);

                        default:
                            return RedirectToRoute("Homepage");
                    }
                }

                //errors
                foreach (var error in registrationResult.Errors)
                    ModelState.AddModelError("", error);
            }

            //If we got this far, something failed, redisplay form
            model = await CustomerModelFactory.PrepareRegisterModelAsync(model, true, customerAttributesXml);

            return View(model);
        }

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual async Task<IActionResult> RegisterResult(int resultId, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("Homepage");

            var model = await CustomerModelFactory.PrepareRegisterResultModelAsync(resultId, returnUrl);
            return View(model);
        }

        [HttpPost]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual async Task<IActionResult> CheckUsernameAvailability(string username)
        {
            var usernameAvailable = false;
            var statusText = await LocalizationService.GetResourceAsync("Account.CheckUsernameAvailability.NotAvailable");

            if (!UsernamePropertyValidator.IsValid(username, CustomerSettings))
            {
                statusText = await LocalizationService.GetResourceAsync("Account.Fields.Username.NotValid");
            }
            else if (CustomerSettings.UsernamesEnabled && !string.IsNullOrWhiteSpace(username))
            {
                var currentCustomer = await WorkContext.GetCurrentCustomerAsync();
                if (currentCustomer != null &&
                    currentCustomer.Username != null &&
                    currentCustomer.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase))
                {
                    statusText = await LocalizationService.GetResourceAsync("Account.CheckUsernameAvailability.CurrentUsername");
                }
                else
                {
                    var customer = await CustomerService.GetCustomerByUsernameAsync(username);
                    if (customer == null)
                    {
                        statusText = await LocalizationService.GetResourceAsync("Account.CheckUsernameAvailability.Available");
                        usernameAvailable = true;
                    }
                }
            }

            return Json(new { Available = usernameAvailable, Text = statusText });
        }

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual async Task<IActionResult> AccountActivation(string token, string email, Guid guid)
        {
            //For backward compatibility with previous versions where email was used as a parameter in the URL
            var customer = await CustomerService.GetCustomerByEmailAsync(email)
                ?? await CustomerService.GetCustomerByGuidAsync(guid);

            if (customer == null)
                return RedirectToRoute("Homepage");

            var model = new AccountActivationModel { ReturnUrl = Url.RouteUrl("Homepage") };
            var cToken = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.AccountActivationTokenAttribute);
            if (string.IsNullOrEmpty(cToken))
            {
                model.Result = await LocalizationService.GetResourceAsync("Account.AccountActivation.AlreadyActivated");
                return View(model);
            }

            if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("Homepage");

            //activate user account
            customer.Active = true;
            await CustomerService.UpdateCustomerAsync(customer);
            await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.AccountActivationTokenAttribute, "");

            //send welcome message
            await WorkflowMessageService.SendCustomerWelcomeMessageAsync(customer, (await WorkContext.GetWorkingLanguageAsync()).Id);

            //raise event       
            await EventPublisher.PublishAsync(new CustomerActivatedEvent(customer));

            //authenticate customer after activation
            await CustomerRegistrationService.SignInCustomerAsync(customer, null, true);

            //activating newsletter if need
            var store = await StoreContext.GetCurrentStoreAsync();
            var newsletter = await NewsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(customer.Email, store.Id);
            if (newsletter != null && !newsletter.Active)
            {
                newsletter.Active = true;
                await NewsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(newsletter);
            }

            model.Result = await LocalizationService.GetResourceAsync("Account.AccountActivation.Activated");
            return View(model);
        }

        #endregion

        #region My account / Info

        public virtual async Task<IActionResult> Info()
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (!await CustomerService.IsRegisteredAsync(customer))
                return Challenge();

            var model = new CustomerInfoModel();
            model = await CustomerModelFactory.PrepareCustomerInfoModelAsync(model, customer, false);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Info(CustomerInfoModel model)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (!await CustomerService.IsRegisteredAsync(customer))
                return Challenge();

            var oldCustomerModel = new CustomerInfoModel();

            //get customer info model before changes for gdpr log
            if (GdprSettings.GdprEnabled & GdprSettings.LogUserProfileChanges)
                oldCustomerModel = await CustomerModelFactory.PrepareCustomerInfoModelAsync(oldCustomerModel, customer, false);

            var form = model.Form;

            //custom customer attributes
            var customerAttributesXml = await ParseCustomCustomerAttributesAsync(form);
            var customerAttributeWarnings = await CustomerAttributeParser.GetAttributeWarningsAsync(customerAttributesXml);
            foreach (var error in customerAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            //GDPR
            if (GdprSettings.GdprEnabled)
            {
                var consents = (await GdprService
                    .GetAllConsentsAsync()).Where(consent => consent.DisplayOnCustomerInfoPage && consent.IsRequired).ToList();

                ValidateRequiredConsents(consents, form);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    //username 
                    if (CustomerSettings.UsernamesEnabled && CustomerSettings.AllowUsersToChangeUsernames)
                    {
                        var userName = model.Username.Trim();
                        if (!customer.Username.Equals(userName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            //change username
                            await CustomerRegistrationService.SetUsernameAsync(customer, userName);

                            //re-authenticate
                            //do not authenticate users in impersonation mode
                            if (WorkContext.OriginalCustomerIfImpersonated == null)
                                await AuthenticationService.SignInAsync(customer, true);
                        }
                    }
                    //email
                    var email = model.Email.Trim();
                    if (!customer.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //change email
                        var requireValidation = CustomerSettings.UserRegistrationType == UserRegistrationType.EmailValidation;
                        await CustomerRegistrationService.SetEmailAsync(customer, email, requireValidation);

                        //do not authenticate users in impersonation mode
                        if (WorkContext.OriginalCustomerIfImpersonated == null)
                        {
                            //re-authenticate (if usernames are disabled)
                            if (!CustomerSettings.UsernamesEnabled && !requireValidation)
                                await AuthenticationService.SignInAsync(customer, true);
                        }
                    }

                    //properties
                    if (DateTimeSettings.AllowCustomersToSetTimeZone)
                    {
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.TimeZoneIdAttribute,
                            model.TimeZoneId);
                    }
                    //VAT number
                    if (TaxSettings.EuVatEnabled)
                    {
                        var prevVatNumber = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.VatNumberAttribute);

                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.VatNumberAttribute,
                            model.VatNumber);
                        if (prevVatNumber != model.VatNumber)
                        {
                            var (vatNumberStatus, _, vatAddress) = await TaxService.GetVatNumberStatusAsync(model.VatNumber);
                            await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.VatNumberStatusIdAttribute, (int)vatNumberStatus);
                            //send VAT number admin notification
                            if (!string.IsNullOrEmpty(model.VatNumber) && TaxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                                await WorkflowMessageService.SendNewVatSubmittedStoreOwnerNotificationAsync(customer,
                                    model.VatNumber, vatAddress, LocalizationSettings.DefaultAdminLanguageId);
                        }
                    }

                    //form fields
                    if (CustomerSettings.GenderEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.GenderAttribute, model.Gender);
                    if (CustomerSettings.FirstNameEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.FirstNameAttribute, model.FirstName);
                    if (CustomerSettings.LastNameEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.LastNameAttribute, model.LastName);
                    if (CustomerSettings.DateOfBirthEnabled)
                    {
                        var dateOfBirth = model.ParseDateOfBirth();
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.DateOfBirthAttribute, dateOfBirth);
                    }
                    if (CustomerSettings.CompanyEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.CompanyAttribute, model.Company);
                    if (CustomerSettings.StreetAddressEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.StreetAddressAttribute, model.StreetAddress);
                    if (CustomerSettings.StreetAddress2Enabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.StreetAddress2Attribute, model.StreetAddress2);
                    if (CustomerSettings.ZipPostalCodeEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.ZipPostalCodeAttribute, model.ZipPostalCode);
                    if (CustomerSettings.CityEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.CityAttribute, model.City);
                    if (CustomerSettings.CountyEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.CountyAttribute, model.County);
                    if (CustomerSettings.CountryEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.CountryIdAttribute, model.CountryId);
                    if (CustomerSettings.CountryEnabled && CustomerSettings.StateProvinceEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.StateProvinceIdAttribute, model.StateProvinceId);
                    if (CustomerSettings.PhoneEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.PhoneAttribute, model.Phone);
                    if (CustomerSettings.FaxEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.FaxAttribute, model.Fax);

                    //newsletter
                    if (CustomerSettings.NewsletterEnabled)
                    {
                        //save newsletter value
                        var store = await StoreContext.GetCurrentStoreAsync();
                        var newsletter = await NewsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(customer.Email, store.Id);
                        if (newsletter != null)
                        {
                            if (model.Newsletter)
                            {
                                newsletter.Active = true;
                                await NewsLetterSubscriptionService.UpdateNewsLetterSubscriptionAsync(newsletter);
                            }
                            else
                            {
                                await NewsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(newsletter);
                            }
                        }
                        else
                        {
                            if (model.Newsletter)
                            {
                                await NewsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(new NewsLetterSubscription
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = customer.Email,
                                    Active = true,
                                    StoreId = store.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                            }
                        }
                    }

                    if (ForumSettings.ForumsEnabled && ForumSettings.SignaturesEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SignatureAttribute, model.Signature);

                    //save customer attributes
                    await GenericAttributeService.SaveAttributeAsync(customer,
                        NopCustomerDefaults.CustomCustomerAttributes, customerAttributesXml);

                    //GDPR
                    if (GdprSettings.GdprEnabled)
                        await LogGdprAsync(customer, oldCustomerModel, model, form);

                    return RedirectToRoute("CustomerInfo");
                }
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
            }

            //If we got this far, something failed, redisplay form
            model = await CustomerModelFactory.PrepareCustomerInfoModelAsync(model, customer, true, customerAttributesXml);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> RemoveExternalAssociation(int id)
        {
            if (!await CustomerService.IsRegisteredAsync(await WorkContext.GetCurrentCustomerAsync()))
                return Challenge();

            //ensure it's our record
            var ear = await ExternalAuthenticationService.GetExternalAuthenticationRecordByIdAsync(id);

            if (ear == null)
            {
                return Json(new
                {
                    redirect = Url.Action("Info"),
                });
            }

            await ExternalAuthenticationService.DeleteExternalAuthenticationRecordAsync(ear);

            return Json(new
            {
                redirect = Url.Action("Info"),
            });
        }

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual async Task<IActionResult> EmailRevalidation(string token, string email, Guid guid)
        {
            //For backward compatibility with previous versions where email was used as a parameter in the URL
            var customer = await CustomerService.GetCustomerByEmailAsync(email)
                ?? await CustomerService.GetCustomerByGuidAsync(guid);

            if (customer == null)
                return RedirectToRoute("Homepage");

            var model = new EmailRevalidationModel { ReturnUrl = Url.RouteUrl("Homepage") };
            var cToken = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.EmailRevalidationTokenAttribute);
            if (string.IsNullOrEmpty(cToken))
            {
                model.Result = await LocalizationService.GetResourceAsync("Account.EmailRevalidation.AlreadyChanged");
                return View(model);
            }

            if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("Homepage");

            if (string.IsNullOrEmpty(customer.EmailToRevalidate))
                return RedirectToRoute("Homepage");

            if (CustomerSettings.UserRegistrationType != UserRegistrationType.EmailValidation)
                return RedirectToRoute("Homepage");

            //change email
            try
            {
                await CustomerRegistrationService.SetEmailAsync(customer, customer.EmailToRevalidate, false);
            }
            catch (Exception exc)
            {
                model.Result = await LocalizationService.GetResourceAsync(exc.Message);
                return View(model);
            }

            customer.EmailToRevalidate = null;
            await CustomerService.UpdateCustomerAsync(customer);
            await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.EmailRevalidationTokenAttribute, "");

            //authenticate customer after changing email
            await CustomerRegistrationService.SignInCustomerAsync(customer, null, true);

            model.Result = await LocalizationService.GetResourceAsync("Account.EmailRevalidation.Changed");
            return View(model);
        }

        #endregion

        #region My account / Addresses

        public virtual async Task<IActionResult> Addresses()
        {
            if (!await CustomerService.IsRegisteredAsync(await WorkContext.GetCurrentCustomerAsync()))
                return Challenge();

            var model = await CustomerModelFactory.PrepareCustomerAddressListModelAsync();

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddressDelete(int addressId)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (!await CustomerService.IsRegisteredAsync(customer))
                return Challenge();

            //find address (ensure that it belongs to the current customer)
            var address = await CustomerService.GetCustomerAddressAsync(customer.Id, addressId);
            if (address != null)
            {
                await CustomerService.RemoveCustomerAddressAsync(customer, address);
                await CustomerService.UpdateCustomerAsync(customer);
                //now delete the address record
                await AddressService.DeleteAddressAsync(address);
            }

            //redirect to the address list page
            return Json(new
            {
                redirect = Url.RouteUrl("CustomerAddresses"),
            });
        }

        public virtual async Task<IActionResult> AddressAdd()
        {
            if (!await CustomerService.IsRegisteredAsync(await WorkContext.GetCurrentCustomerAsync()))
                return Challenge();

            var model = new CustomerAddressEditModel();
            await AddressModelFactory.PrepareAddressModelAsync(model.Address,
                address: null,
                excludeProperties: false,
                addressSettings: AddressSettings,
                loadCountries: async () => await CountryService.GetAllCountriesAsync((await WorkContext.GetWorkingLanguageAsync()).Id));

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddressAdd(CustomerAddressEditModel model)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (!await CustomerService.IsRegisteredAsync(customer))
                return Challenge();

            //custom address attributes
            var customAttributes = await AddressAttributeParser.ParseCustomAddressAttributesAsync(model.Form);
            var customAttributeWarnings = await AddressAttributeParser.GetAttributeWarningsAsync(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                var address = model.Address.ToEntity();
                address.CustomAttributes = customAttributes;
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;


                await AddressService.InsertAddressAsync(address);

                await CustomerService.InsertCustomerAddressAsync(customer, address);

                return RedirectToRoute("CustomerAddresses");
            }

            //If we got this far, something failed, redisplay form
            await AddressModelFactory.PrepareAddressModelAsync(model.Address,
                address: null,
                excludeProperties: true,
                addressSettings: AddressSettings,
                loadCountries: async () => await CountryService.GetAllCountriesAsync((await WorkContext.GetWorkingLanguageAsync()).Id),
                overrideAttributesXml: customAttributes);

            return View(model);
        }

        public virtual async Task<IActionResult> AddressEdit(int addressId)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (!await CustomerService.IsRegisteredAsync(customer))
                return Challenge();

            //find address (ensure that it belongs to the current customer)
            var address = await CustomerService.GetCustomerAddressAsync(customer.Id, addressId);
            if (address == null)
                //address is not found
                return RedirectToRoute("CustomerAddresses");

            var model = new CustomerAddressEditModel();
            await AddressModelFactory.PrepareAddressModelAsync(model.Address,
                address: address,
                excludeProperties: false,
                addressSettings: AddressSettings,
                loadCountries: async () => await CountryService.GetAllCountriesAsync((await WorkContext.GetWorkingLanguageAsync()).Id));

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddressEdit(CustomerAddressEditModel model, int addressId)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (!await CustomerService.IsRegisteredAsync(customer))
                return Challenge();

            //find address (ensure that it belongs to the current customer)
            var address = await CustomerService.GetCustomerAddressAsync(customer.Id, addressId);
            if (address == null)
                //address is not found
                return RedirectToRoute("CustomerAddresses");

            //custom address attributes
            var customAttributes = await AddressAttributeParser.ParseCustomAddressAttributesAsync(model.Form);
            var customAttributeWarnings = await AddressAttributeParser.GetAttributeWarningsAsync(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                address.CustomAttributes = customAttributes;
                await AddressService.UpdateAddressAsync(address);

                return RedirectToRoute("CustomerAddresses");
            }

            //If we got this far, something failed, redisplay form
            await AddressModelFactory.PrepareAddressModelAsync(model.Address,
                address: address,
                excludeProperties: true,
                addressSettings: AddressSettings,
                loadCountries: async () => await CountryService.GetAllCountriesAsync((await WorkContext.GetWorkingLanguageAsync()).Id),
                overrideAttributesXml: customAttributes);

            return View(model);
        }

        #endregion

        #region My account / Downloadable products

        public virtual async Task<IActionResult> DownloadableProducts()
        {
            if (!await CustomerService.IsRegisteredAsync(await WorkContext.GetCurrentCustomerAsync()))
                return Challenge();

            if (CustomerSettings.HideDownloadableProductsTab)
                return RedirectToRoute("CustomerInfo");

            var model = await CustomerModelFactory.PrepareCustomerDownloadableProductsModelAsync();

            return View(model);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(true)]
        public virtual async Task<IActionResult> UserAgreement(Guid orderItemId)
        {
            var orderItem = await OrderService.GetOrderItemByGuidAsync(orderItemId);
            if (orderItem == null)
                return RedirectToRoute("Homepage");

            var product = await ProductService.GetProductByIdAsync(orderItem.ProductId);

            if (product == null || !product.HasUserAgreement)
                return RedirectToRoute("Homepage");

            var model = await CustomerModelFactory.PrepareUserAgreementModelAsync(orderItem, product);

            return View(model);
        }

        #endregion

        #region My account / Change password

        public virtual async Task<IActionResult> ChangePassword()
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (!await CustomerService.IsRegisteredAsync(customer))
                return Challenge();

            var model = await CustomerModelFactory.PrepareChangePasswordModelAsync();

            //display the cause of the change password 
            if (await CustomerService.IsPasswordExpiredAsync(customer))
                ModelState.AddModelError(string.Empty, await LocalizationService.GetResourceAsync("Account.ChangePassword.PasswordIsExpired"));

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ChangePassword(ChangePasswordModel model, string returnUrl)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (!await CustomerService.IsRegisteredAsync(customer))
                return Challenge();

            if (ModelState.IsValid)
            {
                var changePasswordRequest = new ChangePasswordRequest(customer.Email,
                    true, CustomerSettings.DefaultPasswordFormat, model.NewPassword, model.OldPassword);
                var changePasswordResult = await CustomerRegistrationService.ChangePasswordAsync(changePasswordRequest);
                if (changePasswordResult.Success)
                {
                    NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Account.ChangePassword.Success"));
                    return string.IsNullOrEmpty(returnUrl)  ? View(model) : new RedirectResult(returnUrl);
                }

                //errors
                foreach (var error in changePasswordResult.Errors)
                    ModelState.AddModelError("", error);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region My account / Avatar

        public virtual async Task<IActionResult> Avatar()
        {
            if (!await CustomerService.IsRegisteredAsync(await WorkContext.GetCurrentCustomerAsync()))
                return Challenge();

            if (!CustomerSettings.AllowCustomersToUploadAvatars)
                return RedirectToRoute("CustomerInfo");

            var model = new CustomerAvatarModel();
            model = await CustomerModelFactory.PrepareCustomerAvatarModelAsync(model);

            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [FormValueRequired("upload-avatar")]
        public virtual async Task<IActionResult> UploadAvatar(CustomerAvatarModel model, IFormFile uploadedFile)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (!await CustomerService.IsRegisteredAsync(customer))
                return Challenge();

            if (!CustomerSettings.AllowCustomersToUploadAvatars)
                return RedirectToRoute("CustomerInfo");

            if (ModelState.IsValid)
            {
                try
                {
                    var customerAvatar = await PictureService.GetPictureByIdAsync(await GenericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute));
                    if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
                    {
                        var avatarMaxSize = CustomerSettings.AvatarMaximumSizeBytes;
                        if (uploadedFile.Length > avatarMaxSize)
                            throw new NopException(string.Format(await LocalizationService.GetResourceAsync("Account.Avatar.MaximumUploadedFileSize"), avatarMaxSize));

                        var customerPictureBinary = await DownloadService.GetDownloadBitsAsync(uploadedFile);
                        if (customerAvatar != null)
                            customerAvatar = await PictureService.UpdatePictureAsync(customerAvatar.Id, customerPictureBinary, uploadedFile.ContentType, null);
                        else
                            customerAvatar = await PictureService.InsertPictureAsync(customerPictureBinary, uploadedFile.ContentType, null);
                    }

                    var customerAvatarId = 0;
                    if (customerAvatar != null)
                        customerAvatarId = customerAvatar.Id;

                    await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.AvatarPictureIdAttribute, customerAvatarId);

                    model.AvatarUrl = await PictureService.GetPictureUrlAsync(
                        await GenericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute),
                        MediaSettings.AvatarPictureSize,
                        false);

                    return View(model);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.Message);
                }
            }

            //If we got this far, something failed, redisplay form
            model = await CustomerModelFactory.PrepareCustomerAvatarModelAsync(model);
            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [FormValueRequired("remove-avatar")]
        public virtual async Task<IActionResult> RemoveAvatar(CustomerAvatarModel model)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (!await CustomerService.IsRegisteredAsync(customer))
                return Challenge();

            if (!CustomerSettings.AllowCustomersToUploadAvatars)
                return RedirectToRoute("CustomerInfo");

            var customerAvatar = await PictureService.GetPictureByIdAsync(await GenericAttributeService.GetAttributeAsync<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute));
            if (customerAvatar != null)
                await PictureService.DeletePictureAsync(customerAvatar);
            await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.AvatarPictureIdAttribute, 0);

            return RedirectToRoute("CustomerAvatar");
        }

        #endregion

        #region GDPR tools

        public virtual async Task<IActionResult> GdprTools()
        {
            if (!await CustomerService.IsRegisteredAsync(await WorkContext.GetCurrentCustomerAsync()))
                return Challenge();

            if (!GdprSettings.GdprEnabled)
                return RedirectToRoute("CustomerInfo");

            var model = await CustomerModelFactory.PrepareGdprToolsModelAsync();

            return View(model);
        }

        [HttpPost, ActionName("GdprTools")]
        [FormValueRequired("export-data")]
        public virtual async Task<IActionResult> GdprToolsExport()
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (!await CustomerService.IsRegisteredAsync(customer))
                return Challenge();

            if (!GdprSettings.GdprEnabled)
                return RedirectToRoute("CustomerInfo");

            //log
            await GdprService.InsertLogAsync(customer, 0, GdprRequestType.ExportData, await LocalizationService.GetResourceAsync("Gdpr.Exported"));

            var store = await StoreContext.GetCurrentStoreAsync();

            //export
            var bytes = await ExportManager.ExportCustomerGdprInfoToXlsxAsync(customer, store.Id);

            return File(bytes, MimeTypes.TextXlsx, "customerdata.xlsx");
        }

        [HttpPost, ActionName("GdprTools")]
        [FormValueRequired("delete-account")]
        public virtual async Task<IActionResult> GdprToolsDelete()
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (!await CustomerService.IsRegisteredAsync(customer))
                return Challenge();

            if (!GdprSettings.GdprEnabled)
                return RedirectToRoute("CustomerInfo");

            //log
            await GdprService.InsertLogAsync(customer, 0, GdprRequestType.DeleteCustomer, await LocalizationService.GetResourceAsync("Gdpr.DeleteRequested"));

            var model = await CustomerModelFactory.PrepareGdprToolsModelAsync();
            model.Result = await LocalizationService.GetResourceAsync("Gdpr.DeleteRequested.Success");

            return View(model);
        }

        #endregion

        #region Check gift card balance

        //check gift card balance page
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        public virtual async Task<IActionResult> CheckGiftCardBalance()
        {
            if (!(CaptchaSettings.Enabled && CustomerSettings.AllowCustomersToCheckGiftCardBalance))
            {
                return RedirectToRoute("CustomerInfo");
            }

            var model = await CustomerModelFactory.PrepareCheckGiftCardBalanceModelAsync();

            return View(model);
        }

        [HttpPost, ActionName("CheckGiftCardBalance")]
        [FormValueRequired("checkbalancegiftcard")]
        [ValidateCaptcha]
        public virtual async Task<IActionResult> CheckBalance(CheckGiftCardBalanceModel model, bool captchaValid)
        {
            //validate CAPTCHA
            if (CaptchaSettings.Enabled && !captchaValid)
            {
                ModelState.AddModelError("", await LocalizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                var giftCard = (await GiftCardService.GetAllGiftCardsAsync(giftCardCouponCode: model.GiftCardCode)).FirstOrDefault();
                if (giftCard != null && await GiftCardService.IsGiftCardValidAsync(giftCard))
                {
                    var remainingAmount = await CurrencyService.ConvertFromPrimaryStoreCurrencyAsync(await GiftCardService.GetGiftCardRemainingAmountAsync(giftCard), await WorkContext.GetWorkingCurrencyAsync());
                    model.Result = await PriceFormatter.FormatPriceAsync(remainingAmount, true, false);
                }
                else
                {
                    model.Message = await LocalizationService.GetResourceAsync("CheckGiftCardBalance.GiftCardCouponCode.Invalid");
                }
            }

            return View(model);
        }

        #endregion

        #region Multi-factor Authentication

        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        public virtual async Task<IActionResult> MultiFactorAuthentication()
        {
            if (!await MultiFactorAuthenticationPluginManager.HasActivePluginsAsync())
            {
                return RedirectToRoute("CustomerInfo");
            }

            var model = new MultiFactorAuthenticationModel();
            model = await CustomerModelFactory.PrepareMultiFactorAuthenticationModelAsync(model);
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> MultiFactorAuthentication(MultiFactorAuthenticationModel model)
        {
            var customer = await WorkContext.GetCurrentCustomerAsync();
            if (!await CustomerService.IsRegisteredAsync(customer))
                return Challenge();

            try
            {
                if (ModelState.IsValid)
                {
                    //save MultiFactorIsEnabledAttribute
                    if (!model.IsEnabled)
                    {
                        if (!MultiFactorAuthenticationSettings.ForceMultifactorAuthentication)
                        {
                            await GenericAttributeService
                                .SaveAttributeAsync(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute, string.Empty);

                            //raise change multi-factor authentication provider event       
                            await EventPublisher.PublishAsync(new CustomerChangeMultiFactorAuthenticationProviderEvent(customer));
                        }
                        else
                        {
                            model = await CustomerModelFactory.PrepareMultiFactorAuthenticationModelAsync(model);
                            model.Message = await LocalizationService.GetResourceAsync("Account.MultiFactorAuthentication.Warning.ForceActivation");
                            return View(model);
                        }
                    }
                    else
                    {
                        //save selected multi-factor authentication provider
                        var selectedProvider = await ParseSelectedProviderAsync(model.Form);
                        var lastSavedProvider = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute);
                        if (string.IsNullOrEmpty(selectedProvider) && !string.IsNullOrEmpty(lastSavedProvider))
                        {
                            selectedProvider = lastSavedProvider;
                        }

                        if (selectedProvider != lastSavedProvider)
                        {
                            await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute, selectedProvider);

                            //raise change multi-factor authentication provider event       
                            await EventPublisher.PublishAsync(new CustomerChangeMultiFactorAuthenticationProviderEvent(customer));
                        }
                    }

                    return RedirectToRoute("MultiFactorAuthenticationSettings");
                }
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
            }

            //If we got this far, something failed, redisplay form
            model = await CustomerModelFactory.PrepareMultiFactorAuthenticationModelAsync(model);
            return View(model);
        }

        public virtual async Task<IActionResult> ConfigureMultiFactorAuthenticationProvider(string providerSysName)
        {
            if (!await CustomerService.IsRegisteredAsync(await WorkContext.GetCurrentCustomerAsync()))
                return Challenge();

            var model = new MultiFactorAuthenticationProviderModel();
            model = await CustomerModelFactory.PrepareMultiFactorAuthenticationProviderModelAsync(model, providerSysName);

            return View(model);
        }

        #endregion

        #endregion
    }
}