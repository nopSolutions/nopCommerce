using System.ComponentModel;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the product model factory implementation
/// </summary>
public partial class ProductModelFactory : IProductModelFactory
{
    #region Fields

    protected readonly CatalogSettings _catalogSettings;
    protected readonly CurrencySettings _currencySettings;
    protected readonly IAddressService _addressService;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;
    protected readonly ICategoryService _categoryService;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IDiscountService _discountService;
    protected readonly IDiscountSupportedModelFactory _discountSupportedModelFactory;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILocalizedModelFactory _localizedModelFactory;
    protected readonly IManufacturerService _manufacturerService;
    protected readonly IMeasureService _measureService;
    protected readonly IOrderService _orderService;
    protected readonly IPictureService _pictureService;
    protected readonly IProductAttributeFormatter _productAttributeFormatter;
    protected readonly IProductAttributeParser _productAttributeParser;
    protected readonly IProductAttributeService _productAttributeService;
    protected readonly IProductService _productService;
    protected readonly IProductTagService _productTagService;
    protected readonly IProductTemplateService _productTemplateService;
    protected readonly ISettingModelFactory _settingModelFactory;
    protected readonly ISettingService _settingService;
    protected readonly IShipmentService _shipmentService;
    protected readonly IShippingService _shippingService;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly ISpecificationAttributeService _specificationAttributeService;
    protected readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreService _storeService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IVideoService _videoService;
    protected readonly IWorkContext _workContext;
    protected readonly MeasureSettings _measureSettings;
    protected readonly NopHttpClient _nopHttpClient;
    protected readonly TaxSettings _taxSettings;
    protected readonly VendorSettings _vendorSettings;

    #endregion

    #region Ctor

    public ProductModelFactory(CatalogSettings catalogSettings,
        CurrencySettings currencySettings,
        IAddressService addressService,
        IBaseAdminModelFactory baseAdminModelFactory,
        ICategoryService categoryService,
        ICurrencyService currencyService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IDiscountService discountService,
        IDiscountSupportedModelFactory discountSupportedModelFactory,
        ILocalizationService localizationService,
        ILocalizedModelFactory localizedModelFactory,
        IManufacturerService manufacturerService,
        IMeasureService measureService,
        IOrderService orderService,
        IPictureService pictureService,
        IProductAttributeFormatter productAttributeFormatter,
        IProductAttributeParser productAttributeParser,
        IProductAttributeService productAttributeService,
        IProductService productService,
        IProductTagService productTagService,
        IProductTemplateService productTemplateService,
        ISettingModelFactory settingModelFactory,
        ISettingService settingService,
        IShipmentService shipmentService,
        IShippingService shippingService,
        IShoppingCartService shoppingCartService,
        ISpecificationAttributeService specificationAttributeService,
        IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
        IStoreContext storeContext,
        IStoreService storeService,
        IUrlRecordService urlRecordService,
        IVideoService videoService,
        IWorkContext workContext,
        MeasureSettings measureSettings,
        NopHttpClient nopHttpClient,
        TaxSettings taxSettings,
        VendorSettings vendorSettings)
    {
        _catalogSettings = catalogSettings;
        _currencySettings = currencySettings;
        _addressService = addressService;
        _baseAdminModelFactory = baseAdminModelFactory;
        _categoryService = categoryService;
        _currencyService = currencyService;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _discountService = discountService;
        _discountSupportedModelFactory = discountSupportedModelFactory;
        _localizationService = localizationService;
        _localizedModelFactory = localizedModelFactory;
        _manufacturerService = manufacturerService;
        _measureService = measureService;
        _orderService = orderService;
        _pictureService = pictureService;
        _productAttributeFormatter = productAttributeFormatter;
        _productAttributeParser = productAttributeParser;
        _productAttributeService = productAttributeService;
        _productService = productService;
        _productTagService = productTagService;
        _productTemplateService = productTemplateService;
        _settingModelFactory = settingModelFactory;
        _settingService = settingService;
        _shipmentService = shipmentService;
        _shippingService = shippingService;
        _shoppingCartService = shoppingCartService;
        _specificationAttributeService = specificationAttributeService;
        _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
        _storeContext = storeContext;
        _storeService = storeService;
        _urlRecordService = urlRecordService;
        _videoService = videoService;
        _workContext = workContext;
        _measureSettings = measureSettings;
        _nopHttpClient = nopHttpClient;
        _taxSettings = taxSettings;
        _vendorSettings = vendorSettings;
    }

    #endregion

    #region Utilities

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<string> GetSpecificationAttributeNameAsync(SpecificationAttribute specificationAttribute)
    {
        var name = specificationAttribute.Name;

        if (specificationAttribute.SpecificationAttributeGroupId.HasValue)
        {
            var group = await _specificationAttributeService.GetSpecificationAttributeGroupByIdAsync(specificationAttribute.SpecificationAttributeGroupId.Value);
            if (group != null)
                name = string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.SpecificationAttributes.NameFormat"), group.Name, name);
        }

        return name;
    }

    /// <summary>
    /// Prepare copy product model
    /// </summary>
    /// <param name="model">Copy product model</param>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the copy product model
    /// </returns>
    protected virtual async Task<CopyProductModel> PrepareCopyProductModelAsync(CopyProductModel model, Product product)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.Id = product.Id;
        model.Name = string.Format(await _localizationService.GetResourceAsync("Admin.Catalog.Products.Copy.Name.New"), product.Name);
        model.Published = true;
        model.CopyMultimedia = true;

