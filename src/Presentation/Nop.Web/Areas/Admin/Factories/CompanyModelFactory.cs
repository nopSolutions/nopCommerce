using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Areas.Admin.Models.Companies;
using Nop.Core;
using Nop.Services.Companies;
using Nop.Core.Domain.Companies;
using Nop.Services.Customers;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Core.Domain.Common;
using System.Text;
using System.Net;
using Nop.Services.Common;
using Nop.Core.Domain.Customers;

namespace Nop.Web.Areas.Admin.Factories
{
    public partial class CompanyModelFactory : ICompanyModelFactory
    {
        #region Fields

        private readonly ICompanyService _companyService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly AddressSettings _addressSettings;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IAddressModelFactory _addressModelFactory;

        #endregion

        #region Ctor

        public CompanyModelFactory(ICompanyService companyService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            ICustomerService customerService,
            IWorkContext workContext,
            AddressSettings addressSettings,
            IAddressAttributeFormatter addressAttributeFormatter,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IAddressModelFactory addressModelFactory)
        {
            _companyService = companyService;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _customerService = customerService;
            _workContext = workContext;
            _addressSettings = addressSettings;
            _addressAttributeFormatter = addressAttributeFormatter;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _addressModelFactory = addressModelFactory;
        }

        #endregion

        #region Utilities

        protected virtual CompanyCustomerSearchModel PrepareCompanyCustomerSearchModel(CompanyCustomerSearchModel searchModel, Company company)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (company == null)
                throw new ArgumentNullException(nameof(company));

