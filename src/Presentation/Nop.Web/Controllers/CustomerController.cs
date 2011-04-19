using System;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
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
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models;
using Nop.Web.Models.Common;
using Nop.Web.Models.Customer;

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
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
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
            ICountryService countryService, IOrderTotalCalculationService orderTotalCalculationService,
            ICurrencyService currencyService, IPriceFormatter priceFormatter)
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
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
        }

        #endregion

        #region Utitlies

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
            if (customer == null)
                throw new ArgumentNullException("customer");

            var model = new CustomerNavigationModel();
            model.HideAvatar = !_customerSettings.AllowCustomersToUploadAvatars;
            model.HideRewardPoints = !_rewardPointsSettings.Enabled;
            model.HideForumSubscriptions = !_forumSettings.ForumsEnabled || !_forumSettings.AllowCustomersToManageSubscriptions;
            model.HideReturnRequests = !_orderSettings.ReturnRequestsEnabled;
            //UNDONE make configurable whether to hide/show 'Downloadable products' tab
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

            // If we got this far, something failed, redisplay form
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
                    //now register 'Customer' entity
                    var customer = _workContext.CurrentCustomer;
                    customer = _customerService.RegisterCustomer(customer.Id);
                    //associate it with this user
                    customer.AssociatedUserId = registrationResult.User.Id;
                    //properties
                    if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                        customer.TimeZoneId = model.TimeZoneId;
                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        customer.VatNumber = model.VatNumber;
                        customer.VatNumberStatus = _taxService.GetVatNumberStatus(customer.VatNumber);
                        //TODO send VAT number admin notification
                        //if (!String.IsNullOrEmpty(customer.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                        //    _messageWorkflowService.SendNewVATSubmittedStoreOwnerNotification(customer, vatName, vatAddress, _localizationService.DefaultAdminLanguage.LanguageId);
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

                    //UNDONE notification
                    //if (_customerService.NotifyNewCustomerRegistration)
                    //    _messageWorkflowService.SendNewCustomerNotificationMessage(customer, _localizationService.DefaultAdminLanguage.LanguageId);

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

            // If we got this far, something failed, redisplay form
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
                        customer.VatNumberStatus = _taxService.GetVatNumberStatus(customer.VatNumber);
                        //TODO send VAT number admin notification
                        //if (!String.IsNullOrEmpty(customer.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                        //    _messageWorkflowService.SendNewVATSubmittedStoreOwnerNotification(customer, vatName, vatAddress, _localizationService.DefaultAdminLanguage.LanguageId);
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


            // If we got this far, something failed, redisplay form
            PrepareCustomerInfoModel(model, customer, user, true);
            return View(model);
        }

        [NonAction]
        private string GetVatNumberStatusName(VatNumberStatus status)
        {
            return _localizationService.GetResource(string.Format("Account.Fields.VatNumberStatus.{0}", status.ToString()));
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
            model.VatNumberStatusNote = GetVatNumberStatusName(customer.VatNumberStatus);
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
            foreach (var c in _countryService.GetAllCountries(true))
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

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.NavigationModel = GetCustomerNavigationModel(customer);
            model.NavigationModel.SelectedTab = CustomerNavigationEnum.Addresses;

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
            foreach (var c in _countryService.GetAllCountries(true))
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == address.CountryId) });
            //states
            if (address.Country != null && address.Country.StateProvinces.Count > 0)
            {
                foreach (var s in address.Country.StateProvinces)
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

            if (!ModelState.IsValid)
            {
                return View(model);
            }

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
           
        #endregion

        #region Addresses

        public ActionResult RewardPoints()
        {
            if (!IsCurrentUserRegistered())
                return new HttpUnauthorizedResult();

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

        #endregion
    }
}
