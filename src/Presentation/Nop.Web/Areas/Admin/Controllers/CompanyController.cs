using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Companies;
using Nop.Core.Domain.Discounts;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Companies;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Companies;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class CompanyController : BaseAdminController
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly ICompanyModelFactory _companyModelFactory;
        private readonly ICompanyService _companyService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerModelFactory _customerModelFactory;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressService _addressService;

        #endregion

        #region Ctor

        public CompanyController(IAclService aclService,
            ICompanyModelFactory companyModelFactory,
            ICompanyService companyService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDiscountService discountService,
            IExportManager exportManager,
            IImportManager importManager,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IProductService productService,
            IStaticCacheManager staticCacheManager,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            ICustomerModelFactory customerModelFactory,
            IAddressAttributeParser addressAttributeParser,
            IAddressService addressService)
        {
            _aclService = aclService;
            _companyModelFactory = companyModelFactory;
            _companyService = companyService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _discountService = discountService;
            _exportManager = exportManager;
            _importManager = importManager;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _productService = productService;
            _staticCacheManager = staticCacheManager;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
            _customerModelFactory = customerModelFactory;
            _addressAttributeParser = addressAttributeParser;
            _addressService = addressService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(Company company, CompanyModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(company,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        #endregion

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //prepare model
            var model = await _companyModelFactory.PrepareCompanySearchModelAsync(new CompanySearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(CompanySearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _companyModelFactory.PrepareCompanyListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #region Create / Edit / Delete

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //prepare model
            var model = await _companyModelFactory.PrepareCompanyModelAsync(new CompanyModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(CompanyModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var company = model.ToEntity<Company>();
                await _companyService.InsertCompanyAsync(company);

                //locales
                await UpdateLocalesAsync(company, model);

                await _companyService.UpdateCompanyAsync(company);

                await _staticCacheManager.ClearAsync();

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewCompany",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewCompany"), company.Name), company);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Company.Companies.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = company.Id });
            }

            //prepare model
            model = await _companyModelFactory.PrepareCompanyModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a Company with the specified id
            var company = await _companyService.GetCompanyByIdAsync(id);
            if (company == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _companyModelFactory.PrepareCompanyModelAsync(null, company);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(CompanyModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a Company with the specified id
            var company = await _companyService.GetCompanyByIdAsync(model.Id);
            if (company == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                company = model.ToEntity(company);
                await _companyService.UpdateCompanyAsync(company);

                await _staticCacheManager.ClearAsync();

                //locales
                await UpdateLocalesAsync(company, model);

                //activity log
                await _customerActivityService.InsertActivityAsync("EditCompany",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditCompany"), company.Name), company);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Company.Companies.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = company.Id });
            }

            //prepare model
            model = await _companyModelFactory.PrepareCompanyModelAsync(model, company, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a Company with the specified id
            var company = await _companyService.GetCompanyByIdAsync(id);
            if (company == null)
                return RedirectToAction("List");

            var companyCustomers = await _companyService.GetCompanyCustomersByCompanyIdAsync(id);
            foreach (var companyCustomer in companyCustomers)
            {
                var customer = await _customerService.GetCustomerByIdAsync(companyCustomer.CustomerId);
                var addresses = await _customerService.GetAddressesByCustomerIdAsync(companyCustomer.CustomerId);
                foreach (var address in addresses)
                {
                    await _customerService.RemoveCustomerAddressAsync(customer, address);
                    customer.ShippingAddressId = null;
                    customer.BillingAddressId = null;
                    await _customerService.UpdateCustomerAsync(customer);
                    //now delete the address record
                    await _addressService.DeleteAddressAsync(address);
                    await _companyService.DeleteCompanyCustomerAsync(companyCustomer);
                }
            }
            await _companyService.DeleteCompanyAsync(company);

            await _staticCacheManager.ClearAsync();

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteCompany",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteCompany"), company.Name), company);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Company.Companies.Deleted"));

            return RedirectToAction("List");
        }

        #endregion

        #region Customers

        [HttpPost]
        public virtual async Task<IActionResult> CustomerList(CompanyCustomerSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return await AccessDeniedDataTablesJson();

            //try to get a Company with the specified id
            var company = await _companyService.GetCompanyByIdAsync(searchModel.CompanyId)
                ?? throw new ArgumentException("No Company found with the specified id");

            //prepare model
            var model = await _companyModelFactory.PrepareCompanyCustomerListModelAsync(searchModel, company);

            return Json(model);
        }

        public virtual async Task<IActionResult> CustomerUpdate(CompanyCustomerModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a product Company with the specified id
            var companyCustomer = await _companyService.GetCompanyCustomersByIdAsync(model.Id)
                ?? throw new ArgumentException("No product Company mapping found with the specified id");

            //fill entity from product
            companyCustomer = model.ToEntity(companyCustomer);
            await _companyService.UpdateCompanyCustomerAsync(companyCustomer);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> CustomerDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a product Company with the specified id
            var companyCustomer = await _companyService.GetCompanyCustomersByIdAsync(id)
                ?? throw new ArgumentException("No Customer Company mapping found with the specified id", nameof(id));

            var customer = await _customerService.GetCustomerByIdAsync(companyCustomer.CustomerId);
            var addresses = await _customerService.GetAddressesByCustomerIdAsync(companyCustomer.CustomerId);
            foreach (var address in addresses)
            {
                await _customerService.RemoveCustomerAddressAsync(customer, address);
                customer.ShippingAddressId = null;
                customer.BillingAddressId = null;
                await _customerService.UpdateCustomerAsync(customer);
            }

            await _companyService.DeleteCompanyCustomerAsync(companyCustomer);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> CustomerAddPopup(int companyId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var searchModel = new AddCustomerToCompanySearchModel();
            searchModel.CompanyId = companyId;
            //prepare model
            var model = await _companyModelFactory.PrepareAddCustomerToCompanySearchModelAsync(searchModel);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CustomerAddPopupList(AddCustomerToCompanySearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _companyModelFactory.PrepareAddCustomerToCompanyListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> CustomerAddPopup(AddCustomerToCompanyModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //get selected products
            var selectedCustomers = await _customerService.GetCustomersByIdsAsync(model.SelectedCustomerIds.ToArray());
            if (selectedCustomers.Any())
            {
                var existingCompanyCustomers = await _companyService.GetCompanyCustomersByCompanyIdAsync(model.CompanyId, showHidden: true);
                foreach (var customer in selectedCustomers)
                {
                    //whether product Company with such parameters already exists
                    if (_companyService.FindCompanyCustomer(existingCompanyCustomers, customer.Id, model.CompanyId) != null)
                        continue;

                    var addressId = 0;
                    //foreach (var existingCompanyCustomer in existingCompanyCustomers)
                    //{
                    if (existingCompanyCustomers.Any())
                    {
                        var addresses = await _customerService.GetAddressesByCustomerIdAsync(existingCompanyCustomers.FirstOrDefault().CustomerId);
                        var customerAddresses = await _customerService.GetCustomerAddressesByCustomerIdAsync(existingCompanyCustomers.FirstOrDefault().CustomerId);
                        foreach (var address in addresses)
                        {
                            if (_customerService.FindCustomerAddressMapping(customerAddresses, customer.Id, address.Id) != null)
                            {
                                addressId = address.Id;
                                continue;
                            }

                            await _customerService.InsertCustomerAddressAsync(customer, address);
                            addressId = address.Id;
                        }
                    }
                    //}
                    await _companyService.InsertCompanyCustomerAsync(new CompanyCustomer { CompanyId = model.CompanyId, CustomerId = customer.Id });
                    if (addressId > 0)
                    {
                        customer.ShippingAddressId = addressId;
                        customer.BillingAddressId = addressId;
                        await _customerService.UpdateCustomerAsync(customer);
                    }
                    else if(existingCompanyCustomers.Any())
                    {
                        var newAddressId = 0;
                        foreach (var existingCompanyCustomer in existingCompanyCustomers)
                        {
                            var addressMappings = await _customerService.GetCustomerAddressesByCustomerIdAsync(existingCompanyCustomer.CustomerId);
                            if (addressMappings.Any())
                                newAddressId = addressMappings.FirstOrDefault().AddressId;
                        }
                        if (newAddressId > 0)
                        {
                            customer.ShippingAddressId = newAddressId;
                            customer.BillingAddressId = newAddressId;
                            await _customerService.UpdateCustomerAsync(customer);
                        }
                    }
                }
            }

            ViewBag.RefreshPage = true;

            return View(new AddCustomerToCompanySearchModel());
        }

        #endregion

        #region Addresses

        [HttpPost]
        public virtual async Task<IActionResult> AddressesSelect(CustomerAddressSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return await AccessDeniedDataTablesJson();

            //try to get a company with the specified id
            var company = await _companyService.GetCompanyByIdAsync(searchModel.CustomerId)
                ?? throw new ArgumentException("No company found with the specified id");

            //prepare model
            var model = await _companyModelFactory.PrepareCompanyCustomerAddressListModelAsync(searchModel, company);

            return Json(model);
        }

        public virtual async Task<IActionResult> AddressCreate(int customerId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var company = await _companyService.GetCompanyByIdAsync(customerId);
            if (company == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _companyModelFactory.PrepareCompanyCustomerAddressModelAsync(new CustomerAddressModel(), company, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddressCreate(CustomerAddressModel model, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var companyId = model.CustomerId;
            //try to get a customer with the specified id
            var companies = await _companyService.GetCompanyCustomersByCompanyIdAsync(model.CustomerId);
            if (!companies.Any())
                return RedirectToAction("List");

            //custom address attributes
            var customAttributes = await _addressAttributeParser.ParseCustomAddressAttributesAsync(form);
            var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);
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

                await _addressService.InsertAddressAsync(address);
                foreach (var company in companies)
                {
                    var customer = await _customerService.GetCustomerByIdAsync(company.CustomerId);
                    await _customerService.InsertCustomerAddressAsync(customer, address);
                    customer.ShippingAddressId = address.Id;
                    customer.BillingAddressId = address.Id;
                    await _customerService.UpdateCustomerAsync(customer);

                }
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Companies.Company.Addresses.Added"));
                return RedirectToAction("Edit", new { id = companyId });
            }

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddressDelete(int id, int customerId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();
            var getCompanyCustomers = await _companyService.GetCompanyCustomersByCompanyIdAsync(customerId);
            foreach (var getCompanyCustomer in getCompanyCustomers)
            {
                //try to get a customer with the specified id
                var customer = await _customerService.GetCustomerByIdAsync(getCompanyCustomer.CustomerId)
                    ?? throw new ArgumentException("No customer found with the specified id", nameof(getCompanyCustomer.CustomerId));

                //try to get an address with the specified id
                var address = await _customerService.GetCustomerAddressAsync(customer.Id, id);
                if (address == null)
                    continue;

                await _customerService.RemoveCustomerAddressAsync(customer, address);
                await _customerService.UpdateCustomerAsync(customer);

                //now delete the address record
                await _addressService.DeleteAddressAsync(address);
            }
            return new NullJsonResult();
        }
        #endregion
    }
}