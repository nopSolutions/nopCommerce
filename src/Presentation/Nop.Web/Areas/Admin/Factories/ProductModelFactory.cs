using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
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
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the product model factory implementation
    /// </summary>
    public partial class ProductModelFactory : IProductModelFactory
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly IAddressService _addressService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDiscountService _discountService;
        private readonly IDiscountSupportedModelFactory _discountSupportedModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IManufacturerService _manufacturerService;
        private readonly IMeasureService _measureService;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly ISettingModelFactory _settingModelFactory;
        private readonly IShipmentService _shipmentService;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly MeasureSettings _measureSettings;
        private readonly TaxSettings _taxSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public ProductModelFactory(CatalogSettings catalogSettings,
            CurrencySettings currencySettings,
            IAclSupportedModelFactory aclSupportedModelFactory,
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
            IShipmentService shipmentService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            ISpecificationAttributeService specificationAttributeService,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            MeasureSettings measureSettings,
            TaxSettings taxSettings,
            VendorSettings vendorSettings)
        {
            _catalogSettings = catalogSettings;
            _currencySettings = currencySettings;
            _aclSupportedModelFactory = aclSupportedModelFactory;
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
            _measureSettings = measureSettings;
            _orderService = orderService;
            _pictureService = pictureService;
            _productAttributeFormatter = productAttributeFormatter;
            _productAttributeParser = productAttributeParser;
            _productAttributeService = productAttributeService;
            _productService = productService;
            _productTagService = productTagService;
            _productTemplateService = productTemplateService;
            _settingModelFactory = settingModelFactory;
            _shipmentService = shipmentService;
            _shippingService = shippingService;
            _shoppingCartService = shoppingCartService;
            _specificationAttributeService = specificationAttributeService;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _storeService = storeService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
            _taxSettings = taxSettings;
            _vendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare copy product model
        /// </summary>
        /// <param name="model">Copy product model</param>
        /// <param name="product">Product</param>
        /// <returns>Copy product model</returns>
        protected virtual CopyProductModel PrepareCopyProductModel(CopyProductModel model, Product product)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.Id = product.Id;
            model.Name = string.Format(_localizationService.GetResource("Admin.Catalog.Products.Copy.Name.New"), product.Name);
            model.Published = true;
            model.CopyImages = true;

            return model;
        }

        /// <summary>
        /// Prepare product warehouse inventory models
        /// </summary>
        /// <param name="models">List of product warehouse inventory models</param>
        /// <param name="product">Product</param>
        protected virtual void PrepareProductWarehouseInventoryModels(IList<ProductWarehouseInventoryModel> models, Product product)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            foreach (var warehouse in _shippingService.GetAllWarehouses())
            {
                var model = new ProductWarehouseInventoryModel
                {
                    WarehouseId = warehouse.Id,
                    WarehouseName = warehouse.Name
                };

                if (product != null)
                {
                    var productWarehouseInventory = _productService.GetAllProductWarehouseInventoryRecords(product.Id)?.FirstOrDefault(inventory => inventory.WarehouseId == warehouse.Id);
                    if (productWarehouseInventory != null)
                    {
                        model.WarehouseUsed = true;
                        model.StockQuantity = productWarehouseInventory.StockQuantity;
                        model.ReservedQuantity = productWarehouseInventory.ReservedQuantity;
                        model.PlannedQuantity = _shipmentService.GetQuantityInShipments(product, productWarehouseInventory.WarehouseId, true, true);
                    }
                }

                models.Add(model);
            }
        }

        /// <summary>
        /// Prepare product attribute mapping validation rules string
        /// </summary>
        /// <param name="attributeMapping">Product attribute mapping</param>
        /// <returns>Validation rules string</returns>
        protected virtual string PrepareProductAttributeMappingValidationRulesString(ProductAttributeMapping attributeMapping)
        {
            if (!attributeMapping.ValidationRulesAllowed())
                return string.Empty;

            var validationRules = new StringBuilder(string.Empty);
            if (attributeMapping.ValidationMinLength.HasValue)
            {
                validationRules.AppendFormat("{0}: {1}<br />",
                    _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.MinLength"),
                    attributeMapping.ValidationMinLength);
            }

            if (attributeMapping.ValidationMaxLength.HasValue)
            {
                validationRules.AppendFormat("{0}: {1}<br />",
                    _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.MaxLength"),
                    attributeMapping.ValidationMaxLength);
            }

            if (!string.IsNullOrEmpty(attributeMapping.ValidationFileAllowedExtensions))
            {
                validationRules.AppendFormat("{0}: {1}<br />",
                    _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.FileAllowedExtensions"),
                    WebUtility.HtmlEncode(attributeMapping.ValidationFileAllowedExtensions));
            }

            if (attributeMapping.ValidationFileMaximumSize.HasValue)
            {
                validationRules.AppendFormat("{0}: {1}<br />",
                    _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.FileMaximumSize"),
                    attributeMapping.ValidationFileMaximumSize);
            }

            if (!string.IsNullOrEmpty(attributeMapping.DefaultValue))
            {
                validationRules.AppendFormat("{0}: {1}<br />",
                    _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.DefaultValue"),
                    WebUtility.HtmlEncode(attributeMapping.DefaultValue));
            }

            return validationRules.ToString();
        }

        /// <summary>
        /// Prepare product attribute condition model
        /// </summary>
        /// <param name="model">Product attribute condition model</param>
        /// <param name="productAttributeMapping">Product attribute mapping</param>
        protected virtual void PrepareProductAttributeConditionModel(ProductAttributeConditionModel model,
            ProductAttributeMapping productAttributeMapping)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (productAttributeMapping == null)
                throw new ArgumentNullException(nameof(productAttributeMapping));

            model.ProductAttributeMappingId = productAttributeMapping.Id;
            model.EnableCondition = !string.IsNullOrEmpty(productAttributeMapping.ConditionAttributeXml);

            //pre-select attribute and values
            var selectedPva = _productAttributeParser
                .ParseProductAttributeMappings(productAttributeMapping.ConditionAttributeXml)
                .FirstOrDefault();

            var attributes = _productAttributeService.GetProductAttributeMappingsByProductId(productAttributeMapping.ProductId)
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
                    Name = _productAttributeService.GetProductAttributeById(attribute.ProductAttributeId).Name,
                    TextPrompt = attribute.TextPrompt,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
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
                                        _productAttributeParser.ParseProductAttributeValues(productAttributeMapping
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
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

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
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

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
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

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
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

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
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

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
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

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
        /// <returns>Stock quantity history search model</returns>
        protected virtual StockQuantityHistorySearchModel PrepareStockQuantityHistorySearchModel(StockQuantityHistorySearchModel searchModel, Product product)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            searchModel.ProductId = product.Id;

            //prepare available warehouses
            _baseAdminModelFactory.PrepareWarehouses(searchModel.AvailableWarehouses);

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
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

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
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (productAttributeMapping == null)
                throw new ArgumentNullException(nameof(productAttributeMapping));

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
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

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
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

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
        /// <returns>Product search model</returns>
        public virtual ProductSearchModel PrepareProductSearchModel(ProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //a vendor should have access only to his products
            searchModel.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
            searchModel.AllowVendorsToImportProducts = _vendorSettings.AllowVendorsToImportProducts;

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

            //prepare available warehouses
            _baseAdminModelFactory.PrepareWarehouses(searchModel.AvailableWarehouses);

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            //prepare "published" filter (0 - all; 1 - published only; 2 - unpublished only)
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "0",
                Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.All")
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "1",
                Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.PublishedOnly")
            });
            searchModel.AvailablePublishedOptions.Add(new SelectListItem
            {
                Value = "2",
                Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.UnpublishedOnly")
            });

            //prepare grid
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged product list model
        /// </summary>
        /// <param name="searchModel">Product search model</param>
        /// <returns>Product list model</returns>
        public virtual ProductListModel PrepareProductListModel(ProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter comments
            var overridePublished = searchModel.SearchPublishedId == 0 ? null : (bool?)(searchModel.SearchPublishedId == 1);
            if (_workContext.CurrentVendor != null)
                searchModel.SearchVendorId = _workContext.CurrentVendor.Id;
            var categoryIds = new List<int> { searchModel.SearchCategoryId };
            if (searchModel.SearchIncludeSubCategories && searchModel.SearchCategoryId > 0)
            {
                var childCategoryIds = _categoryService.GetChildCategoryIds(parentCategoryId: searchModel.SearchCategoryId, showHidden: true);
                categoryIds.AddRange(childCategoryIds);
            }

            //get products
            var products = _productService.SearchProducts(showHidden: true,
                categoryIds: categoryIds,
                manufacturerId: searchModel.SearchManufacturerId,
                storeId: searchModel.SearchStoreId,
                vendorId: searchModel.SearchVendorId,
                warehouseId: searchModel.SearchWarehouseId,
                productType: searchModel.SearchProductTypeId > 0 ? (ProductType?)searchModel.SearchProductTypeId : null,
                keywords: searchModel.SearchProductName,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,
                overridePublished: overridePublished);

            //prepare list model
            var model = new ProductListModel().PrepareToGrid(searchModel, products, () =>
            {
                return products.Select(product =>
                {
                    //fill in model values from the entity
                    var productModel = product.ToModel<ProductModel>();

                    //little performance optimization: ensure that "FullDescription" is not returned
                    productModel.FullDescription = string.Empty;

                    //fill in additional values (not existing in the entity)
                    productModel.SeName = _urlRecordService.GetSeName(product, 0, true, false);
                    var defaultProductPicture = _pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();
                    productModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(ref defaultProductPicture, 75);
                    productModel.ProductTypeName = _localizationService.GetLocalizedEnum(product.ProductType);
                    if (product.ProductType == ProductType.SimpleProduct && product.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
                        productModel.StockQuantityStr = _productService.GetTotalStockQuantity(product).ToString();

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
        /// <returns>Product model</returns>
        public virtual ProductModel PrepareProductModel(ProductModel model, Product product, bool excludeProperties = false)
        {
            Action<ProductLocalizedModel, int> localizedModelConfiguration = null;

            if (product != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = product.ToModel<ProductModel>();
                    model.SeName = _urlRecordService.GetSeName(product, 0, true, false);
                }

                var parentGroupedProduct = _productService.GetProductById(product.ParentGroupedProductId);
                if (parentGroupedProduct != null)
                {
                    model.AssociatedToProductId = product.ParentGroupedProductId;
                    model.AssociatedToProductName = parentGroupedProduct.Name;
                }

                model.LastStockQuantity = product.StockQuantity;
                model.ProductTags = string.Join(", ", _productTagService.GetAllProductTagsByProductId(product.Id).Select(tag => tag.Name));
                model.ProductAttributesExist = _productAttributeService.GetAllProductAttributes().Any();

                model.CanCreateCombinations = _productAttributeService
                    .GetProductAttributeMappingsByProductId(product.Id).Any(pam => _productAttributeService.GetProductAttributeValues(pam.Id).Any());

                if (!excludeProperties)
                {
                    model.SelectedCategoryIds = _categoryService.GetProductCategoriesByProductId(product.Id, true)
                        .Select(productCategory => productCategory.CategoryId).ToList();
                    model.SelectedManufacturerIds = _manufacturerService.GetProductManufacturersByProductId(product.Id, true)
                        .Select(productManufacturer => productManufacturer.ManufacturerId).ToList();
                }

                //prepare copy product model
                PrepareCopyProductModel(model.CopyProductModel, product);

                //prepare nested search model
                PrepareRelatedProductSearchModel(model.RelatedProductSearchModel, product);
                PrepareCrossSellProductSearchModel(model.CrossSellProductSearchModel, product);
                PrepareAssociatedProductSearchModel(model.AssociatedProductSearchModel, product);
                PrepareProductPictureSearchModel(model.ProductPictureSearchModel, product);
                PrepareProductSpecificationAttributeSearchModel(model.ProductSpecificationAttributeSearchModel, product);
                PrepareProductOrderSearchModel(model.ProductOrderSearchModel, product);
                PrepareTierPriceSearchModel(model.TierPriceSearchModel, product);
                PrepareStockQuantityHistorySearchModel(model.StockQuantityHistorySearchModel, product);
                PrepareProductAttributeMappingSearchModel(model.ProductAttributeMappingSearchModel, product);
                PrepareProductAttributeCombinationSearchModel(model.ProductAttributeCombinationSearchModel, product);

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(product, entity => entity.Name, languageId, false, false);
                    locale.FullDescription = _localizationService.GetLocalized(product, entity => entity.FullDescription, languageId, false, false);
                    locale.ShortDescription = _localizationService.GetLocalized(product, entity => entity.ShortDescription, languageId, false, false);
                    locale.MetaKeywords = _localizationService.GetLocalized(product, entity => entity.MetaKeywords, languageId, false, false);
                    locale.MetaDescription = _localizationService.GetLocalized(product, entity => entity.MetaDescription, languageId, false, false);
                    locale.MetaTitle = _localizationService.GetLocalized(product, entity => entity.MetaTitle, languageId, false, false);
                    locale.SeName = _urlRecordService.GetSeName(product, languageId, false, false);
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

            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.BaseWeightIn = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name;
            model.BaseDimensionIn = _measureService.GetMeasureDimensionById(_measureSettings.BaseDimensionId).Name;
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
            model.HasAvailableSpecificationAttributes =
                _specificationAttributeService.GetSpecificationAttributesWithOptions().Any();

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            //prepare editor settings
            model.ProductEditorSettingsModel = _settingModelFactory.PrepareProductEditorSettingsModel();

            //prepare available product templates
            _baseAdminModelFactory.PrepareProductTemplates(model.AvailableProductTemplates, false);

            //prepare available product types
            var productTemplates = _productTemplateService.GetAllProductTemplates();
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
            _baseAdminModelFactory.PrepareDeliveryDates(model.AvailableDeliveryDates,
                defaultItemText: _localizationService.GetResource("Admin.Catalog.Products.Fields.DeliveryDate.None"));

            //prepare available product availability ranges
            _baseAdminModelFactory.PrepareProductAvailabilityRanges(model.AvailableProductAvailabilityRanges,
                defaultItemText: _localizationService.GetResource("Admin.Catalog.Products.Fields.ProductAvailabilityRange.None"));

            //prepare available vendors
            _baseAdminModelFactory.PrepareVendors(model.AvailableVendors,
                defaultItemText: _localizationService.GetResource("Admin.Catalog.Products.Fields.Vendor.None"));

            //prepare available tax categories
            _baseAdminModelFactory.PrepareTaxCategories(model.AvailableTaxCategories);

            //prepare available warehouses
            _baseAdminModelFactory.PrepareWarehouses(model.AvailableWarehouses,
                defaultItemText: _localizationService.GetResource("Admin.Catalog.Products.Fields.Warehouse.None"));
            PrepareProductWarehouseInventoryModels(model.ProductWarehouseInventoryModels, product);

            //prepare available base price units
            var availableMeasureWeights = _measureService.GetAllMeasureWeights()
                .Select(weight => new SelectListItem { Text = weight.Name, Value = weight.Id.ToString() }).ToList();
            model.AvailableBasepriceUnits = availableMeasureWeights;
            model.AvailableBasepriceBaseUnits = availableMeasureWeights;

            //prepare model categories
            _baseAdminModelFactory.PrepareCategories(model.AvailableCategories, false);
            foreach (var categoryItem in model.AvailableCategories)
            {
                categoryItem.Selected = int.TryParse(categoryItem.Value, out var categoryId)
                    && model.SelectedCategoryIds.Contains(categoryId);
            }

            //prepare model manufacturers
            _baseAdminModelFactory.PrepareManufacturers(model.AvailableManufacturers, false);
            foreach (var manufacturerItem in model.AvailableManufacturers)
            {
                manufacturerItem.Selected = int.TryParse(manufacturerItem.Value, out var manufacturerId)
                    && model.SelectedManufacturerIds.Contains(manufacturerId);
            }

            //prepare model discounts
            var availableDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToSkus, showHidden: true);
            _discountSupportedModelFactory.PrepareModelDiscounts(model, product, availableDiscounts, excludeProperties);

            //prepare model customer roles
            _aclSupportedModelFactory.PrepareModelCustomerRoles(model, product, excludeProperties);

            //prepare model stores
            _storeMappingSupportedModelFactory.PrepareModelStores(model, product, excludeProperties);

            var productTags = _productTagService.GetAllProductTags();
            var productTagsSb = new StringBuilder();
            productTagsSb.Append("var initialProductTags = [");
            for (var i = 0; i < productTags.Count; i++)
            {
                var tag = productTags[i];
                productTagsSb.Append("'");
                productTagsSb.Append(JavaScriptEncoder.Default.Encode(tag.Name));
                productTagsSb.Append("'");
                if (i != productTags.Count - 1) 
                    productTagsSb.Append(",");
            }
            productTagsSb.Append("]");

            model.InitialProductTags = productTagsSb.ToString();

            return model;
        }

        /// <summary>
        /// Prepare required product search model to add to the product
        /// </summary>
        /// <param name="searchModel">Required product search model to add to the product</param>
        /// <returns>Required product search model to add to the product</returns>
        public virtual AddRequiredProductSearchModel PrepareAddRequiredProductSearchModel(AddRequiredProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

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
        /// Prepare required product list model to add to the product
        /// </summary>
        /// <param name="searchModel">Required product search model to add to the product</param>
        /// <returns>Required product list model to add to the product</returns>
        public virtual AddRequiredProductListModel PrepareAddRequiredProductListModel(AddRequiredProductSearchModel searchModel)
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
            var model = new AddRequiredProductListModel().PrepareToGrid(searchModel, products, () =>
            {
                return products.Select(product =>
                {
                    var productModel = product.ToModel<ProductModel>();
                    productModel.SeName = _urlRecordService.GetSeName(product, 0, true, false);

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
        /// <returns>Related product list model</returns>
        public virtual RelatedProductListModel PrepareRelatedProductListModel(RelatedProductSearchModel searchModel, Product product)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //get related products
            var relatedProducts = _productService
                .GetRelatedProductsByProductId1(productId1: product.Id, showHidden: true).ToPagedList(searchModel);

            //prepare grid model
            var model = new RelatedProductListModel().PrepareToGrid(searchModel, relatedProducts, () =>
            {
                return relatedProducts.Select(relatedProduct =>
                {
                    //fill in model values from the entity
                    var relatedProductModel = relatedProduct.ToModel<RelatedProductModel>();

                    //fill in additional values (not existing in the entity)
                    relatedProductModel.Product2Name = _productService.GetProductById(relatedProduct.ProductId2)?.Name;

                    return relatedProductModel;
                });
            });
            return model;
        }

        /// <summary>
        /// Prepare related product search model to add to the product
        /// </summary>
        /// <param name="searchModel">Related product search model to add to the product</param>
        /// <returns>Related product search model to add to the product</returns>
        public virtual AddRelatedProductSearchModel PrepareAddRelatedProductSearchModel(AddRelatedProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

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
        /// Prepare paged related product list model to add to the product
        /// </summary>
        /// <param name="searchModel">Related product search model to add to the product</param>
        /// <returns>Related product list model to add to the product</returns>
        public virtual AddRelatedProductListModel PrepareAddRelatedProductListModel(AddRelatedProductSearchModel searchModel)
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
            var model = new AddRelatedProductListModel().PrepareToGrid(searchModel, products, () =>
            {
                return products.Select(product =>
                {
                    var productModel = product.ToModel<ProductModel>();
                    productModel.SeName = _urlRecordService.GetSeName(product, 0, true, false);

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
        /// <returns>Cross-sell product list model</returns>
        public virtual CrossSellProductListModel PrepareCrossSellProductListModel(CrossSellProductSearchModel searchModel, Product product)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //get cross-sell products
            var crossSellProducts = _productService
                .GetCrossSellProductsByProductId1(productId1: product.Id, showHidden: true).ToPagedList(searchModel);

            //prepare grid model
            var model = new CrossSellProductListModel().PrepareToGrid(searchModel, crossSellProducts, () =>
            {
                return crossSellProducts.Select(crossSellProduct =>
                {
                    //fill in model values from the entity
                    var crossSellProductModel = new CrossSellProductModel
                    {
                        Id = crossSellProduct.Id,
                        ProductId2 = crossSellProduct.ProductId2
                    };

                    //fill in additional values (not existing in the entity)
                    crossSellProductModel.Product2Name = _productService.GetProductById(crossSellProduct.ProductId2)?.Name;

                    return crossSellProductModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare cross-sell product search model to add to the product
        /// </summary>
        /// <param name="searchModel">Cross-sell product search model to add to the product</param>
        /// <returns>Cross-sell product search model to add to the product</returns>
        public virtual AddCrossSellProductSearchModel PrepareAddCrossSellProductSearchModel(AddCrossSellProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

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
        /// Prepare paged crossSell product list model to add to the product
        /// </summary>
        /// <param name="searchModel">CrossSell product search model to add to the product</param>
        /// <returns>CrossSell product list model to add to the product</returns>
        public virtual AddCrossSellProductListModel PrepareAddCrossSellProductListModel(AddCrossSellProductSearchModel searchModel)
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
            var model = new AddCrossSellProductListModel().PrepareToGrid(searchModel, products, () =>
            {
                return products.Select(product =>
                {
                    var productModel = product.ToModel<ProductModel>();
                    productModel.SeName = _urlRecordService.GetSeName(product, 0, true, false);

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
        /// <returns>Associated product list model</returns>
        public virtual AssociatedProductListModel PrepareAssociatedProductListModel(AssociatedProductSearchModel searchModel, Product product)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //get associated products
            var associatedProducts = _productService.GetAssociatedProducts(showHidden: true,
                parentGroupedProductId: product.Id,
                vendorId: _workContext.CurrentVendor?.Id ?? 0).ToPagedList(searchModel);

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
        /// <returns>Associated product search model to add to the product</returns>
        public virtual AddAssociatedProductSearchModel PrepareAddAssociatedProductSearchModel(AddAssociatedProductSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

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
        /// Prepare paged associated product list model to add to the product
        /// </summary>
        /// <param name="searchModel">Associated product search model to add to the product</param>
        /// <returns>Associated product list model to add to the product</returns>
        public virtual AddAssociatedProductListModel PrepareAddAssociatedProductListModel(AddAssociatedProductSearchModel searchModel)
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
            var model = new AddAssociatedProductListModel().PrepareToGrid(searchModel, products, () =>
            {
                return products.Select(product =>
                {
                    //fill in model values from the entity
                    var productModel = product.ToModel<ProductModel>();

                    //fill in additional values (not existing in the entity)
                    productModel.SeName = _urlRecordService.GetSeName(product, 0, true, false);
                    var parentGroupedProduct = _productService.GetProductById(product.ParentGroupedProductId);
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
        /// <returns>Product picture list model</returns>
        public virtual ProductPictureListModel PrepareProductPictureListModel(ProductPictureSearchModel searchModel, Product product)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //get product pictures
            var productPictures = _productService.GetProductPicturesByProductId(product.Id).ToPagedList(searchModel);

            //prepare grid model
            var model = new ProductPictureListModel().PrepareToGrid(searchModel, productPictures, () =>
            {
                return productPictures.Select(productPicture =>
                {
                    //fill in model values from the entity
                    var productPictureModel = productPicture.ToModel<ProductPictureModel>();

                    //fill in additional values (not existing in the entity)
                    var picture = _pictureService.GetPictureById(productPicture.PictureId)
                                  ?? throw new Exception("Picture cannot be loaded");

                    productPictureModel.PictureUrl = _pictureService.GetPictureUrl(ref picture);
                    productPictureModel.OverrideAltAttribute = picture.AltAttribute;
                    productPictureModel.OverrideTitleAttribute = picture.TitleAttribute;

                    return productPictureModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged product specification attribute list model
        /// </summary>
        /// <param name="searchModel">Product specification attribute search model</param>
        /// <param name="product">Product</param>
        /// <returns>Product specification attribute list model</returns>
        public virtual ProductSpecificationAttributeListModel PrepareProductSpecificationAttributeListModel(
            ProductSpecificationAttributeSearchModel searchModel, Product product)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //get product specification attributes
            var productSpecificationAttributes = _specificationAttributeService
                .GetProductSpecificationAttributes(product.Id).ToPagedList(searchModel);

            //prepare grid model
            var model = new ProductSpecificationAttributeListModel().PrepareToGrid(searchModel, productSpecificationAttributes, () =>
            {
                return productSpecificationAttributes.Select(attribute =>
                {
                    //fill in model values from the entity
                    var productSpecificationAttributeModel = attribute.ToModel<ProductSpecificationAttributeModel>();

                    var specAttributeOption = _specificationAttributeService.GetSpecificationAttributeOptionById(attribute.SpecificationAttributeOptionId);
                    var specAttribute = _specificationAttributeService.GetSpecificationAttributeById(specAttributeOption.SpecificationAttributeId);

                    //fill in additional values (not existing in the entity)
                    productSpecificationAttributeModel.AttributeTypeName = _localizationService.GetLocalizedEnum(attribute.AttributeType);
                    productSpecificationAttributeModel.AttributeId = specAttribute.Id;
                    productSpecificationAttributeModel.AttributeName = specAttribute.Name;

                    switch (attribute.AttributeType)
                    {
                        case SpecificationAttributeType.Option:
                            productSpecificationAttributeModel.ValueRaw = WebUtility.HtmlEncode(specAttributeOption.Name);
                            productSpecificationAttributeModel.SpecificationAttributeOptionId = specAttributeOption.Id;
                            break;
                        case SpecificationAttributeType.CustomText:
                            productSpecificationAttributeModel.ValueRaw = WebUtility.HtmlEncode(_localizationService.GetLocalized(attribute, x => x.CustomValue, _workContext.WorkingLanguage.Id));
                            break;
                        case SpecificationAttributeType.CustomHtmlText:
                            productSpecificationAttributeModel.ValueRaw = _localizationService.GetLocalized(attribute, x => x.CustomValue, _workContext.WorkingLanguage.Id);
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
        /// <returns>Product specification attribute model</returns>
        public virtual AddSpecificationAttributeModel PrepareAddSpecificationAttributeModel(int productId, int? specificationId)
        {
            if (!specificationId.HasValue)
            {
                return new AddSpecificationAttributeModel
                {
                    AvailableAttributes = _specificationAttributeService.GetSpecificationAttributesWithOptions()
                        .Select(attributeWithOption => new SelectListItem(attributeWithOption.Name, attributeWithOption.Id.ToString()))
                        .ToList(),
                    ProductId = productId,
                    Locales = _localizedModelFactory.PrepareLocalizedModels<AddSpecificationAttributeLocalizedModel>()
                };
            }

            var attribute = _specificationAttributeService.GetProductSpecificationAttributeById(specificationId.Value);

            if (attribute == null)
            {
                throw new ArgumentException("No specification attribute found with the specified id");
            }

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && _productService.GetProductById(attribute.ProductId).VendorId != _workContext.CurrentVendor.Id)
                throw new UnauthorizedAccessException("This is not your product");

            var specAttributeOption = _specificationAttributeService.GetSpecificationAttributeOptionById(attribute.SpecificationAttributeOptionId);
            var specAttribute = _specificationAttributeService.GetSpecificationAttributeById(specAttributeOption.SpecificationAttributeId);

            var model = attribute.ToModel<AddSpecificationAttributeModel>();
            model.SpecificationId = attribute.Id;
            model.AttributeId = specAttribute.Id;
            model.AttributeTypeName = _localizationService.GetLocalizedEnum(attribute.AttributeType);
            model.AttributeName = specAttribute.Name;

            model.AvailableAttributes = _specificationAttributeService.GetSpecificationAttributesWithOptions()
                .Select(attributeWithOption => new SelectListItem(attributeWithOption.Name, attributeWithOption.Id.ToString()))
                .ToList();

            model.AvailableOptions = _specificationAttributeService
                .GetSpecificationAttributeOptionsBySpecificationAttribute(model.AttributeId)
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

            void localizedModelConfiguration(AddSpecificationAttributeLocalizedModel locale, int languageId)
            {
                switch (attribute.AttributeType)
                {
                    case SpecificationAttributeType.CustomHtmlText:
                        locale.ValueRaw = _localizationService.GetLocalized(attribute, entity => entity.CustomValue, languageId, false, false);
                        break;
                    case SpecificationAttributeType.CustomText:
                        locale.Value = _localizationService.GetLocalized(attribute, entity => entity.CustomValue, languageId, false, false);
                        break;
                    case SpecificationAttributeType.Option:
                        break;
                    case SpecificationAttributeType.Hyperlink:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            model.Locales = _localizedModelFactory.PrepareLocalizedModels((Action<AddSpecificationAttributeLocalizedModel, int>)localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare product tag search model
        /// </summary>
        /// <param name="searchModel">Product tag search model</param>
        /// <returns>Product tag search model</returns>
        public virtual ProductTagSearchModel PrepareProductTagSearchModel(ProductTagSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged product tag list model
        /// </summary>
        /// <param name="searchModel">Product tag search model</param>
        /// <returns>Product tag list model</returns>
        public virtual ProductTagListModel PrepareProductTagListModel(ProductTagSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get product tags
            var productTags = _productTagService.GetAllProductTags(tagName : searchModel.SearchTagName)
                .OrderByDescending(tag => _productTagService.GetProductCount(tag.Id, storeId: 0, showHidden: true)).ToList()
                .ToPagedList(searchModel);

            //prepare list model
            var model = new ProductTagListModel().PrepareToGrid(searchModel, productTags, () =>
            {
                return productTags.Select(tag =>
                {
                    //fill in model values from the entity
                    var productTagModel = tag.ToModel<ProductTagModel>();

                    //fill in additional values (not existing in the entity)
                    productTagModel.ProductCount = _productTagService.GetProductCount(tag.Id, storeId: 0, showHidden: true);

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
        /// <returns>Product tag model</returns>
        public virtual ProductTagModel PrepareProductTagModel(ProductTagModel model, ProductTag productTag, bool excludeProperties = false)
        {
            Action<ProductTagLocalizedModel, int> localizedModelConfiguration = null;

            if (productTag != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = productTag.ToModel<ProductTagModel>();
                }

                model.ProductCount = _productTagService.GetProductCount(productTag.Id, storeId: 0, showHidden: true);

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(productTag, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare paged product order list model
        /// </summary>
        /// <param name="searchModel">Product order search model</param>
        /// <param name="product">Product</param>
        /// <returns>Product order list model</returns>
        public virtual ProductOrderListModel PrepareProductOrderListModel(ProductOrderSearchModel searchModel, Product product)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //get orders
            var orders = _orderService.SearchOrders(productId: searchModel.ProductId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new ProductOrderListModel().PrepareToGrid(searchModel, orders, () =>
            {
                return orders.Select(order =>
                {
                    var billingAddress = _addressService.GetAddressById(order.BillingAddressId);

                    //fill in model values from the entity
                    var orderModel = new OrderModel
                    {
                        Id = order.Id,
                        CustomerEmail = billingAddress.Email,
                        CustomOrderNumber = order.CustomOrderNumber
                    };

                    //convert dates to the user time
                    orderModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    orderModel.StoreName = _storeService.GetStoreById(order.StoreId)?.Name ?? "Deleted";
                    orderModel.OrderStatus = _localizationService.GetLocalizedEnum(order.OrderStatus);
                    orderModel.PaymentStatus = _localizationService.GetLocalizedEnum(order.PaymentStatus);
                    orderModel.ShippingStatus = _localizationService.GetLocalizedEnum(order.ShippingStatus);

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
        /// <returns>Tier price list model</returns>
        public virtual TierPriceListModel PrepareTierPriceListModel(TierPriceSearchModel searchModel, Product product)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //get tier prices
            var tierPrices = _productService.GetTierPricesByProduct(product.Id)
                .OrderBy(price => price.StoreId).ThenBy(price => price.Quantity).ThenBy(price => price.CustomerRoleId)
                .ToList().ToPagedList(searchModel);

            //prepare grid model
            var model = new TierPriceListModel().PrepareToGrid(searchModel, tierPrices, () =>
            {
                return tierPrices.Select(price =>
                {
                    //fill in model values from the entity
                    var tierPriceModel = price.ToModel<TierPriceModel>();

                    //fill in additional values (not existing in the entity)   
                    tierPriceModel.Store = price.StoreId > 0
                        ? (_storeService.GetStoreById(price.StoreId)?.Name ?? "Deleted")
                        : _localizationService.GetResource("Admin.Catalog.Products.TierPrices.Fields.Store.All");
                    tierPriceModel.CustomerRoleId = price.CustomerRoleId ?? 0;
                    tierPriceModel.CustomerRole = price.CustomerRoleId.HasValue
                        ? _customerService.GetCustomerRoleById(price.CustomerRoleId.Value).Name
                        : _localizationService.GetResource("Admin.Catalog.Products.TierPrices.Fields.CustomerRole.All");

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
        /// <returns>Tier price model</returns>
        public virtual TierPriceModel PrepareTierPriceModel(TierPriceModel model,
            Product product, TierPrice tierPrice, bool excludeProperties = false)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (tierPrice != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = tierPrice.ToModel<TierPriceModel>();
                }
            }

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(model.AvailableStores);

            //prepare available customer roles
            _baseAdminModelFactory.PrepareCustomerRoles(model.AvailableCustomerRoles);

            return model;
        }

        /// <summary>
        /// Prepare paged stock quantity history list model
        /// </summary>
        /// <param name="searchModel">Stock quantity history search model</param>
        /// <param name="product">Product</param>
        /// <returns>Stock quantity history list model</returns>
        public virtual StockQuantityHistoryListModel PrepareStockQuantityHistoryListModel(StockQuantityHistorySearchModel searchModel, Product product)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //get stock quantity history
            var stockQuantityHistory = _productService.GetStockQuantityHistory(product: product,
                warehouseId: searchModel.WarehouseId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new StockQuantityHistoryListModel().PrepareToGrid(searchModel, stockQuantityHistory, () =>
            {
                return stockQuantityHistory.Select(historyEntry =>
                {
                    //fill in model values from the entity
                    var stockQuantityHistoryModel = historyEntry.ToModel<StockQuantityHistoryModel>();

                    //convert dates to the user time
                    stockQuantityHistoryModel.CreatedOn =
                        _dateTimeHelper.ConvertToUserTime(historyEntry.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    var combination =
                        _productAttributeService.GetProductAttributeCombinationById(historyEntry.CombinationId ?? 0);
                    if (combination != null)
                    {
                        stockQuantityHistoryModel.AttributeCombination = _productAttributeFormatter.FormatAttributes(
                            product,
                            combination.AttributesXml, _workContext.CurrentCustomer, renderGiftCardAttributes: false);
                    }

                    stockQuantityHistoryModel.WarehouseName = historyEntry.WarehouseId.HasValue
                        ? _shippingService.GetWarehouseById(historyEntry.WarehouseId.Value)?.Name ?? "Deleted"
                        : _localizationService.GetResource("Admin.Catalog.Products.Fields.Warehouse.None");

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
        /// <returns>Product attribute mapping list model</returns>
        public virtual ProductAttributeMappingListModel PrepareProductAttributeMappingListModel(ProductAttributeMappingSearchModel searchModel,
            Product product)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //get product attribute mappings
            var productAttributeMappings = _productAttributeService
                .GetProductAttributeMappingsByProductId(product.Id).ToPagedList(searchModel);

            //prepare grid model
            var model = new ProductAttributeMappingListModel().PrepareToGrid(searchModel, productAttributeMappings, () =>
            {
                return productAttributeMappings.Select(attributeMapping =>
                {
                    //fill in model values from the entity
                    var productAttributeMappingModel = attributeMapping.ToModel<ProductAttributeMappingModel>();

                    //fill in additional values (not existing in the entity)
                    productAttributeMappingModel.ConditionString = string.Empty;
                    productAttributeMappingModel.ValidationRulesString = PrepareProductAttributeMappingValidationRulesString(attributeMapping);
                    productAttributeMappingModel.ProductAttribute = _productAttributeService
                        .GetProductAttributeById(attributeMapping.ProductAttributeId)?.Name;
                    productAttributeMappingModel.AttributeControlType = _localizationService.GetLocalizedEnum(attributeMapping.AttributeControlType);
                    var conditionAttribute = _productAttributeParser
                        .ParseProductAttributeMappings(attributeMapping.ConditionAttributeXml).FirstOrDefault();
                    if (conditionAttribute == null)
                        return productAttributeMappingModel;

                    var conditionValue = _productAttributeParser.ParseProductAttributeValues(attributeMapping.ConditionAttributeXml).FirstOrDefault();
                    if (conditionValue != null)
                    {
                        productAttributeMappingModel.ConditionString =
                            $"{WebUtility.HtmlEncode(_productAttributeService.GetProductAttributeById(conditionAttribute.ProductAttributeId).Name)}: {WebUtility.HtmlEncode(conditionValue.Name)}";
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
        /// <returns>Product attribute mapping model</returns>
        public virtual ProductAttributeMappingModel PrepareProductAttributeMappingModel(ProductAttributeMappingModel model,
            Product product, ProductAttributeMapping productAttributeMapping, bool excludeProperties = false)
        {
            Action<ProductAttributeMappingLocalizedModel, int> localizedModelConfiguration = null;

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (productAttributeMapping != null)
            {
                //fill in model values from the entity
                model ??= new ProductAttributeMappingModel
                {
                    Id = productAttributeMapping.Id
                };

                model.ProductAttribute = _productAttributeService.GetProductAttributeById(productAttributeMapping.ProductAttributeId).Name;
                model.AttributeControlType = _localizationService.GetLocalizedEnum(productAttributeMapping.AttributeControlType);

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
                PrepareProductAttributeConditionModel(model.ConditionModel, productAttributeMapping);

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.TextPrompt = _localizationService.GetLocalized(productAttributeMapping, entity => entity.TextPrompt, languageId, false, false);
                    locale.DefaultValue = _localizationService.GetLocalized(productAttributeMapping, entity => entity.DefaultValue, languageId, false, false);
                };

                //prepare nested search model
                PrepareProductAttributeValueSearchModel(model.ProductAttributeValueSearchModel, productAttributeMapping);
            }

            model.ProductId = product.Id;

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            //prepare available product attributes
            model.AvailableProductAttributes = _productAttributeService.GetAllProductAttributes().Select(productAttribute => new SelectListItem
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
        /// <returns>Product attribute value list model</returns>
        public virtual ProductAttributeValueListModel PrepareProductAttributeValueListModel(ProductAttributeValueSearchModel searchModel,
            ProductAttributeMapping productAttributeMapping)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (productAttributeMapping == null)
                throw new ArgumentNullException(nameof(productAttributeMapping));

            //get product attribute values
            var productAttributeValues = _productAttributeService
                .GetProductAttributeValues(productAttributeMapping.Id).ToPagedList(searchModel);

            //prepare list model
            var model = new ProductAttributeValueListModel().PrepareToGrid(searchModel, productAttributeValues, () =>
            {
                return productAttributeValues.Select(value =>
                {
                    //fill in model values from the entity
                    var productAttributeValueModel = value.ToModel<ProductAttributeValueModel>();
                    
                    //fill in additional values (not existing in the entity)
                    productAttributeValueModel.AttributeValueTypeName = _localizationService.GetLocalizedEnum(value.AttributeValueType);
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
                        productAttributeValueModel
                            .AssociatedProductName = _productService.GetProductById(value.AssociatedProductId)?.Name ?? string.Empty;
                    }

                    var pictureThumbnailUrl = _pictureService.GetPictureUrl(value.PictureId, 75, false);
                    //little hack here. Grid is rendered wrong way with <img> without "src" attribute
                    if (string.IsNullOrEmpty(pictureThumbnailUrl))
                        pictureThumbnailUrl = _pictureService.GetDefaultPictureUrl(targetSize: 1);
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
        /// <returns>Product attribute value model</returns>
        public virtual ProductAttributeValueModel PrepareProductAttributeValueModel(ProductAttributeValueModel model,
            ProductAttributeMapping productAttributeMapping, ProductAttributeValue productAttributeValue, bool excludeProperties = false)
        {
            if (productAttributeMapping == null)
                throw new ArgumentNullException(nameof(productAttributeMapping));

            Action<ProductAttributeValueLocalizedModel, int> localizedModelConfiguration = null;

            if (productAttributeValue != null)
            {
                //fill in model values from the entity
                model ??= new ProductAttributeValueModel
                {
                    ProductAttributeMappingId = productAttributeValue.ProductAttributeMappingId,
                    AttributeValueTypeId = productAttributeValue.AttributeValueTypeId,
                    AttributeValueTypeName = _localizationService.GetLocalizedEnum(productAttributeValue.AttributeValueType),
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
                    PictureId = productAttributeValue.PictureId
                };

                model.AssociatedProductName = _productService.GetProductById(productAttributeValue.AssociatedProductId)?.Name;

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(productAttributeValue, entity => entity.Name, languageId, false, false);
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
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            //prepare picture models
            var productPictures = _productService.GetProductPicturesByProductId(productAttributeMapping.ProductId);
            model.ProductPictureModels = productPictures.Select(productPicture => new ProductPictureModel
            {
                Id = productPicture.Id,
                ProductId = productPicture.ProductId,
                PictureId = productPicture.PictureId,
                PictureUrl = _pictureService.GetPictureUrl(productPicture.PictureId),
                DisplayOrder = productPicture.DisplayOrder
            }).ToList();

            return model;
        }

        /// <summary>
        /// Prepare product model to associate to the product attribute value
        /// </summary>
        /// <param name="searchModel">Product model to associate to the product attribute value</param>
        /// <returns>Product model to associate to the product attribute value</returns>
        public virtual AssociateProductToAttributeValueSearchModel PrepareAssociateProductToAttributeValueSearchModel(
            AssociateProductToAttributeValueSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

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
        /// Prepare paged product model to associate to the product attribute value
        /// </summary>
        /// <param name="searchModel">Product model to associate to the product attribute value</param>
        /// <returns>Product model to associate to the product attribute value</returns>
        public virtual AssociateProductToAttributeValueListModel PrepareAssociateProductToAttributeValueListModel(
            AssociateProductToAttributeValueSearchModel searchModel)
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
            var model = new AssociateProductToAttributeValueListModel().PrepareToGrid(searchModel, products, () =>
            {
                //fill in model values from the entity
                return products.Select(product =>
                {
                    var productModel = product.ToModel<ProductModel>();
                    productModel.SeName = _urlRecordService.GetSeName(product, 0, true, false);

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
        /// <returns>Product attribute combination list model</returns>
        public virtual ProductAttributeCombinationListModel PrepareProductAttributeCombinationListModel(
            ProductAttributeCombinationSearchModel searchModel, Product product)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //get product attribute combinations
            var productAttributeCombinations = _productAttributeService
                .GetAllProductAttributeCombinations(product.Id).ToPagedList(searchModel);

            //prepare grid model
            var model = new ProductAttributeCombinationListModel().PrepareToGrid(searchModel, productAttributeCombinations, () =>
            {
                return productAttributeCombinations.Select(combination =>
                {
                    //fill in model values from the entity
                    var productAttributeCombinationModel = combination.ToModel<ProductAttributeCombinationModel>();

                    //fill in additional values (not existing in the entity)
                    productAttributeCombinationModel.AttributesXml = _productAttributeFormatter
                        .FormatAttributes(product, combination.AttributesXml, _workContext.CurrentCustomer, "<br />", true, true, true, false);
                    var pictureThumbnailUrl = _pictureService.GetPictureUrl(combination.PictureId, 75, false);
                    //little hack here. Grid is rendered wrong way with <img> without "src" attribute
                    if (string.IsNullOrEmpty(pictureThumbnailUrl))
                        pictureThumbnailUrl = _pictureService.GetDefaultPictureUrl(targetSize: 1);
                        
                    productAttributeCombinationModel.PictureThumbnailUrl = pictureThumbnailUrl;
                    var warnings = _shoppingCartService.GetShoppingCartItemAttributeWarnings(_workContext.CurrentCustomer,
                        ShoppingCartType.ShoppingCart, product,
                        attributesXml: combination.AttributesXml,
                        ignoreNonCombinableAttributes: true).Aggregate(string.Empty, (message, warning) => $"{message}{warning}<br />");
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
        /// <returns>Product attribute combination model</returns>
        public virtual ProductAttributeCombinationModel PrepareProductAttributeCombinationModel(ProductAttributeCombinationModel model,
            Product product, ProductAttributeCombination productAttributeCombination, bool excludeProperties = false)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

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
                    PictureId = productAttributeCombination.PictureId,
                    ProductId = productAttributeCombination.ProductId,
                    Sku = productAttributeCombination.Sku,
                    StockQuantity = productAttributeCombination.StockQuantity
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
            var productPictures = _productService.GetProductPicturesByProductId(product.Id);
            model.ProductPictureModels = productPictures.Select(productPicture => new ProductPictureModel
            {
                Id = productPicture.Id,
                ProductId = productPicture.ProductId,
                PictureId = productPicture.PictureId,
                PictureUrl = _pictureService.GetPictureUrl(productPicture.PictureId),
                DisplayOrder = productPicture.DisplayOrder
            }).ToList();

            //prepare product attribute mappings (exclude non-combinable attributes)
            var attributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id)
                .Where(productAttributeMapping => !productAttributeMapping.IsNonCombinable()).ToList();

            foreach (var attribute in attributes)
            {
                var attributeModel = new ProductAttributeCombinationModel.ProductAttributeModel
                {
                    Id = attribute.Id,
                    ProductAttributeId = attribute.ProductAttributeId,
                    Name = _productAttributeService.GetProductAttributeById(attribute.ProductAttributeId).Name,
                    TextPrompt = attribute.TextPrompt,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
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
}