        return model;
    }

    /// <summary>
    /// Prepare product warehouse inventory models
    /// </summary>
    /// <param name="models">List of product warehouse inventory models</param>
    /// <param name="product">Product</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareProductWarehouseInventoryModelsAsync(IList<ProductWarehouseInventoryModel> models, Product product)
    {
        ArgumentNullException.ThrowIfNull(models);

        foreach (var warehouse in await _shippingService.GetAllWarehousesAsync())
        {
            var model = new ProductWarehouseInventoryModel
            {
                WarehouseId = warehouse.Id,
                WarehouseName = warehouse.Name
            };

            if (product != null)
            {
                var productWarehouseInventory = (await _productService.GetAllProductWarehouseInventoryRecordsAsync(product.Id))?.FirstOrDefault(inventory => inventory.WarehouseId == warehouse.Id);
                if (productWarehouseInventory != null)
                {
                    model.WarehouseUsed = true;
                    model.StockQuantity = productWarehouseInventory.StockQuantity;
                    model.ReservedQuantity = productWarehouseInventory.ReservedQuantity;
                    model.PlannedQuantity = await _shipmentService.GetQuantityInShipmentsAsync(product, productWarehouseInventory.WarehouseId, true, true);
                }
            }

            models.Add(model);
        }
    }

    /// <summary>
    /// Prepare product attribute mapping validation rules string
    /// </summary>
    /// <param name="attributeMapping">Product attribute mapping</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the validation rules string
    /// </returns>
    protected virtual async Task<string> PrepareProductAttributeMappingValidationRulesStringAsync(ProductAttributeMapping attributeMapping)
    {
        if (!attributeMapping.ValidationRulesAllowed())
            return string.Empty;

        var validationRules = new StringBuilder(string.Empty);
        if (attributeMapping.ValidationMinLength.HasValue)
        {
            validationRules.AppendFormat("{0}: {1}<br />",
                await _localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.MinLength"),
                attributeMapping.ValidationMinLength);
        }

        if (attributeMapping.ValidationMaxLength.HasValue)
        {
            validationRules.AppendFormat("{0}: {1}<br />",
                await _localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.MaxLength"),
                attributeMapping.ValidationMaxLength);
        }

        if (!string.IsNullOrEmpty(attributeMapping.ValidationFileAllowedExtensions))
        {
            validationRules.AppendFormat("{0}: {1}<br />",
                await _localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.FileAllowedExtensions"),
                WebUtility.HtmlEncode(attributeMapping.ValidationFileAllowedExtensions));
        }

        if (attributeMapping.ValidationFileMaximumSize.HasValue)
        {
            validationRules.AppendFormat("{0}: {1}<br />",
                await _localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.FileMaximumSize"),
                attributeMapping.ValidationFileMaximumSize);
        }

        if (!string.IsNullOrEmpty(attributeMapping.DefaultValue))
        {
            validationRules.AppendFormat("{0}: {1}<br />",
                await _localizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.DefaultValue"),
                WebUtility.HtmlEncode(attributeMapping.DefaultValue));
        }

        return validationRules.ToString();
    }

    /// <summary>
    /// Prepare product attribute condition model
    /// </summary>
    /// <param name="model">Product attribute condition model</param>
    /// <param name="productAttributeMapping">Product attribute mapping</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PrepareProductAttributeConditionModelAsync(ProductAttributeConditionModel model,
        ProductAttributeMapping productAttributeMapping)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(productAttributeMapping);

        model.ProductAttributeMappingId = productAttributeMapping.Id;
        model.EnableCondition = !string.IsNullOrEmpty(productAttributeMapping.ConditionAttributeXml);

        //pre-select attribute and values
        var selectedPva = (await _productAttributeParser
                .ParseProductAttributeMappingsAsync(productAttributeMapping.ConditionAttributeXml))
            .FirstOrDefault();

        var attributes = (await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(productAttributeMapping.ProductId))
            //ignore non-combinable attributes (should have selectable values)
            .Where(x => x.CanBeUsedAsCondition())
            //ignore this attribute (it cannot depend on itself)
            .Where(x => x.Id != productAttributeMapping.Id)
            .ToList();
        foreach (var attribute in attributes)
        {
            var attributeModel = new ProductAttributeConditionModel.ProductAttributeModel
            {
                Id = attribute.Id,
                ProductAttributeId = attribute.ProductAttributeId,
                Name = (await _productAttributeService.GetProductAttributeByIdAsync(attribute.ProductAttributeId)).Name,
                TextPrompt = attribute.TextPrompt,
                IsRequired = attribute.IsRequired,
                AttributeControlType = attribute.AttributeControlType
            };

            if (attribute.ShouldHaveValues())
            {
                //values
                var attributeValues = await _productAttributeService.GetProductAttributeValuesAsync(attribute.Id);
                foreach (var attributeValue in attributeValues)
                {
                    var attributeValueModel = new ProductAttributeConditionModel.ProductAttributeValueModel
                    {
                        Id = attributeValue.Id,
                        Name = attributeValue.Name,
                        IsPreSelected = attributeValue.IsPreSelected
                    };
                    attributeModel.Values.Add(attributeValueModel);
                }

                //pre-select attribute and value
                if (selectedPva != null && attribute.Id == selectedPva.Id)
                {
                    //attribute
                    model.SelectedProductAttributeId = selectedPva.Id;

                    //values
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.Checkboxes:
                        case AttributeControlType.ColorSquares:
                        case AttributeControlType.ImageSquares:
                            if (!string.IsNullOrEmpty(productAttributeMapping.ConditionAttributeXml))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues =
                                    await _productAttributeParser.ParseProductAttributeValuesAsync(productAttributeMapping
                                        .ConditionAttributeXml);
                                foreach (var attributeValue in selectedValues)
                                foreach (var item in attributeModel.Values)
                                    if (attributeValue.Id == item.Id)
                                        item.IsPreSelected = true;
                            }

                            break;
                        case AttributeControlType.ReadonlyCheckboxes:
                        case AttributeControlType.TextBox:
                        case AttributeControlType.MultilineTextbox:
                        case AttributeControlType.Datepicker:
                        case AttributeControlType.FileUpload:
                        default:
                            //these attribute types are supported as conditions
                            break;
                    }
                }
            }

            model.ProductAttributes.Add(attributeModel);
        }
    }

    /// <summary>
    /// Prepare related product search model
    /// </summary>
    /// <param name="searchModel">Related product search model</param>
    /// <param name="product">Product</param>
    /// <returns>Related product search model</returns>
    protected virtual RelatedProductSearchModel PrepareRelatedProductSearchModel(RelatedProductSearchModel searchModel, Product product)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(product);

        searchModel.ProductId = product.Id;

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare cross-sell product search model
    /// </summary>
    /// <param name="searchModel">Cross-sell product search model</param>
    /// <param name="product">Product</param>
    /// <returns>Cross-sell product search model</returns>
    protected virtual CrossSellProductSearchModel PrepareCrossSellProductSearchModel(CrossSellProductSearchModel searchModel, Product product)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(product);

        searchModel.ProductId = product.Id;

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare associated product search model
    /// </summary>
    /// <param name="searchModel">Associated product search model</param>
    /// <param name="product">Product</param>
    /// <returns>Associated product search model</returns>
    protected virtual AssociatedProductSearchModel PrepareAssociatedProductSearchModel(AssociatedProductSearchModel searchModel, Product product)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(product);

        searchModel.ProductId = product.Id;

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare product picture search model
    /// </summary>
    /// <param name="searchModel">Product picture search model</param>
    /// <param name="product">Product</param>
    /// <returns>Product picture search model</returns>
    protected virtual ProductPictureSearchModel PrepareProductPictureSearchModel(ProductPictureSearchModel searchModel, Product product)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(product);

        searchModel.ProductId = product.Id;

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare product video search model
    /// </summary>
    /// <param name="searchModel">Product video search model</param>
    /// <param name="product">Product</param>
    /// <returns>Product video search model</returns>
    protected virtual ProductVideoSearchModel PrepareProductVideoSearchModel(ProductVideoSearchModel searchModel, Product product)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(product);

        searchModel.ProductId = product.Id;

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare product order search model
    /// </summary>
    /// <param name="searchModel">Product order search model</param>
    /// <param name="product">Product</param>
    /// <returns>Product order search model</returns>
    protected virtual ProductOrderSearchModel PrepareProductOrderSearchModel(ProductOrderSearchModel searchModel, Product product)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(product);

        searchModel.ProductId = product.Id;

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare tier price search model
    /// </summary>
    /// <param name="searchModel">Tier price search model</param>
    /// <param name="product">Product</param>
    /// <returns>Tier price search model</returns>
    protected virtual TierPriceSearchModel PrepareTierPriceSearchModel(TierPriceSearchModel searchModel, Product product)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(product);

        searchModel.ProductId = product.Id;

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare stock quantity history search model
    /// </summary>
    /// <param name="searchModel">Stock quantity history search model</param>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the stock quantity history search model
    /// </returns>
    protected virtual async Task<StockQuantityHistorySearchModel> PrepareStockQuantityHistorySearchModelAsync(StockQuantityHistorySearchModel searchModel, Product product)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(product);

        searchModel.ProductId = product.Id;

        //prepare available warehouses
        await _baseAdminModelFactory.PrepareWarehousesAsync(searchModel.AvailableWarehouses);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare product attribute mapping search model
    /// </summary>
    /// <param name="searchModel">Product attribute mapping search model</param>
    /// <param name="product">Product</param>
    /// <returns>Product attribute mapping search model</returns>
    protected virtual ProductAttributeMappingSearchModel PrepareProductAttributeMappingSearchModel(ProductAttributeMappingSearchModel searchModel,
        Product product)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(product);

        searchModel.ProductId = product.Id;

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare product attribute value search model
    /// </summary>
    /// <param name="searchModel">Product attribute value search model</param>
    /// <param name="productAttributeMapping">Product attribute mapping</param>
    /// <returns>Product attribute value search model</returns>
    protected virtual ProductAttributeValueSearchModel PrepareProductAttributeValueSearchModel(ProductAttributeValueSearchModel searchModel,
        ProductAttributeMapping productAttributeMapping)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(productAttributeMapping);

        searchModel.ProductAttributeMappingId = productAttributeMapping.Id;

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare product attribute combination search model
    /// </summary>
    /// <param name="searchModel">Product attribute combination search model</param>
    /// <param name="product">Product</param>
    /// <returns>Product attribute combination search model</returns>
    protected virtual ProductAttributeCombinationSearchModel PrepareProductAttributeCombinationSearchModel(
        ProductAttributeCombinationSearchModel searchModel, Product product)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(product);

        searchModel.ProductId = product.Id;

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare product specification attribute search model
    /// </summary>
    /// <param name="searchModel">Product specification attribute search model</param>
    /// <param name="product">Product</param>
    /// <returns>Product specification attribute search model</returns>
    protected virtual ProductSpecificationAttributeSearchModel PrepareProductSpecificationAttributeSearchModel(
        ProductSpecificationAttributeSearchModel searchModel, Product product)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(product);

        searchModel.ProductId = product.Id;

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare product search model
    /// </summary>
    /// <param name="searchModel">Product search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product search model
    /// </returns>
    public virtual async Task<ProductSearchModel> PrepareProductSearchModelAsync(ProductSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //a vendor should have access only to his products
        searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;
        searchModel.AllowVendorsToImportProducts = _vendorSettings.AllowVendorsToImportProducts;

        var licenseCheckModel = new LicenseCheckModel();
        try
        {
            var result = await _nopHttpClient.GetLicenseCheckDetailsAsync();
            if (!string.IsNullOrEmpty(result))
            {
                licenseCheckModel = JsonConvert.DeserializeObject<LicenseCheckModel>(result);
                if (licenseCheckModel.DisplayWarning == false && licenseCheckModel.BlockPages == false)
                    await _settingService.SetSettingAsync($"{nameof(AdminAreaSettings)}.{nameof(AdminAreaSettings.CheckLicense)}", false);
            }
        }
        catch { }
        searchModel.LicenseCheckModel = licenseCheckModel;

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

        //prepare available warehouses
        await _baseAdminModelFactory.PrepareWarehousesAsync(searchModel.AvailableWarehouses);

        searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

        //prepare "published" filter (0 - all; 1 - published only; 2 - unpublished only)
        searchModel.AvailablePublishedOptions.Add(new SelectListItem
        {
            Value = "0",
            Text = await _localizationService.GetResourceAsync("Admin.Catalog.Products.List.SearchPublished.All")
        });
        searchModel.AvailablePublishedOptions.Add(new SelectListItem
        {
            Value = "1",
            Text = await _localizationService.GetResourceAsync("Admin.Catalog.Products.List.SearchPublished.PublishedOnly")
        });
        searchModel.AvailablePublishedOptions.Add(new SelectListItem
        {
            Value = "2",
            Text = await _localizationService.GetResourceAsync("Admin.Catalog.Products.List.SearchPublished.UnpublishedOnly")
        });

        //prepare grid
        searchModel.SetGridPageSize();

        return searchModel;
    }

    /// <summary>
    /// Prepare paged product list model
    /// </summary>
    /// <param name="searchModel">Product search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product list model
    /// </returns>
    public virtual async Task<ProductListModel> PrepareProductListModelAsync(ProductSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get parameters to filter comments
        var overridePublished = searchModel.SearchPublishedId == 0 ? null : (bool?)(searchModel.SearchPublishedId == 1);
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null)
            searchModel.SearchVendorId = currentVendor.Id;
        var categoryIds = new List<int> { searchModel.SearchCategoryId };
        if (searchModel.SearchIncludeSubCategories && searchModel.SearchCategoryId > 0)
        {
            var childCategoryIds = await _categoryService.GetChildCategoryIdsAsync(parentCategoryId: searchModel.SearchCategoryId, showHidden: true);
            categoryIds.AddRange(childCategoryIds);
        }

        //get products
        var products = await _productService.SearchProductsAsync(showHidden: true,
            categoryIds: categoryIds,
            manufacturerIds: new List<int> { searchModel.SearchManufacturerId },
            storeId: searchModel.SearchStoreId,
            vendorId: searchModel.SearchVendorId,
            warehouseId: searchModel.SearchWarehouseId,
            productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
            keywords: searchModel.SearchProductName,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,
            overridePublished: overridePublished);

        //prepare list model
        var model = await new ProductListModel().PrepareToGridAsync(searchModel, products, () =>
        {
            return products.SelectAwait(async product =>
            {
                //fill in model values from the entity
                var productModel = product.ToModel<ProductModel>();

                //little performance optimization: ensure that "FullDescription" is not returned
                productModel.FullDescription = string.Empty;

                //fill in additional values (not existing in the entity)
                productModel.SeName = await _urlRecordService.GetSeNameAsync(product, 0, true, false);
                var defaultProductPicture = (await _pictureService.GetPicturesByProductIdAsync(product.Id, 1)).FirstOrDefault();
                (productModel.PictureThumbnailUrl, _) = await _pictureService.GetPictureUrlAsync(defaultProductPicture, 75);
                productModel.ProductTypeName = await _localizationService.GetLocalizedEnumAsync(product.ProductType);
                if (product.ProductType == ProductType.SimpleProduct && product.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
                    productModel.StockQuantityStr = (await _productService.GetTotalStockQuantityAsync(product)).ToString();

                return productModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare product model
    /// </summary>
    /// <param name="model">Product model</param>
    /// <param name="product">Product</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product model
    /// </returns>
    public virtual async Task<ProductModel> PrepareProductModelAsync(ProductModel model, Product product, bool excludeProperties = false)
    {
        Func<ProductLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (product != null)
        {
            //fill in model values from the entity
            if (model == null)
            {
                model = product.ToModel<ProductModel>();
                model.SeName = await _urlRecordService.GetSeNameAsync(product, 0, true, false);
            }

            var parentGroupedProduct = await _productService.GetProductByIdAsync(product.ParentGroupedProductId);
            if (parentGroupedProduct != null)
            {
                model.AssociatedToProductId = product.ParentGroupedProductId;
                model.AssociatedToProductName = parentGroupedProduct.Name;
            }

            model.LastStockQuantity = product.StockQuantity;

            model.SelectedProductTags = (await _productTagService.GetAllProductTagsByProductIdAsync(product.Id)).Select(tag => tag.Name).ToList();
            model.AvailableProductTags = (await _productTagService.GetAllProductTagsAsync())
                .Select(pt => new SelectListItem { Text = pt.Name, Value = pt.Name }).ToList();

            model.ProductAttributesExist = (await _productAttributeService.GetAllProductAttributesAsync()).Any();

            model.CanCreateCombinations = await (await _productAttributeService
                .GetProductAttributeMappingsByProductIdAsync(product.Id)).AnyAwaitAsync(async pam => (await _productAttributeService.GetProductAttributeValuesAsync(pam.Id)).Any());

            if (!excludeProperties)
            {
                model.SelectedCategoryIds = (await _categoryService.GetProductCategoriesByProductIdAsync(product.Id, true))
                    .Select(productCategory => productCategory.CategoryId).ToList();
                model.SelectedManufacturerIds = (await _manufacturerService.GetProductManufacturersByProductIdAsync(product.Id, true))
                    .Select(productManufacturer => productManufacturer.ManufacturerId).ToList();
            }

            //prepare copy product model
            await PrepareCopyProductModelAsync(model.CopyProductModel, product);

            //prepare nested search model
            PrepareRelatedProductSearchModel(model.RelatedProductSearchModel, product);
            PrepareCrossSellProductSearchModel(model.CrossSellProductSearchModel, product);
            PrepareAssociatedProductSearchModel(model.AssociatedProductSearchModel, product);
            PrepareProductPictureSearchModel(model.ProductPictureSearchModel, product);
            PrepareProductVideoSearchModel(model.ProductVideoSearchModel, product);
            PrepareProductSpecificationAttributeSearchModel(model.ProductSpecificationAttributeSearchModel, product);
            PrepareProductOrderSearchModel(model.ProductOrderSearchModel, product);
            PrepareTierPriceSearchModel(model.TierPriceSearchModel, product);
            await PrepareStockQuantityHistorySearchModelAsync(model.StockQuantityHistorySearchModel, product);
            PrepareProductAttributeMappingSearchModel(model.ProductAttributeMappingSearchModel, product);
            PrepareProductAttributeCombinationSearchModel(model.ProductAttributeCombinationSearchModel, product);

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(product, entity => entity.Name, languageId, false, false);
                locale.FullDescription = await _localizationService.GetLocalizedAsync(product, entity => entity.FullDescription, languageId, false, false);
                locale.ShortDescription = await _localizationService.GetLocalizedAsync(product, entity => entity.ShortDescription, languageId, false, false);
                locale.MetaKeywords = await _localizationService.GetLocalizedAsync(product, entity => entity.MetaKeywords, languageId, false, false);
                locale.MetaDescription = await _localizationService.GetLocalizedAsync(product, entity => entity.MetaDescription, languageId, false, false);
                locale.MetaTitle = await _localizationService.GetLocalizedAsync(product, entity => entity.MetaTitle, languageId, false, false);
                locale.SeName = await _urlRecordService.GetSeNameAsync(product, languageId, false, false);
            };
        }

        //set default values for the new model
        if (product == null)
        {
            model.MaximumCustomerEnteredPrice = 1000;
            model.MaxNumberOfDownloads = 10;
            model.RecurringCycleLength = 100;
            model.RecurringTotalCycles = 10;
            model.RentalPriceLength = 1;
            model.StockQuantity = 10000;
            model.NotifyAdminForQuantityBelow = 1;
            model.OrderMinimumQuantity = 1;
            model.OrderMaximumQuantity = 10000;
            model.TaxCategoryId = _taxSettings.DefaultTaxCategoryId;
            model.UnlimitedDownloads = true;
            model.IsShipEnabled = true;
            model.AllowCustomerReviews = true;
            model.Published = true;
            model.VisibleIndividually = true;
        }

        model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;
        model.BaseWeightIn = (await _measureService.GetMeasureWeightByIdAsync(_measureSettings.BaseWeightId)).Name;
        model.BaseDimensionIn = (await _measureService.GetMeasureDimensionByIdAsync(_measureSettings.BaseDimensionId)).Name;
        model.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;
        model.HasAvailableSpecificationAttributes =
            (await _specificationAttributeService.GetSpecificationAttributesWithOptionsAsync()).Any();

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        //prepare editor settings
        model.ProductEditorSettingsModel = await _settingModelFactory.PrepareProductEditorSettingsModelAsync();

        //prepare available product templates
        await _baseAdminModelFactory.PrepareProductTemplatesAsync(model.AvailableProductTemplates, false);

        //prepare available product types
        var productTemplates = await _productTemplateService.GetAllProductTemplatesAsync();
        foreach (var productType in Enum.GetValues(typeof(ProductType)).OfType<ProductType>())
        {
            model.ProductsTypesSupportedByProductTemplates.Add((int)productType, new List<SelectListItem>());
            foreach (var template in productTemplates)
            {
                var list = (IList<int>)TypeDescriptor.GetConverter(typeof(List<int>)).ConvertFrom(template.IgnoredProductTypes) ?? new List<int>();
                if (string.IsNullOrEmpty(template.IgnoredProductTypes) || !list.Contains((int)productType))
                {
                    model.ProductsTypesSupportedByProductTemplates[(int)productType].Add(new SelectListItem
                    {
                        Text = template.Name,
                        Value = template.Id.ToString()
                    });
                }
            }
        }

        //prepare available delivery dates
        await _baseAdminModelFactory.PrepareDeliveryDatesAsync(model.AvailableDeliveryDates,
            defaultItemText: await _localizationService.GetResourceAsync("Admin.Catalog.Products.Fields.DeliveryDate.None"));

        //prepare available product availability ranges
        await _baseAdminModelFactory.PrepareProductAvailabilityRangesAsync(model.AvailableProductAvailabilityRanges,
            defaultItemText: await _localizationService.GetResourceAsync("Admin.Catalog.Products.Fields.ProductAvailabilityRange.None"));

        //prepare available vendors
        await _baseAdminModelFactory.PrepareVendorsAsync(model.AvailableVendors,
            defaultItemText: await _localizationService.GetResourceAsync("Admin.Catalog.Products.Fields.Vendor.None"));

        //prepare available tax categories
        await _baseAdminModelFactory.PrepareTaxCategoriesAsync(model.AvailableTaxCategories);

        //prepare available warehouses
        await _baseAdminModelFactory.PrepareWarehousesAsync(model.AvailableWarehouses,
            defaultItemText: await _localizationService.GetResourceAsync("Admin.Catalog.Products.Fields.Warehouse.None"));
        await PrepareProductWarehouseInventoryModelsAsync(model.ProductWarehouseInventoryModels, product);

        //prepare available base price units
        var availableMeasureWeights = (await _measureService.GetAllMeasureWeightsAsync())
            .Select(weight => new SelectListItem { Text = weight.Name, Value = weight.Id.ToString() }).ToList();
        model.AvailableBasepriceUnits = availableMeasureWeights;
        model.AvailableBasepriceBaseUnits = availableMeasureWeights;

        //prepare model categories
        await _baseAdminModelFactory.PrepareCategoriesAsync(model.AvailableCategories, false);
        foreach (var categoryItem in model.AvailableCategories)
        {
            categoryItem.Selected = int.TryParse(categoryItem.Value, out var categoryId)
                                    && model.SelectedCategoryIds.Contains(categoryId);
        }

        //prepare model manufacturers
        await _baseAdminModelFactory.PrepareManufacturersAsync(model.AvailableManufacturers, false);
        foreach (var manufacturerItem in model.AvailableManufacturers)
        {
            manufacturerItem.Selected = int.TryParse(manufacturerItem.Value, out var manufacturerId)
                                        && model.SelectedManufacturerIds.Contains(manufacturerId);
        }

        //prepare model discounts
        var availableDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToSkus, showHidden: true, isActive: null);
        await _discountSupportedModelFactory.PrepareModelDiscountsAsync(model, product, availableDiscounts, excludeProperties);
        
        //prepare model stores
        await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, product, excludeProperties);

        return model;
    }

    /// <summary>
    /// Prepare required product search model to add to the product
    /// </summary>
    /// <param name="searchModel">Required product search model to add to the product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the required product search model to add to the product
    /// </returns>
    public virtual async Task<AddRequiredProductSearchModel> PrepareAddRequiredProductSearchModelAsync(AddRequiredProductSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

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
    /// Prepare required product list model to add to the product
    /// </summary>
    /// <param name="searchModel">Required product search model to add to the product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the required product list model to add to the product
    /// </returns>
    public virtual async Task<AddRequiredProductListModel> PrepareAddRequiredProductListModelAsync(AddRequiredProductSearchModel searchModel)
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
        var model = await new AddRequiredProductListModel().PrepareToGridAsync(searchModel, products, () =>
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

    /// <summary>
    /// Prepare paged related product list model
    /// </summary>
    /// <param name="searchModel">Related product search model</param>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the related product list model
    /// </returns>
    public virtual async Task<RelatedProductListModel> PrepareRelatedProductListModelAsync(RelatedProductSearchModel searchModel, Product product)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(product);

        //get related products
        var relatedProducts = (await _productService
            .GetRelatedProductsByProductId1Async(productId1: product.Id, showHidden: true)).ToPagedList(searchModel);

        //prepare grid model
        var model = await new RelatedProductListModel().PrepareToGridAsync(searchModel, relatedProducts, () =>
        {
            return relatedProducts.SelectAwait(async relatedProduct =>
            {
                //fill in model values from the entity
                var relatedProductModel = relatedProduct.ToModel<RelatedProductModel>();

                //fill in additional values (not existing in the entity)
                relatedProductModel.Product2Name = (await _productService.GetProductByIdAsync(relatedProduct.ProductId2))?.Name;

                return relatedProductModel;
            });
        });
        return model;
    }

    /// <summary>
    /// Prepare related product search model to add to the product
    /// </summary>
    /// <param name="searchModel">Related product search model to add to the product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the related product search model to add to the product
    /// </returns>
    public virtual async Task<AddRelatedProductSearchModel> PrepareAddRelatedProductSearchModelAsync(AddRelatedProductSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

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
    /// Prepare paged related product list model to add to the product
    /// </summary>
    /// <param name="searchModel">Related product search model to add to the product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the related product list model to add to the product
    /// </returns>
    public virtual async Task<AddRelatedProductListModel> PrepareAddRelatedProductListModelAsync(AddRelatedProductSearchModel searchModel)
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
        var model = await new AddRelatedProductListModel().PrepareToGridAsync(searchModel, products, () =>
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

    /// <summary>
    /// Prepare paged cross-sell product list model
    /// </summary>
    /// <param name="searchModel">Cross-sell product search model</param>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the cross-sell product list model
    /// </returns>
    public virtual async Task<CrossSellProductListModel> PrepareCrossSellProductListModelAsync(CrossSellProductSearchModel searchModel, Product product)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(product);

        //get cross-sell products
        var crossSellProducts = (await _productService
            .GetCrossSellProductsByProductId1Async(productId1: product.Id, showHidden: true)).ToPagedList(searchModel);

        //prepare grid model
        var model = await new CrossSellProductListModel().PrepareToGridAsync(searchModel, crossSellProducts, () =>
        {
            return crossSellProducts.SelectAwait(async crossSellProduct =>
            {
                //fill in model values from the entity
                var crossSellProductModel = new CrossSellProductModel
                {
                    Id = crossSellProduct.Id,
                    ProductId2 = crossSellProduct.ProductId2
                };

                //fill in additional values (not existing in the entity)
                crossSellProductModel.Product2Name = (await _productService.GetProductByIdAsync(crossSellProduct.ProductId2))?.Name;

                return crossSellProductModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare cross-sell product search model to add to the product
    /// </summary>
    /// <param name="searchModel">Cross-sell product search model to add to the product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the cross-sell product search model to add to the product
    /// </returns>
    public virtual async Task<AddCrossSellProductSearchModel> PrepareAddCrossSellProductSearchModelAsync(AddCrossSellProductSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

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
    /// Prepare paged crossSell product list model to add to the product
    /// </summary>
    /// <param name="searchModel">CrossSell product search model to add to the product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the crossSell product list model to add to the product
    /// </returns>
    public virtual async Task<AddCrossSellProductListModel> PrepareAddCrossSellProductListModelAsync(AddCrossSellProductSearchModel searchModel)
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
        var model = await new AddCrossSellProductListModel().PrepareToGridAsync(searchModel, products, () =>
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

    /// <summary>
    /// Prepare paged associated product list model
    /// </summary>
    /// <param name="searchModel">Associated product search model</param>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the associated product list model
    /// </returns>
    public virtual async Task<AssociatedProductListModel> PrepareAssociatedProductListModelAsync(AssociatedProductSearchModel searchModel, Product product)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(product);

        var vendor = await _workContext.GetCurrentVendorAsync();
        //get associated products
        var associatedProducts = (await _productService.GetAssociatedProductsAsync(showHidden: true,
            parentGroupedProductId: product.Id,
            vendorId: vendor?.Id ?? 0)).ToPagedList(searchModel);

        //prepare grid model
        var model = new AssociatedProductListModel().PrepareToGrid(searchModel, associatedProducts, () =>
        {
            return associatedProducts.Select(associatedProduct =>
            {
                var associatedProductModel = associatedProduct.ToModel<AssociatedProductModel>();
                associatedProductModel.ProductName = associatedProduct.Name;

                return associatedProductModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare associated product search model to add to the product
    /// </summary>
    /// <param name="searchModel">Associated product search model to add to the product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the associated product search model to add to the product
    /// </returns>
    public virtual async Task<AddAssociatedProductSearchModel> PrepareAddAssociatedProductSearchModelAsync(AddAssociatedProductSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

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
    /// Prepare paged associated product list model to add to the product
    /// </summary>
    /// <param name="searchModel">Associated product search model to add to the product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the associated product list model to add to the product
    /// </returns>
    public virtual async Task<AddAssociatedProductListModel> PrepareAddAssociatedProductListModelAsync(AddAssociatedProductSearchModel searchModel)
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
        var model = await new AddAssociatedProductListModel().PrepareToGridAsync(searchModel, products, () =>
        {
            return products.SelectAwait(async product =>
            {
                //fill in model values from the entity
                var productModel = product.ToModel<ProductModel>();

                //fill in additional values (not existing in the entity)
                productModel.SeName = await _urlRecordService.GetSeNameAsync(product, 0, true, false);
                var parentGroupedProduct = await _productService.GetProductByIdAsync(product.ParentGroupedProductId);

                if (parentGroupedProduct == null)
                    return productModel;

                productModel.AssociatedToProductId = product.ParentGroupedProductId;
                productModel.AssociatedToProductName = parentGroupedProduct.Name;

                return productModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare paged product picture list model
    /// </summary>
    /// <param name="searchModel">Product picture search model</param>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product picture list model
    /// </returns>
    public virtual async Task<ProductPictureListModel> PrepareProductPictureListModelAsync(ProductPictureSearchModel searchModel, Product product)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(product);

        //get product pictures
        var productPictures = (await _productService.GetProductPicturesByProductIdAsync(product.Id)).ToPagedList(searchModel);

        //prepare grid model
        var model = await new ProductPictureListModel().PrepareToGridAsync(searchModel, productPictures, () =>
        {
            return productPictures.SelectAwait(async productPicture =>
            {
                //fill in model values from the entity
                var productPictureModel = productPicture.ToModel<ProductPictureModel>();

                //fill in additional values (not existing in the entity)
                var picture = (await _pictureService.GetPictureByIdAsync(productPicture.PictureId))
                              ?? throw new Exception("Picture cannot be loaded");

                productPictureModel.PictureUrl = (await _pictureService.GetPictureUrlAsync(picture)).Url;

                productPictureModel.OverrideAltAttribute = picture.AltAttribute;
                productPictureModel.OverrideTitleAttribute = picture.TitleAttribute;

                return productPictureModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare paged product video list model
    /// </summary>
    /// <param name="searchModel">Product video search model</param>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product video list model
    /// </returns>
    public virtual async Task<ProductVideoListModel> PrepareProductVideoListModelAsync(ProductVideoSearchModel searchModel, Product product)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(product);

        //get product videos
        var productVideos = (await _productService.GetProductVideosByProductIdAsync(product.Id)).ToPagedList(searchModel);

        //prepare grid model
        var model = await new ProductVideoListModel().PrepareToGridAsync(searchModel, productVideos, () =>
        {
            return productVideos.SelectAwait(async productVideo =>
            {
                //fill in model values from the entity
                var productVideoModel = productVideo.ToModel<ProductVideoModel>();

                //fill in additional values (not existing in the entity)
                var video = (await _videoService.GetVideoByIdAsync(productVideo.VideoId))
                            ?? throw new Exception("Video cannot be loaded");

                productVideoModel.VideoUrl = video.VideoUrl;

                return productVideoModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare paged product specification attribute list model
    /// </summary>
    /// <param name="searchModel">Product specification attribute search model</param>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product specification attribute list model
    /// </returns>
    public virtual async Task<ProductSpecificationAttributeListModel> PrepareProductSpecificationAttributeListModelAsync(
        ProductSpecificationAttributeSearchModel searchModel, Product product)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(product);

        //get product specification attributes
        var productSpecificationAttributes = (await _specificationAttributeService
            .GetProductSpecificationAttributesAsync(product.Id)).ToPagedList(searchModel);

        //prepare grid model
        var model = await new ProductSpecificationAttributeListModel().PrepareToGridAsync(searchModel, productSpecificationAttributes, () =>
        {
            return productSpecificationAttributes.SelectAwait(async attribute =>
            {
                //fill in model values from the entity
                var productSpecificationAttributeModel = attribute.ToModel<ProductSpecificationAttributeModel>();

                var specAttributeOption = await _specificationAttributeService
                    .GetSpecificationAttributeOptionByIdAsync(attribute.SpecificationAttributeOptionId);
                var specAttribute = await _specificationAttributeService
                    .GetSpecificationAttributeByIdAsync(specAttributeOption.SpecificationAttributeId);

                //fill in additional values (not existing in the entity)
                productSpecificationAttributeModel.AttributeTypeName = await _localizationService.GetLocalizedEnumAsync(attribute.AttributeType);

                productSpecificationAttributeModel.AttributeId = specAttribute.Id;
                productSpecificationAttributeModel.AttributeName = await GetSpecificationAttributeNameAsync(specAttribute);
                var currentLanguage = await _workContext.GetWorkingLanguageAsync();

                switch (attribute.AttributeType)
                {
                    case SpecificationAttributeType.Option:
                        productSpecificationAttributeModel.ValueRaw = WebUtility.HtmlEncode(specAttributeOption.Name);
                        productSpecificationAttributeModel.SpecificationAttributeOptionId = specAttributeOption.Id;
                        break;
                    case SpecificationAttributeType.CustomText:
                        productSpecificationAttributeModel.ValueRaw = WebUtility.HtmlEncode(await _localizationService.GetLocalizedAsync(attribute, x => x.CustomValue, currentLanguage?.Id));
                        break;
                    case SpecificationAttributeType.CustomHtmlText:
                        productSpecificationAttributeModel.ValueRaw = await _localizationService
                            .GetLocalizedAsync(attribute, x => x.CustomValue, currentLanguage?.Id);
                        break;
                    case SpecificationAttributeType.Hyperlink:
                        productSpecificationAttributeModel.ValueRaw = attribute.CustomValue;
                        break;
                }

                return productSpecificationAttributeModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare paged product specification attribute model
    /// </summary>
    /// <param name="productId">Product id</param>
    /// <param name="specificationId">Specification attribute id</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product specification attribute model
    /// </returns>
    public virtual async Task<AddSpecificationAttributeModel> PrepareAddSpecificationAttributeModelAsync(int productId, int? specificationId)
    {
        if (!specificationId.HasValue)
        {
            return new AddSpecificationAttributeModel
            {
                AvailableAttributes = await (await _specificationAttributeService.GetSpecificationAttributesWithOptionsAsync())
                    .SelectAwait(async attributeWithOption =>
                    {
                        var attributeName = await GetSpecificationAttributeNameAsync(attributeWithOption);

                        return new SelectListItem(attributeName, attributeWithOption.Id.ToString());
                    }).ToListAsync(),
                ProductId = productId,
                Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync<AddSpecificationAttributeLocalizedModel>()
            };
        }

        var attribute = await _specificationAttributeService.GetProductSpecificationAttributeByIdAsync(specificationId.Value) 
                        ?? throw new ArgumentException("No specification attribute found with the specified id");

        //a vendor should have access only to his products
        var currentVendor = await _workContext.GetCurrentVendorAsync();
        if (currentVendor != null && (await _productService.GetProductByIdAsync(attribute.ProductId)).VendorId != currentVendor.Id)
            throw new UnauthorizedAccessException("This is not your product");

        var specAttributeOption = await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(attribute.SpecificationAttributeOptionId);
        var specAttribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(specAttributeOption.SpecificationAttributeId);

        var model = attribute.ToModel<AddSpecificationAttributeModel>();
        model.SpecificationId = attribute.Id;
        model.AttributeId = specAttribute.Id;
        model.AttributeTypeName = await _localizationService.GetLocalizedEnumAsync(attribute.AttributeType);
        model.AttributeName = specAttribute.Name;

        model.AvailableAttributes = await (await _specificationAttributeService.GetSpecificationAttributesWithOptionsAsync())
            .SelectAwait(async attributeWithOption =>
            {
                var attributeName = await GetSpecificationAttributeNameAsync(attributeWithOption);

                return new SelectListItem(attributeName, attributeWithOption.Id.ToString());
            })
            .ToListAsync();

        model.AvailableOptions = (await _specificationAttributeService
                .GetSpecificationAttributeOptionsBySpecificationAttributeAsync(model.AttributeId))
            .Select(option => new SelectListItem { Text = option.Name, Value = option.Id.ToString() })
            .ToList();

        switch (attribute.AttributeType)
        {
            case SpecificationAttributeType.Option:
                model.ValueRaw = WebUtility.HtmlEncode(specAttributeOption.Name);
                model.SpecificationAttributeOptionId = specAttributeOption.Id;
                break;
            case SpecificationAttributeType.CustomText:
                model.Value = WebUtility.HtmlDecode(attribute.CustomValue);
                break;
            case SpecificationAttributeType.CustomHtmlText:
                model.ValueRaw = attribute.CustomValue;
                break;
            case SpecificationAttributeType.Hyperlink:
                model.Value = attribute.CustomValue;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(attribute.AttributeType));
        }

        model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(
            async (AddSpecificationAttributeLocalizedModel locale, int languageId) =>
            {
                switch (attribute.AttributeType)
                {
                    case SpecificationAttributeType.CustomHtmlText:
                        locale.ValueRaw = await _localizationService.GetLocalizedAsync(attribute, entity => entity.CustomValue, languageId, false, false);
                        break;
                    case SpecificationAttributeType.CustomText:
                        locale.Value = await _localizationService.GetLocalizedAsync(attribute, entity => entity.CustomValue, languageId, false, false);
                        break;
                    case SpecificationAttributeType.Option:
                        break;
                    case SpecificationAttributeType.Hyperlink:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });

        return model;
    }

    /// <summary>
    /// Prepare product tag search model
    /// </summary>
    /// <param name="searchModel">Product tag search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product tag search model
    /// </returns>
    public virtual Task<ProductTagSearchModel> PrepareProductTagSearchModelAsync(ProductTagSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //prepare page parameters
        searchModel.SetGridPageSize();

        return Task.FromResult(searchModel);
    }

    /// <summary>
    /// Prepare paged product tag list model
    /// </summary>
    /// <param name="searchModel">Product tag search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product tag list model
    /// </returns>
    public virtual async Task<ProductTagListModel> PrepareProductTagListModelAsync(ProductTagSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

        //get product tags
        var productTags = (await (await _productTagService.GetAllProductTagsAsync(tagName: searchModel.SearchTagName))
                .OrderByDescendingAwait(async tag => await _productTagService.GetProductCountByProductTagIdAsync(tag.Id, storeId: 0, showHidden: true)).ToListAsync())
            .ToPagedList(searchModel);

        //prepare list model
        var model = await new ProductTagListModel().PrepareToGridAsync(searchModel, productTags, () =>
        {
            return productTags.SelectAwait(async tag =>
            {
                //fill in model values from the entity
                var productTagModel = tag.ToModel<ProductTagModel>();

                //fill in additional values (not existing in the entity)
                productTagModel.ProductCount = await _productTagService.GetProductCountByProductTagIdAsync(tag.Id, storeId: 0, showHidden: true);

                return productTagModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare product tag model
    /// </summary>
    /// <param name="model">Product tag model</param>
    /// <param name="productTag">Product tag</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product tag model
    /// </returns>
    public virtual async Task<ProductTagModel> PrepareProductTagModelAsync(ProductTagModel model, ProductTag productTag, bool excludeProperties = false)
    {
        Func<ProductTagLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (productTag != null)
        {
            //fill in model values from the entity
            if (model == null)
            {
                model = productTag.ToModel<ProductTagModel>();
            }

            model.ProductCount = await _productTagService.GetProductCountByProductTagIdAsync(productTag.Id, storeId: 0, showHidden: true);

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(productTag, entity => entity.Name, languageId, false, false);
            };
        }

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        return model;
    }

    /// <summary>
    /// Prepare paged product order list model
    /// </summary>
    /// <param name="searchModel">Product order search model</param>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product order list model
    /// </returns>
    public virtual async Task<ProductOrderListModel> PrepareProductOrderListModelAsync(ProductOrderSearchModel searchModel, Product product)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(product);

        //get orders
        var orders = await _orderService.SearchOrdersAsync(productId: searchModel.ProductId,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare grid model
        var model = await new ProductOrderListModel().PrepareToGridAsync(searchModel, orders, () =>
        {
            return orders.SelectAwait(async order =>
            {
                var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

                //fill in model values from the entity
                var orderModel = new OrderModel
                {
                    Id = order.Id,
                    CustomerEmail = billingAddress.Email,
                    CustomOrderNumber = order.CustomOrderNumber
                };

                //convert dates to the user time
                orderModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                orderModel.StoreName = (await _storeService.GetStoreByIdAsync(order.StoreId))?.Name ?? "Deleted";
                orderModel.OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus);
                orderModel.PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus);
                orderModel.ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus);

                return orderModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare paged tier price list model
    /// </summary>
    /// <param name="searchModel">Tier price search model</param>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ier price list model
    /// </returns>
    public virtual async Task<TierPriceListModel> PrepareTierPriceListModelAsync(TierPriceSearchModel searchModel, Product product)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(product);

        //get tier prices
        var tierPrices = (await _productService.GetTierPricesByProductAsync(product.Id))
            .OrderBy(price => price.StoreId).ThenBy(price => price.Quantity).ThenBy(price => price.CustomerRoleId)
            .ToList().ToPagedList(searchModel);

        //prepare grid model
        var model = await new TierPriceListModel().PrepareToGridAsync(searchModel, tierPrices, () =>
        {
            return tierPrices.SelectAwait(async price =>
            {
                //fill in model values from the entity
                var tierPriceModel = price.ToModel<TierPriceModel>();

                //fill in additional values (not existing in the entity)   
                tierPriceModel.Store = price.StoreId > 0
                    ? ((await _storeService.GetStoreByIdAsync(price.StoreId))?.Name ?? "Deleted")
                    : await _localizationService.GetResourceAsync("Admin.Catalog.Products.TierPrices.Fields.Store.All");
                tierPriceModel.CustomerRoleId = price.CustomerRoleId ?? 0;
                tierPriceModel.CustomerRole = price.CustomerRoleId.HasValue
                    ? (await _customerService.GetCustomerRoleByIdAsync(price.CustomerRoleId.Value))?.Name
                    : await _localizationService.GetResourceAsync("Admin.Catalog.Products.TierPrices.Fields.CustomerRole.All");

                return tierPriceModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare tier price model
    /// </summary>
    /// <param name="model">Tier price model</param>
    /// <param name="product">Product</param>
    /// <param name="tierPrice">Tier price</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ier price model
    /// </returns>
    public virtual async Task<TierPriceModel> PrepareTierPriceModelAsync(TierPriceModel model,
        Product product, TierPrice tierPrice, bool excludeProperties = false)
    {
        ArgumentNullException.ThrowIfNull(product);

        if (tierPrice != null)
        {
            //fill in model values from the entity
            if (model == null)
                model = tierPrice.ToModel<TierPriceModel>();
        }

        //prepare available stores
        await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores);

        //prepare available customer roles
        await _baseAdminModelFactory.PrepareCustomerRolesAsync(model.AvailableCustomerRoles);

        return model;
    }

    /// <summary>
    /// Prepare paged stock quantity history list model
    /// </summary>
    /// <param name="searchModel">Stock quantity history search model</param>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the stock quantity history list model
    /// </returns>
    public virtual async Task<StockQuantityHistoryListModel> PrepareStockQuantityHistoryListModelAsync(StockQuantityHistorySearchModel searchModel, Product product)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(product);

        //get stock quantity history
        var stockQuantityHistory = await _productService.GetStockQuantityHistoryAsync(product: product,
            warehouseId: searchModel.WarehouseId,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var currentStore = await _storeContext.GetCurrentStoreAsync();

        //prepare grid model
        var model = await new StockQuantityHistoryListModel().PrepareToGridAsync(searchModel, stockQuantityHistory, () =>
        {
            return stockQuantityHistory.SelectAwait(async historyEntry =>
            {
                //fill in model values from the entity
                var stockQuantityHistoryModel = historyEntry.ToModel<StockQuantityHistoryModel>();

                //convert dates to the user time
                stockQuantityHistoryModel.CreatedOn =
                    await _dateTimeHelper.ConvertToUserTimeAsync(historyEntry.CreatedOnUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                var combination = await _productAttributeService.GetProductAttributeCombinationByIdAsync(historyEntry.CombinationId ?? 0);
                if (combination != null)
                {
                    stockQuantityHistoryModel.AttributeCombination = await _productAttributeFormatter
                        .FormatAttributesAsync(product, combination.AttributesXml, currentCustomer, currentStore, renderGiftCardAttributes: false);
                }

                stockQuantityHistoryModel.WarehouseName = historyEntry.WarehouseId.HasValue
                    ? (await _shippingService.GetWarehouseByIdAsync(historyEntry.WarehouseId.Value))?.Name ?? "Deleted"
                    : await _localizationService.GetResourceAsync("Admin.Catalog.Products.Fields.Warehouse.None");

                return stockQuantityHistoryModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare paged product attribute mapping list model
    /// </summary>
    /// <param name="searchModel">Product attribute mapping search model</param>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute mapping list model
    /// </returns>
    public virtual async Task<ProductAttributeMappingListModel> PrepareProductAttributeMappingListModelAsync(ProductAttributeMappingSearchModel searchModel,
        Product product)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(product);

        //get product attribute mappings
        var productAttributeMappings = (await _productAttributeService
            .GetProductAttributeMappingsByProductIdAsync(product.Id)).ToPagedList(searchModel);

        //prepare grid model
        var model = await new ProductAttributeMappingListModel().PrepareToGridAsync(searchModel, productAttributeMappings, () =>
        {
            return productAttributeMappings.SelectAwait(async attributeMapping =>
            {
                //fill in model values from the entity
                var productAttributeMappingModel = attributeMapping.ToModel<ProductAttributeMappingModel>();

                //fill in additional values (not existing in the entity)
                productAttributeMappingModel.ConditionString = string.Empty;

                productAttributeMappingModel.ValidationRulesString = await PrepareProductAttributeMappingValidationRulesStringAsync(attributeMapping);
                productAttributeMappingModel.ProductAttribute = (await _productAttributeService.GetProductAttributeByIdAsync(attributeMapping.ProductAttributeId))?.Name;
                productAttributeMappingModel.AttributeControlType = await _localizationService.GetLocalizedEnumAsync(attributeMapping.AttributeControlType);
                var conditionAttribute = (await _productAttributeParser
                        .ParseProductAttributeMappingsAsync(attributeMapping.ConditionAttributeXml))
                    .FirstOrDefault();
                if (conditionAttribute == null)
                    return productAttributeMappingModel;

                var conditionValue = (await _productAttributeParser
                        .ParseProductAttributeValuesAsync(attributeMapping.ConditionAttributeXml))
                    .FirstOrDefault();
                if (conditionValue != null)
                {
                    productAttributeMappingModel.ConditionString =
                        $"{WebUtility.HtmlEncode((await _productAttributeService.GetProductAttributeByIdAsync(conditionAttribute.ProductAttributeId)).Name)}: {WebUtility.HtmlEncode(conditionValue.Name)}";
                }

                return productAttributeMappingModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare product attribute mapping model
    /// </summary>
    /// <param name="model">Product attribute mapping model</param>
    /// <param name="product">Product</param>
    /// <param name="productAttributeMapping">Product attribute mapping</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute mapping model
    /// </returns>
    public virtual async Task<ProductAttributeMappingModel> PrepareProductAttributeMappingModelAsync(ProductAttributeMappingModel model,
        Product product, ProductAttributeMapping productAttributeMapping, bool excludeProperties = false)
    {
        Func<ProductAttributeMappingLocalizedModel, int, Task> localizedModelConfiguration = null;

        ArgumentNullException.ThrowIfNull(product);

        if (productAttributeMapping != null)
        {
            //fill in model values from the entity
            model ??= new ProductAttributeMappingModel
            {
                Id = productAttributeMapping.Id
            };

            model.ProductAttribute = (await _productAttributeService.GetProductAttributeByIdAsync(productAttributeMapping.ProductAttributeId)).Name;
            model.AttributeControlType = await _localizationService.GetLocalizedEnumAsync(productAttributeMapping.AttributeControlType);

            if (!excludeProperties)
            {
                model.ProductAttributeId = productAttributeMapping.ProductAttributeId;
                model.TextPrompt = productAttributeMapping.TextPrompt;
                model.IsRequired = productAttributeMapping.IsRequired;
                model.AttributeControlTypeId = productAttributeMapping.AttributeControlTypeId;
                model.DisplayOrder = productAttributeMapping.DisplayOrder;
                model.ValidationMinLength = productAttributeMapping.ValidationMinLength;
                model.ValidationMaxLength = productAttributeMapping.ValidationMaxLength;
                model.ValidationFileAllowedExtensions = productAttributeMapping.ValidationFileAllowedExtensions;
                model.ValidationFileMaximumSize = productAttributeMapping.ValidationFileMaximumSize;
                model.DefaultValue = productAttributeMapping.DefaultValue;
            }

            //prepare condition attributes model
            model.ConditionAllowed = true;
            await PrepareProductAttributeConditionModelAsync(model.ConditionModel, productAttributeMapping);

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.TextPrompt = await _localizationService.GetLocalizedAsync(productAttributeMapping, entity => entity.TextPrompt, languageId, false, false);
                locale.DefaultValue = await _localizationService.GetLocalizedAsync(productAttributeMapping, entity => entity.DefaultValue, languageId, false, false);
            };

            //prepare nested search model
            PrepareProductAttributeValueSearchModel(model.ProductAttributeValueSearchModel, productAttributeMapping);
        }

        model.ProductId = product.Id;

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        //prepare available product attributes
        model.AvailableProductAttributes = (await _productAttributeService.GetAllProductAttributesAsync()).Select(productAttribute => new SelectListItem
        {
            Text = productAttribute.Name,
            Value = productAttribute.Id.ToString()
        }).ToList();

        return model;
    }

    /// <summary>
    /// Prepare paged product attribute value list model
    /// </summary>
    /// <param name="searchModel">Product attribute value search model</param>
    /// <param name="productAttributeMapping">Product attribute mapping</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute value list model
    /// </returns>
    public virtual async Task<ProductAttributeValueListModel> PrepareProductAttributeValueListModelAsync(ProductAttributeValueSearchModel searchModel,
        ProductAttributeMapping productAttributeMapping)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(productAttributeMapping);

        //get product attribute values
        var productAttributeValues = (await _productAttributeService
            .GetProductAttributeValuesAsync(productAttributeMapping.Id)).ToPagedList(searchModel);

        //prepare list model
        var model = await new ProductAttributeValueListModel().PrepareToGridAsync(searchModel, productAttributeValues, () =>
        {
            return productAttributeValues.SelectAwait(async value =>
            {
                //fill in model values from the entity
                var productAttributeValueModel = value.ToModel<ProductAttributeValueModel>();

                //fill in additional values (not existing in the entity)
                productAttributeValueModel.AttributeValueTypeName = await _localizationService.GetLocalizedEnumAsync(value.AttributeValueType);

                productAttributeValueModel.Name = productAttributeMapping.AttributeControlType != AttributeControlType.ColorSquares
                    ? value.Name : $"{value.Name} - {value.ColorSquaresRgb}";
                if (value.AttributeValueType == AttributeValueType.Simple)
                {
                    productAttributeValueModel.PriceAdjustmentStr = value.PriceAdjustment.ToString("G29");
                    if (value.PriceAdjustmentUsePercentage)
                        productAttributeValueModel.PriceAdjustmentStr += " %";
                    productAttributeValueModel.WeightAdjustmentStr = value.WeightAdjustment.ToString("G29");
                }

                if (value.AttributeValueType == AttributeValueType.AssociatedToProduct)
                {
                    productAttributeValueModel.AssociatedProductName = (await _productService.GetProductByIdAsync(value.AssociatedProductId))?.Name ?? string.Empty;
                }

                var valuePicture = (await _productAttributeService.GetProductAttributeValuePicturesAsync(value.Id)).FirstOrDefault();
                var pictureThumbnailUrl = await _pictureService.GetPictureUrlAsync(valuePicture?.PictureId ?? 0, 75, false);

                //little hack here. Grid is rendered wrong way with <img> without "src" attribute
                if (string.IsNullOrEmpty(pictureThumbnailUrl))
                    pictureThumbnailUrl = await _pictureService.GetDefaultPictureUrlAsync(targetSize: 1);

                productAttributeValueModel.PictureThumbnailUrl = pictureThumbnailUrl;

                return productAttributeValueModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare product attribute value model
    /// </summary>
    /// <param name="model">Product attribute value model</param>
    /// <param name="productAttributeMapping">Product attribute mapping</param>
    /// <param name="productAttributeValue">Product attribute value</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute value model
    /// </returns>
    public virtual async Task<ProductAttributeValueModel> PrepareProductAttributeValueModelAsync(ProductAttributeValueModel model,
        ProductAttributeMapping productAttributeMapping, ProductAttributeValue productAttributeValue, bool excludeProperties = false)
    {
        ArgumentNullException.ThrowIfNull(productAttributeMapping);

        Func<ProductAttributeValueLocalizedModel, int, Task> localizedModelConfiguration = null;

        if (productAttributeValue != null)
        {
            //fill in model values from the entity
            model ??= new ProductAttributeValueModel
            {
                ProductAttributeMappingId = productAttributeValue.ProductAttributeMappingId,
                AttributeValueTypeId = productAttributeValue.AttributeValueTypeId,
                AttributeValueTypeName = await _localizationService.GetLocalizedEnumAsync(productAttributeValue.AttributeValueType),
                AssociatedProductId = productAttributeValue.AssociatedProductId,
                Name = productAttributeValue.Name,
                ColorSquaresRgb = productAttributeValue.ColorSquaresRgb,
                DisplayColorSquaresRgb = productAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares,
                ImageSquaresPictureId = productAttributeValue.ImageSquaresPictureId,
                DisplayImageSquaresPicture = productAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares,
                PriceAdjustment = productAttributeValue.PriceAdjustment,
                PriceAdjustmentUsePercentage = productAttributeValue.PriceAdjustmentUsePercentage,
                WeightAdjustment = productAttributeValue.WeightAdjustment,
                Cost = productAttributeValue.Cost,
                CustomerEntersQty = productAttributeValue.CustomerEntersQty,
                Quantity = productAttributeValue.Quantity,
                IsPreSelected = productAttributeValue.IsPreSelected,
                DisplayOrder = productAttributeValue.DisplayOrder,
                PictureIds = (await _productAttributeService.GetProductAttributeValuePicturesAsync(productAttributeValue.Id))
                    .Select(c => c.PictureId).ToList(),
            };

            model.AssociatedProductName = (await _productService.GetProductByIdAsync(productAttributeValue.AssociatedProductId))?.Name;

            //define localized model configuration action
            localizedModelConfiguration = async (locale, languageId) =>
            {
                locale.Name = await _localizationService.GetLocalizedAsync(productAttributeValue, entity => entity.Name, languageId, false, false);
            };
        }

        model.ProductAttributeMappingId = productAttributeMapping.Id;
        model.DisplayColorSquaresRgb = productAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares;
        model.DisplayImageSquaresPicture = productAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares;

        //set default values for the new model
        if (productAttributeValue == null)
            model.Quantity = 1;

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        //prepare picture models
        var productPictures = await _productService.GetProductPicturesByProductIdAsync(productAttributeMapping.ProductId);
        model.ProductPictureModels = await productPictures.SelectAwait(async productPicture => new ProductPictureModel
        {
            Id = productPicture.Id,
            ProductId = productPicture.ProductId,
            PictureId = productPicture.PictureId,
            PictureUrl = await _pictureService.GetPictureUrlAsync(productPicture.PictureId),
            DisplayOrder = productPicture.DisplayOrder
        }).ToListAsync();

        return model;
    }

    /// <summary>
    /// Prepare product model to associate to the product attribute value
    /// </summary>
    /// <param name="searchModel">Product model to associate to the product attribute value</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product model to associate to the product attribute value
    /// </returns>
    public virtual async Task<AssociateProductToAttributeValueSearchModel> PrepareAssociateProductToAttributeValueSearchModelAsync(
        AssociateProductToAttributeValueSearchModel searchModel)
    {
        ArgumentNullException.ThrowIfNull(searchModel);

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
    /// Prepare paged product model to associate to the product attribute value
    /// </summary>
    /// <param name="searchModel">Product model to associate to the product attribute value</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product model to associate to the product attribute value
    /// </returns>
    public virtual async Task<AssociateProductToAttributeValueListModel> PrepareAssociateProductToAttributeValueListModelAsync(
        AssociateProductToAttributeValueSearchModel searchModel)
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
        var model = await new AssociateProductToAttributeValueListModel().PrepareToGridAsync(searchModel, products, () =>
        {
            //fill in model values from the entity
            return products.SelectAwait(async product =>
            {
                var productModel = product.ToModel<ProductModel>();

                productModel.SeName = await _urlRecordService.GetSeNameAsync(product, 0, true, false);

                return productModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare paged product attribute combination list model
    /// </summary>
    /// <param name="searchModel">Product attribute combination search model</param>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute combination list model
    /// </returns>
    public virtual async Task<ProductAttributeCombinationListModel> PrepareProductAttributeCombinationListModelAsync(
        ProductAttributeCombinationSearchModel searchModel, Product product)
    {
        ArgumentNullException.ThrowIfNull(searchModel);
        ArgumentNullException.ThrowIfNull(product);

        //get product attribute combinations
        var productAttributeCombinations = (await _productAttributeService
            .GetAllProductAttributeCombinationsAsync(product.Id)).ToPagedList(searchModel);

        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        var currentStore = await _storeContext.GetCurrentStoreAsync();

        //prepare grid model
        var model = await new ProductAttributeCombinationListModel().PrepareToGridAsync(searchModel, productAttributeCombinations, () =>
        {
            return productAttributeCombinations.SelectAwait(async combination =>
            {
                //fill in model values from the entity
                var productAttributeCombinationModel = combination.ToModel<ProductAttributeCombinationModel>();

                //fill in additional values (not existing in the entity)
                productAttributeCombinationModel.AttributesXml = await _productAttributeFormatter
                    .FormatAttributesAsync(product, combination.AttributesXml, currentCustomer, currentStore, "<br />", true, true, true, false);

                var combinationPicture = (await _productAttributeService.GetProductAttributeCombinationPicturesAsync(combination.Id)).FirstOrDefault();
                var pictureThumbnailUrl = await _pictureService.GetPictureUrlAsync(combinationPicture?.PictureId ?? 0, 75, false);
                    
                //little hack here. Grid is rendered wrong way with <img> without "src" attribute
                if (string.IsNullOrEmpty(pictureThumbnailUrl))
                    pictureThumbnailUrl = await _pictureService.GetDefaultPictureUrlAsync(targetSize: 1);

                productAttributeCombinationModel.PictureThumbnailUrl = pictureThumbnailUrl;
                var warnings = (await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(currentCustomer,
                        ShoppingCartType.ShoppingCart, product,
                        attributesXml: combination.AttributesXml,
                        ignoreNonCombinableAttributes: true)
                    ).Aggregate(string.Empty, (message, warning) => $"{message}{warning}<br />");
                productAttributeCombinationModel.Warnings = new List<string> { warnings };

                return productAttributeCombinationModel;
            });
        });

        return model;
    }

    /// <summary>
    /// Prepare product attribute combination model
    /// </summary>
    /// <param name="model">Product attribute combination model</param>
    /// <param name="product">Product</param>
    /// <param name="productAttributeCombination">Product attribute combination</param>
    /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the product attribute combination model
    /// </returns>
    public virtual async Task<ProductAttributeCombinationModel> PrepareProductAttributeCombinationModelAsync(ProductAttributeCombinationModel model,
        Product product, ProductAttributeCombination productAttributeCombination, bool excludeProperties = false)
    {
        ArgumentNullException.ThrowIfNull(product);

        if (productAttributeCombination != null)
        {
            //fill in model values from the entity
            model ??= new ProductAttributeCombinationModel
            {
                AllowOutOfStockOrders = productAttributeCombination.AllowOutOfStockOrders,
                AttributesXml = productAttributeCombination.AttributesXml,
                Gtin = productAttributeCombination.Gtin,
                Id = productAttributeCombination.Id,
                ManufacturerPartNumber = productAttributeCombination.ManufacturerPartNumber,
                NotifyAdminForQuantityBelow = productAttributeCombination.NotifyAdminForQuantityBelow,
                OverriddenPrice = productAttributeCombination.OverriddenPrice,
                PictureIds = (await _productAttributeService.GetProductAttributeCombinationPicturesAsync(productAttributeCombination.Id))
                    .Select(c => c.PictureId).ToList(),
                ProductId = productAttributeCombination.ProductId,
                Sku = productAttributeCombination.Sku,
                StockQuantity = productAttributeCombination.StockQuantity,
                MinStockQuantity = productAttributeCombination.MinStockQuantity
            };
        }

        model.ProductId = product.Id;

        //set default values for the new model
        if (productAttributeCombination == null)
        {
            model.ProductId = product.Id;
            model.StockQuantity = 10000;
            model.NotifyAdminForQuantityBelow = 1;
        }

        //prepare picture models
        var productPictures = await _productService.GetProductPicturesByProductIdAsync(product.Id);
        model.ProductPictureModels = await productPictures.SelectAwait(async productPicture => new ProductPictureModel
        {
            Id = productPicture.Id,
            ProductId = productPicture.ProductId,
            PictureId = productPicture.PictureId,
            PictureUrl = await _pictureService.GetPictureUrlAsync(productPicture.PictureId),
            DisplayOrder = productPicture.DisplayOrder
        }).ToListAsync();

        //prepare product attribute mappings (exclude non-combinable attributes)
        var attributes = (await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id))
            .Where(productAttributeMapping => !productAttributeMapping.IsNonCombinable()).ToList();

        foreach (var attribute in attributes)
        {
            var attributeModel = new ProductAttributeCombinationModel.ProductAttributeModel
            {
                Id = attribute.Id,
                ProductAttributeId = attribute.ProductAttributeId,
                Name = (await _productAttributeService.GetProductAttributeByIdAsync(attribute.ProductAttributeId)).Name,
                TextPrompt = attribute.TextPrompt,
                IsRequired = attribute.IsRequired,
                AttributeControlType = attribute.AttributeControlType
            };

            if (attribute.ShouldHaveValues())
            {
                //values
                var attributeValues = await _productAttributeService.GetProductAttributeValuesAsync(attribute.Id);
                var preSelectedValue = _productAttributeParser.ParseValues(model.AttributesXml, attribute.Id);
                foreach (var attributeValue in attributeValues)
                {
                    var attributeValueModel = new ProductAttributeCombinationModel.ProductAttributeValueModel
                    {
                        Id = attributeValue.Id,
                        Name = attributeValue.Name,
                        IsPreSelected = preSelectedValue.Contains(attributeValue.Id.ToString())
                    };
                    attributeModel.Values.Add(attributeValueModel);
                }
            }

            model.ProductAttributes.Add(attributeModel);
        }

        return model;
    }

    #endregion
}