            searchModel.CompanyId = company.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        public virtual async Task<CompanySearchModel> PrepareCompanySearchModelAsync(CompanySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var customer = await _workContext.GetCurrentCustomerAsync();

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual async Task<CompanyListModel> PrepareCompanyListModelAsync(CompanySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            //get categories
            var companies = await _companyService.GetAllCompaniesAsync(name: searchModel.SearchCompanyName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new CompanyListModel().PrepareToGridAsync(searchModel, companies, () =>
            {
                return companies.SelectAwait(async company =>
                {
                    var customer = await _workContext.GetCurrentCustomerAsync();
                    //fill in model values from the entity
                    var companyModel = company.ToModel<CompanyModel>();
                    return companyModel;
                });
            });

            return model;
        }

        public virtual async Task<CompanyModel> PrepareCompanyModelAsync(CompanyModel model, Company company, bool excludeProperties = false)
        {
            Action<CompanyLocalizedModel, int> localizedModelConfiguration = null;

            if (company != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = company.ToModel<CompanyModel>();
                }

                //prepare nested search model
                PrepareCompanyCustomerSearchModel(model.CompanyCustomerSearchModel, company);

                //define localized model configuration action
                localizedModelConfiguration = async (locale, languageId) =>
                {
                    locale.Name = await _localizationService.GetLocalizedAsync(company, entity => entity.Name, languageId, false, false);
                };
                var companyCustomers = await _companyService.GetCompanyCustomersByCompanyIdAsync(company.Id);
                if (companyCustomers.Any())
                    model.CustomerExist = true;
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        public virtual async Task<CompanyCustomerListModel> PrepareCompanyCustomerListModelAsync(CompanyCustomerSearchModel searchModel, Company company)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (company == null)
                throw new ArgumentNullException(nameof(company));

            //get product categories
            var companyCustomers = await _companyService.GetCompanyCustomersByCompanyIdAsync(company.Id,
                showHidden: true,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = await new CompanyCustomerListModel().PrepareToGridAsync(searchModel, companyCustomers, () =>
            {
                return companyCustomers.SelectAwait(async companyCustomer =>
                {
                    //fill in model values from the entity
                    var companyCustomerModel = companyCustomer.ToModel<CompanyCustomerModel>();

                    //fill in additional values (not existing in the entity)
                    companyCustomerModel.CustomerFullName = (await _customerService.GetCustomerFullNameAsync(await _customerService.GetCustomerByIdAsync(companyCustomer.CustomerId)));
                    companyCustomerModel.CustomerId = companyCustomer.CustomerId;
                    companyCustomerModel.Email = (await _customerService.GetCustomerByIdAsync(companyCustomer.CustomerId))?.Email;

                    return companyCustomerModel;
                });
            });

            return model;
        }
        public virtual async Task<AddCustomerToCompanySearchModel> PrepareAddCustomerToCompanySearchModelAsync(AddCustomerToCompanySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var customer = await _workContext.GetCurrentCustomerAsync();

            //prepare page parameters
            searchModel.SetPopupGridPageSize();
            return searchModel;
        }

        public virtual async Task<AddCustomerToCompanyListModel> PrepareAddCustomerToCompanyListModelAsync(AddCustomerToCompanySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var customerRoleIds = new[] { (await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName)).Id };
            //get customers
            var customers = await _customerService.GetAllCustomersAsync(
                 email: searchModel.SearchCustomerEmail, customerRoleIds: customerRoleIds,
                 pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            IPagedList<Customer> filteredCustomers = new PagedList<Customer>(new List<Customer>(), searchModel.Page - 1, searchModel.PageSize, customers.TotalCount);
            foreach (var customer in customers)
            {
                var companyCustomerMapping = await _companyService.GetCompanyCustomersByCustomerIdAsync(customer.Id);
                if (companyCustomerMapping.Count == 0)
                    filteredCustomers.Add(customer);
            }

            //prepare grid model
            var model = await new AddCustomerToCompanyListModel().PrepareToGridAsync(searchModel, filteredCustomers, () =>
            {
                return filteredCustomers.SelectAwait(async customer =>
                {
                    var customerModel = customer.ToModel<CustomerModel>();

                    customerModel.FullName = (await _customerService.GetCustomerFullNameAsync(await _customerService.GetCustomerByIdAsync(customer.Id)));
                    customerModel.Id = customer.Id;
                    customerModel.Active = customer.Active;
                    customerModel.Email = (await _customerService.GetCustomerByIdAsync(customer.Id))?.Email;

                    return customerModel;
                });
            });

            return model;
        }
        public virtual async Task<CustomerAddressListModel> PrepareCompanyCustomerAddressListModelAsync(CustomerAddressSearchModel searchModel, Company company)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (company == null)
                throw new ArgumentNullException(nameof(company));

            var model = new CustomerAddressListModel();
            var getCompanyCustomers = await _companyService.GetCompanyCustomersByCompanyIdAsync(company.Id);
            IPagedList<Address> addressesList = new PagedList<Address>(new List<Address>(), 0, 1);
            foreach (var getCompanyCustomer in getCompanyCustomers)
            {
                var customer = await _customerService.GetCustomerByIdAsync(getCompanyCustomer.CustomerId);
                //get customer addresses
                var addresses = (await _customerService.GetAddressesByCustomerIdAsync(customer.Id))
                    .OrderByDescending(address => address.CreatedOnUtc).ThenByDescending(address => address.Id).ToList()
                    .ToPagedList(searchModel);

                foreach (var address in addresses)
                {
                    if (!addressesList.Where(x => x.Id == address.Id).Any())
                        addressesList.Add(address);
                }
                searchModel.CustomerId = customer.Id;
            }

            //prepare list model
            model = await new CustomerAddressListModel().PrepareToGridAsync(searchModel, addressesList, () =>
            {
                return addressesList.SelectAwait(async address =>
                {
                    //fill in model values from the entity        
                    var addressModel = address.ToModel<AddressModel>();

                    var customerAddressMapping = await _customerService.GetCustomerAddressesByAddressIdAsync(address.Id);
                    addressModel.CustomerId = customerAddressMapping.Any() ? customerAddressMapping.FirstOrDefault().CustomerId : 0;
                    addressModel.CountryName = (await _countryService.GetCountryByAddressAsync(address))?.Name;
                    addressModel.StateProvinceName = (await _stateProvinceService.GetStateProvinceByAddressAsync(address))?.Name;

                    //fill in additional values (not existing in the entity)
                    await PrepareModelAddressHtmlAsync(addressModel, address);

                    return addressModel;
                });
            });
            return model;
        }

        public virtual async Task<CustomerAddressModel> PrepareCompanyCustomerAddressModelAsync(CustomerAddressModel model,
           Company company, Address address, bool excludeProperties = false)
        {
            if (company == null)
                throw new ArgumentNullException(nameof(company));

            if (address != null)
            {
                //fill in model values from the entity
                model ??= new CustomerAddressModel();

                //whether to fill in some of properties
                if (!excludeProperties)
                    model.Address = address.ToModel(model.Address);
            }

            model.CustomerId = company.Id;

            //prepare address model
            await _addressModelFactory.PrepareAddressModelAsync(model.Address, address);
            model.Address.FirstNameRequired = true;
            model.Address.LastNameRequired = true;
            model.Address.EmailRequired = true;
            model.Address.CompanyRequired = _addressSettings.CompanyRequired;
            model.Address.CityRequired = _addressSettings.CityRequired;
            model.Address.CountyRequired = _addressSettings.CountyRequired;
            model.Address.StreetAddressRequired = _addressSettings.StreetAddressRequired;
            model.Address.StreetAddress2Required = _addressSettings.StreetAddress2Required;
            model.Address.ZipPostalCodeRequired = _addressSettings.ZipPostalCodeRequired;
            model.Address.PhoneRequired = _addressSettings.PhoneRequired;
            model.Address.FaxRequired = _addressSettings.FaxRequired;

            return model;
        }

        protected virtual async Task PrepareModelAddressHtmlAsync(AddressModel model, Address address)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var addressHtmlSb = new StringBuilder("<div>");

            if (_addressSettings.CompanyEnabled && !string.IsNullOrEmpty(model.Company))
                addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.Company));

