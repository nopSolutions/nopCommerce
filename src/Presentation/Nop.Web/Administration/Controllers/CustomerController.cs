using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Admin.Models.Customers;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
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
using Nop.Core.Domain.Shipping;
using Nop.Services.Catalog;
using Nop.Admin.Models.Common;
using Nop.Services.Orders;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public class CustomerController : BaseNopController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly ICustomerReportService _customerReportService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly TaxSettings _taxSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IAddressService _addressService;
        private readonly IUserService _userService;
        private readonly CustomerSettings _customerSettings;
        private readonly ITaxService _taxService;
        private readonly IWorkContext _workContext;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderService _orderService;

        #endregion

        #region Constructors

        public CustomerController(ICustomerService customerService,
            ICustomerReportService customerReportService, IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService, DateTimeSettings dateTimeSettings,
            TaxSettings taxSettings, RewardPointsSettings rewardPointsSettings,
            ICountryService countryService, IStateProvinceService stateProvinceService, 
            IAddressService addressService, IUserService userService, 
            CustomerSettings customerSettings, ITaxService taxService, 
            IWorkContext workContext, IPriceFormatter priceFormatter,
            IOrderService orderService)
        {
            this._customerService = customerService;
            this._customerReportService = customerReportService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._dateTimeSettings = dateTimeSettings;
            this._taxSettings = taxSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._addressService = addressService;
            this._userService = userService;
            this._customerSettings = customerSettings;
            this._taxService = taxService;
            this._workContext = workContext;
            this._priceFormatter = priceFormatter;
            this._orderService = orderService;
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
            var customers = _customerService.GetAllCustomers(null,null, null, null, 0, 10);
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
                    model1.FullName = x.GetFullName();
                    model1.CustomerRoleNames = GetCustomerRolesNames(x.CustomerRoles.ToList());
                    model1.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
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

            string searchCustomerEmail = command.FilterDescriptors.GetValueFromAppliedFilters("searchCustomerEmail");

            var customers = _customerService.GetAllCustomers(null, null, 
                searchCustomerRoleIds.ToArray(), searchCustomerEmail, command.Page - 1, command.PageSize);
            var gridModel = new GridModel<CustomerModel>
            {
                Data = customers.Select(x =>
                {
                    var model = x.ToModel();
                    model.FullName = x.GetFullName();
                    model.CustomerRoleNames = GetCustomerRolesNames(x.CustomerRoles.ToList());
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
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

            ViewData["searchCustomerEmail"] = model.SearchEmail;

            //customer roles
            var customerRoles = _customerService.GetAllCustomerRoles(true);
            model.AvailableCustomerRoles = customerRoles.ToList();

            //laod customers
            var customers = _customerService.GetAllCustomers(null, null,
               model.SearchCustomerRoleIds, model.SearchEmail, 0, 10);
            //customer list
            model.Customers = new GridModel<CustomerModel>
            {
                Data = customers.Select(x =>
                {
                    var model1 = x.ToModel();
                    model1.FullName = x.GetFullName();
                    model1.CustomerRoleNames = GetCustomerRolesNames(x.CustomerRoles.ToList());
                    model1.CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc);
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
            model.GenderEnabled = _customerSettings.GenderEnabled;
            model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _customerSettings.CompanyEnabled;

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
                if (_customerSettings.GenderEnabled)
                    _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.Gender, model.Gender);
                _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
                _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
                if (_customerSettings.DateOfBirthEnabled)
                    _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.DateOfBirth, model.DateOfBirth);
                if (_customerSettings.CompanyEnabled)
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
            model.GenderEnabled = _customerSettings.GenderEnabled;
            model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _customerSettings.CompanyEnabled;
            return View(model);

        }

        public ActionResult Edit(int id)
        {
            var customer = _customerService.GetCustomerById(id);
            if (customer == null || customer.Deleted)
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
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(customer.CreatedOnUtc, DateTimeKind.Utc);
            //form fields
            model.GenderEnabled = _customerSettings.GenderEnabled;
            model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _customerSettings.CompanyEnabled;
            //customer roles
            var customerRoles = _customerService.GetAllCustomerRoles(true);
            model.AvailableCustomerRoles = customerRoles.ToList();
            model.SelectedCustomerRoleIds = customer.CustomerRoles.Select(cr => cr.Id).ToArray();
            //reward points gistory
            model.DisplayRewardPointsHistory = _rewardPointsSettings.Enabled;
            model.AddRewardPointsValue = 0;
            model.AddRewardPointsMessage = "Some comment here...";

            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Edit(CustomerModel model, bool continueEditing)
        {
            var customer = _customerService.GetCustomerById(model.Id);
            if (customer == null || customer.Deleted)
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
                if (_customerSettings.GenderEnabled)
                    _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.Gender, model.Gender);
                _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
                _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
                if (_customerSettings.DateOfBirthEnabled)
                    _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.DateOfBirth, model.DateOfBirth);
                if (_customerSettings.CompanyEnabled)
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
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(customer.CreatedOnUtc, DateTimeKind.Utc);
            //form fields
            model.GenderEnabled = _customerSettings.GenderEnabled;
            model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _customerSettings.CompanyEnabled;
            //customer roles
            var customerRoles = _customerService.GetAllCustomerRoles(true);
            model.AvailableCustomerRoles = customerRoles.ToList();
            //reward points gistory
            model.DisplayRewardPointsHistory = _rewardPointsSettings.Enabled;
            model.AddRewardPointsValue = 0;
            model.AddRewardPointsMessage = "Some comment here...";
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
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(rph.CreatedOnUtc, DateTimeKind.Utc)
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
                throw new ArgumentException("No customer found with the specified id");

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
            model.Address.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(true))
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString() });
            model.Address.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });

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
            model.Address.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(true))
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == model.Address.CountryId) });
            //states
            var states = model.Address.CountryId.HasValue ? _stateProvinceService.GetStateProvincesByCountryId(model.Address.CountryId.Value, true).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == model.Address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });
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
            model.Address.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(true))
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == address.CountryId) });
            //states
            var states = address.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(address.Country.Id, true).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });

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
                throw new ArgumentException("No address found with the specified id");

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
            model.Address.AvailableCountries.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
            foreach (var c in _countryService.GetAllCountries(true))
                model.Address.AvailableCountries.Add(new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = (c.Id == address.CountryId) });
            //states
            var states = address.Country != null ? _stateProvinceService.GetStateProvincesByCountryId(address.Country.Id, true).ToList() : new List<StateProvince>();
            if (states.Count > 0)
            {
                foreach (var s in states)
                    model.Address.AvailableStates.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString(), Selected = (s.Id == address.StateProvinceId) });
            }
            else
                model.Address.AvailableStates.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });

            return View(model);
        }

        #endregion

        #region User accounts

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult AssociatedUserSelect(GridCommand command, int customerId)
        {
            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id", "customerId");

            var users = customer.AssociatedUsers;
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
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc)
                    };
                }),
                Total = users.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult UserSelectAll(GridCommand command, string userEmailStartsWith)
        {
            var users = _userService.GetUsers(userEmailStartsWith, null, command.Page - 1, command.PageSize);

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
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc)
                    };
                }),
                Total = users.TotalCount
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }


        [HttpPost]
        public ActionResult AssociateNewUserAccount(int userId, int customerId)
        {
            var user = _userService.GetUserById(userId);
            if (user == null)
                throw new ArgumentException("No user found with the specified id", "userId");

            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id", "customerId");

            customer.AssociatedUsers.Add(user);
            _customerService.UpdateCustomer(customer);

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeassociateNewUserAccount(int userId, int customerId)
        {
            var user = _userService.GetUserById(userId);
            if (user == null)
                throw new ArgumentException("No user found with the specified id", "userId");

            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id", "customerId");

            customer.AssociatedUsers.Remove(user);
            _customerService.UpdateCustomer(customer);

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Orders
        
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult OrderList(int customerId, GridCommand command)
        {
            var orders = _orderService.GetOrdersByCustomerId(customerId);

            var model = new GridModel<CustomerModel.OrderModel>
            {
                Data = orders.OrderBy(x => x.CreatedOnUtc).PagedForCommand(command)
                    .Select(order =>
                    {
                        var orderModel = new CustomerModel.OrderModel();
                        orderModel.Id = order.Id;
                        orderModel.OrderStatus = order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext);
                        orderModel.PaymentStatus = order.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext);
                        orderModel.ShippingStatus = order.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext);
                        orderModel.OrderTotal = _priceFormatter.FormatPrice(order.OrderTotal, true, false);
                        orderModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc);
                        return orderModel;
                    }),
                Total = orders.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }


        #endregion

        #region Reports

        public ActionResult Reports()
        {
            var model = new CustomerReportsModel();
            //customers by number of orders
            model.BestCustomersByNumberOfOrders = new BestCustomersReportModel();
            model.BestCustomersByNumberOfOrders.AvailableOrderStatuses = OrderStatus.Pending.ToSelectList(false).ToList();
            model.BestCustomersByNumberOfOrders.AvailableOrderStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            model.BestCustomersByNumberOfOrders.AvailablePaymentStatuses = PaymentStatus.Pending.ToSelectList(false).ToList();
            model.BestCustomersByNumberOfOrders.AvailablePaymentStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            model.BestCustomersByNumberOfOrders.AvailableShippingStatuses = ShippingStatus.NotYetShipped.ToSelectList(false).ToList();
            model.BestCustomersByNumberOfOrders.AvailableShippingStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //customers by order total
            model.BestCustomersByOrderTotal = new BestCustomersReportModel();
            model.BestCustomersByOrderTotal.AvailableOrderStatuses = OrderStatus.Pending.ToSelectList(false).ToList();
            model.BestCustomersByOrderTotal.AvailableOrderStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            model.BestCustomersByOrderTotal.AvailablePaymentStatuses = PaymentStatus.Pending.ToSelectList(false).ToList();
            model.BestCustomersByOrderTotal.AvailablePaymentStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            model.BestCustomersByOrderTotal.AvailableShippingStatuses = ShippingStatus.NotYetShipped.ToSelectList(false).ToList();
            model.BestCustomersByOrderTotal.AvailableShippingStatuses.Insert(0, new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            
            return View(model);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ReportBestCustomersByOrderTotalList(GridCommand command, BestCustomersReportModel model)
        {
            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            OrderStatus? orderStatus = model.OrderStatusId > 0 ? (OrderStatus?)(model.OrderStatusId) : null;
            PaymentStatus? paymentStatus = model.PaymentStatusId > 0 ? (PaymentStatus?)(model.PaymentStatusId) : null;
            ShippingStatus? shippingStatus = model.ShippingStatusId > 0 ? (ShippingStatus?)(model.ShippingStatusId) : null;


            var items = _customerReportService.GetBestCustomersReport(startDateValue, endDateValue,
                orderStatus, paymentStatus, shippingStatus, 1);
            var gridModel = new GridModel<BestCustomerReportLineModel>
            {
                Data = items.Select(x =>
                {
                    var m = new BestCustomerReportLineModel()
                    {
                        CustomerId = x.CustomerId,
                        OrderTotal = _priceFormatter.FormatPrice(x.OrderTotal, true, false),
                        OrderCount = x.OrderCount,
                    };
                    var customer = _customerService.GetCustomerById(x.CustomerId);
                    if (customer != null)
                        m.CustomerName = customer.IsGuest() ? "Guest" : customer.GetFullName();
                    return m;
                }),
                Total = items.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ReportBestCustomersByNumberOfOrdersList(GridCommand command, BestCustomersReportModel model)
        {
            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            OrderStatus? orderStatus = model.OrderStatusId > 0 ? (OrderStatus?)(model.OrderStatusId) : null;
            PaymentStatus? paymentStatus = model.PaymentStatusId > 0 ? (PaymentStatus?)(model.PaymentStatusId) : null;
            ShippingStatus? shippingStatus = model.ShippingStatusId > 0 ? (ShippingStatus?)(model.ShippingStatusId) : null;


            var items = _customerReportService.GetBestCustomersReport(startDateValue, endDateValue,
                orderStatus, paymentStatus, shippingStatus, 2);
            var gridModel = new GridModel<BestCustomerReportLineModel>
            {
                Data = items.Select(x =>
                {
                    var m = new BestCustomerReportLineModel()
                    {
                        CustomerId = x.CustomerId,
                        OrderTotal = _priceFormatter.FormatPrice(x.OrderTotal, true, false),
                        OrderCount = x.OrderCount,
                    };
                    var customer = _customerService.GetCustomerById(x.CustomerId);
                    if (customer != null)
                        m.CustomerName = customer.IsGuest() ? "Guest" : customer.GetFullName();
                    return m;
                }),
                Total = items.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [NonAction]
        protected virtual IList<RegisteredCustomerReportLineModel> GetReportRegisteredCustomersModel()
        {
            var report = new List<RegisteredCustomerReportLineModel>();
            report.Add(new RegisteredCustomerReportLineModel()
            {
                Period = _localizationService.GetResource("Admin.Customers.Reports.RegisteredCustomers.Fields.Period.7days"),
                Customers = _customerReportService.GetRegisteredCustomersReport(7)
            });

            report.Add(new RegisteredCustomerReportLineModel()
            {
                Period = _localizationService.GetResource("Admin.Customers.Reports.RegisteredCustomers.Fields.Period.14days"),
                Customers = _customerReportService.GetRegisteredCustomersReport(14)
            });
            report.Add(new RegisteredCustomerReportLineModel()
            {
                Period = _localizationService.GetResource("Admin.Customers.Reports.RegisteredCustomers.Fields.Period.month"),
                Customers = _customerReportService.GetRegisteredCustomersReport(30)
            });
            report.Add(new RegisteredCustomerReportLineModel()
            {
                Period = _localizationService.GetResource("Admin.Customers.Reports.RegisteredCustomers.Fields.Period.year"),
                Customers = _customerReportService.GetRegisteredCustomersReport(365)
            });

            return report;
        }
        [ChildActionOnly]
        public ActionResult ReportRegisteredCustomers()
        {
            var model = GetReportRegisteredCustomersModel();
            return PartialView(model);
        }
        [GridAction(EnableCustomBinding = true)]
        public ActionResult ReportRegisteredCustomersList(GridCommand command)
        {
            var model = GetReportRegisteredCustomersModel();
            var gridModel = new GridModel<RegisteredCustomerReportLineModel>
            {
                Data = model,
                Total = model.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }
        
        #endregion
    }
}
