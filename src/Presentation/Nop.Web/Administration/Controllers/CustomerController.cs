using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Tax;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
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
        private readonly IAddressService _addressService;
        
        #endregion Fields

        #region Constructors

        public CustomerController(ICustomerService customerService, IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService, DateTimeSettings dateTimeSettings,
            TaxSettings taxSettings, RewardPointsSettings rewardPointsSettings,
            ICountryService countryService, IAddressService addressService)
        {
            this._customerService = customerService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
            this._dateTimeSettings = dateTimeSettings;
            this._taxSettings = taxSettings;
            this._rewardPointsSettings = rewardPointsSettings;
            this._countryService = countryService;
            this._addressService = addressService;
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


        /// <summary>
        /// Gets VAT Number status name
        /// </summary>
        /// <param name="status">VAT Number status</param>
        /// <returns>VAT Number status name</returns>
        [NonAction]
        private string GetVatNumberStatusName(VatNumberStatus status)
        {
            return _localizationService.GetResource(string.Format("Admin.Common.VatNumberStatus.{0}", status.ToString()));
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            //TODO add filtering by customer role, registration date, email, etc
            var customers = _customerService.GetAllCustomers(null,null, 0, 10);
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
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            var customers = _customerService.GetAllCustomers(null, null, command.Page - 1, command.PageSize);
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

        public ActionResult Create()
        {
            var model = new CustomerModel();
            model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem() { Text = tzi.DisplayName, Value = tzi.Id, Selected = (tzi.Id == _dateTimeHelper.DefaultStoreTimeZone.Id) });
            model.DisplayVatNumber = false;
            model.VatNumberStatusNote = GetVatNumberStatusName(VatNumberStatus.Empty);
            //customer roles
            var customerRoles = _customerService.GetAllCustomerRoles(true);
            model.AvailableCustomerRoles = customerRoles.ToList();
            model.SelectedCustomerRoleIds = new int[0];

            return View(model);
        }

        [HttpPost, FormValueExists("save", "save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Create(CustomerModel model, bool continueEditing)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var customer = model.ToEntity();
            customer.CustomerGuid = Guid.NewGuid();
            customer.CreatedOnUtc = DateTime.UtcNow;
            _customerService.InsertCustomer(customer);
            _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.Gender, model.Gender);
            _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
            _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
            _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.DateOfBirth, model.DateOfBirth);
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
            model.VatNumberStatusNote = GetVatNumberStatusName(customer.VatNumberStatus);
            model.FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName);
            model.LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName);
            model.Gender = customer.GetAttribute<string>(SystemCustomerAttributeNames.Gender);
            model.DateOfBirth = customer.GetAttribute<DateTime?>(SystemCustomerAttributeNames.DateOfBirth);
            model.CreatedOnStr = _dateTimeHelper.ConvertToUserTime(customer.CreatedOnUtc, DateTimeKind.Utc).ToString();
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var customer = _customerService.GetCustomerById(model.Id);
            if (customer == null) 
                throw new ArgumentException("No customer found with the specified id", "id");

            customer = model.ToEntity(customer);

            //UNDONE set VAT number after country is saved
            //TODO get country from default address (can this be "hacked" during checkout? for example, selecting a new address with new country)
            //if (_taxSettings.EuVatEnabled)
            //{
            //    string prevVatNumber = customer.VatNumber;
            //    customer.VatNumber = txtVatNumber.Text;
            //    //set VAT number status
            //    if (!txtVatNumber.Text.Trim().Equals(prevVatNumber))
            //        customer.VatNumberStatus = _taxService.GetVatNumberStatus(_countryService.GetCountryById(_countryId), customer.VatNumber);
            //}

            _customerService.UpdateCustomer(customer);
            //customer attributes
            _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.Gender, model.Gender);
            _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
            _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
            _customerService.SaveCustomerAttribute(customer, SystemCustomerAttributeNames.DateOfBirth, model.DateOfBirth);
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
        
        //Reward points history
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

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult RewardPointsHistoryAdd(int customerId, int addRewardPointsValue, string addRewardPointsMessage)
        {
            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id", "id");

            customer.AddRewardPointsHistoryEntry(addRewardPointsValue, addRewardPointsMessage);
            _customerService.UpdateCustomer(customer);

            return Json(new { Result = "1" }, JsonRequestBehavior.AllowGet);
        }

        //Addresses
        [GridAction]
        public ActionResult AddressesSelect(int customerId)
        {
            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id", "id");

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
        public ActionResult AddressDelete(int customerId, int addressId)
        {
            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id", "id");

            var address = customer.Addresses.Where(a => a.Id == addressId).FirstOrDefault();
            customer.RemoveAddress(address);
            _customerService.UpdateCustomer(customer);
            //TODO should we delete the address record?

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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var customer = _customerService.GetCustomerById(model.CustomerId);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id");

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

        public ActionResult AddressEdit(int addressId, int customerId)
        {
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
        public ActionResult AddressEdit(CustomerAddressModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var address = _addressService.GetAddressById(model.Address.Id);
            address = model.Address.ToEntity(address);
            _addressService.UpdateAddress(address);

            return RedirectToAction("AddressEdit", new { addressId = model.Address.Id, customerId = model.CustomerId });
        }
           
        #endregion
    }
}