            if (_addressSettings.StreetAddressEnabled && !string.IsNullOrEmpty(model.Address1))
                addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.Address1));

            if (_addressSettings.StreetAddress2Enabled && !string.IsNullOrEmpty(model.Address2))
                addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.Address2));

            if (_addressSettings.CityEnabled && !string.IsNullOrEmpty(model.City))
                addressHtmlSb.AppendFormat("{0},", WebUtility.HtmlEncode(model.City));

            if (_addressSettings.CountyEnabled && !string.IsNullOrEmpty(model.County))
                addressHtmlSb.AppendFormat("{0},", WebUtility.HtmlEncode(model.County));

            if (_addressSettings.StateProvinceEnabled && !string.IsNullOrEmpty(model.StateProvinceName))
                addressHtmlSb.AppendFormat("{0},", WebUtility.HtmlEncode(model.StateProvinceName));

            if (_addressSettings.ZipPostalCodeEnabled && !string.IsNullOrEmpty(model.ZipPostalCode))
                addressHtmlSb.AppendFormat("{0}<br />", WebUtility.HtmlEncode(model.ZipPostalCode));

            if (_addressSettings.CountryEnabled && !string.IsNullOrEmpty(model.CountryName))
                addressHtmlSb.AppendFormat("{0}", WebUtility.HtmlEncode(model.CountryName));

            var customAttributesFormatted = await _addressAttributeFormatter.FormatAttributesAsync(address?.CustomAttributes);
            if (!string.IsNullOrEmpty(customAttributesFormatted))
            {
                //already encoded
                addressHtmlSb.AppendFormat("<br />{0}", customAttributesFormatted);
            }

            addressHtmlSb.Append("</div>");

            model.AddressHtml = addressHtmlSb.ToString();
        }

        #endregion
    }
}