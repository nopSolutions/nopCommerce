using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the customer role model factory implementation
/// </summary>
public partial class CustomerRoleModelFactory : ICustomerRoleModelFactory
{
    #region Fields

    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICustomerService _customerService;
    protected readonly IProductService _productService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public CustomerRoleModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
        ICustomerService customerService,
        IProductService productService,
        IUrlRecordService urlRecordService,
        IWorkContext workContext)
    {
        _baseAdminModelFactory = baseAdminModelFactory;
        _customerService = customerService;
        _productService = productService;
        _urlRecordService = urlRecordService;
        _workContext = workContext;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare customer role search model
    /// </summary>
    /// <param name="searchModel">Customer role search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer role search model
    /// </returns>
    public virtual Task<CustomerRoleSearchModel> PrepareCustomerRoleSearchModelAsync(CustomerRoleSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged customer role list model
    /// </summary>
    /// <param name="searchModel">Customer role search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer role list model
    /// </returns>
    public virtual async Task<CustomerRoleListModel> PrepareCustomerRoleListModelAsync(CustomerRoleSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get customer roles
        var customerRoles = (await _customerService.GetAllCustomerRolesAsync(true)).ToPagedList(searchModel);

        //prepare grid model
        var model = await new CustomerRoleListModel().PrepareToGridAsync(searchModel, customerRoles, () =>
        {
            return customerRoles.SelectAwait(async role =>
            {
                //fill in model values from the entity
                var customerRoleModel = role.ToModel<CustomerRoleModel>();

                //fill in additional values (not existing in the entity)
                customerRoleModel.PurchasedWithProductName = (await _productService.GetProductByIdAsync(role.PurchasedWithProductId))?.Name;

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
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer role model
    /// </returns>
    public virtual async Task<CustomerRoleModel> PrepareCustomerRoleModelAsync(CustomerRoleModel model, CustomerRole customerRole, bool excludeProperties = false)
    {
        if (customerRole != null)
        {
            //fill in model values from the entity
            model ??= customerRole.ToModel<CustomerRoleModel>();
            model.PurchasedWithProductName = (await _productService.GetProductByIdAsync(customerRole.PurchasedWithProductId))?.Name;
        }

        //set default values for the new model
        if (customerRole == null)
            model.Active = true;

        //prepare available tax display types
        await _baseAdminModelFactory.PrepareTaxDisplayTypesAsync(model.TaxDisplayTypeValues, false);

        return model;
    }

    /// <summary>
    /// Prepare customer role product search model
    /// </summary>
    /// <param name="searchModel">Customer role product search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer role product search model
    /// </returns>
    public virtual async Task<CustomerRoleProductSearchModel> PrepareCustomerRoleProductSearchModelAsync(CustomerRoleProductSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //a vendor should have access only to his products
        searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;

        //prepare available categories
        await _baseAdminModelFactory.PrepareCategoriesAsync(searchModel.AvailableCategories);

        //prepare available manufacturers
        await _baseAdminModelFactory.PrepareManufacturersAsync(searchModel.AvailableManufacturers);

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

        //prepare available vendors
        await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

        //prepare available product types
        await _baseAdminModelFactory.PrepareProductTypesAsync(searchModel.AvailableProductTypes);

        //prepare page parameters
        searchModel.SetPopupGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare paged customer role product list model
    /// </summary>
    /// <param name="searchModel">Customer role product search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer role product list model
    /// </returns>
    public virtual async Task<CustomerRoleProductListModel> PrepareCustomerRoleProductListModelAsync(CustomerRoleProductSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
            searchModel.SearchVendorId = currentVendor.Id;

        //get products
        var products = await _productService.SearchProductsAsync(showHidden: true,
            categoryIds: new List<int> { searchModel.SearchCategoryId },
            manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
            storeId: searchModel.SearchStoreId,
            vendorId: searchModel.SearchVendorId,
            productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
            keywords: searchModel.SearchProductName,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare grid model
        var model = await new CustomerRoleProductListModel().PrepareToGridAsync(searchModel, products, () =>
        {
            return products.SelectAwait(async product =>
            {
                var productModel = product.ToModel<ProductModel>();

                productModel.SeName = await _urlRecordService.GetSeNameAsync(product, 0, true, false);

                return productModel;
            });
        });

        return model;
    }

    #endregion
}