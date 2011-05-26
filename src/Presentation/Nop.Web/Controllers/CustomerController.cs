using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Extensions;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models;
using Nop.Web.Models.Common;
using Nop.Web.Models.Customer;
using Nop.Services.Media;

namespace Nop.Web.Controllers
{
    public class CustomerController : BaseNopController
    {
        #region Fields

        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;
        private readonly UserSettings _userSettings;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly TaxSettings _taxSettings;
        private readonly FormFieldSettings _formFieldSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly ITaxService _taxService;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly ForumSettings _forumSettings;
        private readonly OrderSettings _orderSettings;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPictureService _pictureService;
        private readonly MediaSettings _mediaSettings;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        public CustomerController(IAuthenticationService authenticationService,
            IUserService userService, UserSettings userSettings, IDateTimeHelper dateTimeHelper,
            DateTimeSettings dateTimeSettings, TaxSettings taxSettings,
            FormFieldSettings formFieldSettings, ILocalizationService localizationService,
            IWorkContext workContext, ICustomerService customerService,
            ITaxService taxService, RewardPointsSettings rewardPointsSettings,
            CustomerSettings customerSettings, ForumSettings forumSettings,
            OrderSettings orderSettings, IAddressService addressService,
            ICountryService countryService, IStateProvinceService stateProvinceService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IOrderProcessingService orderProcessingService, IOrderService orderService,
            ICurrencyService currencyService, IPriceFormatter priceFormatter,
            IPictureService pictureService, MediaSettings mediaSettings,
            IWorkflowMessageService workflowMessageService, LocalizationSettings localizationSettings)
        {
            this._authenticationService = authenticationService;
            this._userService = userService;
            this._userSettings = userSettings;
            this._dateTimeHelper = dateTimeHelper;
            this._dateTimeSettings = dateTimeSettings;
            this._taxSettings = taxSettings;
            this._formFieldSettings = formFieldSettings;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._customerService = customerService;
            this._taxService = taxService;
            this._rewardPointsSettings = rewardPointsSettings;
            this._customerSettings = customerSettings;
            this._forumSettings = forumSettings;
            this._orderSettings = orderSettings;
            this._addressService = addressService;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._orderProcessingService = orderProcessingService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._orderService = orderService;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._pictureService = pictureService;
            this._mediaSettings = mediaSettings;
            this._workflowMessageService = workflowMessageService;
            this._localizationSettings = localizationSettings;
        }

        #endregion

        #region Utilities

        [NonAction]
        private bool IsCurrentUserRegistered()
        {
            return _workContext.CurrentCustomer != null &&
                _workContext.CurrentCustomer.IsRegistered() &&
                _authenticationService.GetAuthenticatedUser() != null;
        }

        [NonAction]
        private CustomerNavigationModel GetCustomerNavigationModel(Customer customer)
        {
            var model = new CustomerNavigationModel();
            model.HideAvatar = !_customerSettings.AllowCustomersToUploadAvatars;
            model.HideRewardPoints = !_rewardPointsSettings.Enabled;
            model.HideForumSubscriptions = !_forumSettings.ForumsEnabled || !_forumSettings.AllowCustomersToManageSubscriptions;
            model.HideReturnRequests = !_orderSettings.ReturnRequestsEnabled || _orderService.SearchReturnRequests(customer.Id, 0, null).Count == 0;
            model.HideDownloadableProducts = _customerSettings.HideDownloadableProductsTab;
            return model;
        }

        #endregion

        #region Login / logout / register

        public ActionResult Login()
        {
            var model = new LoginModel();
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (_userService.ValidateUser(_userSettings.UsernamesEnabled ? model.UserName : model.Email, model.Password))
                {
                    //TODO migrate shopping cart

                    var user = _userSettings.UsernamesEnabled ? _userService.GetUserByUsername(model.UserName) : _userService.GetUserByEmail(model.Email);
                    _authenticationService.SignIn(user, model.RememberMe);

                    //UNDONE Return URL doesn't work
                    if (!String.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "The credentials provided is incorrect.");
                }
            }

