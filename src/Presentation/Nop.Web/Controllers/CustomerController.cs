using System;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Services;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Models;
using Nop.Web.Models.Customer;

namespace Nop.Web.Controllers
{
    public class CustomerController : BaseNopController
    {
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

        public CustomerController(IAuthenticationService authenticationService,
            IUserService userService, UserSettings userSettings, IDateTimeHelper dateTimeHelper,
            DateTimeSettings dateTimeSettings, TaxSettings taxSettings,
            FormFieldSettings formFieldSettings, ILocalizationService localizationService,
            IWorkContext workContext, ICustomerService customerService,
            ITaxService taxService)
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
        }

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
            //TODO Get and save VAT number (if enabled)
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
    }
}
