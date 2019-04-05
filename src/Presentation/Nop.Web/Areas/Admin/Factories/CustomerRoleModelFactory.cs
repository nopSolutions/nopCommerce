using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Models.DataTables;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the customer role model factory implementation
    /// </summary>
    public partial class CustomerRoleModelFactory : ICustomerRoleModelFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CustomerRoleModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IProductService productService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _baseAdminModelFactory = baseAdminModelFactory;
            _customerService = customerService;
            _localizationService = localizationService;
            _productService = productService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare customer roles datatables model
        /// </summary>
        /// <param name="searchModel">Customer roles search model</param>
        /// <returns>Customer roles datatables model</returns>
        protected virtual DataTablesModel PrepareCustomerRoleGridModel(CustomerRoleSearchModel searchModel)
        {
            //prepare common properties
            var model = new DataTablesModel
            {
                Name = "customerroles-grid",
                UrlRead = new DataUrl("List", "CustomerRole", null),
                Length = searchModel.PageSize,
                LengthMenu = searchModel.AvailablePageSizes
            };

            //prepare filters to search
            model.Filters = null;

            //prepare model columns
            model.ColumnCollection = new List<ColumnProperty>()
            {
                new ColumnProperty(nameof(CustomerRoleModel.Name))
                {
                    Title = _localizationService.GetResource("Admin.Customers.CustomerRoles.Fields.Name"),
                    Width = "300"
                },
                new ColumnProperty(nameof(CustomerRoleModel.FreeShipping))
                {
                    Title = _localizationService.GetResource("Admin.Customers.CustomerRoles.Fields.FreeShipping"),
                    Width = "100",
                    Render = new RenderBoolean()
                },
                new ColumnProperty(nameof(CustomerRoleModel.TaxExempt))
                {
                    Title = _localizationService.GetResource("Admin.Customers.CustomerRoles.Fields.TaxExempt"),
                    Width = "100",
                    Render = new RenderBoolean()
                },
                new ColumnProperty(nameof(CustomerRoleModel.Active))
                {
                    Title = _localizationService.GetResource("Admin.Customers.CustomerRoles.Fields.Active"),
                    Width = "100",
                    Render = new RenderBoolean()
                },
                new ColumnProperty(nameof(CustomerRoleModel.IsSystemRole))
                {
                    Title = _localizationService.GetResource("Admin.Customers.CustomerRoles.Fields.IsSystemRole"),
                    Width = "100",
                    Render = new RenderBoolean()
                },
                new ColumnProperty(nameof(CustomerRoleModel.PurchasedWithProductName))
                {
                    Title = _localizationService.GetResource("Admin.Customers.CustomerRoles.Fields.PurchasedWithProduct"),
                    Width = "200"
                },
                new ColumnProperty(nameof(CustomerRoleModel.Id))
                {
                    Title = _localizationService.GetResource("Admin.Common.Edit"),
                    Width = "100",
                    Render = new RenderButtonEdit(new DataUrl("Edit"))
                }

            };

            //prepare column definitions
            model.ColumnDefinitions = new List<ColumnDefinition>
            {
                new ColumnDefinition()
                {
                    Targets = "-1",
                    ClassName =  StyleColumn.CenterAll
                }
            };

            return model;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare customer role search model
        /// </summary>
        /// <param name="searchModel">Customer role search model</param>
        /// <returns>Customer role search model</returns>
        public virtual CustomerRoleSearchModel PrepareCustomerRoleSearchModel(CustomerRoleSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.Grid = PrepareCustomerRoleGridModel(searchModel);

            return searchModel;
        }

        /// <summary>
        /// Prepare paged customer role list model
        /// </summary>
        /// <param name="searchModel">Customer role search model</param>
        /// <returns>Customer role list model</returns>
        public virtual CustomerRoleListModel PrepareCustomerRoleListModel(CustomerRoleSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var customerRoles = _customerService.GetAllCustomerRoles(true).ToPagedList(searchModel);

            //prepare grid model
            var model = new CustomerRoleListModel().PrepareToGrid(searchModel, customerRoles, () =>
            {
                return customerRoles.Select(role =>
                {
                    //fill in model values from the entity
                    var customerRoleModel = role.ToModel<CustomerRoleModel>();

                    //fill in additional values (not existing in the entity)
                    customerRoleModel.PurchasedWithProductName = _productService.GetProductById(role.PurchasedWithProductId)?.Name;

                    return customerRoleModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare customer role model
        /// </summary>
        /// <param name="model">Customer role model</param>
        /// <param name="customerRole">Customer role</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Customer role model</returns>
        public virtual CustomerRoleModel PrepareCustomerRoleModel(CustomerRoleModel model, CustomerRole customerRole, bool excludeProperties = false)
        {
            if (customerRole != null)
            {
                //fill in model values from the entity
                model = model ?? customerRole.ToModel<CustomerRoleModel>();
                model.PurchasedWithProductName = _productService.GetProductById(customerRole.PurchasedWithProductId)?.Name;
            }

            //set default values for the new model
            if (customerRole == null)
                model.Active = true;

            //prepare available tax display types
            _baseAdminModelFactory.PrepareTaxDisplayTypes(model.TaxDisplayTypeValues, false);

            return model;
        }

        /// <summary>
        /// Prepare customer role product search model
        /// </summary>
        /// <param name="searchModel">Customer role product search model</param>
        /// <returns>Customer role product search model</returns>
        public virtual CustomerRoleProductSearchModel PrepareCustomerRoleProductSearchModel(CustomerRoleProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his products
            searchModel.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

            //prepare available categories
            _baseAdminModelFactory.PrepareCategories(searchModel.AvailableCategories);

            //prepare available manufacturers
            _baseAdminModelFactory.PrepareManufacturers(searchModel.AvailableManufacturers);

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(searchModel.AvailableStores);

            //prepare available vendors
            _baseAdminModelFactory.PrepareVendors(searchModel.AvailableVendors);

            //prepare available product types
            _baseAdminModelFactory.PrepareProductTypes(searchModel.AvailableProductTypes);

            //prepare page parameters
            searchModel.SetPopupGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged customer role product list model
        /// </summary>
        /// <param name="searchModel">Customer role product search model</param>
        /// <returns>Customer role product list model</returns>
        public virtual CustomerRoleProductListModel PrepareCustomerRoleProductListModel(CustomerRoleProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
                searchModel.SearchVendorId = _workContext.CurrentVendor.Id;

            //get products
            var products = _productService.SearchProducts(showHidden: true,
                categoryIds: new List<int> { searchModel.SearchCategoryId },
                manufacturerId: searchModel.SearchManufacturerId,
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
                keywords: searchModel.SearchProductName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new CustomerRoleProductListModel
            {
                //fill in model values from the entity
                Data = products.Select(product =>
                {
                    var productModel = product.ToModel<ProductModel>();
                    productModel.SeName = _urlRecordService.GetSeName(product, 0, true, false);

                    return productModel;
                }),
                Total = products.TotalCount
            };

            return model;
        }

        #endregion
    }
}