            //If we got this far, something failed, redisplay form
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            return View(model);
        }

        public ActionResult Register()
        {
            //check whether registration is allowed
            if (_userSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToAction("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

            //TODO Get and save DateOfBirth (if enabled)
            //TODO mark register button as default one (pressing 'Enter' button should cause appropriate form submit)

            var model = new RegisterModel();
            model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == _dateTimeHelper.DefaultStoreTimeZone.Id) });
            model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            //form fields
            model.GenderEnabled = _formFieldSettings.GenderEnabled;
            model.CompanyEnabled = _formFieldSettings.CompanyEnabled;
            //model.DateOfBirthEnabled = _formFieldSettings.DateOfBirthEnabled;
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;

            return View(model);
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            //check whether registration is allowed
            if (_userSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToAction("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

            if (ModelState.IsValid)
            {
                //register 'User' entity
                //UNDONE set password format
                bool isApproved = _userSettings.UserRegistrationType == UserRegistrationType.Standard;
                var registrationRequest = new UserRegistrationRequest(model.Email,
                    model.Username, model.Password, PasswordFormat.Clear, "",
                    "", isApproved);
                var registrationResult = _userService.RegisterUser(registrationRequest);
                if (registrationResult.Success)
                {
                    if (_workContext.CurrentCustomer.IsRegistered())
                    {
                        //already registered customer. save a new record
                        _workContext.CurrentCustomer = _customerService.InsertGuestCustomer();
                    }

                    var customer = _workContext.CurrentCustomer;
                    //now register 'Customer' entity
                    _customerService.RegisterCustomer(customer);
                    //associate it with this user
                    customer.AssociatedUsers.Add(registrationResult.User);
                    //properties
                    if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                        customer.TimeZoneId = model.TimeZoneId;
                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        customer.VatNumber = model.VatNumber;
                        
                        string vatName = string.Empty;
                        string vatAddress = string.Empty;
                        customer.VatNumberStatus = _taxService.GetVatNumberStatus(customer.VatNumber, out vatName, out vatAddress);
                        //send VAT number admin notification
                        if (!String.IsNullOrEmpty(customer.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                            _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer, customer.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);

                    }
                    //save
                    _customerService.UpdateCustomer(customer);

                    //form fields
                    if (_formFieldSettings.GenderEnabled)
                        _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.Gender, model.Gender);
                    _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
                    _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
                    //if (_formFieldSettings.DateOfBirthEnabled)
                    //    _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.DateOfBirth, model.DateOfBirth);
                    if (_formFieldSettings.CompanyEnabled)
                        _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.Company, model.Company);


                    //TODO migrate shopping cart

                    //login customer now
                    if (isApproved)
                        _authenticationService.SignIn(registrationResult.User, true);

                    //notifications
                    //UNDONE perhaps, we need to load the latest user account (not default one) to appropriate email methods:
                    //1. SendCustomerRegisteredNotificationMessage
                    //2. SendCustomerEmailValidationMessage
                    //3. SendCustomerWelcomeMessage
                    if (_customerSettings.NotifyNewCustomerRegistration)
                        _workflowMessageService.SendCustomerRegisteredNotificationMessage(customer, _localizationSettings.DefaultAdminLanguageId);
                    if (_userSettings.UserRegistrationType == UserRegistrationType.EmailValidation)
                    {
                        _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.AccountActivationToken, Guid.NewGuid().ToString());
                        _workflowMessageService.SendCustomerEmailValidationMessage(customer,  _workContext.WorkingLanguage.Id);
                    }
                    else
                    {
                        _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);
                    }
                    switch (_userSettings.UserRegistrationType)
                    {
                        case UserRegistrationType.EmailValidation:
                            {
                                //UNDONE send email validation email to a user (not a customer)
                                return RedirectToAction("RegisterResult", new { resultId = (int)UserRegistrationType.EmailValidation });
                            }
                            break;
                        case UserRegistrationType.AdminApproval:
                            {
                                return RedirectToAction("RegisterResult", new { resultId = (int)UserRegistrationType.AdminApproval });
                            }
                            break;
                        case UserRegistrationType.Standard:
                            {
                                //TODO send SendCustomerWelcomeMessage
                                return RedirectToAction("RegisterResult", new { resultId = (int)UserRegistrationType.Standard });
                            }
                            break;
                        default:
                            {
                                return RedirectToAction("Index", "Home");
                            }
                            break;
                    }
                }
                else
                {
                    foreach (var error in registrationResult.Errors)
                        ModelState.AddModelError("", error);
                }
            }

            //If we got this far, something failed, redisplay form
            model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == _dateTimeHelper.DefaultStoreTimeZone.Id) });
            model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            //form fields
            model.GenderEnabled = _formFieldSettings.GenderEnabled;
            model.CompanyEnabled = _formFieldSettings.CompanyEnabled;
            //model.DateOfBirthEnabled = _formFieldSettings.DateOfBirthEnabled;
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            return View(model);
        }

        public ActionResult RegisterResult(int resultId)
        {
            var resultText = "";
            switch ((UserRegistrationType)resultId)
            {
                case UserRegistrationType.Disabled:
                    resultText = _localizationService.GetResource("Account.Register.Result.Disabled");
                    break;
                case UserRegistrationType.Standard:
                    resultText = _localizationService.GetResource("Account.Register.Result.Standard");
                    break;
                case UserRegistrationType.AdminApproval:
                    resultText = _localizationService.GetResource("Account.Register.Result.AdminApproval");
                    break;
                case UserRegistrationType.EmailValidation:
                    resultText = _localizationService.GetResource("Account.Register.Result.EmailValidation");
                    break;
                default:
                    break;
            }
            var model = new RegisterResultModel()
            {
                Result = resultText
            };
            return View(model);
        }
        
        public ActionResult Logout()
        {
            _authenticationService.SignOut();

            return this.RedirectToAction("Index", "Home");
        }

        #endregion

        #region My account

        public ActionResult MyAccount()
        {
            return RedirectToAction("info");
        }

        #region Info

        public ActionResult Info()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;
            var user = _authenticationService.GetAuthenticatedUser();

            var model = new CustomerInfoModel();
            PrepareCustomerInfoModel(model, customer, user, false);

            return View(model);
        }

        [HttpPost]
        public ActionResult Info(CustomerInfoModel model)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;
            var user = _authenticationService.GetAuthenticatedUser();

            //email
            if (String.IsNullOrEmpty(model.Email))
                ModelState.AddModelError("", "Email is not provided.");
            //username 
            if (_userSettings.UsernamesEnabled &&
                this._userSettings.AllowUsersToChangeUsernames)
            {
                if (String.IsNullOrEmpty(model.Username))
                    ModelState.AddModelError("", "Username is not provided.");
            }

            if (ModelState.IsValid)
            {
                //username 
                if (_userSettings.UsernamesEnabled &&
                    this._userSettings.AllowUsersToChangeUsernames)
                {
                    if (!user.Username.Equals(model.Username.Trim(), StringComparison.InvariantCultureIgnoreCase))
                        //TODO handle an error if an exception is thrown
                        _userService.SetUsername(user, model.Username);
                }
                //email
                if (!user.Email.Equals(model.Email.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    //TODO handle an error if an exception is thrown
                    _userService.SetEmail(user, model.Email);

                //properties
                if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    customer.TimeZoneId = model.TimeZoneId;
                //VAT number
                if (_taxSettings.EuVatEnabled)
                {
                    var prevVatNumber = customer.VatNumber;
                    customer.VatNumber = model.VatNumber;
                    if (prevVatNumber != model.VatNumber)
                    {
                        string vatName = string.Empty;
                        string vatAddress = string.Empty;
                        customer.VatNumberStatus = _taxService.GetVatNumberStatus(customer.VatNumber, out vatName, out vatAddress);
                        //send VAT number admin notification
                        if (!String.IsNullOrEmpty(customer.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                            _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer, customer.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
                    }
                }
                //save
                _customerService.UpdateCustomer(customer);

                //form fields
                if (_formFieldSettings.GenderEnabled)
                    _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.Gender, model.Gender);
                _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
                _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
                //TODO save DateOfBirth (if enabled)
                //if (_formFieldSettings.DateOfBirthEnabled)
                //    _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.DateOfBirth, model.DateOfBirth);
                if (_formFieldSettings.CompanyEnabled)
                    _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.Company, model.Company);

                return RedirectToAction("info");
            }


            //If we got this far, something failed, redisplay form
            PrepareCustomerInfoModel(model, customer, user, true);
            return View(model);
        }
        
        [NonAction]
        private void PrepareCustomerInfoModel(CustomerInfoModel model, Customer customer, User user, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (customer == null)
                throw new ArgumentNullException("customer");

            if (user == null)
                throw new ArgumentNullException("user");
            //TODO Get and save DateOfBirth (if enabled)

            model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == customer.TimeZoneId) });

            if (!excludeProperties)
            {
                model.VatNumber = customer.VatNumber;
                model.FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName);
                model.LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName);
                model.Gender = customer.GetAttribute<string>(SystemCustomerAttributeNames.Gender);
                //model.DateOfBirth = customer.GetAttribute<DateTime?>(SystemCustomerAttributeNames.DateOfBirth);
                model.Company = customer.GetAttribute<string>(SystemCustomerAttributeNames.Company);

                model.Email = user.Email;
                model.Username = user.Username;
            }
            else
            {
                if (_userSettings.UsernamesEnabled && !_userSettings.AllowUsersToChangeUsernames)
                    model.Username = user.Username;
            }
            model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            model.VatNumberStatusNote = customer.VatNumberStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.GenderEnabled = _formFieldSettings.GenderEnabled;
            //model.DateOfBirthEnabled = _formFieldSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _formFieldSettings.CompanyEnabled;
            model.UsernamesEnabled = _userSettings.UsernamesEnabled;
            model.AllowUsersToChangeUsernames = _userSettings.AllowUsersToChangeUsernames;

            model.NavigationModel = GetCustomerNavigationModel(customer);
            model.NavigationModel.SelectedTab = CustomerNavigationEnum.Info;
        }

        #endregion

        #region Addresses

        public ActionResult Addresses()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            var model = new CustomerAddressListModel();
            model.NavigationModel = GetCustomerNavigationModel(customer);
            model.NavigationModel.SelectedTab = CustomerNavigationEnum.Addresses;
            foreach (var address in customer.Addresses)
            {
                //use auto-mapper
                var addressModel = address.ToModel();
                if (address.Country != null)
                    addressModel.CountryName = address.Country.Name;
                if (address.StateProvince != null)
                    addressModel.StateProvinceName = address.StateProvince.Name;

                model.Addresses.Add(addressModel);
            }
            return View(model);
        }

        public ActionResult AddressDelete(int addressId)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            //find address (ensure that it belongs to the current customer)
            var address = customer.Addresses.Where(a => a.Id == addressId).FirstOrDefault();
            if (address != null)
            {
                customer.RemoveAddress(address);
                _customerService.UpdateCustomer(customer);
                _addressService.DeleteAddress(address);
            }

            return RedirectToAction("Addresses");
        }

        public ActionResult AddressAdd()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            var model = new CustomerAddressEditModel();
            model.NavigationModel = GetCustomerNavigationModel(customer);
            model.NavigationModel.SelectedTab = CustomerNavigationEnum.Addresses;

            model.Address = new AddressModel();
            //countries
            model.Address.AvailableCountries.Add(new SelectListItem() { Text = "Select country", Value = "0" });
            foreach (var c in _countryService.GetAllCountries())
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
            model.Address.AvailableStates.Add(new SelectListItem() { Text = "Other (Non US)", Value = "0" });

            return View(model);
        }

        [HttpPost]
        public ActionResult AddressAdd(CustomerAddressEditModel model)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            model.NavigationModel = GetCustomerNavigationModel(customer);
            model.NavigationModel.SelectedTab = CustomerNavigationEnum.Addresses;

            if (ModelState.IsValid)
            {
                var address = model.Address.ToEntity();
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                customer.AddAddress(address);
                _customerService.UpdateCustomer(customer);

                return RedirectToAction("Addresses");
            }
            

            //If we got this far, something failed, redisplay form
            //countries
            model.Address.AvailableCountries.Add(new SelectListItem() { Text = "Select country", Value = "0" });
            foreach (var c in _countryService.GetAllCountries())
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.Address.CountryId) });
            //states
            var states = model.Address.CountryId.HasValue ? _stateProvinceService.GetStateProvincesByCountryId(model.Address.CountryId.Value).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == model.Address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem() { Text = "Other (Non US)", Value = "0" });

            return View(model);
        }

        public ActionResult AddressEdit(int addressId)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            var model = new CustomerAddressEditModel();
            model.NavigationModel = GetCustomerNavigationModel(customer);
            model.NavigationModel.SelectedTab = CustomerNavigationEnum.Addresses;

            //find address (ensure that it belongs to the current customer)
            var address = customer.Addresses.Where(a => a.Id == addressId).FirstOrDefault();
            if (address == null)
                //address is not found
                return RedirectToAction("Addresses");

            model.Address = address.ToModel();
            //countries
            model.Address.AvailableCountries.Add(new SelectListItem() { Text = "Select country", Value = "0" });
            foreach (var c in _countryService.GetAllCountries())
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == address.CountryId) });
            //states
            var states = address.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(address.Country.Id).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem() { Text = "Other (Non US)", Value = "0" });

            return View(model);
        }

        [HttpPost]
        public ActionResult AddressEdit(CustomerAddressEditModel model, int addressId)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            model.NavigationModel = GetCustomerNavigationModel(customer);
            model.NavigationModel.SelectedTab = CustomerNavigationEnum.Addresses;

            if (ModelState.IsValid)
            {
                model.NavigationModel = GetCustomerNavigationModel(customer);
                model.NavigationModel.SelectedTab = CustomerNavigationEnum.Addresses;

                //find address (ensure that it belongs to the current customer)
                var address = customer.Addresses.Where(a => a.Id == addressId).FirstOrDefault();
                if (address == null)
                    //address is not found
                    return RedirectToAction("Addresses");

                address = model.Address.ToEntity(address);
                _addressService.UpdateAddress(address);

                return RedirectToAction("Addresses");
            }

            //If we got this far, something failed, redisplay form
            //countries
            model.Address.AvailableCountries.Add(new SelectListItem() { Text = "Select country", Value = "0" });
            foreach (var c in _countryService.GetAllCountries())
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.Address.CountryId) });
            //states
            var states = model.Address.CountryId.HasValue ? _stateProvinceService.GetStateProvincesByCountryId(model.Address.CountryId.Value).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == model.Address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem() { Text = "Other (Non US)", Value = "0" });

            return View(model);
        }
           
        #endregion

        #region Orders

        public ActionResult Orders()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;
            var model = PrepareCustomerOrderListModel(customer);
            return View(model);
        }

        [HttpPost, ActionName("Orders")]
        [FormValueRequired(FormValueRequirement.StartsWith, "cancelRecurringPayment")]
        public ActionResult CancelRecurringPayment(FormCollection form)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            //get recurring payment identifier
            int recurringPaymentId = 0;
            foreach (var formValue in form.AllKeys)
                if (formValue.StartsWith("cancelRecurringPayment", StringComparison.InvariantCultureIgnoreCase))
                    recurringPaymentId = Convert.ToInt32(formValue.Substring("cancelRecurringPayment".Length));

            var recurringPayment = _orderService.GetRecurringPaymentById(recurringPaymentId);
            if (_orderProcessingService.CanCancelRecurringPayment(customer, recurringPayment))
            {
                var errors = _orderProcessingService.CancelRecurringPayment(recurringPayment);
                //UNDONE display errors (if errors.Count > 0)
            }
            var model = PrepareCustomerOrderListModel(customer);
            return View(model);
        }

        private CustomerOrderListModel PrepareCustomerOrderListModel(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var model = new CustomerOrderListModel();
            model.NavigationModel = GetCustomerNavigationModel(customer);
            model.NavigationModel.SelectedTab = CustomerNavigationEnum.Orders;
            var orders = _orderService.GetOrdersByCustomerId(customer.Id);
            foreach (var order in orders)
            {
                var orderModel = new CustomerOrderListModel.OrderDetailsModel()
                {
                    Id = order.Id,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc).ToString(),
                    OrderStatus = order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                    IsReturnRequestAllowed = _orderProcessingService.IsReturnRequestAllowed(order)
                };
                var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
                orderModel.OrderTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false);

                model.Orders.Add(orderModel);
            }

            var recurringPayments = _orderService.SearchRecurringPayments(customer.Id, 0, null);
            foreach (var recurringPayment in recurringPayments)
            {
                var recurringPaymentModel = new CustomerOrderListModel.RecurringOrderModel()
                {
                    Id = recurringPayment.Id,
                    StartDate = _dateTimeHelper.ConvertToUserTime(recurringPayment.StartDateUtc, DateTimeKind.Utc).ToString(),
                    CycleInfo =string.Format("{0} {1}", recurringPayment.CycleLength, recurringPayment.CyclePeriod.GetLocalizedEnum(_localizationService,_workContext)),
                    NextPayment = recurringPayment.NextPaymentDate.HasValue ? _dateTimeHelper.ConvertToUserTime(recurringPayment.NextPaymentDate.Value, DateTimeKind.Utc).ToString() : "",
                    TotalCycles = recurringPayment.TotalCycles,
                    CyclesRemaining = recurringPayment.CyclesRemaining,
                    InitialOrderId = recurringPayment.InitialOrder.Id,
                    CanCancel = _orderProcessingService.CanCancelRecurringPayment(customer, recurringPayment),
                };

                model.RecurringOrders.Add(recurringPaymentModel);
            }

            return model;
        }

        #endregion

        #region Return request

        public ActionResult ReturnRequests()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            var model = new CustomeReturnRequestsModel();
            model.NavigationModel = GetCustomerNavigationModel(customer);
            model.NavigationModel.SelectedTab = CustomerNavigationEnum.ReturnRequests;
            var returnRequests = _orderService.SearchReturnRequests(customer.Id, 0, null);
            foreach (var returnRequest in returnRequests)
            {
                var opv = _orderService.GetOrderProductVariantById(returnRequest.OrderProductVariantId);
                var pv = opv.ProductVariant;

                var itemModel = new CustomeReturnRequestsModel.ReturnRequestModel()
                {
                    Id = returnRequest.Id,
                    ReturnRequestStatus = returnRequest.ReturnRequestStatus.GetLocalizedEnum(_localizationService, _workContext),
                    ProductId = pv.ProductId,
                    ProductSeName = pv.Product.GetSeName(),
                    Quantity = returnRequest.Quantity,
                    ReturnAction = returnRequest.RequestedAction,
                    ReturnReason = returnRequest.ReasonForReturn,
                    Comments = returnRequest.CustomerComments,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(returnRequest.CreatedOnUtc, DateTimeKind.Utc).ToString(),
                };
                model.Items.Add(itemModel);

                if (!String.IsNullOrEmpty(pv.GetLocalized(x => x.Name)))
                    itemModel.ProductName = string.Format("{0} ({1})", pv.Product.GetLocalized(x => x.Name), pv.GetLocalized(x => x.Name));
                else
                    itemModel.ProductName = pv.Product.GetLocalized(x => x.Name);
            }

            return View(model);
        }

        #endregion

        #region Downloable products

        public ActionResult DownloadableProducts()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();
            
            var customer = _workContext.CurrentCustomer;

            var model = new CustomerDownloadableProductsModel();
            model.NavigationModel = GetCustomerNavigationModel(customer);
            model.NavigationModel.SelectedTab = CustomerNavigationEnum.DownloadableProducts;
            var items = _orderService.GetAllOrderProductVariants(null, customer.Id, null, null,
                null, null, null, true);
            foreach (var item in items)
            {
                var itemModel = new CustomerDownloadableProductsModel.DownloadableProductsModel()
                {
                    OrderProductVariantGuid = item.OrderProductVariantGuid,
                    OrderId = item.OrderId,
                    CreatedOnStr = _dateTimeHelper.ConvertToUserTime(item.Order.CreatedOnUtc, DateTimeKind.Utc).ToString("d"),
                    ProductSeName = item.ProductVariant.Product.GetSeName(),
                    ProductAttributes = item.AttributeDescription,
                    ProductId = item.ProductVariant.ProductId
                };
                model.Items.Add(itemModel);

                //product name
                if (!String.IsNullOrEmpty(item.ProductVariant.GetLocalized(x => x.Name)))
                    itemModel.ProductName = string.Format("{0} ({1})", item.ProductVariant.Product.GetLocalized(x => x.Name), item.ProductVariant.GetLocalized(x => x.Name));
                else
                    itemModel.ProductName = item.ProductVariant.Product.GetLocalized(x => x.Name);

                if (_orderProcessingService.IsDownloadAllowed(item))
                    itemModel.DownloadId = item.ProductVariant.DownloadId;

                if (_orderProcessingService.IsLicenseDownloadAllowed(item))
                    itemModel.LicenseId = item.LicenseDownloadId.HasValue ? item.LicenseDownloadId.Value : 0;
            }
            
            return View(model);
        }

        public ActionResult UserAgreement(Guid opvId)
        {
            var opv = _orderService.GetOrderProductVariantByGuid(opvId);
            if (opv == null)
                return RedirectToAction("Index", "Home");


            var productVariant = opv.ProductVariant;
            if (productVariant == null || !productVariant.HasUserAgreement)
                return RedirectToAction("Index", "Home");

            var model = new UserAgreementModel();
            model.UserAgreementText = productVariant.UserAgreementText;
            model.OrderProductVariantGuid = opvId;
            
            return View(model);
        }

        #endregion

        #region Reward points

        public ActionResult RewardPoints()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            if (!_rewardPointsSettings.Enabled)
                return RedirectToAction("MyAccount");

            var customer = _workContext.CurrentCustomer;

            var model = new CustomerRewardPointsModel();
            model.NavigationModel = GetCustomerNavigationModel(customer);
            model.NavigationModel.SelectedTab = CustomerNavigationEnum.RewardPoints;
            foreach (var rph in customer.RewardPointsHistory.OrderByDescending(rph => rph.CreatedOnUtc).ThenByDescending(rph => rph.Id))
            {
                model.RewardPoints.Add(new CustomerRewardPointsModel.RewardPointsHistoryModel()
                {
                    Points = rph.Points,
                    PointsBalance = rph.PointsBalance,
                    Message = rph.Message,
                    CreatedOnStr = _dateTimeHelper.ConvertToUserTime(rph.CreatedOnUtc, DateTimeKind.Utc).ToString()
                });
            }
            int rewardPointsBalance = customer.GetRewardPointsBalance();
            decimal rewardPointsAmountBase = _orderTotalCalculationService.ConvertRewardPointsToAmount(rewardPointsBalance);
            decimal rewardPointsAmount =_currencyService.ConvertFromPrimaryStoreCurrency(rewardPointsAmountBase,_workContext.WorkingCurrency);
            model.RewardPointsBalance = string.Format(_localizationService.GetResource("RewardPoints.CurrentBalance"), rewardPointsBalance, _priceFormatter.FormatPrice(rewardPointsAmount, true, false));
            
            return View(model);
        }

        #endregion

        #region Change password

        public ActionResult ChangePassword()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;
            var user = _authenticationService.GetAuthenticatedUser();

            var model = new ChangePasswordModel();
            model.NavigationModel = GetCustomerNavigationModel(customer);
            model.NavigationModel.SelectedTab = CustomerNavigationEnum.ChangePassword;
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;
            var user = _authenticationService.GetAuthenticatedUser();

            model.NavigationModel = GetCustomerNavigationModel(customer);
            model.NavigationModel.SelectedTab = CustomerNavigationEnum.ChangePassword;

            if (ModelState.IsValid)
            {
                var changePasswordRequest = new ChangePasswordRequest(user.Email, true, model.NewPassword, model.OldPassword);
                var changePasswordResult = _userService.ChangePassword(changePasswordRequest);
                if (changePasswordResult.Success)
                {
                    model.Result = _localizationService.GetResource("Account.ChangePassword.Success");
                    return View(model);
                }
                else
                {
                    foreach (var error in changePasswordResult.Errors)
                        ModelState.AddModelError("", error);
                }
            }


            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Avatar

        public ActionResult Avatar()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();
            
            if (!_customerSettings.AllowCustomersToUploadAvatars)
                return RedirectToAction("MyAccount");

            var customer = _workContext.CurrentCustomer;

            var model = new CustomerAvatarModel();
            model.NavigationModel = GetCustomerNavigationModel(customer);
            model.NavigationModel.SelectedTab = CustomerNavigationEnum.Avatar;
            model.AvatarUrl = _pictureService.GetPictureUrl(
                customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                _mediaSettings.AvatarPictureSize,
                false);
            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [FormValueRequired("upload-avatar")]
        public ActionResult UploadAvatar(CustomerAvatarModel model, HttpPostedFileBase uploadedFile)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();
            
            if (!_customerSettings.AllowCustomersToUploadAvatars)
                return RedirectToAction("MyAccount");

            var customer = _workContext.CurrentCustomer;

            model.NavigationModel = GetCustomerNavigationModel(customer);
            model.NavigationModel.SelectedTab = CustomerNavigationEnum.Avatar;


            if (ModelState.IsValid)
            {
                try
                {
                    var customerAvatar = _pictureService.GetPictureById(customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId));
                    if ((uploadedFile != null) && (!String.IsNullOrEmpty(uploadedFile.FileName)))
                    {
                        int avatarMaxSize = _customerSettings.AvatarMaximumSizeBytes;
                        if (uploadedFile.ContentLength > avatarMaxSize)
                            throw new NopException(string.Format("Maximum avatar size is {0} bytes", avatarMaxSize));

                        byte[] customerPictureBinary = uploadedFile.GetPictureBits();
                        if (customerAvatar != null)
                            customerAvatar = _pictureService.UpdatePicture(customerAvatar.Id, customerPictureBinary, uploadedFile.ContentType, true);
                        else
                            customerAvatar = _pictureService.InsertPicture(customerPictureBinary, uploadedFile.ContentType, true);
                    }

                    int customerAvatarId = 0;
                    if (customerAvatar != null)
                        customerAvatarId = customerAvatar.Id;

                    _customerService.SaveCustomerAttribute<int>(customer, SystemCustomerAttributeNames.AvatarPictureId, customerAvatarId);

                    model.AvatarUrl = _pictureService.GetPictureUrl(
                        customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                        _mediaSettings.AvatarPictureSize,
                        false);
                    return View(model);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.Message);
                }
            }


            //If we got this far, something failed, redisplay form
            model.AvatarUrl = _pictureService.GetPictureUrl(
                customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId), 
                _mediaSettings.AvatarPictureSize, 
                false);
            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [FormValueRequired("remove-avatar")]
        public ActionResult RemoveAvatar(CustomerAvatarModel model, HttpPostedFileBase uploadedFile)
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

            if (!_customerSettings.AllowCustomersToUploadAvatars)
                return RedirectToAction("MyAccount");

            var customer = _workContext.CurrentCustomer;

            model.NavigationModel = GetCustomerNavigationModel(customer);
            model.NavigationModel.SelectedTab = CustomerNavigationEnum.Avatar;
            
            var customerAvatar = _pictureService.GetPictureById(customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId));
            if (customerAvatar != null)
                _pictureService.DeletePicture(customerAvatar);
            _customerService.SaveCustomerAttribute<int>(customer, SystemCustomerAttributeNames.AvatarPictureId, 0);

            return RedirectToAction("Avatar");
        }

        #endregion

        #endregion
    }
}
