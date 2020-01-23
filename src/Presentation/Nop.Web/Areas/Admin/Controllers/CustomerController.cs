using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Defaults;
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

        private readonly CustomerSettings _customerSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly ForumSettings _forumSettings;
        private readonly GdprSettings _gdprSettings;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressService _addressService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly ICustomerModelFactory _customerModelFactory;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IExportManager _exportManager;
        private readonly IForumService _forumService;
        private readonly IGdprService _gdprService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ITaxService _taxService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly TaxSettings _taxSettings;

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
            _customerSettings = customerSettings;
            _dateTimeSettings = dateTimeSettings;
            _emailAccountSettings = emailAccountSettings;
            _forumSettings = forumSettings;
            _gdprSettings = gdprSettings;
            _addressAttributeParser = addressAttributeParser;
            _addressService = addressService;
            _customerActivityService = customerActivityService;
            _customerAttributeParser = customerAttributeParser;
            _customerAttributeService = customerAttributeService;
            _customerModelFactory = customerModelFactory;
            _customerRegistrationService = customerRegistrationService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _emailAccountService = emailAccountService;
            _exportManager = exportManager;
            _forumService = forumService;
            _gdprService = gdprService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _queuedEmailService = queuedEmailService;
            _rewardPointService = rewardPointService;
            _storeContext = storeContext;
            _storeService = storeService;
            _taxService = taxService;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _taxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        protected virtual string ValidateCustomerRoles(IList<CustomerRole> customerRoles, IList<CustomerRole> existingCustomerRoles)
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
                if (!_permissionService.Authorize(StandardPermissionProvider.ManageAcl))
                    return _localizationService.GetResource("Admin.Customers.Customers.CustomerRolesManagingError");
            }

            //ensure a customer is not added to both 'Guests' and 'Registered' customer roles
            //ensure that a customer is in at least one required role ('Guests' and 'Registered')
            var isInGuestsRole = customerRoles.FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.GuestsRoleName) != null;
            var isInRegisteredRole = customerRoles.FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.RegisteredRoleName) != null;
            if (isInGuestsRole && isInRegisteredRole)
                return _localizationService.GetResource("Admin.Customers.Customers.GuestsAndRegisteredRolesError");
            if (!isInGuestsRole && !isInRegisteredRole)
                return _localizationService.GetResource("Admin.Customers.Customers.AddCustomerToGuestsOrRegisteredRoleError");

            //no errors
            return string.Empty;
        }

        protected virtual string ParseCustomCustomerAttributes(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = string.Empty;
            var customerAttributes = _customerAttributeService.GetAllCustomerAttributes();
            foreach (var attribute in customerAttributes)
            {
                var controlId = $"{NopAttributePrefixDefaults.Customer}{attribute.Id}";
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
                                attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
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
                                    attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }

                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        //load read-only (already server-side selected) values
                        var attributeValues = _customerAttributeService.GetCustomerAttributeValues(attribute.Id);
                        foreach (var selectedAttributeId in attributeValues
                            .Where(v => v.IsPreSelected)
                            .Select(v => v.Id)
                            .ToList())
                        {
                            attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                attribute, selectedAttributeId.ToString());
                        }

                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        ctrlAttributes = form[controlId];
                        if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                        {
                            var enteredText = ctrlAttributes.ToString().Trim();
                            attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
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

        private bool SecondAdminAccountExists(Customer customer)
        {
            var customers = _customerService.GetAllCustomers(customerRoleIds: new[] { _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.AdministratorsRoleName).Id });

            return customers.Any(c => c.Active && c.Id != customer.Id);
        }

        #endregion

        #region Customers

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //prepare model
            var model = _customerModelFactory.PrepareCustomerSearchModel(new CustomerSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult CustomerList(CustomerSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _customerModelFactory.PrepareCustomerListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //prepare model
            var model = _customerModelFactory.PrepareCustomerModel(new CustomerModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Create(CustomerModel model, bool continueEditing, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            if (!string.IsNullOrWhiteSpace(model.Email) && _customerService.GetCustomerByEmail(model.Email) != null)
                ModelState.AddModelError(string.Empty, "Email is already registered");

            if (!string.IsNullOrWhiteSpace(model.Username) && _customerSettings.UsernamesEnabled &&
                _customerService.GetCustomerByUsername(model.Username) != null)
            {
                ModelState.AddModelError(string.Empty, "Username is already registered");
            }

            //validate customer roles
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            var newCustomerRoles = new List<CustomerRole>();
            foreach (var customerRole in allCustomerRoles)
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                    newCustomerRoles.Add(customerRole);
            var customerRolesError = ValidateCustomerRoles(newCustomerRoles, new List<CustomerRole>());
            if (!string.IsNullOrEmpty(customerRolesError))
            {
                ModelState.AddModelError(string.Empty, customerRolesError);
                _notificationService.ErrorNotification(customerRolesError);
            }

            // Ensure that valid email address is entered if Registered role is checked to avoid registered customers with empty email address
            if (newCustomerRoles.Any() && newCustomerRoles.FirstOrDefault(c => c.SystemName == NopCustomerDefaults.RegisteredRoleName) != null &&
                !CommonHelper.IsValidEmail(model.Email))
            {
                ModelState.AddModelError(string.Empty, _localizationService.GetResource("Admin.Customers.Customers.ValidEmailRequiredRegisteredRole"));

                _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.ValidEmailRequiredRegisteredRole"));
            }

            //custom customer attributes
            var customerAttributesXml = ParseCustomCustomerAttributes(form);
            if (newCustomerRoles.Any() && newCustomerRoles.FirstOrDefault(c => c.SystemName == NopCustomerDefaults.RegisteredRoleName) != null)
            {
                var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
                foreach (var error in customerAttributeWarnings)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            if (ModelState.IsValid)
            {
                //fill entity from model
                var customer = model.ToEntity<Customer>();

                customer.CustomerGuid = Guid.NewGuid();
                customer.CreatedOnUtc = DateTime.UtcNow;
                customer.LastActivityDateUtc = DateTime.UtcNow;
                customer.RegisteredInStoreId = _storeContext.CurrentStore.Id;

                _customerService.InsertCustomer(customer);

                //form fields
                if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.TimeZoneIdAttribute, model.TimeZoneId);
                if (_customerSettings.GenderEnabled)
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.GenderAttribute, model.Gender);
                if (_customerSettings.FirstNameEnabled)
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FirstNameAttribute, model.FirstName);
                if (_customerSettings.LastNameEnabled)
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.LastNameAttribute, model.LastName);
                if (_customerSettings.DateOfBirthEnabled)
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.DateOfBirthAttribute, model.DateOfBirth);
                if (_customerSettings.CompanyEnabled)
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CompanyAttribute, model.Company);
                if (_customerSettings.StreetAddressEnabled)
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddressAttribute, model.StreetAddress);
                if (_customerSettings.StreetAddress2Enabled)
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddress2Attribute, model.StreetAddress2);
                if (_customerSettings.ZipPostalCodeEnabled)
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.ZipPostalCodeAttribute, model.ZipPostalCode);
                if (_customerSettings.CityEnabled)
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CityAttribute, model.City);
                if (_customerSettings.CountyEnabled)
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CountyAttribute, model.County);
                if (_customerSettings.CountryEnabled)
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CountryIdAttribute, model.CountryId);
                if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StateProvinceIdAttribute, model.StateProvinceId);
                if (_customerSettings.PhoneEnabled)
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PhoneAttribute, model.Phone);
                if (_customerSettings.FaxEnabled)
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FaxAttribute, model.Fax);

                //custom customer attributes
                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CustomCustomerAttributes, customerAttributesXml);

                //newsletter subscriptions
                if (!string.IsNullOrEmpty(customer.Email))
                {
                    var allStores = _storeService.GetAllStores();
                    foreach (var store in allStores)
                    {
                        var newsletterSubscription = _newsLetterSubscriptionService
                            .GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, store.Id);
                        if (model.SelectedNewsletterSubscriptionStoreIds != null &&
                            model.SelectedNewsletterSubscriptionStoreIds.Contains(store.Id))
                        {
                            //subscribed
                            if (newsletterSubscription == null)
                            {
                                _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
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
                                _newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletterSubscription);
                            }
                        }
                    }
                }

                //password
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    var changePassRequest = new ChangePasswordRequest(model.Email, false, _customerSettings.DefaultPasswordFormat, model.Password);
                    var changePassResult = _customerRegistrationService.ChangePassword(changePassRequest);
                    if (!changePassResult.Success)
                    {
                        foreach (var changePassError in changePassResult.Errors)
                            _notificationService.ErrorNotification(changePassError);
                    }
                }

                //customer roles
                foreach (var customerRole in newCustomerRoles)
                {
                    //ensure that the current customer cannot add to "Administrators" system role if he's not an admin himself
                    if (customerRole.SystemName == NopCustomerDefaults.AdministratorsRoleName && !_customerService.IsAdmin(_workContext.CurrentCustomer))
                        continue;

                    _customerService.AddCustomerRoleMapping(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = customerRole.Id });
                }

                _customerService.UpdateCustomer(customer);

                //ensure that a customer with a vendor associated is not in "Administrators" role
                //otherwise, he won't have access to other functionality in admin area
                if (_customerService.IsAdmin(customer) && customer.VendorId > 0)
                {
                    customer.VendorId = 0;
                    _customerService.UpdateCustomer(customer);

                    _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.AdminCouldNotbeVendor"));
                }

                //ensure that a customer in the Vendors role has a vendor account associated.
                //otherwise, he will have access to ALL products
                if (_customerService.IsVendor(customer) && customer.VendorId == 0)
                {
                    var vendorRole = _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.VendorsRoleName);
                    _customerService.RemoveCustomerRoleMapping(customer, vendorRole);

                    _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.CannotBeInVendoRoleWithoutVendorAssociated"));
                }

                //activity log
                _customerActivityService.InsertActivity("AddNewCustomer",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewCustomer"), customer.Id), customer);
                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = customer.Id });
            }

            //prepare model
            model = _customerModelFactory.PrepareCustomerModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(id);
            if (customer == null || customer.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = _customerModelFactory.PrepareCustomerModel(null, customer);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Edit(CustomerModel model, bool continueEditing, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(model.Id);
            if (customer == null || customer.Deleted)
                return RedirectToAction("List");

            //validate customer roles
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            var newCustomerRoles = new List<CustomerRole>();
            foreach (var customerRole in allCustomerRoles)
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                    newCustomerRoles.Add(customerRole);

            var customerRolesError = ValidateCustomerRoles(newCustomerRoles, _customerService.GetCustomerRoles(customer));

            if (!string.IsNullOrEmpty(customerRolesError))
            {
                ModelState.AddModelError(string.Empty, customerRolesError);
                _notificationService.ErrorNotification(customerRolesError);
            }

            // Ensure that valid email address is entered if Registered role is checked to avoid registered customers with empty email address
            if (newCustomerRoles.Any() && newCustomerRoles.FirstOrDefault(c => c.SystemName == NopCustomerDefaults.RegisteredRoleName) != null &&
                !CommonHelper.IsValidEmail(model.Email))
            {
                ModelState.AddModelError(string.Empty, _localizationService.GetResource("Admin.Customers.Customers.ValidEmailRequiredRegisteredRole"));
                _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.ValidEmailRequiredRegisteredRole"));
            }

            //custom customer attributes
            var customerAttributesXml = ParseCustomCustomerAttributes(form);
            if (newCustomerRoles.Any() && newCustomerRoles.FirstOrDefault(c => c.SystemName == NopCustomerDefaults.RegisteredRoleName) != null)
            {
                var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
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
                    if (!_customerService.IsAdmin(customer) || model.Active || SecondAdminAccountExists(customer))
                        customer.Active = model.Active;
                    else
                        _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.AdminAccountShouldExists.Deactivate"));

                    //email
                    if (!string.IsNullOrWhiteSpace(model.Email))
                        _customerRegistrationService.SetEmail(customer, model.Email, false);
                    else
                        customer.Email = model.Email;

                    //username
                    if (_customerSettings.UsernamesEnabled)
                    {
                        if (!string.IsNullOrWhiteSpace(model.Username))
                            _customerRegistrationService.SetUsername(customer, model.Username);
                        else
                            customer.Username = model.Username;
                    }

                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        var prevVatNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.VatNumberAttribute);

                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.VatNumberAttribute, model.VatNumber);
                        //set VAT number status
                        if (!string.IsNullOrEmpty(model.VatNumber))
                        {
                            if (!model.VatNumber.Equals(prevVatNumber, StringComparison.InvariantCultureIgnoreCase))
                            {
                                _genericAttributeService.SaveAttribute(customer,
                                    NopCustomerDefaults.VatNumberStatusIdAttribute,
                                    (int)_taxService.GetVatNumberStatus(model.VatNumber));
                            }
                        }
                        else
                        {
                            _genericAttributeService.SaveAttribute(customer,
                                NopCustomerDefaults.VatNumberStatusIdAttribute,
                                (int)VatNumberStatus.Empty);
                        }
                    }

                    //vendor
                    customer.VendorId = model.VendorId;

                    //form fields
                    if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.TimeZoneIdAttribute, model.TimeZoneId);
                    if (_customerSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.GenderAttribute, model.Gender);
                    if (_customerSettings.FirstNameEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FirstNameAttribute, model.FirstName);
                    if (_customerSettings.LastNameEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.LastNameAttribute, model.LastName);
                    if (_customerSettings.DateOfBirthEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.DateOfBirthAttribute, model.DateOfBirth);
                    if (_customerSettings.CompanyEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CompanyAttribute, model.Company);
                    if (_customerSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddressAttribute, model.StreetAddress);
                    if (_customerSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddress2Attribute, model.StreetAddress2);
                    if (_customerSettings.ZipPostalCodeEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.ZipPostalCodeAttribute, model.ZipPostalCode);
                    if (_customerSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CityAttribute, model.City);
                    if (_customerSettings.CountyEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CountyAttribute, model.County);
                    if (_customerSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CountryIdAttribute, model.CountryId);
                    if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StateProvinceIdAttribute, model.StateProvinceId);
                    if (_customerSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PhoneAttribute, model.Phone);
                    if (_customerSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FaxAttribute, model.Fax);

                    //custom customer attributes
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CustomCustomerAttributes, customerAttributesXml);

                    //newsletter subscriptions
                    if (!string.IsNullOrEmpty(customer.Email))
                    {
                        var allStores = _storeService.GetAllStores();
                        foreach (var store in allStores)
                        {
                            var newsletterSubscription = _newsLetterSubscriptionService
                                .GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, store.Id);
                            if (model.SelectedNewsletterSubscriptionStoreIds != null &&
                                model.SelectedNewsletterSubscriptionStoreIds.Contains(store.Id))
                            {
                                //subscribed
                                if (newsletterSubscription == null)
                                {
                                    _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
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
                                    _newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletterSubscription);
                                }
                            }
                        }
                    }

                    //customer roles
                    foreach (var customerRole in allCustomerRoles)
                    {
                        //ensure that the current customer cannot add/remove to/from "Administrators" system role
                        //if he's not an admin himself
                        if (customerRole.SystemName == NopCustomerDefaults.AdministratorsRoleName &&
                            !_customerService.IsAdmin(_workContext.CurrentCustomer))
                            continue;

                        if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                        {
                            //new role
                            if (!_customerService.IsInCustomerRole(customer, customerRole.SystemName))
                                _customerService.AddCustomerRoleMapping(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = customerRole.Id });
                        }
                        else
                        {
                            //prevent attempts to delete the administrator role from the user, if the user is the last active administrator
                            if (customerRole.SystemName == NopCustomerDefaults.AdministratorsRoleName && !SecondAdminAccountExists(customer))
                            {
                                _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.AdminAccountShouldExists.DeleteRole"));
                                continue;
                            }

                            //remove role
                            if (_customerService.IsInCustomerRole(customer, customerRole.SystemName))
                            {
                                _customerService.RemoveCustomerRoleMapping(customer, customerRole);
                            }
                        }
                    }

                    _customerService.UpdateCustomer(customer);

                    //ensure that a customer with a vendor associated is not in "Administrators" role
                    //otherwise, he won't have access to the other functionality in admin area
                    if (_customerService.IsAdmin(customer) && customer.VendorId > 0)
                    {
                        customer.VendorId = 0;
                        _customerService.UpdateCustomer(customer);
                        _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.AdminCouldNotbeVendor"));
                    }

                    //ensure that a customer in the Vendors role has a vendor account associated.
                    //otherwise, he will have access to ALL products
                    if (_customerService.IsVendor(customer) && customer.VendorId == 0)
                    {
                        var vendorRole = _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.VendorsRoleName);
                        _customerService.RemoveCustomerRoleMapping(customer, vendorRole);

                        _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.CannotBeInVendoRoleWithoutVendorAssociated"));
                    }

                    //activity log
                    _customerActivityService.InsertActivity("EditCustomer",
                        string.Format(_localizationService.GetResource("ActivityLog.EditCustomer"), customer.Id), customer);

                    _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Updated"));

                    if (!continueEditing)
                        return RedirectToAction("List");

                    return RedirectToAction("Edit", new { id = customer.Id });
                }
                catch (Exception exc)
                {
                    _notificationService.ErrorNotification(exc.Message);
                }
            }

            //prepare model
            model = _customerModelFactory.PrepareCustomerModel(model, customer, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("changepassword")]
        public virtual IActionResult ChangePassword(CustomerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(model.Id);
            if (customer == null)
                return RedirectToAction("List");

            //ensure that the current customer cannot change passwords of "Administrators" if he's not an admin himself
            if (_customerService.IsAdmin(customer) && !_customerService.IsAdmin(_workContext.CurrentCustomer))
            {
                _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.OnlyAdminCanChangePassword"));
                return RedirectToAction("Edit", new { id = customer.Id });
            }

            if (!ModelState.IsValid)
                return RedirectToAction("Edit", new { id = customer.Id });

            var changePassRequest = new ChangePasswordRequest(model.Email,
                false, _customerSettings.DefaultPasswordFormat, model.Password);
            var changePassResult = _customerRegistrationService.ChangePassword(changePassRequest);
            if (changePassResult.Success)
                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.PasswordChanged"));
            else
                foreach (var error in changePassResult.Errors)
                    _notificationService.ErrorNotification(error);

            return RedirectToAction("Edit", new { id = customer.Id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("markVatNumberAsValid")]
        public virtual IActionResult MarkVatNumberAsValid(CustomerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(model.Id);
            if (customer == null)
                return RedirectToAction("List");

            _genericAttributeService.SaveAttribute(customer,
                NopCustomerDefaults.VatNumberStatusIdAttribute,
                (int)VatNumberStatus.Valid);

            return RedirectToAction("Edit", new { id = customer.Id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("markVatNumberAsInvalid")]
        public virtual IActionResult MarkVatNumberAsInvalid(CustomerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(model.Id);
            if (customer == null)
                return RedirectToAction("List");

            _genericAttributeService.SaveAttribute(customer,
                NopCustomerDefaults.VatNumberStatusIdAttribute,
                (int)VatNumberStatus.Invalid);

            return RedirectToAction("Edit", new { id = customer.Id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("remove-affiliate")]
        public virtual IActionResult RemoveAffiliate(CustomerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(model.Id);
            if (customer == null)
                return RedirectToAction("List");

            customer.AffiliateId = 0;
            _customerService.UpdateCustomer(customer);

            return RedirectToAction("Edit", new { id = customer.Id });
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(id);
            if (customer == null)
                return RedirectToAction("List");

            try
            {
                //prevent attempts to delete the user, if it is the last active administrator
                if (_customerService.IsAdmin(customer) && !SecondAdminAccountExists(customer))
                {
                    _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.AdminAccountShouldExists.DeleteAdministrator"));
                    return RedirectToAction("Edit", new { id = customer.Id });
                }

                //ensure that the current customer cannot delete "Administrators" if he's not an admin himself
                if (_customerService.IsAdmin(customer) && !_customerService.IsAdmin(_workContext.CurrentCustomer))
                {
                    _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.OnlyAdminCanDeleteAdmin"));
                    return RedirectToAction("Edit", new { id = customer.Id });
                }

                //delete
                _customerService.DeleteCustomer(customer);

                //remove newsletter subscription (if exists)
                foreach (var store in _storeService.GetAllStores())
                {
                    var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, store.Id);
                    if (subscription != null)
                        _newsLetterSubscriptionService.DeleteNewsLetterSubscription(subscription);
                }

                //activity log
                _customerActivityService.InsertActivity("DeleteCustomer",
                    string.Format(_localizationService.GetResource("ActivityLog.DeleteCustomer"), customer.Id), customer);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Deleted"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = customer.Id });
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("impersonate")]
        public virtual IActionResult Impersonate(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AllowCustomerImpersonation))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(id);
            if (customer == null)
                return RedirectToAction("List");

            if (!customer.Active)
            {
                _notificationService.WarningNotification(
                    _localizationService.GetResource("Admin.Customers.Customers.Impersonate.Inactive"));
                return RedirectToAction("Edit", customer.Id);
            }

            //ensure that a non-admin user cannot impersonate as an administrator
            //otherwise, that user can simply impersonate as an administrator and gain additional administrative privileges
            if (!_customerService.IsAdmin(_workContext.CurrentCustomer) && _customerService.IsAdmin(customer))
            {
                _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.NonAdminNotImpersonateAsAdminError"));
                return RedirectToAction("Edit", customer.Id);
            }

            //activity log
            _customerActivityService.InsertActivity("Impersonation.Started",
                string.Format(_localizationService.GetResource("ActivityLog.Impersonation.Started.StoreOwner"), customer.Email, customer.Id), customer);
            _customerActivityService.InsertActivity(customer, "Impersonation.Started",
                string.Format(_localizationService.GetResource("ActivityLog.Impersonation.Started.Customer"), _workContext.CurrentCustomer.Email, _workContext.CurrentCustomer.Id), _workContext.CurrentCustomer);

            //ensure login is not required
            customer.RequireReLogin = false;
            _customerService.UpdateCustomer(customer);
            _genericAttributeService.SaveAttribute<int?>(_workContext.CurrentCustomer, NopCustomerDefaults.ImpersonatedCustomerIdAttribute, customer.Id);

            return RedirectToAction("Index", "Home", new { area = string.Empty });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("send-welcome-message")]
        public virtual IActionResult SendWelcomeMessage(CustomerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(model.Id);
            if (customer == null)
                return RedirectToAction("List");

            _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.SendWelcomeMessage.Success"));

            return RedirectToAction("Edit", new { id = customer.Id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("resend-activation-message")]
        public virtual IActionResult ReSendActivationMessage(CustomerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(model.Id);
            if (customer == null)
                return RedirectToAction("List");

            //email validation message
            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
            _workflowMessageService.SendCustomerEmailValidationMessage(customer, _workContext.WorkingLanguage.Id);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.ReSendActivationMessage.Success"));

            return RedirectToAction("Edit", new { id = customer.Id });
        }

        public virtual IActionResult SendEmail(CustomerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(model.Id);
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

                var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
                if (emailAccount == null)
                    emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
                if (emailAccount == null)
                    throw new NopException("Email account can't be loaded");
                var email = new QueuedEmail
                {
                    Priority = QueuedEmailPriority.High,
                    EmailAccountId = emailAccount.Id,
                    FromName = emailAccount.DisplayName,
                    From = emailAccount.Email,
                    ToName = _customerService.GetCustomerFullName(customer),
                    To = customer.Email,
                    Subject = model.SendEmail.Subject,
                    Body = model.SendEmail.Body,
                    CreatedOnUtc = DateTime.UtcNow,
                    DontSendBeforeDateUtc = model.SendEmail.SendImmediately || !model.SendEmail.DontSendBeforeDate.HasValue ?
                        null : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.SendEmail.DontSendBeforeDate.Value)
                };
                _queuedEmailService.InsertQueuedEmail(email);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.SendEmail.Queued"));
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc.Message);
            }

            return RedirectToAction("Edit", new { id = customer.Id });
        }

        public virtual IActionResult SendPm(CustomerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(model.Id);
            if (customer == null)
                return RedirectToAction("List");

            try
            {
                if (!_forumSettings.AllowPrivateMessages)
                    throw new NopException("Private messages are disabled");
                if (_customerService.IsGuest(customer))
                    throw new NopException("Customer should be registered");
                if (string.IsNullOrWhiteSpace(model.SendPm.Subject))
                    throw new NopException("PM subject is empty");
                if (string.IsNullOrWhiteSpace(model.SendPm.Message))
                    throw new NopException("PM message is empty");

                var privateMessage = new PrivateMessage
                {
                    StoreId = _storeContext.CurrentStore.Id,
                    ToCustomerId = customer.Id,
                    FromCustomerId = _workContext.CurrentCustomer.Id,
                    Subject = model.SendPm.Subject,
                    Text = model.SendPm.Message,
                    IsDeletedByAuthor = false,
                    IsDeletedByRecipient = false,
                    IsRead = false,
                    CreatedOnUtc = DateTime.UtcNow
                };

                _forumService.InsertPrivateMessage(privateMessage);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.SendPM.Sent"));
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc.Message);
            }

            return RedirectToAction("Edit", new { id = customer.Id });
        }

        #endregion

        #region Reward points history

        [HttpPost]
        public virtual IActionResult RewardPointsHistorySelect(CustomerRewardPointsSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedDataTablesJson();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(searchModel.CustomerId)
                ?? throw new ArgumentException("No customer found with the specified id");

            //prepare model
            var model = _customerModelFactory.PrepareRewardPointsListModel(searchModel, customer);

            return Json(model);
        }

        public virtual IActionResult RewardPointsHistoryAdd(AddRewardPointsToCustomerModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //prevent adding a new row with zero value
            if (model.Points == 0)
                return ErrorJson(_localizationService.GetResource("Admin.Customers.Customers.RewardPoints.AddingZeroValueNotAllowed"));

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(model.CustomerId);
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
            _rewardPointService.AddRewardPointsHistoryEntry(customer, model.Points, model.StoreId, model.Message,
                activatingDate: activatingDate, endDate: endDate);

            return Json(new { Result = true });
        }

        #endregion

        #region Addresses

        [HttpPost]
        public virtual IActionResult AddressesSelect(CustomerAddressSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedDataTablesJson();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(searchModel.CustomerId)
                ?? throw new ArgumentException("No customer found with the specified id");

            //prepare model
            var model = _customerModelFactory.PrepareCustomerAddressListModel(searchModel, customer);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult AddressDelete(int id, int customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(customerId)
                ?? throw new ArgumentException("No customer found with the specified id", nameof(customerId));

            //try to get an address with the specified id
            var address = _customerService.GetCustomerAddress(customer.Id, id);            

            if (address == null)
                return Content("No address found with the specified id");

            _customerService.RemoveCustomerAddress(customer, address);
            _customerService.UpdateCustomer(customer);

            //now delete the address record
            _addressService.DeleteAddress(address);

            return new NullJsonResult();
        }

        public virtual IActionResult AddressCreate(int customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                return RedirectToAction("List");

            //prepare model
            var model = _customerModelFactory.PrepareCustomerAddressModel(new CustomerAddressModel(), customer, null);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult AddressCreate(CustomerAddressModel model, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(model.CustomerId);
            if (customer == null)
                return RedirectToAction("List");

            //custom address attributes
            var customAttributes = _addressAttributeParser.ParseCustomAddressAttributes(form);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
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

                _addressService.InsertAddress(address);

                _customerService.InsertCustomerAddress(customer, address);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Addresses.Added"));

                return RedirectToAction("AddressEdit", new { addressId = address.Id, customerId = model.CustomerId });
            }

            //prepare model
            model = _customerModelFactory.PrepareCustomerAddressModel(model, customer, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult AddressEdit(int addressId, int customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                return RedirectToAction("List");

            //try to get an address with the specified id
            var address = _addressService.GetAddressById(addressId);
            if (address == null)
                return RedirectToAction("Edit", new { id = customer.Id });

            //prepare model
            var model = _customerModelFactory.PrepareCustomerAddressModel(null, customer, address);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult AddressEdit(CustomerAddressModel model, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(model.CustomerId);
            if (customer == null)
                return RedirectToAction("List");

            //try to get an address with the specified id
            var address = _addressService.GetAddressById(model.Address.Id);
            if (address == null)
                return RedirectToAction("Edit", new { id = customer.Id });

            //custom address attributes
            var customAttributes = _addressAttributeParser.ParseCustomAddressAttributes(form);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                address.CustomAttributes = customAttributes;
                _addressService.UpdateAddress(address);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Addresses.Updated"));

                return RedirectToAction("AddressEdit", new { addressId = model.Address.Id, customerId = model.CustomerId });
            }

            //prepare model
            model = _customerModelFactory.PrepareCustomerAddressModel(model, customer, address, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Orders

        [HttpPost]
        public virtual IActionResult OrderList(CustomerOrderSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedDataTablesJson();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(searchModel.CustomerId)
                ?? throw new ArgumentException("No customer found with the specified id");

            //prepare model
            var model = _customerModelFactory.PrepareCustomerOrderListModel(searchModel, customer);

            return Json(model);
        }

        #endregion

        #region Customer

        public virtual IActionResult LoadCustomerStatistics(string period)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return Content(string.Empty);

            var result = new List<object>();

            var nowDt = _dateTimeHelper.ConvertToUserTime(DateTime.Now);
            var timeZone = _dateTimeHelper.CurrentTimeZone;
            var searchCustomerRoleIds = new[] { _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.RegisteredRoleName).Id };

            var culture = new CultureInfo(_workContext.WorkingLanguage.LanguageCulture);

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
                            value = _customerService.GetAllCustomers(
                                createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser, timeZone),
                                createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser.AddMonths(1), timeZone),
                                customerRoleIds: searchCustomerRoleIds,
                                pageIndex: 0,
                                pageSize: 1, getOnlyTotalCount: true).TotalCount.ToString()
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
                            value = _customerService.GetAllCustomers(
                                createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser, timeZone),
                                createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser.AddDays(1), timeZone),
                                customerRoleIds: searchCustomerRoleIds,
                                pageIndex: 0,
                                pageSize: 1, getOnlyTotalCount: true).TotalCount.ToString()
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
                            value = _customerService.GetAllCustomers(
                                createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser, timeZone),
                                createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser.AddDays(1), timeZone),
                                customerRoleIds: searchCustomerRoleIds,
                                pageIndex: 0,
                                pageSize: 1, getOnlyTotalCount: true).TotalCount.ToString()
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
        public virtual IActionResult GetCartList(CustomerShoppingCartSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedDataTablesJson();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(searchModel.CustomerId)
                ?? throw new ArgumentException("No customer found with the specified id");

            //prepare model
            var model = _customerModelFactory.PrepareCustomerShoppingCartListModel(searchModel, customer);

            return Json(model);
        }

        #endregion

        #region Activity log

        [HttpPost]
        public virtual IActionResult ListActivityLog(CustomerActivityLogSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedDataTablesJson();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(searchModel.CustomerId)
                ?? throw new ArgumentException("No customer found with the specified id");

            //prepare model
            var model = _customerModelFactory.PrepareCustomerActivityLogListModel(searchModel, customer);

            return Json(model);
        }

        #endregion

        #region Back in stock subscriptions

        [HttpPost]
        public virtual IActionResult BackInStockSubscriptionList(CustomerBackInStockSubscriptionSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedDataTablesJson();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(searchModel.CustomerId)
                ?? throw new ArgumentException("No customer found with the specified id");

            //prepare model
            var model = _customerModelFactory.PrepareCustomerBackInStockSubscriptionListModel(searchModel, customer);

            return Json(model);
        }

        #endregion

        #region GDPR

        public virtual IActionResult GdprLog()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //prepare model
            var model = _customerModelFactory.PrepareGdprLogSearchModel(new GdprLogSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult GdprLogList(GdprLogSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _customerModelFactory.PrepareGdprLogListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult GdprDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(id);
            if (customer == null)
                return RedirectToAction("List");

            if (!_gdprSettings.GdprEnabled)
                return RedirectToAction("List");

            try
            {
                //prevent attempts to delete the user, if it is the last active administrator
                if (_customerService.IsAdmin(customer) && !SecondAdminAccountExists(customer))
                {
                    _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.AdminAccountShouldExists.DeleteAdministrator"));
                    return RedirectToAction("Edit", new { id = customer.Id });
                }

                //ensure that the current customer cannot delete "Administrators" if he's not an admin himself
                if (_customerService.IsAdmin(customer) && !_customerService.IsAdmin(_workContext.CurrentCustomer))
                {
                    _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.OnlyAdminCanDeleteAdmin"));
                    return RedirectToAction("Edit", new { id = customer.Id });
                }

                //delete
                _gdprService.PermanentDeleteCustomer(customer);

                //activity log
                _customerActivityService.InsertActivity("DeleteCustomer",
                    string.Format(_localizationService.GetResource("ActivityLog.DeleteCustomer"), customer.Id), customer);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Deleted"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = customer.Id });
            }
        }

        public virtual IActionResult GdprExport(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(id);
            if (customer == null)
                return RedirectToAction("List");

            try
            {
                //log
                //_gdprService.InsertLog(customer, 0, GdprRequestType.ExportData, _localizationService.GetResource("Gdpr.Exported"));
                //export
                //export
                var bytes = _exportManager.ExportCustomerGdprInfoToXlsx(customer, _storeContext.CurrentStore.Id);

                return File(bytes, MimeTypes.TextXlsx, $"customerdata-{customer.Id}.xlsx");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = customer.Id });
            }
        }
        #endregion

        #region Export / Import

        [HttpPost, ActionName("ExportExcel")]
        [FormValueRequired("exportexcel-all")]
        public virtual IActionResult ExportExcelAll(CustomerSearchModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customers = _customerService.GetAllCustomers(customerRoleIds: model.SelectedCustomerRoleIds.ToArray(),
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
                var bytes = _exportManager.ExportCustomersToXlsx(customers);
                return File(bytes, MimeTypes.TextXlsx, "customers.xlsx");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual IActionResult ExportExcelSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customers = new List<Customer>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                customers.AddRange(_customerService.GetCustomersByIds(ids));
            }

            try
            {
                var bytes = _exportManager.ExportCustomersToXlsx(customers);
                return File(bytes, MimeTypes.TextXlsx, "customers.xlsx");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost, ActionName("ExportXML")]
        [FormValueRequired("exportxml-all")]
        public virtual IActionResult ExportXmlAll(CustomerSearchModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customers = _customerService.GetAllCustomers(customerRoleIds: model.SelectedCustomerRoleIds.ToArray(),
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
                var xml = _exportManager.ExportCustomersToXml(customers);
                return File(Encoding.UTF8.GetBytes(xml), "application/xml", "customers.xml");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual IActionResult ExportXmlSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customers = new List<Customer>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                customers.AddRange(_customerService.GetCustomersByIds(ids));
            }

            var xml = _exportManager.ExportCustomersToXml(customers);
            return File(Encoding.UTF8.GetBytes(xml), "application/xml", "customers.xml");
        }

        #endregion
    }
}