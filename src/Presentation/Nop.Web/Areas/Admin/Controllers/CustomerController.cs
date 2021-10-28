using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.ExportImport;
using Nop.Services.Forums;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class CustomerController : BaseAdminController
    {
        #region Fields

        protected CustomerSettings CustomerSettings { get; }
        protected DateTimeSettings DateTimeSettings { get; }
        protected EmailAccountSettings EmailAccountSettings { get; }
        protected ForumSettings ForumSettings { get; }
        protected GdprSettings GdprSettings { get; }
        protected IAddressAttributeParser AddressAttributeParser { get; }
        protected IAddressService AddressService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected ICustomerAttributeParser CustomerAttributeParser { get; }
        protected ICustomerAttributeService CustomerAttributeService { get; }
        protected ICustomerModelFactory CustomerModelFactory { get; }
        protected ICustomerRegistrationService CustomerRegistrationService { get; }
        protected ICustomerService CustomerService { get; }
        protected IDateTimeHelper DateTimeHelper { get; }
        protected IEmailAccountService EmailAccountService { get; }
        protected IEventPublisher EventPublisher { get; }
        protected IExportManager ExportManager { get; }
        protected IForumService ForumService { get; }
        protected IGdprService GdprService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected INewsLetterSubscriptionService NewsLetterSubscriptionService { get; }
        protected INotificationService NotificationService { get; }
        protected IPermissionService PermissionService { get; }
        protected IQueuedEmailService QueuedEmailService { get; }
        protected IRewardPointService RewardPointService { get; }
        protected IStoreContext StoreContext { get; }
        protected IStoreService StoreService { get; }
        protected ITaxService TaxService { get; }
        protected IWorkContext WorkContext { get; }
        protected IWorkflowMessageService WorkflowMessageService { get; }
        protected TaxSettings TaxSettings { get; }

        #endregion

        #region Ctor

        public CustomerController(CustomerSettings customerSettings,
            DateTimeSettings dateTimeSettings,
            EmailAccountSettings emailAccountSettings,
            ForumSettings forumSettings,
            GdprSettings gdprSettings,
            IAddressAttributeParser addressAttributeParser,
            IAddressService addressService,
            ICustomerActivityService customerActivityService,
            ICustomerAttributeParser customerAttributeParser,
            ICustomerAttributeService customerAttributeService,
            ICustomerModelFactory customerModelFactory,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IEmailAccountService emailAccountService,
            IEventPublisher eventPublisher,
            IExportManager exportManager,
            IForumService forumService,
            IGdprService gdprService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IQueuedEmailService queuedEmailService,
            IRewardPointService rewardPointService,
            IStoreContext storeContext,
            IStoreService storeService,
            ITaxService taxService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            TaxSettings taxSettings)
        {
            CustomerSettings = customerSettings;
            DateTimeSettings = dateTimeSettings;
            EmailAccountSettings = emailAccountSettings;
            ForumSettings = forumSettings;
            GdprSettings = gdprSettings;
            AddressAttributeParser = addressAttributeParser;
            AddressService = addressService;
            CustomerActivityService = customerActivityService;
            CustomerAttributeParser = customerAttributeParser;
            CustomerAttributeService = customerAttributeService;
            CustomerModelFactory = customerModelFactory;
            CustomerRegistrationService = customerRegistrationService;
            CustomerService = customerService;
            DateTimeHelper = dateTimeHelper;
            EmailAccountService = emailAccountService;
            EventPublisher = eventPublisher;
            ExportManager = exportManager;
            ForumService = forumService;
            GdprService = gdprService;
            GenericAttributeService = genericAttributeService;
            LocalizationService = localizationService;
            NewsLetterSubscriptionService = newsLetterSubscriptionService;
            NotificationService = notificationService;
            PermissionService = permissionService;
            QueuedEmailService = queuedEmailService;
            RewardPointService = rewardPointService;
            StoreContext = storeContext;
            StoreService = storeService;
            TaxService = taxService;
            WorkContext = workContext;
            WorkflowMessageService = workflowMessageService;
            TaxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        protected virtual async Task<string> ValidateCustomerRolesAsync(IList<CustomerRole> customerRoles, IList<CustomerRole> existingCustomerRoles)
        {
            if (customerRoles == null)
                throw new ArgumentNullException(nameof(customerRoles));

            if (existingCustomerRoles == null)
                throw new ArgumentNullException(nameof(existingCustomerRoles));

            //check ACL permission to manage customer roles
            var rolesToAdd = customerRoles.Except(existingCustomerRoles);
            var rolesToDelete = existingCustomerRoles.Except(customerRoles);
            if (rolesToAdd.Any(role => role.SystemName != NopCustomerDefaults.RegisteredRoleName) || rolesToDelete.Any())
            {
                if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageAcl))
                    return await LocalizationService.GetResourceAsync("Admin.Customers.Customers.CustomerRolesManagingError");
            }

            //ensure a customer is not added to both 'Guests' and 'Registered' customer roles
            //ensure that a customer is in at least one required role ('Guests' and 'Registered')
            var isInGuestsRole = customerRoles.FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.GuestsRoleName) != null;
            var isInRegisteredRole = customerRoles.FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.RegisteredRoleName) != null;
            if (isInGuestsRole && isInRegisteredRole)
                return await LocalizationService.GetResourceAsync("Admin.Customers.Customers.GuestsAndRegisteredRolesError");
            if (!isInGuestsRole && !isInRegisteredRole)
                return await LocalizationService.GetResourceAsync("Admin.Customers.Customers.AddCustomerToGuestsOrRegisteredRoleError");

            //no errors
            return string.Empty;
        }

        protected virtual async Task<string> ParseCustomCustomerAttributesAsync(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = string.Empty;
            var customerAttributes = await CustomerAttributeService.GetAllCustomerAttributesAsync();
            foreach (var attribute in customerAttributes)
            {
                var controlId = $"{NopCustomerServicesDefaults.CustomerAttributePrefix}{attribute.Id}";
                StringValues ctrlAttributes;

                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        ctrlAttributes = form[controlId];
                        if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                        {
                            var selectedAttributeId = int.Parse(ctrlAttributes);
                            if (selectedAttributeId > 0)
                                attributesXml = CustomerAttributeParser.AddCustomerAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                        }

                        break;
                    case AttributeControlType.Checkboxes:
                        var cblAttributes = form[controlId];
                        if (!StringValues.IsNullOrEmpty(cblAttributes))
                        {
                            foreach (var item in cblAttributes.ToString()
                                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                var selectedAttributeId = int.Parse(item);
                                if (selectedAttributeId > 0)
                                    attributesXml = CustomerAttributeParser.AddCustomerAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }

                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
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

                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        ctrlAttributes = form[controlId];
                        if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                        {
                            var enteredText = ctrlAttributes.ToString().Trim();
                            attributesXml = CustomerAttributeParser.AddCustomerAttribute(attributesXml,
                                attribute, enteredText);
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

        private async Task<bool> SecondAdminAccountExistsAsync(Customer customer)
        {
            var customers = await CustomerService.GetAllCustomersAsync(customerRoleIds: new[] { (await CustomerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.AdministratorsRoleName)).Id });

            return customers.Any(c => c.Active && c.Id != customer.Id);
        }

        #endregion

        #region Customers

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //prepare model
            var model = await CustomerModelFactory.PrepareCustomerSearchModelAsync(new CustomerSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CustomerList(CustomerSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await CustomerModelFactory.PrepareCustomerListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //prepare model
            var model = await CustomerModelFactory.PrepareCustomerModelAsync(new CustomerModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Create(CustomerModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            if (!string.IsNullOrWhiteSpace(model.Email) && await CustomerService.GetCustomerByEmailAsync(model.Email) != null)
                ModelState.AddModelError(string.Empty, "Email is already registered");

            if (!string.IsNullOrWhiteSpace(model.Username) && CustomerSettings.UsernamesEnabled &&
                await CustomerService.GetCustomerByUsernameAsync(model.Username) != null)
            {
                ModelState.AddModelError(string.Empty, "Username is already registered");
            }

            //validate customer roles
            var allCustomerRoles = await CustomerService.GetAllCustomerRolesAsync(true);
            var newCustomerRoles = new List<CustomerRole>();
            foreach (var customerRole in allCustomerRoles)
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                    newCustomerRoles.Add(customerRole);
            var customerRolesError = await ValidateCustomerRolesAsync(newCustomerRoles, new List<CustomerRole>());
            if (!string.IsNullOrEmpty(customerRolesError))
            {
                ModelState.AddModelError(string.Empty, customerRolesError);
                NotificationService.ErrorNotification(customerRolesError);
            }

            // Ensure that valid email address is entered if Registered role is checked to avoid registered customers with empty email address
            if (newCustomerRoles.Any() && newCustomerRoles.FirstOrDefault(c => c.SystemName == NopCustomerDefaults.RegisteredRoleName) != null &&
                !CommonHelper.IsValidEmail(model.Email))
            {
                ModelState.AddModelError(string.Empty, await LocalizationService.GetResourceAsync("Admin.Customers.Customers.ValidEmailRequiredRegisteredRole"));

                NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.ValidEmailRequiredRegisteredRole"));
            }

            //custom customer attributes
            var customerAttributesXml = await ParseCustomCustomerAttributesAsync(model.Form);
            if (newCustomerRoles.Any() && newCustomerRoles.FirstOrDefault(c => c.SystemName == NopCustomerDefaults.RegisteredRoleName) != null)
            {
                var customerAttributeWarnings = await CustomerAttributeParser.GetAttributeWarningsAsync(customerAttributesXml);
                foreach (var error in customerAttributeWarnings)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            if (ModelState.IsValid)
            {
                //fill entity from model
                var customer = model.ToEntity<Customer>();
                var currentStore = await StoreContext.GetCurrentStoreAsync();

                customer.CustomerGuid = Guid.NewGuid();
                customer.CreatedOnUtc = DateTime.UtcNow;
                customer.LastActivityDateUtc = DateTime.UtcNow;
                customer.RegisteredInStoreId = currentStore.Id;

                await CustomerService.InsertCustomerAsync(customer);

                //form fields
                if (DateTimeSettings.AllowCustomersToSetTimeZone)
                    await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.TimeZoneIdAttribute, model.TimeZoneId);
                if (CustomerSettings.GenderEnabled)
                    await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.GenderAttribute, model.Gender);
                if (CustomerSettings.FirstNameEnabled)
                    await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.FirstNameAttribute, model.FirstName);
                if (CustomerSettings.LastNameEnabled)
                    await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.LastNameAttribute, model.LastName);
                if (CustomerSettings.DateOfBirthEnabled)
                    await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.DateOfBirthAttribute, model.DateOfBirth);
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

                //custom customer attributes
                await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.CustomCustomerAttributes, customerAttributesXml);

                //newsletter subscriptions
                if (!string.IsNullOrEmpty(customer.Email))
                {
                    var allStores = await StoreService.GetAllStoresAsync();
                    foreach (var store in allStores)
                    {
                        var newsletterSubscription = await NewsLetterSubscriptionService
                            .GetNewsLetterSubscriptionByEmailAndStoreIdAsync(customer.Email, store.Id);
                        if (model.SelectedNewsletterSubscriptionStoreIds != null &&
                            model.SelectedNewsletterSubscriptionStoreIds.Contains(store.Id))
                        {
                            //subscribed
                            if (newsletterSubscription == null)
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
                        else
                        {
                            //not subscribed
                            if (newsletterSubscription != null)
                            {
                                await NewsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(newsletterSubscription);
                            }
                        }
                    }
                }

                //password
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    var changePassRequest = new ChangePasswordRequest(model.Email, false, CustomerSettings.DefaultPasswordFormat, model.Password);
                    var changePassResult = await CustomerRegistrationService.ChangePasswordAsync(changePassRequest);
                    if (!changePassResult.Success)
                    {
                        foreach (var changePassError in changePassResult.Errors)
                            NotificationService.ErrorNotification(changePassError);
                    }
                }

                //customer roles
                foreach (var customerRole in newCustomerRoles)
                {
                    //ensure that the current customer cannot add to "Administrators" system role if he's not an admin himself
                    if (customerRole.SystemName == NopCustomerDefaults.AdministratorsRoleName && !await CustomerService.IsAdminAsync(await WorkContext.GetCurrentCustomerAsync()))
                        continue;

                    await CustomerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = customerRole.Id });
                }

                await CustomerService.UpdateCustomerAsync(customer);

                //ensure that a customer with a vendor associated is not in "Administrators" role
                //otherwise, he won't have access to other functionality in admin area
                if (await CustomerService.IsAdminAsync(customer) && customer.VendorId > 0)
                {
                    customer.VendorId = 0;
                    await CustomerService.UpdateCustomerAsync(customer);

                    NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.AdminCouldNotbeVendor"));
                }

                //ensure that a customer in the Vendors role has a vendor account associated.
                //otherwise, he will have access to ALL products
                if (await CustomerService.IsVendorAsync(customer) && customer.VendorId == 0)
                {
                    var vendorRole = await CustomerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.VendorsRoleName);
                    await CustomerService.RemoveCustomerRoleMappingAsync(customer, vendorRole);

                    NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.CannotBeInVendoRoleWithoutVendorAssociated"));
                }

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewCustomer",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewCustomer"), customer.Id), customer);
                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = customer.Id });
            }

            //prepare model
            model = await CustomerModelFactory.PrepareCustomerModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(id);
            if (customer == null || customer.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = await CustomerModelFactory.PrepareCustomerModelAsync(null, customer);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Edit(CustomerModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(model.Id);
            if (customer == null || customer.Deleted)
                return RedirectToAction("List");

            //validate customer roles
            var allCustomerRoles = await CustomerService.GetAllCustomerRolesAsync(true);
            var newCustomerRoles = new List<CustomerRole>();
            foreach (var customerRole in allCustomerRoles)
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                    newCustomerRoles.Add(customerRole);

            var customerRolesError = await ValidateCustomerRolesAsync(newCustomerRoles, await CustomerService.GetCustomerRolesAsync(customer));

            if (!string.IsNullOrEmpty(customerRolesError))
            {
                ModelState.AddModelError(string.Empty, customerRolesError);
                NotificationService.ErrorNotification(customerRolesError);
            }

            // Ensure that valid email address is entered if Registered role is checked to avoid registered customers with empty email address
            if (newCustomerRoles.Any() && newCustomerRoles.FirstOrDefault(c => c.SystemName == NopCustomerDefaults.RegisteredRoleName) != null &&
                !CommonHelper.IsValidEmail(model.Email))
            {
                ModelState.AddModelError(string.Empty, await LocalizationService.GetResourceAsync("Admin.Customers.Customers.ValidEmailRequiredRegisteredRole"));
                NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.ValidEmailRequiredRegisteredRole"));
            }

            //custom customer attributes
            var customerAttributesXml = await ParseCustomCustomerAttributesAsync(model.Form);
            if (newCustomerRoles.Any() && newCustomerRoles.FirstOrDefault(c => c.SystemName == NopCustomerDefaults.RegisteredRoleName) != null)
            {
                var customerAttributeWarnings = await CustomerAttributeParser.GetAttributeWarningsAsync(customerAttributesXml);
                foreach (var error in customerAttributeWarnings)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    customer.AdminComment = model.AdminComment;
                    customer.IsTaxExempt = model.IsTaxExempt;

                    //prevent deactivation of the last active administrator
                    if (!await CustomerService.IsAdminAsync(customer) || model.Active || await SecondAdminAccountExistsAsync(customer))
                        customer.Active = model.Active;
                    else
                        NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.AdminAccountShouldExists.Deactivate"));

                    //email
                    if (!string.IsNullOrWhiteSpace(model.Email))
                        await CustomerRegistrationService.SetEmailAsync(customer, model.Email, false);
                    else
                        customer.Email = model.Email;

                    //username
                    if (CustomerSettings.UsernamesEnabled)
                    {
                        if (!string.IsNullOrWhiteSpace(model.Username))
                            await CustomerRegistrationService.SetUsernameAsync(customer, model.Username);
                        else
                            customer.Username = model.Username;
                    }

                    //VAT number
                    if (TaxSettings.EuVatEnabled)
                    {
                        var prevVatNumber = await GenericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.VatNumberAttribute);

                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.VatNumberAttribute, model.VatNumber);
                        //set VAT number status
                        if (!string.IsNullOrEmpty(model.VatNumber))
                        {
                            if (!model.VatNumber.Equals(prevVatNumber, StringComparison.InvariantCultureIgnoreCase))
                            {
                                await GenericAttributeService.SaveAttributeAsync(customer,
                                    NopCustomerDefaults.VatNumberStatusIdAttribute,
                                    (int)(await TaxService.GetVatNumberStatusAsync(model.VatNumber)).vatNumberStatus);
                            }
                        }
                        else
                        {
                            await GenericAttributeService.SaveAttributeAsync(customer,
                                NopCustomerDefaults.VatNumberStatusIdAttribute,
                                (int)VatNumberStatus.Empty);
                        }
                    }

                    //vendor
                    customer.VendorId = model.VendorId;

                    //form fields
                    if (DateTimeSettings.AllowCustomersToSetTimeZone)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.TimeZoneIdAttribute, model.TimeZoneId);
                    if (CustomerSettings.GenderEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.GenderAttribute, model.Gender);
                    if (CustomerSettings.FirstNameEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.FirstNameAttribute, model.FirstName);
                    if (CustomerSettings.LastNameEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.LastNameAttribute, model.LastName);
                    if (CustomerSettings.DateOfBirthEnabled)
                        await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.DateOfBirthAttribute, model.DateOfBirth);
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

                    //custom customer attributes
                    await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.CustomCustomerAttributes, customerAttributesXml);

                    //newsletter subscriptions
                    if (!string.IsNullOrEmpty(customer.Email))
                    {
                        var allStores = await StoreService.GetAllStoresAsync();
                        foreach (var store in allStores)
                        {
                            var newsletterSubscription = await NewsLetterSubscriptionService
                                .GetNewsLetterSubscriptionByEmailAndStoreIdAsync(customer.Email, store.Id);
                            if (model.SelectedNewsletterSubscriptionStoreIds != null &&
                                model.SelectedNewsletterSubscriptionStoreIds.Contains(store.Id))
                            {
                                //subscribed
                                if (newsletterSubscription == null)
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
                            else
                            {
                                //not subscribed
                                if (newsletterSubscription != null)
                                {
                                    await NewsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(newsletterSubscription);
                                }
                            }
                        }
                    }

                    var currentCustomerRoleIds = await CustomerService.GetCustomerRoleIdsAsync(customer, true);

                    //customer roles
                    foreach (var customerRole in allCustomerRoles)
                    {
                        //ensure that the current customer cannot add/remove to/from "Administrators" system role
                        //if he's not an admin himself
                        if (customerRole.SystemName == NopCustomerDefaults.AdministratorsRoleName &&
                            !await CustomerService.IsAdminAsync(await WorkContext.GetCurrentCustomerAsync()))
                            continue;

                        if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                        {
                            //new role
                            if (currentCustomerRoleIds.All(roleId => roleId != customerRole.Id))
                                await CustomerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = customerRole.Id });
                        }
                        else
                        {
                            //prevent attempts to delete the administrator role from the user, if the user is the last active administrator
                            if (customerRole.SystemName == NopCustomerDefaults.AdministratorsRoleName && !await SecondAdminAccountExistsAsync(customer))
                            {
                                NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.AdminAccountShouldExists.DeleteRole"));
                                continue;
                            }

                            //remove role
                            if (currentCustomerRoleIds.Any(roleId => roleId == customerRole.Id))
                                await CustomerService.RemoveCustomerRoleMappingAsync(customer, customerRole);
                        }
                    }

                    await CustomerService.UpdateCustomerAsync(customer);

                    //ensure that a customer with a vendor associated is not in "Administrators" role
                    //otherwise, he won't have access to the other functionality in admin area
                    if (await CustomerService.IsAdminAsync(customer) && customer.VendorId > 0)
                    {
                        customer.VendorId = 0;
                        await CustomerService.UpdateCustomerAsync(customer);
                        NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.AdminCouldNotbeVendor"));
                    }

                    //ensure that a customer in the Vendors role has a vendor account associated.
                    //otherwise, he will have access to ALL products
                    if (await CustomerService.IsVendorAsync(customer) && customer.VendorId == 0)
                    {
                        var vendorRole = await CustomerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.VendorsRoleName);
                        await CustomerService.RemoveCustomerRoleMappingAsync(customer, vendorRole);

                        NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.CannotBeInVendoRoleWithoutVendorAssociated"));
                    }

                    //activity log
                    await CustomerActivityService.InsertActivityAsync("EditCustomer",
                        string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditCustomer"), customer.Id), customer);

                    NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.Updated"));

                    if (!continueEditing)
                        return RedirectToAction("List");

                    return RedirectToAction("Edit", new { id = customer.Id });
                }
                catch (Exception exc)
                {
                    NotificationService.ErrorNotification(exc.Message);
                }
            }

            //prepare model
            model = await CustomerModelFactory.PrepareCustomerModelAsync(model, customer, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("changepassword")]
        public virtual async Task<IActionResult> ChangePassword(CustomerModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(model.Id);
            if (customer == null)
                return RedirectToAction("List");

            //ensure that the current customer cannot change passwords of "Administrators" if he's not an admin himself
            if (await CustomerService.IsAdminAsync(customer) && !await CustomerService.IsAdminAsync(await WorkContext.GetCurrentCustomerAsync()))
            {
                NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.OnlyAdminCanChangePassword"));
                return RedirectToAction("Edit", new { id = customer.Id });
            }

            if (!ModelState.IsValid)
                return RedirectToAction("Edit", new { id = customer.Id });

            var changePassRequest = new ChangePasswordRequest(model.Email,
                false, CustomerSettings.DefaultPasswordFormat, model.Password);
            var changePassResult = await CustomerRegistrationService.ChangePasswordAsync(changePassRequest);
            if (changePassResult.Success)
                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.PasswordChanged"));
            else
                foreach (var error in changePassResult.Errors)
                    NotificationService.ErrorNotification(error);

            return RedirectToAction("Edit", new { id = customer.Id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("markVatNumberAsValid")]
        public virtual async Task<IActionResult> MarkVatNumberAsValid(CustomerModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(model.Id);
            if (customer == null)
                return RedirectToAction("List");

            await GenericAttributeService.SaveAttributeAsync(customer,
                NopCustomerDefaults.VatNumberStatusIdAttribute,
                (int)VatNumberStatus.Valid);

            return RedirectToAction("Edit", new { id = customer.Id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("markVatNumberAsInvalid")]
        public virtual async Task<IActionResult> MarkVatNumberAsInvalid(CustomerModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(model.Id);
            if (customer == null)
                return RedirectToAction("List");

            await GenericAttributeService.SaveAttributeAsync(customer,
                NopCustomerDefaults.VatNumberStatusIdAttribute,
                (int)VatNumberStatus.Invalid);

            return RedirectToAction("Edit", new { id = customer.Id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("remove-affiliate")]
        public virtual async Task<IActionResult> RemoveAffiliate(CustomerModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(model.Id);
            if (customer == null)
                return RedirectToAction("List");

            customer.AffiliateId = 0;
            await CustomerService.UpdateCustomerAsync(customer);

            return RedirectToAction("Edit", new { id = customer.Id });
        }

        [HttpPost]
        public virtual async Task<IActionResult> RemoveBindMFA(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return RedirectToAction("List");

            await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute, string.Empty);

            //raise event       
            await EventPublisher.PublishAsync(new CustomerChangeMultiFactorAuthenticationProviderEvent(customer));

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.UnbindMFAProvider"));

            return RedirectToAction("Edit", new { id = customer.Id });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return RedirectToAction("List");

            try
            {
                //prevent attempts to delete the user, if it is the last active administrator
                if (await CustomerService.IsAdminAsync(customer) && !await SecondAdminAccountExistsAsync(customer))
                {
                    NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.AdminAccountShouldExists.DeleteAdministrator"));
                    return RedirectToAction("Edit", new { id = customer.Id });
                }

                //ensure that the current customer cannot delete "Administrators" if he's not an admin himself
                if (await CustomerService.IsAdminAsync(customer) && !await CustomerService.IsAdminAsync(await WorkContext.GetCurrentCustomerAsync()))
                {
                    NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.OnlyAdminCanDeleteAdmin"));
                    return RedirectToAction("Edit", new { id = customer.Id });
                }

                //delete
                await CustomerService.DeleteCustomerAsync(customer);

                //remove newsletter subscription (if exists)
                foreach (var store in await StoreService.GetAllStoresAsync())
                {
                    var subscription = await NewsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(customer.Email, store.Id);
                    if (subscription != null)
                        await NewsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);
                }

                //activity log
                await CustomerActivityService.InsertActivityAsync("DeleteCustomer",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteCustomer"), customer.Id), customer);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.Deleted"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                NotificationService.ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = customer.Id });
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("impersonate")]
        public virtual async Task<IActionResult> Impersonate(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.AllowCustomerImpersonation))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return RedirectToAction("List");

            if (!customer.Active)
            {
                NotificationService.WarningNotification(
                    await LocalizationService.GetResourceAsync("Admin.Customers.Customers.Impersonate.Inactive"));
                return RedirectToAction("Edit", customer.Id);
            }

            //ensure that a non-admin user cannot impersonate as an administrator
            //otherwise, that user can simply impersonate as an administrator and gain additional administrative privileges
            var currentCustomer = await WorkContext.GetCurrentCustomerAsync();
            if (!await CustomerService.IsAdminAsync(currentCustomer) && await CustomerService.IsAdminAsync(customer))
            {
                NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.NonAdminNotImpersonateAsAdminError"));
                return RedirectToAction("Edit", customer.Id);
            }

            //activity log
            await CustomerActivityService.InsertActivityAsync("Impersonation.Started",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.Impersonation.Started.StoreOwner"), customer.Email, customer.Id), customer);
            await CustomerActivityService.InsertActivityAsync(customer, "Impersonation.Started",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.Impersonation.Started.Customer"), currentCustomer.Email, currentCustomer.Id), currentCustomer);

            //ensure login is not required
            customer.RequireReLogin = false;
            await CustomerService.UpdateCustomerAsync(customer);
            await GenericAttributeService.SaveAttributeAsync<int?>(currentCustomer, NopCustomerDefaults.ImpersonatedCustomerIdAttribute, customer.Id);

            return RedirectToAction("Index", "Home", new { area = string.Empty });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("send-welcome-message")]
        public virtual async Task<IActionResult> SendWelcomeMessage(CustomerModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(model.Id);
            if (customer == null)
                return RedirectToAction("List");

            await WorkflowMessageService.SendCustomerWelcomeMessageAsync(customer, (await WorkContext.GetWorkingLanguageAsync()).Id);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.SendWelcomeMessage.Success"));

            return RedirectToAction("Edit", new { id = customer.Id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("resend-activation-message")]
        public virtual async Task<IActionResult> ReSendActivationMessage(CustomerModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(model.Id);
            if (customer == null)
                return RedirectToAction("List");

            //email validation message
            await GenericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
            await WorkflowMessageService.SendCustomerEmailValidationMessageAsync(customer, (await WorkContext.GetWorkingLanguageAsync()).Id);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.ReSendActivationMessage.Success"));

            return RedirectToAction("Edit", new { id = customer.Id });
        }

        public virtual async Task<IActionResult> SendEmail(CustomerModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(model.Id);
            if (customer == null)
                return RedirectToAction("List");

            try
            {
                if (string.IsNullOrWhiteSpace(customer.Email))
                    throw new NopException("Customer email is empty");
                if (!CommonHelper.IsValidEmail(customer.Email))
                    throw new NopException("Customer email is not valid");
                if (string.IsNullOrWhiteSpace(model.SendEmail.Subject))
                    throw new NopException("Email subject is empty");
                if (string.IsNullOrWhiteSpace(model.SendEmail.Body))
                    throw new NopException("Email body is empty");

                var emailAccount = await EmailAccountService.GetEmailAccountByIdAsync(EmailAccountSettings.DefaultEmailAccountId);
                if (emailAccount == null)
                    emailAccount = (await EmailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();
                if (emailAccount == null)
                    throw new NopException("Email account can't be loaded");
                var email = new QueuedEmail
                {
                    Priority = QueuedEmailPriority.High,
                    EmailAccountId = emailAccount.Id,
                    FromName = emailAccount.DisplayName,
                    From = emailAccount.Email,
                    ToName = await CustomerService.GetCustomerFullNameAsync(customer),
                    To = customer.Email,
                    Subject = model.SendEmail.Subject,
                    Body = model.SendEmail.Body,
                    CreatedOnUtc = DateTime.UtcNow,
                    DontSendBeforeDateUtc = model.SendEmail.SendImmediately || !model.SendEmail.DontSendBeforeDate.HasValue ?
                        null : (DateTime?)DateTimeHelper.ConvertToUtcTime(model.SendEmail.DontSendBeforeDate.Value)
                };
                await QueuedEmailService.InsertQueuedEmailAsync(email);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.SendEmail.Queued"));
            }
            catch (Exception exc)
            {
                NotificationService.ErrorNotification(exc.Message);
            }

            return RedirectToAction("Edit", new { id = customer.Id });
        }

        public virtual async Task<IActionResult> SendPm(CustomerModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(model.Id);
            if (customer == null)
                return RedirectToAction("List");

            try
            {
                if (!ForumSettings.AllowPrivateMessages)
                    throw new NopException("Private messages are disabled");
                if (await CustomerService.IsGuestAsync(customer))
                    throw new NopException("Customer should be registered");
                if (string.IsNullOrWhiteSpace(model.SendPm.Subject))
                    throw new NopException(await LocalizationService.GetResourceAsync("PrivateMessages.SubjectCannotBeEmpty"));
                if (string.IsNullOrWhiteSpace(model.SendPm.Message))
                    throw new NopException(await LocalizationService.GetResourceAsync("PrivateMessages.MessageCannotBeEmpty"));
                
                var store = await StoreContext.GetCurrentStoreAsync();

                var privateMessage = new PrivateMessage
                {
                    StoreId = store.Id,
                    ToCustomerId = customer.Id,
                    FromCustomerId = customer.Id,
                    Subject = model.SendPm.Subject,
                    Text = model.SendPm.Message,
                    IsDeletedByAuthor = false,
                    IsDeletedByRecipient = false,
                    IsRead = false,
                    CreatedOnUtc = DateTime.UtcNow
                };

                await ForumService.InsertPrivateMessageAsync(privateMessage);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.SendPM.Sent"));
            }
            catch (Exception exc)
            {
                NotificationService.ErrorNotification(exc.Message);
            }

            return RedirectToAction("Edit", new { id = customer.Id });
        }

        #endregion

        #region Reward points history

        [HttpPost]
        public virtual async Task<IActionResult> RewardPointsHistorySelect(CustomerRewardPointsSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return await AccessDeniedDataTablesJson();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(searchModel.CustomerId)
                ?? throw new ArgumentException("No customer found with the specified id");

            //prepare model
            var model = await CustomerModelFactory.PrepareRewardPointsListModelAsync(searchModel, customer);

            return Json(model);
        }

        public virtual async Task<IActionResult> RewardPointsHistoryAdd(AddRewardPointsToCustomerModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //prevent adding a new row with zero value
            if (model.Points == 0)
                return ErrorJson(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.RewardPoints.AddingZeroValueNotAllowed"));

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(model.CustomerId);
            if (customer == null)
                return ErrorJson("Customer cannot be loaded");

            //check whether delay is set
            DateTime? activatingDate = null;
            if (!model.ActivatePointsImmediately && model.ActivationDelay > 0)
            {
                var delayPeriod = (RewardPointsActivatingDelayPeriod)model.ActivationDelayPeriodId;
                var delayInHours = delayPeriod.ToHours(model.ActivationDelay);
                activatingDate = DateTime.UtcNow.AddHours(delayInHours);
            }

            //whether points validity is set
            DateTime? endDate = null;
            if (model.PointsValidity > 0)
                endDate = (activatingDate ?? DateTime.UtcNow).AddDays(model.PointsValidity.Value);

            //add reward points
            await RewardPointService.AddRewardPointsHistoryEntryAsync(customer, model.Points, model.StoreId, model.Message,
                activatingDate: activatingDate, endDate: endDate);

            return Json(new { Result = true });
        }

        #endregion

        #region Addresses

        [HttpPost]
        public virtual async Task<IActionResult> AddressesSelect(CustomerAddressSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return await AccessDeniedDataTablesJson();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(searchModel.CustomerId)
                ?? throw new ArgumentException("No customer found with the specified id");

            //prepare model
            var model = await CustomerModelFactory.PrepareCustomerAddressListModelAsync(searchModel, customer);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddressDelete(int id, int customerId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(customerId)
                ?? throw new ArgumentException("No customer found with the specified id", nameof(customerId));

            //try to get an address with the specified id
            var address = await CustomerService.GetCustomerAddressAsync(customer.Id, id);            

            if (address == null)
                return Content("No address found with the specified id");

            await CustomerService.RemoveCustomerAddressAsync(customer, address);
            await CustomerService.UpdateCustomerAsync(customer);

            //now delete the address record
            await AddressService.DeleteAddressAsync(address);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> AddressCreate(int customerId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(customerId);
            if (customer == null)
                return RedirectToAction("List");

            //prepare model
            var model = await CustomerModelFactory.PrepareCustomerAddressModelAsync(new CustomerAddressModel(), customer, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddressCreate(CustomerAddressModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(model.CustomerId);
            if (customer == null)
                return RedirectToAction("List");

            //custom address attributes
            var customAttributes = await AddressAttributeParser.ParseCustomAddressAttributesAsync(model.Form);
            var customAttributeWarnings = await AddressAttributeParser.GetAttributeWarningsAsync(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            if (ModelState.IsValid)
            {
                var address = model.Address.ToEntity<Address>();
                address.CustomAttributes = customAttributes;
                address.CreatedOnUtc = DateTime.UtcNow;

                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;

                await AddressService.InsertAddressAsync(address);

                await CustomerService.InsertCustomerAddressAsync(customer, address);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.Addresses.Added"));

                return RedirectToAction("AddressEdit", new { addressId = address.Id, customerId = model.CustomerId });
            }

            //prepare model
            model = await CustomerModelFactory.PrepareCustomerAddressModelAsync(model, customer, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> AddressEdit(int addressId, int customerId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(customerId);
            if (customer == null)
                return RedirectToAction("List");

            //try to get an address with the specified id
            var address = await AddressService.GetAddressByIdAsync(addressId);
            if (address == null)
                return RedirectToAction("Edit", new { id = customer.Id });

            //prepare model
            var model = await CustomerModelFactory.PrepareCustomerAddressModelAsync(null, customer, address);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddressEdit(CustomerAddressModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(model.CustomerId);
            if (customer == null)
                return RedirectToAction("List");

            //try to get an address with the specified id
            var address = await AddressService.GetAddressByIdAsync(model.Address.Id);
            if (address == null)
                return RedirectToAction("Edit", new { id = customer.Id });

            //custom address attributes
            var customAttributes = await AddressAttributeParser.ParseCustomAddressAttributesAsync(model.Form);
            var customAttributeWarnings = await AddressAttributeParser.GetAttributeWarningsAsync(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                address.CustomAttributes = customAttributes;
                await AddressService.UpdateAddressAsync(address);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.Addresses.Updated"));

                return RedirectToAction("AddressEdit", new { addressId = model.Address.Id, customerId = model.CustomerId });
            }

            //prepare model
            model = await CustomerModelFactory.PrepareCustomerAddressModelAsync(model, customer, address, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Orders

        [HttpPost]
        public virtual async Task<IActionResult> OrderList(CustomerOrderSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return await AccessDeniedDataTablesJson();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(searchModel.CustomerId)
                ?? throw new ArgumentException("No customer found with the specified id");

            //prepare model
            var model = await CustomerModelFactory.PrepareCustomerOrderListModelAsync(searchModel, customer);

            return Json(model);
        }

        #endregion

        #region Customer

        public virtual async Task<IActionResult> LoadCustomerStatistics(string period)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return Content(string.Empty);

            var result = new List<object>();

            var nowDt = await DateTimeHelper.ConvertToUserTimeAsync(DateTime.Now);
            var timeZone = await DateTimeHelper.GetCurrentTimeZoneAsync();
            var searchCustomerRoleIds = new[] { (await CustomerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName)).Id };

            var culture = new CultureInfo((await WorkContext.GetWorkingLanguageAsync()).LanguageCulture);

            switch (period)
            {
                case "year":
                    //year statistics
                    var yearAgoDt = nowDt.AddYears(-1).AddMonths(1);
                    var searchYearDateUser = new DateTime(yearAgoDt.Year, yearAgoDt.Month, 1);
                    for (var i = 0; i <= 12; i++)
                    {
                        result.Add(new
                        {
                            date = searchYearDateUser.Date.ToString("Y", culture),
                            value = (await CustomerService.GetAllCustomersAsync(
                                createdFromUtc: DateTimeHelper.ConvertToUtcTime(searchYearDateUser, timeZone),
                                createdToUtc: DateTimeHelper.ConvertToUtcTime(searchYearDateUser.AddMonths(1), timeZone),
                                customerRoleIds: searchCustomerRoleIds,
                                pageIndex: 0,
                                pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
                        });

                        searchYearDateUser = searchYearDateUser.AddMonths(1);
                    }

                    break;
                case "month":
                    //month statistics
                    var monthAgoDt = nowDt.AddDays(-30);
                    var searchMonthDateUser = new DateTime(monthAgoDt.Year, monthAgoDt.Month, monthAgoDt.Day);
                    for (var i = 0; i <= 30; i++)
                    {
                        result.Add(new
                        {
                            date = searchMonthDateUser.Date.ToString("M", culture),
                            value = (await CustomerService.GetAllCustomersAsync(
                                createdFromUtc: DateTimeHelper.ConvertToUtcTime(searchMonthDateUser, timeZone),
                                createdToUtc: DateTimeHelper.ConvertToUtcTime(searchMonthDateUser.AddDays(1), timeZone),
                                customerRoleIds: searchCustomerRoleIds,
                                pageIndex: 0,
                                pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
                        });

                        searchMonthDateUser = searchMonthDateUser.AddDays(1);
                    }

                    break;
                case "week":
                default:
                    //week statistics
                    var weekAgoDt = nowDt.AddDays(-7);
                    var searchWeekDateUser = new DateTime(weekAgoDt.Year, weekAgoDt.Month, weekAgoDt.Day);
                    for (var i = 0; i <= 7; i++)
                    {
                        result.Add(new
                        {
                            date = searchWeekDateUser.Date.ToString("d dddd", culture),
                            value = (await CustomerService.GetAllCustomersAsync(
                                createdFromUtc: DateTimeHelper.ConvertToUtcTime(searchWeekDateUser, timeZone),
                                createdToUtc: DateTimeHelper.ConvertToUtcTime(searchWeekDateUser.AddDays(1), timeZone),
                                customerRoleIds: searchCustomerRoleIds,
                                pageIndex: 0,
                                pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
                        });

                        searchWeekDateUser = searchWeekDateUser.AddDays(1);
                    }

                    break;
            }

            return Json(result);
        }

        #endregion

        #region Current shopping cart/ wishlist

        [HttpPost]
        public virtual async Task<IActionResult> GetCartList(CustomerShoppingCartSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return await AccessDeniedDataTablesJson();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(searchModel.CustomerId)
                ?? throw new ArgumentException("No customer found with the specified id");

            //prepare model
            var model = await CustomerModelFactory.PrepareCustomerShoppingCartListModelAsync(searchModel, customer);

            return Json(model);
        }

        #endregion

        #region Activity log

        [HttpPost]
        public virtual async Task<IActionResult> ListActivityLog(CustomerActivityLogSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return await AccessDeniedDataTablesJson();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(searchModel.CustomerId)
                ?? throw new ArgumentException("No customer found with the specified id");

            //prepare model
            var model = await CustomerModelFactory.PrepareCustomerActivityLogListModelAsync(searchModel, customer);

            return Json(model);
        }

        #endregion

        #region Back in stock subscriptions

        [HttpPost]
        public virtual async Task<IActionResult> BackInStockSubscriptionList(CustomerBackInStockSubscriptionSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return await AccessDeniedDataTablesJson();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(searchModel.CustomerId)
                ?? throw new ArgumentException("No customer found with the specified id");

            //prepare model
            var model = await CustomerModelFactory.PrepareCustomerBackInStockSubscriptionListModelAsync(searchModel, customer);

            return Json(model);
        }

        #endregion

        #region GDPR

        public virtual async Task<IActionResult> GdprLog()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //prepare model
            var model = await CustomerModelFactory.PrepareGdprLogSearchModelAsync(new GdprLogSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GdprLogList(GdprLogSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await CustomerModelFactory.PrepareGdprLogListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GdprDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return RedirectToAction("List");

            if (!GdprSettings.GdprEnabled)
                return RedirectToAction("List");

            try
            {
                //prevent attempts to delete the user, if it is the last active administrator
                if (await CustomerService.IsAdminAsync(customer) && !await SecondAdminAccountExistsAsync(customer))
                {
                    NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.AdminAccountShouldExists.DeleteAdministrator"));
                    return RedirectToAction("Edit", new { id = customer.Id });
                }

                //ensure that the current customer cannot delete "Administrators" if he's not an admin himself
                if (await CustomerService.IsAdminAsync(customer) && !await CustomerService.IsAdminAsync(await WorkContext.GetCurrentCustomerAsync()))
                {
                    NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.OnlyAdminCanDeleteAdmin"));
                    return RedirectToAction("Edit", new { id = customer.Id });
                }

                //delete
                await GdprService.PermanentDeleteCustomerAsync(customer);

                //activity log
                await CustomerActivityService.InsertActivityAsync("DeleteCustomer",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteCustomer"), customer.Id), customer);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Customers.Customers.Deleted"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                NotificationService.ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = customer.Id });
            }
        }

        public virtual async Task<IActionResult> GdprExport(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = await CustomerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return RedirectToAction("List");

            try
            {
                //log
                //GdprService.InsertLog(customer, 0, GdprRequestType.ExportData, await LocalizationService.GetResource("Gdpr.Exported"));
                //export
                //export
                var store = await StoreContext.GetCurrentStoreAsync();
                var bytes = await ExportManager.ExportCustomerGdprInfoToXlsxAsync(customer, store.Id);

                return File(bytes, MimeTypes.TextXlsx, $"customerdata-{customer.Id}.xlsx");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("Edit", new { id = customer.Id });
            }
        }
        #endregion

        #region Export / Import

        [HttpPost, ActionName("ExportExcel")]
        [FormValueRequired("exportexcel-all")]
        public virtual async Task<IActionResult> ExportExcelAll(CustomerSearchModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customers = await CustomerService.GetAllCustomersAsync(customerRoleIds: model.SelectedCustomerRoleIds.ToArray(),
                email: model.SearchEmail,
                username: model.SearchUsername,
                firstName: model.SearchFirstName,
                lastName: model.SearchLastName,
                dayOfBirth: int.TryParse(model.SearchDayOfBirth, out var dayOfBirth) ? dayOfBirth : 0,
                monthOfBirth: int.TryParse(model.SearchMonthOfBirth, out var monthOfBirth) ? monthOfBirth : 0,
                company: model.SearchCompany,
                phone: model.SearchPhone,
                zipPostalCode: model.SearchZipPostalCode);

            try
            {
                var bytes = await ExportManager.ExportCustomersToXlsxAsync(customers);
                return File(bytes, MimeTypes.TextXlsx, "customers.xlsx");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ExportExcelSelected(string selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customers = new List<Customer>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                customers.AddRange(await CustomerService.GetCustomersByIdsAsync(ids));
            }

            try
            {
                var bytes = await ExportManager.ExportCustomersToXlsxAsync(customers);
                return File(bytes, MimeTypes.TextXlsx, "customers.xlsx");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost, ActionName("ExportXML")]
        [FormValueRequired("exportxml-all")]
        public virtual async Task<IActionResult> ExportXmlAll(CustomerSearchModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customers = await CustomerService.GetAllCustomersAsync(customerRoleIds: model.SelectedCustomerRoleIds.ToArray(),
                email: model.SearchEmail,
                username: model.SearchUsername,
                firstName: model.SearchFirstName,
                lastName: model.SearchLastName,
                dayOfBirth: int.TryParse(model.SearchDayOfBirth, out var dayOfBirth) ? dayOfBirth : 0,
                monthOfBirth: int.TryParse(model.SearchMonthOfBirth, out var monthOfBirth) ? monthOfBirth : 0,
                company: model.SearchCompany,
                phone: model.SearchPhone,
                zipPostalCode: model.SearchZipPostalCode);

            try
            {
                var xml = await ExportManager.ExportCustomersToXmlAsync(customers);
                return File(Encoding.UTF8.GetBytes(xml), "application/xml", "customers.xml");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ExportXmlSelected(string selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customers = new List<Customer>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                customers.AddRange(await CustomerService.GetCustomersByIdsAsync(ids));
            }

            try
            {
                var xml = await ExportManager.ExportCustomersToXmlAsync(customers);
                return File(Encoding.UTF8.GetBytes(xml), "application/xml", "customers.xml");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        #endregion
    }
}