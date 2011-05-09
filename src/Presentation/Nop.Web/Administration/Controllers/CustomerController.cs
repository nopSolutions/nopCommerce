using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class CustomerController : BaseNopController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly TaxSettings _taxSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IAddressService _addressService;
        private readonly IUserService _userService;
        private readonly FormFieldSettings _formFieldSettings;
        private readonly ITaxService _taxService;
        private readonly IWorkContext _workContext;

        #endregion Fields

        #region Constructors

        public CustomerController(ICustomerService customerService, IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService, DateTimeSettings dateTimeSettings,
            TaxSettings taxSettings, RewardPointsSettings rewardPointsSettings,
            ICountryService countryService, IStateProvinceService stateProvinceService, 
            IAddressService addressService,
            IUserService userService, FormFieldSettings formFieldSettings,
            ITaxService taxService, IWorkContext workContext)
        {
            this._customerService = customerService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._dateTimeSettings = dateTimeSettings;
            this._taxSettings = taxSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._addressService = addressService;
            this._userService = userService;
            this._formFieldSettings = formFieldSettings;
            this._taxService = taxService;
            this._workContext = workContext;
        }

        #endregion Constructors

        #region Utilities

        [NonAction]
        private string GetCustomerRolesNames(IList<CustomerRole> customerRoles, string separator = ",")
        {
            var sb = new StringBuilder();
            for (int i = 0; i < customerRoles.Count; i++)
            {
                sb.Append(customerRoles[i].Name);
                if (i != customerRoles.Count - 1)
                {
                    sb.Append(separator);
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }
        
        #endregion

        #region Customers

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var customers = _customerService.GetAllCustomers(null,null, null, 0, 10);
            var model = new CustomerListModel();
            //customer roles
            var customerRoles = _customerService.GetAllCustomerRoles(true);
            model.AvailableCustomerRoles = customerRoles.ToList();
            //customer list
            model.Customers = new GridModel<CustomerModel>
            {
                Data = customers.Select(x =>
                {
                    var model1 = x.ToModel();
                    model1.FullName = string.Format("{0} {1}", x.GetAttribute<string>(SystemCustomerAttributeNames.FirstName), x.GetAttribute<string>(SystemCustomerAttributeNames.LastName));
                    model1.CustomerRoleNames = GetCustomerRolesNames(x.CustomerRoles.ToList());
                    model1.CreatedOnStr = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc).ToString();
                    return model1;
                }),
                Total = customers.TotalCount
            };
            return View(model);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult CustomerList(GridCommand command)
        {
            //filtering
            //convert to string because passing int[] to grid is no possible
            string searchCustomerRoleIdsStr = command.FilterDescriptors.GetValueFromAppliedFilters("searchCustomerRoleIds");
            var searchCustomerRoleIds = new List<int>();
            foreach (var str1 in searchCustomerRoleIdsStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                searchCustomerRoleIds.Add(Convert.ToInt32(str1));
            //List<int> converter should be registered
            //var searchCustomerRoleIds = TypeDescriptor.GetConverter(typeof(List<int>)).ConvertFrom(searchCustomerRoleIdsStr) as List<int>;
            

            var customers = _customerService.GetAllCustomers(null, null, 
                searchCustomerRoleIds.ToArray(), command.Page - 1, command.PageSize);
            var gridModel = new GridModel<CustomerModel>
            {
                Data = customers.Select(x =>
                {
                    var model = x.ToModel();
                    model.FullName = string.Format("{0} {1}", x.GetAttribute<string>(SystemCustomerAttributeNames.FirstName), x.GetAttribute<string>(SystemCustomerAttributeNames.LastName));
                    model.CustomerRoleNames = GetCustomerRolesNames(x.CustomerRoles.ToList());
                    model.CreatedOnStr = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc).ToString();
                    return model;
                }),
                Total = customers.TotalCount
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("search-customers")]
        public ActionResult Search(CustomerListModel model)
        {
            //convert to string because passing int[] to grid is no possible
            string searchCustomerRoleIdsStr = "";
            if (model.SearchCustomerRoleIds != null)
                foreach (var i in model.SearchCustomerRoleIds)
                    searchCustomerRoleIdsStr+= i + ",";
            ViewData["searchCustomerRoleIds"] = searchCustomerRoleIdsStr;
            //if (model.SearchCustomerRoleIds != null)
                //List<int> converter should be registered
                //ViewData["searchCustomerRoleIds"] = TypeDescriptor.GetConverter(typeof(List<int>)).ConvertTo(model.SearchCustomerRoleIds, typeof(string)) as string;
            

            //customer roles
            var customerRoles = _customerService.GetAllCustomerRoles(true);
            model.AvailableCustomerRoles = customerRoles.ToList();

            //laod customers
            var customers = _customerService.GetAllCustomers(null, null,
               model.SearchCustomerRoleIds, 0, 10);
            //customer list
            model.Customers = new GridModel<CustomerModel>
            {
                Data = customers.Select(x =>
                {
                    var model1 = x.ToModel();
                    model1.FullName = string.Format("{0} {1}", x.GetAttribute<string>(SystemCustomerAttributeNames.FirstName), x.GetAttribute<string>(SystemCustomerAttributeNames.LastName));
                    model1.CustomerRoleNames = GetCustomerRolesNames(x.CustomerRoles.ToList());
                    model1.CreatedOnStr = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc).ToString();
                    return model1;
                }),
                Total = customers.TotalCount
            };
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new CustomerModel();
            model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == _dateTimeHelper.DefaultStoreTimeZone.Id) });
            model.DisplayVatNumber = false;
            //customer roles
            var customerRoles = _customerService.GetAllCustomerRoles(true);
            model.AvailableCustomerRoles = customerRoles.ToList();
            model.SelectedCustomerRoleIds = new int[0];
            //form fields
            model.GenderEnabled = _formFieldSettings.GenderEnabled;
            model.DateOfBirthEnabled = _formFieldSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _formFieldSettings.CompanyEnabled;

            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Create(CustomerModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var customer = model.ToEntity();
                customer.CustomerGuid = Guid.NewGuid();
                customer.CreatedOnUtc = DateTime.UtcNow;
                _customerService.InsertCustomer(customer);
                if (_formFieldSettings.GenderEnabled)
                    _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.Gender, model.Gender);
                _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
                _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
                if (_formFieldSettings.DateOfBirthEnabled)
                    _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.DateOfBirth, model.DateOfBirth);
                if (_formFieldSettings.CompanyEnabled)
                    _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.Company, model.Company);

                //customer roles
                var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
                foreach (var customerRole in allCustomerRoles)
                {
                    if (model.SelectedCustomerRoleIds != null && model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                        customer.CustomerRoles.Add(customerRole);
                }
                _customerService.UpdateCustomer(customer);

                return continueEditing ? RedirectToAction("Edit", new { id = customer.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == model.TimeZoneId) });
            model.DisplayVatNumber = false;
            //customer roles
            var customerRoles = _customerService.GetAllCustomerRoles(true);
            model.AvailableCustomerRoles = customerRoles.ToList();
            //form fields
            model.GenderEnabled = _formFieldSettings.GenderEnabled;
            model.DateOfBirthEnabled = _formFieldSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _formFieldSettings.CompanyEnabled;
            return View(model);

        }

        public ActionResult Edit(int id)
        {
            var customer = _customerService.GetCustomerById(id);
            if (customer == null) 
                throw new ArgumentException("No customer found with the specified id", "id");
            
            var model = customer.ToModel();
            model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == customer.TimeZoneId) });
            model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            model.VatNumber = customer.VatNumber;
            model.VatNumberStatusNote = customer.VatNumberStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName);
            model.LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName);
            model.Gender = customer.GetAttribute<string>(SystemCustomerAttributeNames.Gender);
            model.DateOfBirth = customer.GetAttribute<DateTime?>(SystemCustomerAttributeNames.DateOfBirth);
            model.Company = customer.GetAttribute<string>(SystemCustomerAttributeNames.Company);
            model.CreatedOnStr = _dateTimeHelper.ConvertToUserTime(customer.CreatedOnUtc, DateTimeKind.Utc).ToString();
            //form fields
            model.GenderEnabled = _formFieldSettings.GenderEnabled;
            model.DateOfBirthEnabled = _formFieldSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _formFieldSettings.CompanyEnabled;
            //customer roles
            var customerRoles = _customerService.GetAllCustomerRoles(true);
            model.AvailableCustomerRoles = customerRoles.ToList();
            model.SelectedCustomerRoleIds = customer.CustomerRoles.Select(cr => cr.Id).ToArray();
            //reward points gistory
            model.DisplayRewardPointsHistory = _rewardPointsSettings.Enabled;
            model.AddRewardPointsValue = 0;
            model.AddRewardPointsMessage = "Some comment here...";
            //user account
            model.AssociatedUserId = customer.AssociatedUserId;
            if (customer.AssociatedUserId.HasValue)
            {
                User associatedUser = _userService.GetUserById(customer.AssociatedUserId.Value);
                if (associatedUser != null)
                    model.AssociatedUserEmail = associatedUser.Email;
            }

            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Edit(CustomerModel model, bool continueEditing)
        {
            var customer = _customerService.GetCustomerById(model.Id);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id", "id");

            if (ModelState.IsValid)
            {
                string prevVatNumber = customer.VatNumber;
                customer = model.ToEntity(customer);


                //VAT number
                if (_taxSettings.EuVatEnabled)
                {
                    customer.VatNumber = model.VatNumber;
                    //set VAT number status
                    if (!String.IsNullOrEmpty(customer.VatNumber))
                    {
                        if (!customer.VatNumber.Equals(prevVatNumber, StringComparison.InvariantCultureIgnoreCase))
                            customer.VatNumberStatus = _taxService.GetVatNumberStatus(customer.VatNumber);
                    }
                    else
                        customer.VatNumberStatus = VatNumberStatus.Empty;
                }
                _customerService.UpdateCustomer(customer);

                //customer attributes
                if (_formFieldSettings.GenderEnabled)
                    _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.Gender, model.Gender);
                _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
                _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
                if (_formFieldSettings.DateOfBirthEnabled)
                    _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.DateOfBirth, model.DateOfBirth);
                if (_formFieldSettings.CompanyEnabled)
                    _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.Company, model.Company);

                //customer roles
                var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
                foreach (var customerRole in allCustomerRoles)
                {
                    if (model.SelectedCustomerRoleIds != null && model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                    {
                        //new role
                        if (customer.CustomerRoles.Where(cr => cr.Id == customerRole.Id).Count() == 0)
                            customer.CustomerRoles.Add(customerRole);
                    }
                    else
                    {
                        //removed role
                        if (customer.CustomerRoles.Where(cr => cr.Id == customerRole.Id).Count() > 0)
                            customer.CustomerRoles.Remove(customerRole);
                    }
                }
                _customerService.UpdateCustomer(customer);

                return continueEditing ? RedirectToAction("Edit", customer.Id) : RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == model.TimeZoneId) });
            model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            model.VatNumberStatusNote = customer.VatNumberStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.CreatedOnStr = _dateTimeHelper.ConvertToUserTime(customer.CreatedOnUtc, DateTimeKind.Utc).ToString();
            //form fields
            model.GenderEnabled = _formFieldSettings.GenderEnabled;
            model.DateOfBirthEnabled = _formFieldSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _formFieldSettings.CompanyEnabled;
            //customer roles
            var customerRoles = _customerService.GetAllCustomerRoles(true);
            model.AvailableCustomerRoles = customerRoles.ToList();
            //reward points gistory
            model.DisplayRewardPointsHistory = _rewardPointsSettings.Enabled;
            model.AddRewardPointsValue = 0;
            model.AddRewardPointsMessage = "Some comment here...";
            //user account
            model.AssociatedUserId = customer.AssociatedUserId;
            if (customer.AssociatedUserId.HasValue)
            {
                User associatedUser = _userService.GetUserById(customer.AssociatedUserId.Value);
                if (associatedUser != null)
                    model.AssociatedUserEmail = associatedUser.Email;
            }
            return View(model);
        }
        
        [HttpPost, ActionName("Edit")]
        [FormValueRequired("markVatNumberAsValid")]
        public ActionResult MarkVatNumberAsValid(CustomerModel model)
        {
            var customer = _customerService.GetCustomerById(model.Id);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id", "id");

            customer.VatNumberStatus = VatNumberStatus.Valid;
            _customerService.UpdateCustomer(customer);

            return RedirectToAction("Edit", customer.Id);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("markVatNumberAsInvalid")]
        public ActionResult MarkVatNumberAsInvalid(CustomerModel model)
        {
            var customer = _customerService.GetCustomerById(model.Id);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id", "id");

            customer.VatNumberStatus = VatNumberStatus.Invalid;
            _customerService.UpdateCustomer(customer);

            return RedirectToAction("Edit", customer.Id);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var customer = _customerService.GetCustomerById(id);
            if (customer == null) 
                throw new ArgumentException("No customer found with the specified id", "id");

            _customerService.DeleteCustomer(customer);
            return RedirectToAction("List");
        }
        
        #endregion
        
        #region Reward points history

        [GridAction]
        public ActionResult RewardPointsHistorySelect(int customerId)
        {
            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id", "id");

            var model = new List<CustomerModel.RewardPointsHistoryModel>();
            foreach (var rph in customer.RewardPointsHistory.OrderByDescending(rph => rph.CreatedOnUtc).ThenByDescending(rph => rph.Id))
            {
                model.Add(new CustomerModel.RewardPointsHistoryModel()
                    {
                        Points = rph.Points,
                        PointsBalance = rph.PointsBalance,
                        Message = rph.Message,
                        CreatedOnStr = _dateTimeHelper.ConvertToUserTime(rph.CreatedOnUtc, DateTimeKind.Utc).ToString()
                    });
            } 
            var gridModel = new GridModel<CustomerModel.RewardPointsHistoryModel>
            {
                Data = model,
                Total = model.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [ValidateInput(false)]
        public ActionResult RewardPointsHistoryAdd(int customerId, int addRewardPointsValue, string addRewardPointsMessage)
        {
            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id", "id");

            customer.AddRewardPointsHistoryEntry(addRewardPointsValue, addRewardPointsMessage);
            _customerService.UpdateCustomer(customer);

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }
        
        #endregion
        
        #region Addresses

        [GridAction]
        public ActionResult AddressesSelect(int customerId, GridCommand command)
        {
            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id", "customerId");

            var addresses = customer.Addresses.OrderByDescending(a => a.CreatedOnUtc).ThenByDescending(a => a.Id).ToList();
            var gridModel = new GridModel<AddressModel>
            {
                Data = addresses.Select(x =>
                {
                    var model = x.ToModel();
                    if (x.Country != null)
                        model.CountryName = x.Country.Name;
                    if (x.StateProvince != null)
                        model.StateProvinceName = x.StateProvince.Name;
                    return model;
                }),
                Total = addresses.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [GridAction]
        public ActionResult AddressDelete(int customerId, int addressId, GridCommand command)
        {
            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id", "customerId");

            var address = customer.Addresses.Where(a => a.Id == addressId).FirstOrDefault();
            customer.RemoveAddress(address);
            _customerService.UpdateCustomer(customer);
            //TODO should we delete the address record?

            return AddressesSelect(customerId, command);
        }
        
        public ActionResult AddressCreate(int customerId)
        {
            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id", "customerId");

            var model = new CustomerAddressModel();
            model.Address = new AddressModel();
            model.CustomerId = customerId;
            //countries
            model.Address.AvailableCountries.Add(new SelectListItem() { Text = "Select country", Value = "0" });
            foreach (var c in _countryService.GetAllCountries(true))
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
            model.Address.AvailableStates.Add(new SelectListItem() { Text = "Other (Non US)", Value = "0" });

            return View(model);
        }

        [HttpPost]
        public ActionResult AddressCreate(CustomerAddressModel model)
        {
            var customer = _customerService.GetCustomerById(model.CustomerId);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id");

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

                return RedirectToAction("AddressEdit", new { addressId = address.Id, customerId = model.CustomerId });
            }

            //If we got this far, something failed, redisplay form
            model.CustomerId = customer.Id;
            //countries
            model.Address.AvailableCountries.Add(new SelectListItem() { Text = "Select country", Value = "0" });
            foreach (var c in _countryService.GetAllCountries(true))
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.Address.CountryId) });
            //states
            Country country = model.Address.CountryId.HasValue ? _countryService.GetCountryById(model.Address.CountryId.Value) : null;
            var states = country != null ? _stateProvinceService.GetStateProvincesByCountryId(country.Id).ToList() : new List<StateProvince>();
            if (country != null && states.Count > 0)
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == model.Address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem() { Text = "Other (Non US)", Value = "0" });
            return View(model);
        }

        public ActionResult AddressEdit(int addressId, int customerId)
        {
            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id");

            var address = _addressService.GetAddressById(addressId);
            if (address == null)
                throw new ArgumentException("No address found with the specified id", "addressId");

            var model = new CustomerAddressModel();
            model.CustomerId = customerId;
            model.Address = address.ToModel();
            //countries
            model.Address.AvailableCountries.Add(new SelectListItem() { Text = "Select country", Value = "0" });
            foreach (var c in _countryService.GetAllCountries(true))
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == address.CountryId) });
            //states
            var states = address.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(address.Country.Id).ToList() : new List<StateProvince>();
            if (address.Country != null && states.Count > 0)
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem() { Text = "Other (Non US)", Value = "0" });

            return View(model);
        }

        [HttpPost]
        public ActionResult AddressEdit(CustomerAddressModel model)
        {
            var customer = _customerService.GetCustomerById(model.CustomerId);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id");

            var address = _addressService.GetAddressById(model.Address.Id);
            if (address == null)
                throw new ArgumentException("No address found with the specified id", "addressId");

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                _addressService.UpdateAddress(address);

                return RedirectToAction("AddressEdit", new { addressId = model.Address.Id, customerId = model.CustomerId });
            }

            //If we got this far, something failed, redisplay form
            model.CustomerId = customer.Id;
            model.Address = address.ToModel();
            //countries
            model.Address.AvailableCountries.Add(new SelectListItem() { Text = "Select country", Value = "0" });
            foreach (var c in _countryService.GetAllCountries(true))
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == address.CountryId) });
            //states
            var states = address.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(address.Country.Id).ToList() : new List<StateProvince>();
            if (address.Country != null && states.Count > 0)
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem() { Text = "Other (Non US)", Value = "0" });

            return View(model);
        }

        #endregion

        #region User accounts

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult UserSelect(GridCommand command, CustomerModel model)
        {
            var users = _userService.GetUsers(model.UserEmailStartsWith, null, command.Page - 1, command.PageSize);
            var gridModel = new GridModel<CustomerModel.UserAccountModel>
            {
                Data = users.Select(x =>
                {
                    return new CustomerModel.UserAccountModel()
                    {
                        Id = x.Id,
                        Email = x.Email,
                        IsApproved = x.IsApproved,
                        IsLockedOut = x.IsLockedOut,
                        CreatedOnStr = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc).ToString()
                    };
                }),
                Total = users.TotalCount
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("detach-associated-user")]
        public ActionResult DetachAssociatedUser(CustomerModel model)
        {
            ViewData["selectedTab"] = "useraccount";

            var customer = _customerService.GetCustomerById(model.Id);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id", "id");

            customer.AssociatedUserId = null;
            _customerService.UpdateCustomer(customer);


            model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == model.TimeZoneId) });
            model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            model.VatNumberStatusNote = customer.VatNumberStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.CreatedOnStr = _dateTimeHelper.ConvertToUserTime(customer.CreatedOnUtc, DateTimeKind.Utc).ToString();
            //form fields
            model.GenderEnabled = _formFieldSettings.GenderEnabled;
            model.DateOfBirthEnabled = _formFieldSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _formFieldSettings.CompanyEnabled;
            //customer roles
            var customerRoles = _customerService.GetAllCustomerRoles(true);
            model.AvailableCustomerRoles = customerRoles.ToList();
            //reward points gistory
            model.DisplayRewardPointsHistory = _rewardPointsSettings.Enabled;
            //user account
            model.AssociatedUserEmail = null;
            model.AssociatedUserId = null;

            return View(model);
        }

        [HttpPost]
        public ActionResult AssociateNewUserAccount(int userId, int customerId)
        {
            var user = _userService.GetUserById(userId);
            if (user == null)
                throw new ArgumentException("No user found with the specified id", "id");

            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id", "id");

            customer.AssociatedUserId = userId;
            _customerService.UpdateCustomer(customer);

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
