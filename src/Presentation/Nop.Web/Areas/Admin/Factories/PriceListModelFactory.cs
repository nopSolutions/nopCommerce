using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.PriceLists;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.PriceLists;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Models.PriceLists;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the price list model factory implementation
/// </summary>
public partial class PriceListModelFactory : IPriceListModelFactory
{
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IPriceListService _priceListService;
    protected readonly IProductService _productService;

    #endregion

    #region Ctor

    public PriceListModelFactory(CatalogSettings catalogSettings,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        ILocalizationService localizationService,
        IPriceFormatter priceFormatter,
        IPriceListService priceListService,
        IProductService productService)
    {
        _catalogSettings = catalogSettings;
        _baseAdminModelFactory = baseAdminModelFactory;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _localizationService = localizationService;
        _priceFormatter = priceFormatter;
        _priceListService = priceListService;
        _productService = productService;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Prepare price list product search model
    /// </summary>
    /// <param name="searchModel">Price list product search model</param>
    /// <param name="priceList">Price list</param>
    /// <returns>Price list product search model</returns>
    protected virtual PriceListItemSearchModel PreparePriceListProductSearchModel(PriceListItemSearchModel searchModel, PriceList priceList)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(priceList);

        searchModel.PriceListId = priceList.Id;

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare price list customer search model
    /// </summary>
    /// <param name="searchModel">Price list customer search model</param>
    /// <param name="priceList">Price list</param>
    /// <returns>Price list customer search model</returns>
    protected virtual PriceListCustomerSearchModel PreparePriceListCustomerSearchModel(PriceListCustomerSearchModel searchModel, PriceList priceList)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        ArgumentNullException.ThrowIfNull(priceList);

        searchModel.PriceListId = priceList.Id;

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare price list search model
    /// </summary>
    /// <param name="searchModel">Price list search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price list search model
    /// </returns>
    public virtual async Task<PriceListSearchModel> PreparePriceListSearchModelAsync(PriceListSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        searchModel.AvailableActiveValues = new List<SelectListItem> {
            new(await _localizationService.GetResourceAsync("Admin.Common.All"), string.Empty, true),
            new(await _localizationService.GetResourceAsync("Admin.Common.Yes"), true.ToString()),
            new(await _localizationService.GetResourceAsync("Admin.Common.No"), false.ToString())
        };

        searchModel.HidePriority = _catalogSettings.PriceListStrategy != PriceListStrategy.UseByPriority;

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare paged price list list model
    /// </summary>
    /// <param name="searchModel">Price list search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price list list model
    /// </returns>
    public virtual async Task<PriceListListModel> PreparePriceListListModelAsync(PriceListSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get price lists
        var priceLists = await _priceListService.SearchPriceListsAsync(customerRoleIds: searchModel.SelectedCustomerRoleIds.ToArray(),
            isActive: searchModel.SearchIsActive,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare list model
        var model = await new PriceListListModel().PrepareToGridAsync(searchModel, priceLists, () =>
        {
            return priceLists.SelectAwait(async priceList =>
            {
                //fill in model values from the entity
                var priceListModel = priceList.ToModel<PriceListModel>();

                //fill in additional values (not existing in the entity)
                priceListModel.HidePriority = _catalogSettings.PriceListStrategy != PriceListStrategy.UseByPriority;
                var usePercentage = priceList.PriceCalculationTypeId is
                    ((int)PriceCalculationTypeEnum.PercentageDecrease) or ((int)PriceCalculationTypeEnum.PercentageIncrease);
                priceListModel.PriceCalculationValueFormatted = usePercentage ? (priceList.PriceCalculationValue / 100).ToString("P") : await _priceFormatter.FormatPriceAsync(priceList.PriceCalculationValue);

                priceListModel.PriceCalculationTypeName = await _localizationService.GetLocalizedEnumAsync(priceList.PriceCalculationType);
                priceListModel.CustomerRoleNames = string.Join(", ",
                    (await _priceListService.GetCustomerRolesAsync(priceList)).Select(role => role.Name));

                return priceListModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare price list model
    /// </summary>
    /// <param name="model">Price list model</param>
    /// <param name="priceList">Price list</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price list model
    /// </returns>
    public virtual async Task<PriceListModel> PreparePriceListModelAsync(PriceListModel model, PriceList priceList, bool excludeProperties = false)
    {
        if (priceList != null)
        {
            //fill in model values from the entity
            model ??= new PriceListModel();

            model.Id = priceList.Id;

            //whether to fill in some of properties
            if (!excludeProperties)
            {
                model.Name = priceList.Name;
                model.Active = priceList.Active;
                model.Description = priceList.Description;
                model.Priority = priceList.Priority;
                model.HidePriority = _catalogSettings.PriceListStrategy != PriceListStrategy.UseByPriority;
                model.StartDateUtc = !priceList.StartDateUtc.HasValue
                    ? null
                    : await _dateTimeHelper.ConvertToUserTimeAsync(priceList.StartDateUtc.Value, DateTimeKind.Utc);
                model.EndDateUtc = !priceList.EndDateUtc.HasValue
                    ? null
                    : await _dateTimeHelper.ConvertToUserTimeAsync(priceList.EndDateUtc.Value, DateTimeKind.Utc);
                model.SelectedCustomerRoleIds = (await _priceListService.GetCustomerRoleIdsAsync(priceList)).ToList();
                model.PriceCalculationValue = priceList.PriceCalculationValue;
                model.PriceCalculationTypeId = priceList.PriceCalculationTypeId;
            }

            //prepare nested search model
            PreparePriceListProductSearchModel(model.PriceListItemSearchModel, priceList);
            PreparePriceListCustomerSearchModel(model.PriceListCustomerSearchModel, priceList);
        }

        //set default values for the new model
        if (priceList == null)
        {
            model.Active = true;
            model.Priority = 0;
        }

        //prepare available customer roles
        var availableRoles = await _customerService.GetAllCustomerRolesAsync(showHidden: true);
        model.AvailableCustomerRoles = availableRoles.Select(role => new SelectListItem
        {
            Text = role.Name,
            Value = role.Id.ToString(),
            Selected = model.SelectedCustomerRoleIds.Contains(role.Id)
        }).ToList();

        return model;
    }

    #region Price list item

    /// <summary>
    /// Prepare paged price list item list model
    /// </summary>
    /// <param name="searchModel">Price list item search model</param>
    /// <param name="priceList">Price list</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price list item list model
    /// </returns>
    public virtual async Task<PriceListItemListModel> PreparePriceListItemListModelAsync(PriceListItemSearchModel searchModel, PriceList priceList)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(priceList);

        var priceListItems = await _priceListService.GetPriceListItemsByPriceListIdAsync(priceList.Id,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        var productIds = priceListItems.Select(p => p.ProductId).Distinct().ToArray();
        var products = await _productService.GetProductsByIdsAsync(productIds);
        var productNames = products.ToDictionary(p => p.Id, p => p.Name);

        //prepare grid model
        var model = await new PriceListItemListModel().PrepareToGridAsync(searchModel, priceListItems, () =>
        {
            return priceListItems.SelectAwait(async priceListItem =>
            {
                //fill in model values from the entity
                var priceListProductModel = priceListItem.ToModel<PriceListItemModel>();
                var product = await _productService.GetProductByIdAsync(priceListItem.ProductId);

                priceListProductModel.ProductName = productNames.TryGetValue(priceListItem.ProductId, out var productName)
                    ? productName
                    : null;

                priceListProductModel.StandardPrice = await _priceFormatter.FormatPriceAsync(product.Price);
                priceListProductModel.CalculatedPrice = !priceListItem.ManualPrice.HasValue ? await _priceFormatter.FormatPriceAsync(_priceListService.ApplyAdjustmentPrice(product, priceList)) : "";
                priceListProductModel.ManualPrice = priceListItem.ManualPrice.HasValue ? priceListItem.ManualPrice.Value : null;

                return priceListProductModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare product search model to add to the price list
    /// </summary>
    /// <param name="searchModel">Product search model to add to the price list</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product search model to add to the price list
    /// </returns>
    public virtual async Task<AddProductToPriceListSearchModel> PrepareAddProductToPriceListSearchModelAsync(AddProductToPriceListSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

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
    /// Prepare paged product list model to add to the price list
    /// </summary>
    /// <param name="searchModel">Product search model to add to the price list</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product list model to add to the price list
    /// </returns>
    public virtual async Task<AddProductToPriceListListModel> PrepareAddProductToPriceListListModelAsync(AddProductToPriceListSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

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
        var model = await new AddProductToPriceListListModel().PrepareToGridAsync(searchModel, products, () =>
        {
            return products.SelectAwait(async product =>
            {
                var productModel = product.ToModel<ProductModel>();

                return productModel;
            });
        });

        return model;
    }

    #endregion

    #region Customer mapping

    /// <summary>
    /// Prepare paged price list customer list model
    /// </summary>
    /// <param name="searchModel">Price list customer search model</param>
    /// <param name="priceList">Price list</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the price list customer list model
    /// </returns>
    public virtual async Task<PriceListCustomerListModel> PreparePriceListCustomerListModelAsync(PriceListCustomerSearchModel searchModel, PriceList priceList)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(priceList);

        var priceListCustomers = await _priceListService.GetPriceListCustomersByPriceListIdAsync(priceList.Id,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        var customerIds = priceListCustomers.Select(c => c.CustomerId).Distinct().ToArray();
        var customers = await _customerService.GetCustomersByIdsAsync(customerIds);
        var customerEmails = customers.ToDictionary(p => p.Id, p => p.Email);

        //prepare grid model
        var model = new PriceListCustomerListModel().PrepareToGrid(searchModel, priceListCustomers, () =>
        {
            return priceListCustomers.Select(priceListCustomer =>
            {
                //fill in model values from the entity
                var priceListCustomerModel = priceListCustomer.ToModel<PriceListCustomerModel>();

                priceListCustomerModel.CustomerEmail = customerEmails.TryGetValue(priceListCustomer.CustomerId, out var customerEmail)
                    ? customerEmail
                    : null;

                return priceListCustomerModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare customer search model to add to the price list
    /// </summary>
    /// <param name="searchModel">Customer search model to add to the price list</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer search model to add to the price list
    /// </returns>
    public virtual Task<AddCustomerToPriceListSearchModel> PrepareAddCustomerToPriceListSearchModelAsync(AddCustomerToPriceListSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetPopupGridPageSize();

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged customer list model to add to the price list
    /// </summary>
    /// <param name="searchModel">Customer search model to add to the price list</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer list model to add to the price list
    /// </returns>
    public virtual async Task<AddCustomerToPriceListListModel> PrepareAddCustomerToPriceListListModelAsync(AddCustomerToPriceListSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get customers
        var searchCustomerRoleIds = new[] { (await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName)).Id };
        var customers = await _customerService.GetAllCustomersAsync(
            firstName: searchModel.SearchFirstName,
            lastName: searchModel.SearchLastName,
            email: searchModel.SearchEmail,
            company: searchModel.SearchCompany,
            customerRoleIds: searchCustomerRoleIds,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare grid model
        var model = await new AddCustomerToPriceListListModel().PrepareToGridAsync(searchModel, customers, () =>
        {
            return customers.SelectAwait(async customer => new CustomerModel
            {
                Id = customer.Id,
                Email = customer.Email,
                FullName = await _customerService.GetCustomerFullNameAsync(customer),
                Company = customer.Company,
            });
        });

        return model;
    }

    #endregion

    #endregion
}
