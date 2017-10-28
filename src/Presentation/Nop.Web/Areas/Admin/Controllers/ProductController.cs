using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Helpers;
using Nop.Web.Areas.Admin.Infrastructure.Cache;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ProductController : BaseAdminController
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ICustomerService _customerService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IPictureService _pictureService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IProductTagService _productTagService;
        private readonly ICopyProductService _copyProductService;
        private readonly IPdfService _pdfService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IAclService _aclService;
        private readonly IStoreService _storeService;
        private readonly IOrderService _orderService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IVendorService _vendorService;
        private readonly IDateRangeService _dateRangeService;
        private readonly IShippingService _shippingService;
        private readonly IShipmentService _shipmentService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDiscountService _discountService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IDownloadService _downloadService;
        private readonly ISettingService _settingService;
        private readonly TaxSettings _taxSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public ProductController(IProductService productService,
            IProductTemplateService productTemplateService,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            ICustomerService customerService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            ISpecificationAttributeService specificationAttributeService,
            IPictureService pictureService,
            ITaxCategoryService taxCategoryService,
            IProductTagService productTagService,
            ICopyProductService copyProductService,
            IPdfService pdfService,
            IExportManager exportManager,
            IImportManager importManager,
            ICustomerActivityService customerActivityService,
            IPermissionService permissionService,
            IAclService aclService,
            IStoreService storeService,
            IOrderService orderService,
            IStoreMappingService storeMappingService,
            IVendorService vendorService,
            IDateRangeService dateRangeService,
            IShippingService shippingService,
            IShipmentService shipmentService,
            ICurrencyService currencyService,
            CurrencySettings currencySettings,
            IMeasureService measureService,
            MeasureSettings measureSettings,
            IStaticCacheManager cacheManager,
            IDateTimeHelper dateTimeHelper,
            IDiscountService discountService,
            IProductAttributeService productAttributeService,
            IBackInStockSubscriptionService backInStockSubscriptionService,
            IShoppingCartService shoppingCartService,
            IProductAttributeFormatter productAttributeFormatter,
            IProductAttributeParser productAttributeParser,
            IDownloadService downloadService,
            ISettingService settingService,
            TaxSettings taxSettings,
            VendorSettings vendorSettings)
        {
            this._productService = productService;
            this._productTemplateService = productTemplateService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._customerService = customerService;
            this._urlRecordService = urlRecordService;
            this._workContext = workContext;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._specificationAttributeService = specificationAttributeService;
            this._pictureService = pictureService;
            this._taxCategoryService = taxCategoryService;
            this._productTagService = productTagService;
            this._copyProductService = copyProductService;
            this._pdfService = pdfService;
            this._exportManager = exportManager;
            this._importManager = importManager;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
            this._aclService = aclService;
            this._storeService = storeService;
            this._orderService = orderService;
            this._storeMappingService = storeMappingService;
            this._vendorService = vendorService;
            this._dateRangeService = dateRangeService;
            this._shippingService = shippingService;
            this._shipmentService = shipmentService;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._measureService = measureService;
            this._measureSettings = measureSettings;
            this._cacheManager = cacheManager;
            this._dateTimeHelper = dateTimeHelper;
            this._discountService = discountService;
            this._productAttributeService = productAttributeService;
            this._backInStockSubscriptionService = backInStockSubscriptionService;
            this._shoppingCartService = shoppingCartService;
            this._productAttributeFormatter = productAttributeFormatter;
            this._productAttributeParser = productAttributeParser;
            this._downloadService = downloadService;
            this._settingService = settingService;
            this._taxSettings = taxSettings;
            this._vendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        protected virtual void UpdateLocales(Product product, ProductModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(product,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(product,
                    x => x.ShortDescription,
                    localized.ShortDescription,
                    localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(product,
                    x => x.FullDescription,
                    localized.FullDescription,
                    localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(product,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(product,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId);
                _localizedEntityService.SaveLocalizedValue(product,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId);

                //search engine name
                var seName = product.ValidateSeName(localized.SeName, localized.Name, false);
                _urlRecordService.SaveSlug(product, seName, localized.LanguageId);
            }
        }

        protected virtual void UpdateLocales(ProductTag productTag, ProductTagModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(productTag,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual void UpdateLocales(ProductAttributeMapping pam, ProductModel.ProductAttributeMappingModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(pam,
                    x => x.TextPrompt,
                    localized.TextPrompt,
                    localized.LanguageId);
            }
        }

        protected virtual void UpdateLocales(ProductAttributeValue pav, ProductModel.ProductAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(pav,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual void UpdatePictureSeoNames(Product product)
        {
            foreach (var pp in product.ProductPictures)
                _pictureService.SetSeoFilename(pp.PictureId, _pictureService.GetPictureSeName(product.Name));
        }

        protected virtual void PrepareAclModel(ProductModel model, Product product, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!excludeProperties && product != null)
                model.SelectedCustomerRoleIds = _aclService.GetCustomerRoleIdsWithAccess(product).ToList();

            var allRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var role in allRoles)
            {
                model.AvailableCustomerRoles.Add(new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Id.ToString(),
                    Selected = model.SelectedCustomerRoleIds.Contains(role.Id)
                });
            }
        }

        protected virtual void SaveProductAcl(Product product, ProductModel model)
        {
            product.SubjectToAcl = model.SelectedCustomerRoleIds.Any();

            var existingAclRecords = _aclService.GetAclRecords(product);
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        _aclService.InsertAclRecord(product, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        _aclService.DeleteAclRecord(aclRecordToDelete);
                }
            }
        }

        protected virtual void PrepareStoresMappingModel(ProductModel model, Product product, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!excludeProperties && product != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(product).ToList();

            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                model.AvailableStores.Add(new SelectListItem
                {
                    Text = store.Name,
                    Value = store.Id.ToString(),
                    Selected = model.SelectedStoreIds.Contains(store.Id)
                });
            }
        }

        protected virtual void SaveStoreMappings(Product product, ProductModel model)
        {
            product.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(product);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(product, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        protected virtual void PrepareCategoryMappingModel(ProductModel model, Product product, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!excludeProperties && product != null)
                model.SelectedCategoryIds = _categoryService.GetProductCategoriesByProductId(product.Id, true).Select(c => c.CategoryId).ToList();

            var allCategories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in allCategories)
            {
                c.Selected = model.SelectedCategoryIds.Contains(int.Parse(c.Value));
                model.AvailableCategories.Add(c);
            }
        }

        protected virtual void SaveCategoryMappings(Product product, ProductModel model)
        {
            var existingProductCategories = _categoryService.GetProductCategoriesByProductId(product.Id, true);

            //delete categories
            foreach (var existingProductCategory in existingProductCategories)
                if (!model.SelectedCategoryIds.Contains(existingProductCategory.CategoryId))
                    _categoryService.DeleteProductCategory(existingProductCategory);

            //add categories
            foreach (var categoryId in model.SelectedCategoryIds)
                if (existingProductCategories.FindProductCategory(product.Id, categoryId) == null)
                {
                    //find next display order
                    var displayOrder = 1;
                    var existingCategoryMapping = _categoryService.GetProductCategoriesByCategoryId(categoryId, showHidden: true);
                    if (existingCategoryMapping.Any())
                        displayOrder = existingCategoryMapping.Max(x => x.DisplayOrder) + 1;
                    _categoryService.InsertProductCategory(new ProductCategory
                    {
                        ProductId = product.Id,
                        CategoryId = categoryId,
                        DisplayOrder = displayOrder
                    });
                }
        }

        protected virtual void PrepareManufacturerMappingModel(ProductModel model, Product product, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!excludeProperties && product != null)
                model.SelectedManufacturerIds = _manufacturerService.GetProductManufacturersByProductId(product.Id, true).Select(c => c.ManufacturerId).ToList();

            var allManufacturers = SelectListHelper.GetManufacturerList(_manufacturerService, _cacheManager, true);
            foreach (var m in allManufacturers)
            {
                m.Selected = model.SelectedManufacturerIds.Contains(int.Parse(m.Value));
                model.AvailableManufacturers.Add(m);
            }
        }

        protected virtual void SaveManufacturerMappings(Product product, ProductModel model)
        {
            var existingProductManufacturers = _manufacturerService.GetProductManufacturersByProductId(product.Id, true);

            //delete manufacturers
            foreach (var existingProductManufacturer in existingProductManufacturers)
                if (!model.SelectedManufacturerIds.Contains(existingProductManufacturer.ManufacturerId))
                    _manufacturerService.DeleteProductManufacturer(existingProductManufacturer);

            //add manufacturers
            foreach (var manufacturerId in model.SelectedManufacturerIds)
                if (existingProductManufacturers.FindProductManufacturer(product.Id, manufacturerId) == null)
                {
                    //find next display order
                    var displayOrder = 1;
                    var existingManufacturerMapping = _manufacturerService.GetProductManufacturersByManufacturerId(manufacturerId, showHidden: true);
                    if (existingManufacturerMapping.Any())
                        displayOrder = existingManufacturerMapping.Max(x => x.DisplayOrder) + 1;
                    _manufacturerService.InsertProductManufacturer(new ProductManufacturer()
                    {
                        ProductId = product.Id,
                        ManufacturerId = manufacturerId,
                        DisplayOrder = displayOrder
                    });
                }
        }

        protected virtual void PrepareDiscountMappingModel(ProductModel model, Product product, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!excludeProperties && product != null)
                model.SelectedDiscountIds = product.AppliedDiscounts.Select(d => d.Id).ToList();

            foreach (var discount in _discountService.GetAllDiscounts(DiscountType.AssignedToSkus, showHidden: true))
            {
                model.AvailableDiscounts.Add(new SelectListItem
                {
                    Text = discount.Name,
                    Value = discount.Id.ToString(),
                    Selected = model.SelectedDiscountIds.Contains(discount.Id)
                });
            }
        }

        protected virtual void SaveDiscountMappings(Product product, ProductModel model)
        {
            var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToSkus, showHidden: true);

            foreach (var discount in allDiscounts)
            {
                if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                {
                    //new discount
                    if (product.AppliedDiscounts.Count(d => d.Id == discount.Id) == 0)
                        product.AppliedDiscounts.Add(discount);
                }
                else
                {
                    //remove discount
                    if (product.AppliedDiscounts.Count(d => d.Id == discount.Id) > 0)
                        product.AppliedDiscounts.Remove(discount);
                }
            }

            _productService.UpdateProduct(product);
            _productService.UpdateHasDiscountsApplied(product);
        }

        protected virtual void PrepareAddProductAttributeCombinationModel(AddProductAttributeCombinationModel model, Product product)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            model.ProductId = product.Id;
            model.StockQuantity = 10000;
            model.NotifyAdminForQuantityBelow = 1;

            var attributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id)
                //ignore non-combinable attributes for combinations
                .Where(x => !x.IsNonCombinable())
                .ToList();
            foreach (var attribute in attributes)
            {
                var attributeModel = new AddProductAttributeCombinationModel.ProductAttributeModel
                {
                    Id = attribute.Id,
                    ProductAttributeId = attribute.ProductAttributeId,
                    Name = attribute.ProductAttribute.Name,
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
                        var attributeValueModel = new AddProductAttributeCombinationModel.ProductAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.Name,
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }

                model.ProductAttributes.Add(attributeModel);
            }
        }

        protected virtual string[] ParseProductTags(string productTags)
        {
            var result = new List<string>();
            if (!string.IsNullOrWhiteSpace(productTags))
            {
                var values = productTags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var val1 in values)
                    if (!string.IsNullOrEmpty(val1.Trim()))
                        result.Add(val1.Trim());
            }
            return result.ToArray();
        }
        
        protected virtual void PrepareProductModel(ProductModel model, Product product, bool setPredefinedValues, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (product != null)
            {
                var parentGroupedProduct = _productService.GetProductById(product.ParentGroupedProductId);
                if (parentGroupedProduct != null)
                {
                    model.AssociatedToProductId = product.ParentGroupedProductId;
                    model.AssociatedToProductName = parentGroupedProduct.Name;
                }

                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(product.CreatedOnUtc, DateTimeKind.Utc);
                model.UpdatedOn = _dateTimeHelper.ConvertToUserTime(product.UpdatedOnUtc, DateTimeKind.Utc);
            }

            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.BaseWeightIn = _measureService.GetMeasureWeightById(_measureSettings.BaseWeightId).Name;
            model.BaseDimensionIn = _measureService.GetMeasureDimensionById(_measureSettings.BaseDimensionId).Name;
            
            //little performance hack here
            //there's no need to load attributes when creating a new product
            //anyway they're not used (you need to save a product before you map them)
            if (product != null)
            {
                //product attributes
                model.ProductAttributesExist = _productAttributeService.GetAllProductAttributes().Any();
                //specification attributes
                model.AddSpecificationAttributeModel.AvailableAttributes = _cacheManager
                    .Get(ModelCacheEventConsumer.SPEC_ATTRIBUTES_MODEL_KEY, () =>
                    {
                        var availableSpecificationAttributes = new List<SelectListItem>();
                        foreach (var sa in _specificationAttributeService.GetSpecificationAttributes())
                        {
                            availableSpecificationAttributes.Add(new SelectListItem
                            {
                                Text = sa.Name,
                                Value = sa.Id.ToString()
                            });
                        }
                        return availableSpecificationAttributes;
                    });

                //options of preselected specification attribute
                if (model.AddSpecificationAttributeModel.AvailableAttributes.Any())
                {
                    var selectedAttributeId = int.Parse(model.AddSpecificationAttributeModel.AvailableAttributes.First().Value);
                    foreach (var sao in _specificationAttributeService.GetSpecificationAttributeOptionsBySpecificationAttribute(selectedAttributeId))
                        model.AddSpecificationAttributeModel.AvailableOptions.Add(new SelectListItem
                        {
                            Text = sao.Name,
                            Value = sao.Id.ToString()
                        });
                }
                //default specs values
                model.AddSpecificationAttributeModel.ShowOnProductPage = true;
            }

            //copy product
            if (product != null)
            {
                model.CopyProductModel.Id = product.Id;
                model.CopyProductModel.Name = string.Format(_localizationService.GetResource("Admin.Catalog.Products.Copy.Name.New"), product.Name);
                model.CopyProductModel.Published = true;
                model.CopyProductModel.CopyImages = true;
            }

            //templates
            var templates = _productTemplateService.GetAllProductTemplates();
            foreach (var template in templates)
            {
                model.AvailableProductTemplates.Add(new SelectListItem
                {
                    Text = template.Name,
                    Value = template.Id.ToString()
                });
            }
            //supported product types
            foreach (var productType in ProductType.SimpleProduct.ToSelectList(false).ToList())
            {
                var productTypeId = int.Parse(productType.Value);
                model.ProductsTypesSupportedByProductTemplates.Add(productTypeId, new List<SelectListItem>());
                foreach (var template in templates)
                {
                    if (string.IsNullOrEmpty(template.IgnoredProductTypes) ||
                        !((IList<int>)TypeDescriptor.GetConverter(typeof(List<int>)).ConvertFrom(template.IgnoredProductTypes)).Contains(productTypeId))
                    {
                        model.ProductsTypesSupportedByProductTemplates[productTypeId].Add(new SelectListItem
                        {
                            Text = template.Name,
                            Value = template.Id.ToString()
                        });
                    }
                }
            }

            //vendors
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
            model.AvailableVendors.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Catalog.Products.Fields.Vendor.None"),
                Value = "0"
            });
            var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
            foreach (var v in vendors)
                model.AvailableVendors.Add(v);

            //delivery dates
            model.AvailableDeliveryDates.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Catalog.Products.Fields.DeliveryDate.None"),
                Value = "0"
            });
            var deliveryDates = _dateRangeService.GetAllDeliveryDates();
            foreach (var deliveryDate in deliveryDates)
            {
                model.AvailableDeliveryDates.Add(new SelectListItem
                {
                    Text = deliveryDate.Name,
                    Value = deliveryDate.Id.ToString()
                });
            }

            //product availability ranges
            model.AvailableProductAvailabilityRanges.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Catalog.Products.Fields.ProductAvailabilityRange.None"),
                Value = "0"
            });
            foreach (var range in _dateRangeService.GetAllProductAvailabilityRanges())
            {
                model.AvailableProductAvailabilityRanges.Add(new SelectListItem
                {
                    Text = range.Name,
                    Value = range.Id.ToString()
                });
            }

            //warehouses
            var warehouses = _shippingService.GetAllWarehouses();
            model.AvailableWarehouses.Add(new SelectListItem
            {
                Text = _localizationService.GetResource("Admin.Catalog.Products.Fields.Warehouse.None"),
                Value = "0"
            });
            foreach (var warehouse in warehouses)
            {
                model.AvailableWarehouses.Add(new SelectListItem
                {
                    Text = warehouse.Name,
                    Value = warehouse.Id.ToString()
                });
            }

            //multiple warehouses
            foreach (var warehouse in warehouses)
            {
                var pwiModel = new ProductModel.ProductWarehouseInventoryModel
                {
                    WarehouseId = warehouse.Id,
                    WarehouseName = warehouse.Name
                };
                if (product != null)
                {
                    var pwi = product.ProductWarehouseInventory.FirstOrDefault(x => x.WarehouseId == warehouse.Id);
                    if (pwi != null)
                    {
                        pwiModel.WarehouseUsed = true;
                        pwiModel.StockQuantity = pwi.StockQuantity;
                        pwiModel.ReservedQuantity = pwi.ReservedQuantity;
                        pwiModel.PlannedQuantity = _shipmentService.GetQuantityInShipments(product, pwi.WarehouseId, true, true);
                    }
                }
                model.ProductWarehouseInventoryModels.Add(pwiModel);
            }

            //product tags
            if (product != null)
            {
                var result = new StringBuilder();
                for (var i = 0; i < product.ProductTags.Count; i++)
                {
                    var pt = product.ProductTags.ToList()[i];
                    result.Append(pt.Name);
                    if (i != product.ProductTags.Count - 1)
                        result.Append(", ");
                }
                model.ProductTags = result.ToString();
            }

            //tax categories
            var taxCategories = _taxCategoryService.GetAllTaxCategories();
            model.AvailableTaxCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Configuration.Settings.Tax.TaxCategories.None"), Value = "0" });
            foreach (var tc in taxCategories)
                model.AvailableTaxCategories.Add(new SelectListItem { Text = tc.Name, Value = tc.Id.ToString(), Selected = product != null && !setPredefinedValues && tc.Id == product.TaxCategoryId });

            //baseprice units
            var measureWeights = _measureService.GetAllMeasureWeights();
            foreach (var mw in measureWeights)
                model.AvailableBasepriceUnits.Add(new SelectListItem { Text = mw.Name, Value = mw.Id.ToString(), Selected = product != null && !setPredefinedValues && mw.Id == product.BasepriceUnitId });
            foreach (var mw in measureWeights)
                model.AvailableBasepriceBaseUnits.Add(new SelectListItem { Text = mw.Name, Value = mw.Id.ToString(), Selected = product != null && !setPredefinedValues && mw.Id == product.BasepriceBaseUnitId });

            //last stock quantity
            if (product != null)
            {
                model.LastStockQuantity = product.StockQuantity;
            }

            //default values
            if (setPredefinedValues)
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

            //editor settings
            var productEditorSettings = _settingService.LoadSetting<ProductEditorSettings>();
            model.ProductEditorSettingsModel = productEditorSettings.ToModel();
        }

        protected virtual List<int> GetChildCategoryIds(int parentCategoryId)
        {
            var categoriesIds = new List<int>();
            var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId, true);
            foreach (var category in categories)
            {
                categoriesIds.Add(category.Id);
                categoriesIds.AddRange(GetChildCategoryIds(category.Id));
            }
            return categoriesIds;
        }

        protected virtual void SaveProductWarehouseInventory(Product product, ProductModel model)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (model.ManageInventoryMethodId != (int)ManageInventoryMethod.ManageStock)
                return;

            if (!model.UseMultipleWarehouses)
                return;

            var warehouses = _shippingService.GetAllWarehouses();

            var formData = this.Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());

            foreach (var warehouse in warehouses)
            {
                //parse stock quantity
                var stockQuantity = 0;
                foreach (var formKey in formData.Keys)
                {
                    if (formKey.Equals($"warehouse_qty_{warehouse.Id}", StringComparison.InvariantCultureIgnoreCase))
                    {
                        int.TryParse(formData[formKey], out stockQuantity);
                        break;
                    }
                }

                //parse reserved quantity
                var reservedQuantity = 0;
                foreach (var formKey in formData.Keys)
                    if (formKey.Equals($"warehouse_reserved_{warehouse.Id}", StringComparison.InvariantCultureIgnoreCase))
                    {
                        int.TryParse(formData[formKey], out reservedQuantity);
                        break;
                    }

                //parse "used" field
                var used = false;
                foreach (var formKey in formData.Keys)
                    if (formKey.Equals($"warehouse_used_{warehouse.Id}", StringComparison.InvariantCultureIgnoreCase))
                    {
                        int.TryParse(formData[formKey], out int tmp);
                        used = tmp == warehouse.Id;
                        break;
                    }

                //quantity change history message
                var message = $"{_localizationService.GetResource("Admin.StockQuantityHistory.Messages.MultipleWarehouses")} {_localizationService.GetResource("Admin.StockQuantityHistory.Messages.Edit")}";

                var existingPwI = product.ProductWarehouseInventory.FirstOrDefault(x => x.WarehouseId == warehouse.Id);
                if (existingPwI != null)
                {
                    if (used)
                    {
                        var previousStockQuantity = existingPwI.StockQuantity;

                        //update existing record
                        existingPwI.StockQuantity = stockQuantity;
                        existingPwI.ReservedQuantity = reservedQuantity;
                        _productService.UpdateProduct(product);

                        //quantity change history
                        _productService.AddStockQuantityHistoryEntry(product, existingPwI.StockQuantity - previousStockQuantity, existingPwI.StockQuantity,
                            existingPwI.WarehouseId, message);
                    }
                    else
                    {
                        //delete. no need to store record for qty 0
                        _productService.DeleteProductWarehouseInventory(existingPwI);

                        //quantity change history
                        _productService.AddStockQuantityHistoryEntry(product, -existingPwI.StockQuantity, 0, existingPwI.WarehouseId, message);
                    }
                }
                else
                {
                    if (used)
                    {
                        //no need to insert a record for qty 0
                        existingPwI = new ProductWarehouseInventory
                        {
                            WarehouseId = warehouse.Id,
                            ProductId = product.Id,
                            StockQuantity = stockQuantity,
                            ReservedQuantity = reservedQuantity
                        };
                        product.ProductWarehouseInventory.Add(existingPwI);
                        _productService.UpdateProduct(product);

                        //quantity change history
                        _productService.AddStockQuantityHistoryEntry(product, existingPwI.StockQuantity, existingPwI.StockQuantity,
                            existingPwI.WarehouseId, message);
                    }
                }
            }
        }

        protected virtual void PrepareProductAttributeMappingModel(ProductModel.ProductAttributeMappingModel model, 
            ProductAttributeMapping pam, Product product, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            model.ProductId = product.Id;

            foreach (var productAttribute in _productAttributeService.GetAllProductAttributes())
            {
                model.AvailableProductAttributes.Add(new SelectListItem
                {
                    Text = productAttribute.Name,
                    Value = productAttribute.Id.ToString()
                });
            }

            if (pam == null)
                return;

            model.Id = pam.Id;
            model.ProductAttribute = _productAttributeService.GetProductAttributeById(pam.ProductAttributeId).Name;
            model.AttributeControlType = pam.AttributeControlType.GetLocalizedEnum(_localizationService, _workContext);
            if (!excludeProperties)
            {
                model.ProductAttributeId = pam.ProductAttributeId;
                model.TextPrompt = pam.TextPrompt;
                model.IsRequired = pam.IsRequired;
                model.AttributeControlTypeId = pam.AttributeControlTypeId;
                model.DisplayOrder = pam.DisplayOrder;
                model.ValidationMinLength = pam.ValidationMinLength;
                model.ValidationMaxLength = pam.ValidationMaxLength;
                model.ValidationFileAllowedExtensions = pam.ValidationFileAllowedExtensions;
                model.ValidationFileMaximumSize = pam.ValidationFileMaximumSize;
                model.DefaultValue = pam.DefaultValue;
            }
                
            if (pam.ValidationRulesAllowed())
            {
                var validationRules = new StringBuilder(string.Empty);
                if (pam.ValidationMinLength != null)
                    validationRules.AppendFormat("{0}: {1}<br />",
                        _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.MinLength"),
                        pam.ValidationMinLength);
                if (pam.ValidationMaxLength != null)
                    validationRules.AppendFormat("{0}: {1}<br />",
                        _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.MaxLength"),
                        pam.ValidationMaxLength);
                if (!string.IsNullOrEmpty(pam.ValidationFileAllowedExtensions))
                    validationRules.AppendFormat("{0}: {1}<br />",
                        _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.FileAllowedExtensions"),
                        WebUtility.HtmlEncode(pam.ValidationFileAllowedExtensions));
                if (pam.ValidationFileMaximumSize != null)
                    validationRules.AppendFormat("{0}: {1}<br />",
                        _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.FileMaximumSize"),
                        pam.ValidationFileMaximumSize);
                if (!string.IsNullOrEmpty(pam.DefaultValue))
                    validationRules.AppendFormat("{0}: {1}<br />",
                        _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.ValidationRules.DefaultValue"),
                        WebUtility.HtmlEncode(pam.DefaultValue));
                model.ValidationRulesString = validationRules.ToString();
            }

            //currently any attribute can have condition. why not?
            model.ConditionAllowed = true;
            var conditionAttribute = _productAttributeParser.ParseProductAttributeMappings(pam.ConditionAttributeXml).FirstOrDefault();
            var conditionValue = _productAttributeParser.ParseProductAttributeValues(pam.ConditionAttributeXml).FirstOrDefault();
            if (conditionAttribute != null && conditionValue != null)
                model.ConditionString = $"{WebUtility.HtmlEncode(conditionAttribute.ProductAttribute.Name)}: {WebUtility.HtmlEncode(conditionValue.Name)}";
            else
                model.ConditionString = string.Empty;
        }

        protected virtual void PrepareConditionAttributes(ProductModel.ProductAttributeMappingModel model,
            ProductAttributeMapping productAttributeMapping, Product product)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            //currently any checkout attribute can have condition.
            model.ConditionAllowed = true;

            if (productAttributeMapping == null)
                return;

            model.ConditionModel = new ProductAttributeConditionModel
            {
                ProductAttributeMappingId = productAttributeMapping.Id,
                EnableCondition = !string.IsNullOrEmpty(productAttributeMapping.ConditionAttributeXml)
            };

            //pre-select attribute and values
            var selectedPva = _productAttributeParser
                .ParseProductAttributeMappings(productAttributeMapping.ConditionAttributeXml)
                .FirstOrDefault();

            var attributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id)
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
                    Name = attribute.ProductAttribute.Name,
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
                        model.ConditionModel.SelectedProductAttributeId = selectedPva.Id;

                        //values
                        switch (attribute.AttributeControlType)
                        {
                            case AttributeControlType.DropdownList:
                            case AttributeControlType.RadioList:
                            case AttributeControlType.Checkboxes:
                            case AttributeControlType.ColorSquares:
                            case AttributeControlType.ImageSquares:
                            {
                                if (!string.IsNullOrEmpty(productAttributeMapping.ConditionAttributeXml))
                                {
                                    //clear default selection
                                    foreach (var item in attributeModel.Values)
                                        item.IsPreSelected = false;

                                    //select new values
                                    var selectedValues = _productAttributeParser.ParseProductAttributeValues(productAttributeMapping.ConditionAttributeXml);
                                    foreach (var attributeValue in selectedValues)
                                    foreach (var item in attributeModel.Values)
                                        if (attributeValue.Id == item.Id)
                                            item.IsPreSelected = true;
                                }
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

                model.ConditionModel.ProductAttributes.Add(attributeModel);
            }
        }

        protected virtual void SaveConditionAttributes(ProductAttributeMapping productAttributeMapping, ProductAttributeConditionModel model)
        {
            string attributesXml = null;
            var form = model.Form;
            if (model.EnableCondition)
            {
                var attribute = _productAttributeService.GetProductAttributeMappingById(model.SelectedProductAttributeId);
                if (attribute != null)
                {
                    var controlId = $"product_attribute_{attribute.Id}";
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.ColorSquares:
                        case AttributeControlType.ImageSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                {
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                                }
                                else
                                {
                                    //for conditions we should empty values save even when nothing is selected
                                    //otherwise "attributesXml" will be empty
                                    //hence we won't be able to find a selected attribute
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, "");
                                }
                            }
                            else
                            {
                                //for conditions we should empty values save even when nothing is selected
                                //otherwise "attributesXml" will be empty
                                //hence we won't be able to find a selected attribute
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, "");
                            }
                        }
                            break;
                        case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(cblAttributes))
                            {
                                var anyValueSelected = false;
                                foreach (var item in cblAttributes.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                    {
                                        attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                        anyValueSelected = true;
                                    }
                                }
                                if (!anyValueSelected)
                                {
                                    //for conditions we should save empty values even when nothing is selected
                                    //otherwise "attributesXml" will be empty
                                    //hence we won't be able to find a selected attribute
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, "");
                                }
                            }
                            else
                            {
                                //for conditions we should save empty values even when nothing is selected
                                //otherwise "attributesXml" will be empty
                                //hence we won't be able to find a selected attribute
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, "");
                            }
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
            productAttributeMapping.ConditionAttributeXml = attributesXml;
            _productAttributeService.UpdateProductAttributeMapping(productAttributeMapping);
        }

        #endregion

        #region Methods

        #region Product list / create / edit / delete

        //list products
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var model = new ProductListModel
            {
                //a vendor should have access only to his products
                IsLoggedInAsVendor = _workContext.CurrentVendor != null,
                AllowVendorsToImportProducts = _vendorSettings.AllowVendorsToImportProducts
            };

            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var manufacturers = SelectListHelper.GetManufacturerList(_manufacturerService, _cacheManager, true);
            foreach (var m in manufacturers)
                model.AvailableManufacturers.Add(m);

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //warehouses
            model.AvailableWarehouses.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var wh in _shippingService.GetAllWarehouses())
                model.AvailableWarehouses.Add(new SelectListItem { Text = wh.Name, Value = wh.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
            foreach (var v in vendors)
                model.AvailableVendors.Add(v);

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //"published" property
            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.All"), Value = "0" });
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.PublishedOnly"), Value = "1" });
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.UnpublishedOnly"), Value = "2" });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ProductList(DataSourceRequest command, ProductListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.SearchVendorId = _workContext.CurrentVendor.Id;
            }

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                categoryIds.AddRange(GetChildCategoryIds(model.SearchCategoryId));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var products = _productService.SearchProducts(
                categoryIds: categoryIds,
                manufacturerId: model.SearchManufacturerId,
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                warehouseId: model.SearchWarehouseId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true,
                overridePublished: overridePublished
            );
            var gridModel = new DataSourceResult
            {
                Data = products.Select(x =>
                {
                    var productModel = x.ToModel();
                //little performance optimization: ensure that "FullDescription" is not returned
                productModel.FullDescription = "";

                //picture
                var defaultProductPicture = _pictureService.GetPicturesByProductId(x.Id, 1).FirstOrDefault();
                    productModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(defaultProductPicture, 75, true);
                //product type
                productModel.ProductTypeName = x.ProductType.GetLocalizedEnum(_localizationService, _workContext);
                //friendly stock qantity
                //if a simple product AND "manage inventory" is "Track inventory", then display
                if (x.ProductType == ProductType.SimpleProduct && x.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
                        productModel.StockQuantityStr = x.GetTotalStockQuantity().ToString();
                    return productModel;
                }),
                Total = products.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-product-by-sku")]
        public virtual IActionResult GoToSku(ProductListModel model)
        {
            var sku = model.GoDirectlyToSku;

            //try to load a product entity
            var product = _productService.GetProductBySku(sku);

            //if not found, then try to load a product attribute combination
            if (product == null)
            {
                var combination = _productAttributeService.GetProductAttributeCombinationBySku(sku);
                if (combination != null)
                {
                    product = combination.Product;
                }
            }

            if (product != null)
                return RedirectToAction("Edit", "Product", new { id = product.Id });

            //not found
            return List();
        }

        //create product
        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //validate maximum number of products per vendor
            if (_vendorSettings.MaximumProductNumber > 0 &&
                _workContext.CurrentVendor != null &&
                _productService.GetNumberOfProductsByVendorId(_workContext.CurrentVendor.Id) >= _vendorSettings.MaximumProductNumber)
            {
                ErrorNotification(string.Format(_localizationService.GetResource("Admin.Catalog.Products.ExceededMaximumNumber"), _vendorSettings.MaximumProductNumber));
                return RedirectToAction("List");
            }

            var model = new ProductModel();

            PrepareProductModel(model, null, true, true);
            AddLocales(_languageService, model.Locales);
            PrepareAclModel(model, null, false);
            PrepareStoresMappingModel(model, null, false);
            PrepareCategoryMappingModel(model, null, false);
            PrepareManufacturerMappingModel(model, null, false);
            PrepareDiscountMappingModel(model, null, false);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(ProductModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //validate maximum number of products per vendor
            if (_vendorSettings.MaximumProductNumber > 0 &&
                _workContext.CurrentVendor != null &&
                _productService.GetNumberOfProductsByVendorId(_workContext.CurrentVendor.Id) >= _vendorSettings.MaximumProductNumber)
            {
                ErrorNotification(string.Format(_localizationService.GetResource("Admin.Catalog.Products.ExceededMaximumNumber"), _vendorSettings.MaximumProductNumber));
                return RedirectToAction("List");
            }

            if (ModelState.IsValid)
            {
                //a vendor should have access only to his products
                if (_workContext.CurrentVendor != null)
                {
                    model.VendorId = _workContext.CurrentVendor.Id;
                }

                //vendors cannot edit "Show on home page" property
                if (_workContext.CurrentVendor != null && model.ShowOnHomePage)
                {
                    model.ShowOnHomePage = false;
                }

                //product
                var product = model.ToEntity();
                product.CreatedOnUtc = DateTime.UtcNow;
                product.UpdatedOnUtc = DateTime.UtcNow;
                _productService.InsertProduct(product);
                //search engine name
                model.SeName = product.ValidateSeName(model.SeName, product.Name, true);
                _urlRecordService.SaveSlug(product, model.SeName, 0);
                //locales
                UpdateLocales(product, model);
                //categories
                SaveCategoryMappings(product, model);
                //manufacturers
                SaveManufacturerMappings(product, model);
                //ACL (customer roles)
                SaveProductAcl(product, model);
                //stores
                SaveStoreMappings(product, model);
                //discounts
                SaveDiscountMappings(product, model);
                //tags
                _productTagService.UpdateProductTags(product, ParseProductTags(model.ProductTags));
                //warehouses
                SaveProductWarehouseInventory(product, model);

                //quantity change history
                _productService.AddStockQuantityHistoryEntry(product, product.StockQuantity, product.StockQuantity, product.WarehouseId,
                    _localizationService.GetResource("Admin.StockQuantityHistory.Messages.Edit"));

                //activity log
                _customerActivityService.InsertActivity("AddNewProduct", _localizationService.GetResource("ActivityLog.AddNewProduct"), product.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = product.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareProductModel(model, null, false, true);
            PrepareAclModel(model, null, true);
            PrepareStoresMappingModel(model, null, true);
            PrepareCategoryMappingModel(model, null, true);
            PrepareManufacturerMappingModel(model, null, true);
            PrepareDiscountMappingModel(model, null, true);

            return View(model);
        }

        //edit product
        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var product = _productService.GetProductById(id);
            if (product == null || product.Deleted)
                //No product found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List");

            var model = product.ToModel();
            PrepareProductModel(model, product, false, false);
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = product.GetLocalized(x => x.Name, languageId, false, false);
                locale.ShortDescription = product.GetLocalized(x => x.ShortDescription, languageId, false, false);
                locale.FullDescription = product.GetLocalized(x => x.FullDescription, languageId, false, false);
                locale.MetaKeywords = product.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = product.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = product.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = product.GetSeName(languageId, false, false);
            });

            PrepareAclModel(model, product, false);
            PrepareStoresMappingModel(model, product, false);
            PrepareCategoryMappingModel(model, product, false);
            PrepareManufacturerMappingModel(model, product, false);
            PrepareDiscountMappingModel(model, product, false);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(ProductModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var product = _productService.GetProductById(model.Id);

            if (product == null || product.Deleted)
                //No product found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List");

            //check if the product quantity has been changed while we were editing the product
            //and if it has been changed then we show error notification
            //and redirect on the editing page without data saving
            if (product.StockQuantity != model.LastStockQuantity)
            {
                ErrorNotification(_localizationService.GetResource("Admin.Catalog.Products.Fields.StockQuantity.ChangedWarning"));
                return RedirectToAction("Edit", new { id = product.Id });
            }

            if (ModelState.IsValid)
            {
                //a vendor should have access only to his products
                if (_workContext.CurrentVendor != null)
                {
                    model.VendorId = _workContext.CurrentVendor.Id;
                }

                //we do not validate maximum number of products per vendor when editing existing products (only during creation of new products)

                //vendors cannot edit "Show on home page" property
                if (_workContext.CurrentVendor != null && model.ShowOnHomePage != product.ShowOnHomePage)
                {
                    model.ShowOnHomePage = product.ShowOnHomePage;
                }
                //some previously used values
                var prevTotalStockQuantity = product.GetTotalStockQuantity();
                var prevDownloadId = product.DownloadId;
                var prevSampleDownloadId = product.SampleDownloadId;
                var previousStockQuantity = product.StockQuantity;
                var previousWarehouseId = product.WarehouseId;

                //product
                product = model.ToEntity(product);

                product.UpdatedOnUtc = DateTime.UtcNow;
                _productService.UpdateProduct(product);
                //search engine name
                model.SeName = product.ValidateSeName(model.SeName, product.Name, true);
                _urlRecordService.SaveSlug(product, model.SeName, 0);
                //locales
                UpdateLocales(product, model);
                //tags
                _productTagService.UpdateProductTags(product, ParseProductTags(model.ProductTags));
                //warehouses
                SaveProductWarehouseInventory(product, model);
                //categories
                SaveCategoryMappings(product, model);
                //manufacturers
                SaveManufacturerMappings(product, model);
                //ACL (customer roles)
                SaveProductAcl(product, model);
                //stores
                SaveStoreMappings(product, model);
                //discounts
                SaveDiscountMappings(product, model);
                //picture seo names
                UpdatePictureSeoNames(product);

                //back in stock notifications
                if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                    product.BackorderMode == BackorderMode.NoBackorders &&
                    product.AllowBackInStockSubscriptions &&
                    product.GetTotalStockQuantity() > 0 &&
                    prevTotalStockQuantity <= 0 &&
                    product.Published &&
                    !product.Deleted)
                {
                    _backInStockSubscriptionService.SendNotificationsToSubscribers(product);
                }
                //delete an old "download" file (if deleted or updated)
                if (prevDownloadId > 0 && prevDownloadId != product.DownloadId)
                {
                    var prevDownload = _downloadService.GetDownloadById(prevDownloadId);
                    if (prevDownload != null)
                        _downloadService.DeleteDownload(prevDownload);
                }
                //delete an old "sample download" file (if deleted or updated)
                if (prevSampleDownloadId > 0 && prevSampleDownloadId != product.SampleDownloadId)
                {
                    var prevSampleDownload = _downloadService.GetDownloadById(prevSampleDownloadId);
                    if (prevSampleDownload != null)
                        _downloadService.DeleteDownload(prevSampleDownload);
                }

                //quantity change history
                if (previousWarehouseId != product.WarehouseId)
                {
                    //warehouse is changed 
                    //compose a message
                    var oldWarehouseMessage = string.Empty;
                    if (previousWarehouseId > 0)
                    {
                        var oldWarehouse = _shippingService.GetWarehouseById(previousWarehouseId);
                        if (oldWarehouse != null)
                            oldWarehouseMessage = string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.EditWarehouse.Old"), oldWarehouse.Name);
                    }
                    var newWarehouseMessage = string.Empty;
                    if (product.WarehouseId > 0)
                    {
                        var newWarehouse = _shippingService.GetWarehouseById(product.WarehouseId);
                        if (newWarehouse != null)
                            newWarehouseMessage = string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.EditWarehouse.New"), newWarehouse.Name);
                    }
                    var message = string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.EditWarehouse"), oldWarehouseMessage, newWarehouseMessage);

                    //record history
                    _productService.AddStockQuantityHistoryEntry(product, -previousStockQuantity, 0, previousWarehouseId, message);
                    _productService.AddStockQuantityHistoryEntry(product, product.StockQuantity, product.StockQuantity, product.WarehouseId, message);

                }
                else
                {
                    _productService.AddStockQuantityHistoryEntry(product, product.StockQuantity - previousStockQuantity, product.StockQuantity,
                        product.WarehouseId, _localizationService.GetResource("Admin.StockQuantityHistory.Messages.Edit"));
                }

                //activity log
                _customerActivityService.InsertActivity("EditProduct", _localizationService.GetResource("ActivityLog.EditProduct"), product.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = product.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareProductModel(model, product, false, true);
            PrepareAclModel(model, product, true);
            PrepareStoresMappingModel(model, product, true);
            PrepareCategoryMappingModel(model, product, true);
            PrepareManufacturerMappingModel(model, product, true);
            PrepareDiscountMappingModel(model, product, true);

            return View(model);
        }

        //delete product
        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var product = _productService.GetProductById(id);
            if (product == null)
                //No product found with the specified id
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List");

            _productService.DeleteProduct(product);

            //activity log
            _customerActivityService.InsertActivity("DeleteProduct", _localizationService.GetResource("ActivityLog.DeleteProduct"), product.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                _productService.DeleteProducts(_productService.GetProductsByIds(selectedIds.ToArray()).Where(p => _workContext.CurrentVendor == null || p.VendorId == _workContext.CurrentVendor.Id).ToList());
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult CopyProduct(ProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var copyModel = model.CopyProductModel;
            try
            {
                var originalProduct = _productService.GetProductById(copyModel.Id);

                //a vendor should have access only to his products
                if (_workContext.CurrentVendor != null && originalProduct.VendorId != _workContext.CurrentVendor.Id)
                    return RedirectToAction("List");

                var newProduct = _copyProductService.CopyProduct(originalProduct,
                    copyModel.Name, copyModel.Published, copyModel.CopyImages);
                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Copied"));
                return RedirectToAction("Edit", new { id = newProduct.Id });
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = copyModel.Id });
            }
        }

        #endregion

        #region Required products

        [HttpPost]
        public virtual IActionResult LoadProductFriendlyNames(string productIds)
        {
            var result = "";

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return Json(new { Text = result });

            if (!string.IsNullOrWhiteSpace(productIds))
            {
                var ids = new List<int>();
                var rangeArray = productIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToList();

                foreach (var str1 in rangeArray)
                {
                    if (int.TryParse(str1, out int tmp1))
                        ids.Add(tmp1);
                }

                var products = _productService.GetProductsByIds(ids.ToArray());
                for (var i = 0; i <= products.Count - 1; i++)
                {
                    result += products[i].Name;
                    if (i != products.Count - 1)
                        result += ", ";
                }
            }

            return Json(new { Text = result });
        }

        public virtual IActionResult RequiredProductAddPopup()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var model = new ProductModel.AddRequiredProductModel
            {
                //a vendor should have access only to his products
                IsLoggedInAsVendor = _workContext.CurrentVendor != null
            };

            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var manufacturers = SelectListHelper.GetManufacturerList(_manufacturerService, _cacheManager, true);
            foreach (var m in manufacturers)
                model.AvailableManufacturers.Add(m);

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
            foreach (var v in vendors)
                model.AvailableVendors.Add(v);

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult RequiredProductAddPopupList(DataSourceRequest command, ProductModel.AddRequiredProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.SearchVendorId = _workContext.CurrentVendor.Id;
            }

            var products = _productService.SearchProducts(
                categoryIds: new List<int> { model.SearchCategoryId },
                manufacturerId: model.SearchManufacturerId,
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true
                );
            var gridModel = new DataSourceResult
            {
                Data = products.Select(x => x.ToModel()),
                Total = products.TotalCount
            };

            return Json(gridModel);
        }

        #endregion

        #region Related products

        [HttpPost]
        public virtual IActionResult RelatedProductList(DataSourceRequest command, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                var product = _productService.GetProductById(productId);
                if (product != null && product.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your product");
                }
            }

            var relatedProducts = _productService.GetRelatedProductsByProductId1(productId, true);
            var relatedProductsModel = relatedProducts
                .Select(x => new ProductModel.RelatedProductModel
                {
                    Id = x.Id,
                    //ProductId1 = x.ProductId1,
                    ProductId2 = x.ProductId2,
                    Product2Name = _productService.GetProductById(x.ProductId2).Name,
                    DisplayOrder = x.DisplayOrder
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = relatedProductsModel,
                Total = relatedProductsModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult RelatedProductUpdate(ProductModel.RelatedProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var relatedProduct = _productService.GetRelatedProductById(model.Id);
            if (relatedProduct == null)
                throw new ArgumentException("No related product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                var product = _productService.GetProductById(relatedProduct.ProductId1);
                if (product != null && product.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your product");
                }
            }

            relatedProduct.DisplayOrder = model.DisplayOrder;
            _productService.UpdateRelatedProduct(relatedProduct);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult RelatedProductDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var relatedProduct = _productService.GetRelatedProductById(id);
            if (relatedProduct == null)
                throw new ArgumentException("No related product found with the specified id");

            var productId = relatedProduct.ProductId1;

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                var product = _productService.GetProductById(productId);
                if (product != null && product.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your product");
                }
            }

            _productService.DeleteRelatedProduct(relatedProduct);

            return new NullJsonResult();
        }

        public virtual IActionResult RelatedProductAddPopup(int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var model = new ProductModel.AddRelatedProductModel
            {
                //a vendor should have access only to his products
                IsLoggedInAsVendor = _workContext.CurrentVendor != null
            };

            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var manufacturers = SelectListHelper.GetManufacturerList(_manufacturerService, _cacheManager, true);
            foreach (var m in manufacturers)
                model.AvailableManufacturers.Add(m);

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
            foreach (var v in vendors)
                model.AvailableVendors.Add(v);

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult RelatedProductAddPopupList(DataSourceRequest command, ProductModel.AddRelatedProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.SearchVendorId = _workContext.CurrentVendor.Id;
            }

            var products = _productService.SearchProducts(
                categoryIds: new List<int> { model.SearchCategoryId },
                manufacturerId: model.SearchManufacturerId,
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true
                );
            var gridModel = new DataSourceResult
            {
                Data = products.Select(x => x.ToModel()),
                Total = products.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult RelatedProductAddPopup(ProductModel.AddRelatedProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (model.SelectedProductIds != null)
            {
                foreach (var id in model.SelectedProductIds)
                {
                    var product = _productService.GetProductById(id);
                    if (product != null)
                    {
                        //a vendor should have access only to his products
                        if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                            continue;

                        var existingRelatedProducts = _productService.GetRelatedProductsByProductId1(model.ProductId);
                        if (existingRelatedProducts.FindRelatedProduct(model.ProductId, id) == null)
                        {
                            _productService.InsertRelatedProduct(
                                new RelatedProduct
                                {
                                    ProductId1 = model.ProductId,
                                    ProductId2 = id,
                                    DisplayOrder = 1
                                });
                        }
                    }
                }
            }

            //a vendor should have access only to his products
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
            ViewBag.RefreshPage = true;
            return View(model);
        }

        #endregion

        #region Cross-sell products

        [HttpPost]
        public virtual IActionResult CrossSellProductList(DataSourceRequest command, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                var product = _productService.GetProductById(productId);
                if (product != null && product.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your product");
                }
            }

            var crossSellProducts = _productService.GetCrossSellProductsByProductId1(productId, true);
            var crossSellProductsModel = crossSellProducts
                .Select(x => new ProductModel.CrossSellProductModel
                {
                    Id = x.Id,
                    //ProductId1 = x.ProductId1,
                    ProductId2 = x.ProductId2,
                    Product2Name = _productService.GetProductById(x.ProductId2).Name,
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = crossSellProductsModel,
                Total = crossSellProductsModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult CrossSellProductDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var crossSellProduct = _productService.GetCrossSellProductById(id);
            if (crossSellProduct == null)
                throw new ArgumentException("No cross-sell product found with the specified id");

            var productId = crossSellProduct.ProductId1;

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                var product = _productService.GetProductById(productId);
                if (product != null && product.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your product");
                }
            }

            _productService.DeleteCrossSellProduct(crossSellProduct);

            return new NullJsonResult();
        }

        public virtual IActionResult CrossSellProductAddPopup(int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var model = new ProductModel.AddCrossSellProductModel
            {
                //a vendor should have access only to his products
                IsLoggedInAsVendor = _workContext.CurrentVendor != null
            };

            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var manufacturers = SelectListHelper.GetManufacturerList(_manufacturerService, _cacheManager, true);
            foreach (var m in manufacturers)
                model.AvailableManufacturers.Add(m);

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
            foreach (var v in vendors)
                model.AvailableVendors.Add(v);

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult CrossSellProductAddPopupList(DataSourceRequest command, ProductModel.AddCrossSellProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.SearchVendorId = _workContext.CurrentVendor.Id;
            }

            var products = _productService.SearchProducts(
                categoryIds: new List<int> { model.SearchCategoryId },
                manufacturerId: model.SearchManufacturerId,
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true
                );
            var gridModel = new DataSourceResult
            {
                Data = products.Select(x => x.ToModel()),
                Total = products.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult CrossSellProductAddPopup(ProductModel.AddCrossSellProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (model.SelectedProductIds != null)
            {
                foreach (var id in model.SelectedProductIds)
                {
                    var product = _productService.GetProductById(id);
                    if (product != null)
                    {
                        //a vendor should have access only to his products
                        if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                            continue;

                        var existingCrossSellProducts = _productService.GetCrossSellProductsByProductId1(model.ProductId);
                        if (existingCrossSellProducts.FindCrossSellProduct(model.ProductId, id) == null)
                        {
                            _productService.InsertCrossSellProduct(
                                new CrossSellProduct
                                {
                                    ProductId1 = model.ProductId,
                                    ProductId2 = id,
                                });
                        }
                    }
                }
            }

            //a vendor should have access only to his products
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
            ViewBag.RefreshPage = true;
            return View(model);
        }

        #endregion

        #region Associated products

        [HttpPost]
        public virtual IActionResult AssociatedProductList(DataSourceRequest command, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                var product = _productService.GetProductById(productId);
                if (product != null && product.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your product");
                }
            }

            //a vendor should have access only to his products
            var vendorId = 0;
            if (_workContext.CurrentVendor != null)
            {
                vendorId = _workContext.CurrentVendor.Id;
            }

            var associatedProducts = _productService.GetAssociatedProducts(parentGroupedProductId: productId,
                vendorId: vendorId,
                showHidden: true);
            var associatedProductsModel = associatedProducts
                .Select(x => new ProductModel.AssociatedProductModel
                {
                    Id = x.Id,
                    ProductName = x.Name,
                    DisplayOrder = x.DisplayOrder
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = associatedProductsModel,
                Total = associatedProductsModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult AssociatedProductUpdate(ProductModel.AssociatedProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var associatedProduct = _productService.GetProductById(model.Id);
            if (associatedProduct == null)
                throw new ArgumentException("No associated product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && associatedProduct.VendorId != _workContext.CurrentVendor.Id)
            {
                return Content("This is not your product");
            }

            associatedProduct.DisplayOrder = model.DisplayOrder;
            _productService.UpdateProduct(associatedProduct);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult AssociatedProductDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var product = _productService.GetProductById(id);
            if (product == null)
                throw new ArgumentException("No associated product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            product.ParentGroupedProductId = 0;
            _productService.UpdateProduct(product);

            return new NullJsonResult();
        }

        public virtual IActionResult AssociatedProductAddPopup(int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var model = new ProductModel.AddAssociatedProductModel
            {
                //a vendor should have access only to his products
                IsLoggedInAsVendor = _workContext.CurrentVendor != null
            };

            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var manufacturers = SelectListHelper.GetManufacturerList(_manufacturerService, _cacheManager, true);
            foreach (var m in manufacturers)
                model.AvailableManufacturers.Add(m);

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
            foreach (var v in vendors)
                model.AvailableVendors.Add(v);

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult AssociatedProductAddPopupList(DataSourceRequest command, ProductModel.AddAssociatedProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.SearchVendorId = _workContext.CurrentVendor.Id;
            }

            var products = _productService.SearchProducts(
                categoryIds: new List<int> { model.SearchCategoryId },
                manufacturerId: model.SearchManufacturerId,
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true
                );
            var gridModel = new DataSourceResult
            {
                Data = products.Select(x =>
                {
                    var productModel = x.ToModel();
                //display already associated products
                var parentGroupedProduct = _productService.GetProductById(x.ParentGroupedProductId);
                    if (parentGroupedProduct != null)
                    {
                        productModel.AssociatedToProductId = x.ParentGroupedProductId;
                        productModel.AssociatedToProductName = parentGroupedProduct.Name;
                    }
                    return productModel;
                }),
                Total = products.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult AssociatedProductAddPopup(ProductModel.AddAssociatedProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (model.SelectedProductIds != null)
            {
                foreach (var id in model.SelectedProductIds)
                {
                    var product = _productService.GetProductById(id);
                    if (product != null)
                    {
                        //a vendor should have access only to his products
                        if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                            continue;

                        product.ParentGroupedProductId = model.ProductId;
                        _productService.UpdateProduct(product);
                    }
                }
            }

            //a vendor should have access only to his products
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
            ViewBag.RefreshPage = true;
            return View(model);
        }

        #endregion

        #region Product pictures

        public virtual IActionResult ProductPictureAdd(int pictureId, int displayOrder,
            string overrideAltAttribute, string overrideTitleAttribute,
            int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (pictureId == 0)
                throw new ArgumentException();

            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List");

            var picture = _pictureService.GetPictureById(pictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                overrideAltAttribute,
                overrideTitleAttribute);

            _pictureService.SetSeoFilename(pictureId, _pictureService.GetPictureSeName(product.Name));

            _productService.InsertProductPicture(new ProductPicture
            {
                PictureId = pictureId,
                ProductId = productId,
                DisplayOrder = displayOrder,
            });

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult ProductPictureList(DataSourceRequest command, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                var product = _productService.GetProductById(productId);
                if (product != null && product.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your product");
                }
            }

            var productPictures = _productService.GetProductPicturesByProductId(productId);
            var productPicturesModel = productPictures
                .Select(x =>
                {
                    var picture = _pictureService.GetPictureById(x.PictureId);
                    if (picture == null)
                        throw new Exception("Picture cannot be loaded");
                    var m = new ProductModel.ProductPictureModel
                    {
                        Id = x.Id,
                        ProductId = x.ProductId,
                        PictureId = x.PictureId,
                        PictureUrl = _pictureService.GetPictureUrl(picture),
                        OverrideAltAttribute = picture.AltAttribute,
                        OverrideTitleAttribute = picture.TitleAttribute,
                        DisplayOrder = x.DisplayOrder
                    };
                    return m;
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = productPicturesModel,
                Total = productPicturesModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult ProductPictureUpdate(ProductModel.ProductPictureModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var productPicture = _productService.GetProductPictureById(model.Id);
            if (productPicture == null)
                throw new ArgumentException("No product picture found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                var product = _productService.GetProductById(productPicture.ProductId);
                if (product != null && product.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your product");
                }
            }

            var picture = _pictureService.GetPictureById(productPicture.PictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                model.OverrideAltAttribute,
                model.OverrideTitleAttribute);

            productPicture.DisplayOrder = model.DisplayOrder;
            _productService.UpdateProductPicture(productPicture);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult ProductPictureDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var productPicture = _productService.GetProductPictureById(id);
            if (productPicture == null)
                throw new ArgumentException("No product picture found with the specified id");

            var productId = productPicture.ProductId;

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                var product = _productService.GetProductById(productId);
                if (product != null && product.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your product");
                }
            }
            var pictureId = productPicture.PictureId;
            _productService.DeleteProductPicture(productPicture);

            var picture = _pictureService.GetPictureById(pictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");
            _pictureService.DeletePicture(picture);

            return new NullJsonResult();
        }

        #endregion

        #region Product specification attributes

        public virtual IActionResult ProductSpecificationAttributeAdd(int attributeTypeId, int specificationAttributeOptionId,
            string customValue, bool allowFiltering, bool showOnProductPage,
            int displayOrder, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                var product = _productService.GetProductById(productId);
                if (product != null && product.VendorId != _workContext.CurrentVendor.Id)
                {
                    return RedirectToAction("List");
                }
            }

            //we allow filtering only for "Option" attribute type
            if (attributeTypeId != (int)SpecificationAttributeType.Option)
            {
                allowFiltering = false;
            }
            //we don't allow CustomValue for "Option" attribute type
            if (attributeTypeId == (int)SpecificationAttributeType.Option)
            {
                customValue = null;
            }

            var psa = new ProductSpecificationAttribute
            {
                AttributeTypeId = attributeTypeId,
                SpecificationAttributeOptionId = specificationAttributeOptionId,
                ProductId = productId,
                CustomValue = customValue,
                AllowFiltering = allowFiltering,
                ShowOnProductPage = showOnProductPage,
                DisplayOrder = displayOrder,
            };
            _specificationAttributeService.InsertProductSpecificationAttribute(psa);

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult ProductSpecAttrList(DataSourceRequest command, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                var product = _productService.GetProductById(productId);
                if (product != null && product.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your product");
                }
            }

            var productrSpecs = _specificationAttributeService.GetProductSpecificationAttributes(productId);

            var productrSpecsModel = productrSpecs
                .Select(x =>
                {
                    var psaModel = new ProductSpecificationAttributeModel
                    {
                        Id = x.Id,
                        AttributeTypeId = x.AttributeTypeId,
                        AttributeTypeName = x.AttributeType.GetLocalizedEnum(_localizationService, _workContext),
                        AttributeId = x.SpecificationAttributeOption.SpecificationAttribute.Id,
                        AttributeName = x.SpecificationAttributeOption.SpecificationAttribute.Name,
                        AllowFiltering = x.AllowFiltering,
                        ShowOnProductPage = x.ShowOnProductPage,
                        DisplayOrder = x.DisplayOrder
                    };
                    switch (x.AttributeType)
                    {
                        case SpecificationAttributeType.Option:
                            psaModel.ValueRaw = WebUtility.HtmlEncode(x.SpecificationAttributeOption.Name);
                            psaModel.SpecificationAttributeOptionId = x.SpecificationAttributeOptionId;
                            break;
                        case SpecificationAttributeType.CustomText:
                            psaModel.ValueRaw = WebUtility.HtmlEncode(x.CustomValue);
                            break;
                        case SpecificationAttributeType.CustomHtmlText:
                            //do not encode?
                            //psaModel.ValueRaw = x.CustomValue;
                            psaModel.ValueRaw = WebUtility.HtmlEncode(x.CustomValue);
                            break;
                        case SpecificationAttributeType.Hyperlink:
                            psaModel.ValueRaw = x.CustomValue;
                            break;
                        default:
                            break;
                    }
                    return psaModel;
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = productrSpecsModel,
                Total = productrSpecsModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult ProductSpecAttrUpdate(ProductSpecificationAttributeModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var psa = _specificationAttributeService.GetProductSpecificationAttributeById(model.Id);
            if (psa == null)
                return Content("No product specification attribute found with the specified id");

            var productId = psa.Product.Id;

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                var product = _productService.GetProductById(productId);
                if (product != null && product.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your product");
                }
            }

            //we allow filtering and change option only for "Option" attribute type
            if (model.AttributeTypeId == (int)SpecificationAttributeType.Option)
            {
                psa.AllowFiltering = model.AllowFiltering;
                psa.SpecificationAttributeOptionId = model.SpecificationAttributeOptionId;
            }

            psa.ShowOnProductPage = model.ShowOnProductPage;
            psa.DisplayOrder = model.DisplayOrder;
            _specificationAttributeService.UpdateProductSpecificationAttribute(psa);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult ProductSpecAttrDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var psa = _specificationAttributeService.GetProductSpecificationAttributeById(id);
            if (psa == null)
                throw new ArgumentException("No specification attribute found with the specified id");

            var productId = psa.ProductId;

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                var product = _productService.GetProductById(productId);
                if (product != null && product.VendorId != _workContext.CurrentVendor.Id)
                {
                    return Content("This is not your product");
                }
            }

            _specificationAttributeService.DeleteProductSpecificationAttribute(psa);

            return new NullJsonResult();
        }

        #endregion

        #region Product tags

        public virtual IActionResult ProductTags()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual IActionResult ProductTags(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedKendoGridJson();

            var tags = _productTagService.GetAllProductTags()
                //order by product count
                .OrderByDescending(x => _productTagService.GetProductCount(x.Id, 0))
                .Select(x => new ProductTagModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    ProductCount = _productTagService.GetProductCount(x.Id, 0)
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = tags.PagedForCommand(command),
                Total = tags.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult ProductTagDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedView();

            var tag = _productTagService.GetProductTagById(id);
            if (tag == null)
                throw new ArgumentException("No product tag found with the specified id");
            _productTagService.DeleteProductTag(tag);

            return new NullJsonResult();
        }

        //edit
        public virtual IActionResult EditProductTag(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedView();

            var productTag = _productTagService.GetProductTagById(id);
            if (productTag == null)
                //No product tag found with the specified id
                return RedirectToAction("List");

            var model = new ProductTagModel
            {
                Id = productTag.Id,
                Name = productTag.Name,
                ProductCount = _productTagService.GetProductCount(productTag.Id, 0)
            };
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = productTag.GetLocalized(x => x.Name, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult EditProductTag(ProductTagModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedView();

            var productTag = _productTagService.GetProductTagById(model.Id);
            if (productTag == null)
                //No product tag found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                productTag.Name = model.Name;
                _productTagService.UpdateProductTag(productTag);
                //locales
                UpdateLocales(productTag, model);

                ViewBag.RefreshPage = true;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Purchased with order

        [HttpPost]
        public virtual IActionResult PurchasedWithOrders(DataSourceRequest command, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            var orders = _orderService.SearchOrders(
                productId: productId,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = orders.Select(x =>
                {
                    var store = _storeService.GetStoreById(x.StoreId);
                    return new OrderModel
                    {
                        Id = x.Id,
                        StoreName = store != null ? store.Name : "Unknown",
                        OrderStatus = x.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                        PaymentStatus = x.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                        ShippingStatus = x.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext),
                        CustomerEmail = x.BillingAddress.Email,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc),
                        CustomOrderNumber = x.CustomOrderNumber
                    };
                }),
                Total = orders.TotalCount
            };

            return Json(gridModel);
        }

        #endregion

        #region Export / Import

        [HttpPost, ActionName("List")]
        [FormValueRequired("download-catalog-pdf")]
        public virtual IActionResult DownloadCatalogAsPdf(ProductListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.SearchVendorId = _workContext.CurrentVendor.Id;
            }

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                categoryIds.AddRange(GetChildCategoryIds(model.SearchCategoryId));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var products = _productService.SearchProducts(
                categoryIds: categoryIds,
                manufacturerId: model.SearchManufacturerId,
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                warehouseId: model.SearchWarehouseId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                showHidden: true,
                overridePublished: overridePublished
            );

            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    _pdfService.PrintProductsToPdf(stream, products);
                    bytes = stream.ToArray();
                }
                return File(bytes, MimeTypes.ApplicationPdf, "pdfcatalog.pdf");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("exportxml-all")]
        public virtual IActionResult ExportXmlAll(ProductListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.SearchVendorId = _workContext.CurrentVendor.Id;
            }

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                categoryIds.AddRange(GetChildCategoryIds(model.SearchCategoryId));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var products = _productService.SearchProducts(
                categoryIds: categoryIds,
                manufacturerId: model.SearchManufacturerId,
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                warehouseId: model.SearchWarehouseId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                showHidden: true,
                overridePublished: overridePublished
            );

            try
            {
                var xml = _exportManager.ExportProductsToXml(products);

                return File(Encoding.UTF8.GetBytes(xml), "application/xml", "products.xml");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual IActionResult ExportXmlSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var products = new List<Product>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                products.AddRange(_productService.GetProductsByIds(ids));
            }
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                products = products.Where(p => p.VendorId == _workContext.CurrentVendor.Id).ToList();
            }

            var xml = _exportManager.ExportProductsToXml(products);

            return File(Encoding.UTF8.GetBytes(xml), "application/xml", "products.xml");
        }
        
        [HttpPost, ActionName("List")]
        [FormValueRequired("exportexcel-all")]
        public virtual IActionResult ExportExcelAll(ProductListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.SearchVendorId = _workContext.CurrentVendor.Id;
            }

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                categoryIds.AddRange(GetChildCategoryIds(model.SearchCategoryId));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var products = _productService.SearchProducts(
                categoryIds: categoryIds,
                manufacturerId: model.SearchManufacturerId,
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                warehouseId: model.SearchWarehouseId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                showHidden: true,
                overridePublished: overridePublished
            );
            try
            {
                var bytes = _exportManager.ExportProductsToXlsx(products);

                return File(bytes, MimeTypes.TextXlsx, "products.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual IActionResult ExportExcelSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var products = new List<Product>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                products.AddRange(_productService.GetProductsByIds(ids));
            }
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                products = products.Where(p => p.VendorId == _workContext.CurrentVendor.Id).ToList();
            }

            var bytes = _exportManager.ExportProductsToXlsx(products);

            return File(bytes, MimeTypes.TextXlsx, "products.xlsx");
        }

        [HttpPost]
        public virtual IActionResult ImportExcel(IFormFile importexcelfile)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            
            if (_workContext.CurrentVendor != null && !_vendorSettings.AllowVendorsToImportProducts)
                //a vendor can not import products
                return AccessDeniedView();

            try
            {
                if (importexcelfile != null && importexcelfile.Length > 0)
                {
                    _importManager.ImportProductsFromXlsx(importexcelfile.OpenReadStream());
                }
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
                    return RedirectToAction("List");
                }

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Imported"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        #endregion

        #region Low stock reports

        public virtual IActionResult LowStockReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual IActionResult LowStockReportList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            var vendorId = 0;
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;

            IList<Product> products = _productService.GetLowStockProducts(vendorId);
            IList<ProductAttributeCombination> combinations = _productService.GetLowStockProductCombinations(vendorId);

            var models = new List<LowStockProductModel>();
            //products
            foreach (var product in products)
            {
                var lowStockModel = new LowStockProductModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    ManageInventoryMethod = product.ManageInventoryMethod.GetLocalizedEnum(_localizationService, _workContext.WorkingLanguage.Id),
                    StockQuantity = product.GetTotalStockQuantity(),
                    Published = product.Published
                };
                models.Add(lowStockModel);
            }
            //combinations
            foreach (var combination in combinations)
            {
                var product = combination.Product;
                var lowStockModel = new LowStockProductModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Attributes = _productAttributeFormatter.FormatAttributes(product, combination.AttributesXml, _workContext.CurrentCustomer, "<br />", true, true, true, false),
                    ManageInventoryMethod = product.ManageInventoryMethod.GetLocalizedEnum(_localizationService, _workContext.WorkingLanguage.Id),
                    StockQuantity = combination.StockQuantity,
                    Published = product.Published
                };
                models.Add(lowStockModel);
            }
            var gridModel = new DataSourceResult
            {
                Data = models.PagedForCommand(command),
                Total = models.Count
            };

            return Json(gridModel);
        }

        #endregion

        #region Bulk editing

        public virtual IActionResult BulkEdit()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var model = new BulkEditListModel();
            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var manufacturers = SelectListHelper.GetManufacturerList(_manufacturerService, _cacheManager, true);
            foreach (var m in manufacturers)
                model.AvailableManufacturers.Add(m);

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult BulkEditSelect(DataSourceRequest command, BulkEditListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            var vendorId = 0;
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;

            var products = _productService.SearchProducts(categoryIds: new List<int> { model.SearchCategoryId },
                manufacturerId: model.SearchManufacturerId,
                vendorId: vendorId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true);

            var gridModel = new DataSourceResult
            {
                Data = products.Select(x =>
                {
                    var productModel = new BulkEditProductModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Sku = x.Sku,
                        OldPrice = x.OldPrice,
                        Price = x.Price,
                        ManageInventoryMethod = x.ManageInventoryMethod.GetLocalizedEnum(_localizationService, _workContext.WorkingLanguage.Id),
                        StockQuantity = x.StockQuantity,
                        Published = x.Published
                    };

                    if (x.ManageInventoryMethod == ManageInventoryMethod.ManageStock && x.UseMultipleWarehouses)
                    {
                    //multi-warehouse supported
                    //TODO localize
                    productModel.ManageInventoryMethod += " (multi-warehouse)";
                    }

                    return productModel;
                }),

                Total = products.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult BulkEditUpdate(IEnumerable<BulkEditProductModel> products)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (products != null)
            {
                foreach (var pModel in products)
                {
                    //update
                    var product = _productService.GetProductById(pModel.Id);
                    if (product != null)
                    {
                        //a vendor should have access only to his products
                        if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                            continue;

                        var prevTotalStockQuantity = product.GetTotalStockQuantity();
                        var previousStockQuantity = product.StockQuantity;

                        product.Name = pModel.Name;
                        product.Sku = pModel.Sku;
                        product.Price = pModel.Price;
                        product.OldPrice = pModel.OldPrice;
                        product.StockQuantity = pModel.StockQuantity;
                        product.Published = pModel.Published;
                        product.UpdatedOnUtc = DateTime.UtcNow;
                        _productService.UpdateProduct(product);

                        //back in stock notifications
                        if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                            product.BackorderMode == BackorderMode.NoBackorders &&
                            product.AllowBackInStockSubscriptions &&
                            product.GetTotalStockQuantity() > 0 &&
                            prevTotalStockQuantity <= 0 &&
                            product.Published &&
                            !product.Deleted)
                        {
                            _backInStockSubscriptionService.SendNotificationsToSubscribers(product);
                        }

                        //quantity change history
                        _productService.AddStockQuantityHistoryEntry(product, product.StockQuantity - previousStockQuantity, product.StockQuantity,
                            product.WarehouseId, _localizationService.GetResource("Admin.StockQuantityHistory.Messages.Edit"));
                    }
                }
            }

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult BulkEditDelete(IEnumerable<BulkEditProductModel> products)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (products != null)
            {
                foreach (var pModel in products)
                {
                    //delete
                    var product = _productService.GetProductById(pModel.Id);
                    if (product != null)
                    {
                        //a vendor should have access only to his products
                        if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                            continue;

                        _productService.DeleteProduct(product);
                    }
                }
            }
            return new NullJsonResult();
        }

        #endregion

        #region Tier prices

        [HttpPost]
        public virtual IActionResult TierPriceList(DataSourceRequest command, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            var tierPricesModel = product.TierPrices.OrderBy(x => x.StoreId).ThenBy(x => x.Quantity).ThenBy(x => x.CustomerRoleId).Select(x =>
            {
                string storeName;
                if (x.StoreId > 0)
                {
                    var store = _storeService.GetStoreById(x.StoreId);
                    storeName = store != null ? store.Name : "Deleted";
                }
                else
                    storeName = _localizationService.GetResource("Admin.Catalog.Products.TierPrices.Fields.Store.All");

                return new ProductModel.TierPriceModel
                {
                    Id = x.Id,
                    StoreId = x.StoreId,
                    Store = storeName,
                    CustomerRole = x.CustomerRoleId.HasValue ? _customerService.GetCustomerRoleById(x.CustomerRoleId.Value).Name : _localizationService.GetResource("Admin.Catalog.Products.TierPrices.Fields.CustomerRole.All"),
                    ProductId = x.ProductId,
                    CustomerRoleId = x.CustomerRoleId.HasValue ? x.CustomerRoleId.Value : 0,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    StartDateTimeUtc = x.StartDateTimeUtc,
                    EndDateTimeUtc = x.EndDateTimeUtc
                };
            }).ToList();

            var gridModel = new DataSourceResult
            {
                Data = tierPricesModel,
                Total = tierPricesModel.Count
            };

            return Json(gridModel);
        }

        public virtual IActionResult TierPriceCreatePopup()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var model = new ProductModel.TierPriceModel();

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var store in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString() });

            //customer roles
            model.AvailableCustomerRoles.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var role in _customerService.GetAllCustomerRoles(true))
                model.AvailableCustomerRoles.Add(new SelectListItem { Text = role.Name, Value = role.Id.ToString() });

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult TierPriceCreatePopup(ProductModel.TierPriceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var product = _productService.GetProductById(model.ProductId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");

            if (ModelState.IsValid)
            {
                var tierPrice = new TierPrice
                {
                    ProductId = model.ProductId,
                    StoreId = model.StoreId,
                    CustomerRoleId = model.CustomerRoleId > 0 ? model.CustomerRoleId : (int?)null,
                    Quantity = model.Quantity,
                    Price = model.Price,
                    StartDateTimeUtc = model.StartDateTimeUtc,
                    EndDateTimeUtc = model.EndDateTimeUtc
                };
                _productService.InsertTierPrice(tierPrice);

                //update "HasTierPrices" property
                _productService.UpdateHasTierPricesProperty(product);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //If we got this far, something failed, redisplay form

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var store in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString() });

            //customer roles
            model.AvailableCustomerRoles.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var role in _customerService.GetAllCustomerRoles(true))
                model.AvailableCustomerRoles.Add(new SelectListItem { Text = role.Name, Value = role.Id.ToString() });

            return View(model);
        }

        public virtual IActionResult TierPriceEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var tierPrice = _productService.GetTierPriceById(id);
            if (tierPrice == null)
                return RedirectToAction("List", "Product");

            var product = _productService.GetProductById(tierPrice.ProductId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");

            var model = new ProductModel.TierPriceModel
            {
                Id = tierPrice.Id,
                CustomerRoleId = tierPrice.CustomerRoleId.HasValue ? tierPrice.CustomerRoleId.Value : 0,
                StoreId = tierPrice.StoreId,
                Quantity = tierPrice.Quantity,
                Price = tierPrice.Price,
                StartDateTimeUtc = tierPrice.StartDateTimeUtc,
                EndDateTimeUtc = tierPrice.EndDateTimeUtc
            };

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var store in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString() });

            //customer roles
            model.AvailableCustomerRoles.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var role in _customerService.GetAllCustomerRoles(true))
                model.AvailableCustomerRoles.Add(new SelectListItem { Text = role.Name, Value = role.Id.ToString() });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult TierPriceEditPopup(ProductModel.TierPriceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var tierPrice = _productService.GetTierPriceById(model.Id);
            if (tierPrice == null)
                return RedirectToAction("List", "Product");

            var product = _productService.GetProductById(tierPrice.ProductId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");

            if (ModelState.IsValid)
            {
                tierPrice.StoreId = model.StoreId;
                tierPrice.CustomerRoleId = model.CustomerRoleId > 0 ? model.CustomerRoleId : (int?)null;
                tierPrice.Quantity = model.Quantity;
                tierPrice.Price = model.Price;
                tierPrice.StartDateTimeUtc = model.StartDateTimeUtc;
                tierPrice.EndDateTimeUtc = model.EndDateTimeUtc;
                _productService.UpdateTierPrice(tierPrice);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //If we got this far, something failed, redisplay form

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var store in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = store.Name, Value = store.Id.ToString() });

            //customer roles
            model.AvailableCustomerRoles.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var role in _customerService.GetAllCustomerRoles(true))
                model.AvailableCustomerRoles.Add(new SelectListItem { Text = role.Name, Value = role.Id.ToString() });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult TierPriceDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var tierPrice = _productService.GetTierPriceById(id);
            if (tierPrice == null)
                throw new ArgumentException("No tier price found with the specified id");

            var product = _productService.GetProductById(tierPrice.ProductId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            _productService.DeleteTierPrice(tierPrice);

            //update "HasTierPrices" property
            _productService.UpdateHasTierPricesProperty(product);

            return new NullJsonResult();
        }

        #endregion

        #region Product attributes

        [HttpPost]
        public virtual IActionResult ProductAttributeMappingList(DataSourceRequest command, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            var attributes = _productAttributeService.GetProductAttributeMappingsByProductId(productId);
            var attributesModel = attributes
                .Select(x =>
                {
                    var attributeModel = new ProductModel.ProductAttributeMappingModel();
                    PrepareProductAttributeMappingModel(attributeModel, x, x.Product, false);
                    return attributeModel;
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = attributesModel,
                Total = attributesModel.Count
            };

            return Json(gridModel);
        }

        public virtual IActionResult ProductAttributeMappingCreate(int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
            {
                ErrorNotification(_localizationService.GetResource("This is not your product"));
                return RedirectToAction("List");
            }

            var model = new ProductModel.ProductAttributeMappingModel();
            PrepareProductAttributeMappingModel(model, null, product, false);
            //condition
            PrepareConditionAttributes(model, null, product);
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult ProductAttributeMappingCreate(ProductModel.ProductAttributeMappingModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var product = _productService.GetProductById(model.ProductId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
            {
                ErrorNotification(_localizationService.GetResource("This is not your product"));
                return RedirectToAction("List");
            }

            //ensure this attribute is not mapped yet
            if (_productAttributeService.GetProductAttributeMappingsByProductId(product.Id)
                .Any(x => x.ProductAttributeId == model.ProductAttributeId))
            {
                //redisplay form
                ErrorNotification(_localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.AlreadyExists"));
                //model
                PrepareProductAttributeMappingModel(model, null, product, true);
                //condition
                PrepareConditionAttributes(model, null, product);
                return View(model);
            }

            //insert mapping
            var productAttributeMapping = new ProductAttributeMapping
            {
                ProductId = model.ProductId,
                ProductAttributeId = model.ProductAttributeId,
                TextPrompt = model.TextPrompt,
                IsRequired = model.IsRequired,
                AttributeControlTypeId = model.AttributeControlTypeId,
                DisplayOrder = model.DisplayOrder,
                ValidationMinLength = model.ValidationMinLength,
                ValidationMaxLength = model.ValidationMaxLength,
                ValidationFileAllowedExtensions = model.ValidationFileAllowedExtensions,
                ValidationFileMaximumSize = model.ValidationFileMaximumSize,
                DefaultValue = model.DefaultValue
            };
            _productAttributeService.InsertProductAttributeMapping(productAttributeMapping);
            UpdateLocales(productAttributeMapping, model);

            //predefined values
            var predefinedValues = _productAttributeService.GetPredefinedProductAttributeValues(model.ProductAttributeId);
            foreach (var predefinedValue in predefinedValues)
            {
                var pav = new ProductAttributeValue
                {
                    ProductAttributeMappingId = productAttributeMapping.Id,
                    AttributeValueType = AttributeValueType.Simple,
                    Name = predefinedValue.Name,
                    PriceAdjustment = predefinedValue.PriceAdjustment,
                    WeightAdjustment = predefinedValue.WeightAdjustment,
                    Cost = predefinedValue.Cost,
                    IsPreSelected = predefinedValue.IsPreSelected,
                    DisplayOrder = predefinedValue.DisplayOrder
                };
                _productAttributeService.InsertProductAttributeValue(pav);
                //locales
                var languages = _languageService.GetAllLanguages(true);
                //localization
                foreach (var lang in languages)
                {
                    var name = predefinedValue.GetLocalized(x => x.Name, lang.Id, false, false);
                    if (!string.IsNullOrEmpty(name))
                        _localizedEntityService.SaveLocalizedValue(pav, x => x.Name, name, lang.Id);
                }
            }

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.Added"));

            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("ProductAttributeMappingEdit", new {id = productAttributeMapping.Id});
            }

            SaveSelectedTabName("tab-product-attributes");
            return RedirectToAction("Edit", new {id = product.Id});
        }

        public virtual IActionResult ProductAttributeMappingEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var pam = _productAttributeService.GetProductAttributeMappingById(id);
            if (pam == null)
                throw new ArgumentException("No product attribute mapping found with the specified id");

            var product = _productService.GetProductById(pam.ProductId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
            {
                ErrorNotification(_localizationService.GetResource("This is not your product"));
                return RedirectToAction("List");
            }

            var model = new ProductModel.ProductAttributeMappingModel();
            PrepareProductAttributeMappingModel(model, pam, product, false);
            //condition
            PrepareConditionAttributes(model, pam, product);
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.TextPrompt = pam.GetLocalized(x => x.TextPrompt, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult ProductAttributeMappingEdit(ProductModel.ProductAttributeMappingModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var productAttributeMapping = _productAttributeService.GetProductAttributeMappingById(model.Id);
            if (productAttributeMapping == null)
                throw new ArgumentException("No product attribute mapping found with the specified id");

            var product = _productService.GetProductById(productAttributeMapping.ProductId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
            {
                ErrorNotification(_localizationService.GetResource("This is not your product"));
                return RedirectToAction("List");
            }

            //ensure this attribute is not mapped yet
            if (_productAttributeService.GetProductAttributeMappingsByProductId(product.Id)
                .Any(x => x.ProductAttributeId == model.ProductAttributeId && x.Id != productAttributeMapping.Id))
            {
                //redisplay form
                ErrorNotification(_localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.AlreadyExists"));
                //model
                PrepareProductAttributeMappingModel(model, productAttributeMapping, product, true);
                //condition
                PrepareConditionAttributes(model, productAttributeMapping, product);
                return View(model);
            }

            productAttributeMapping.ProductAttributeId = model.ProductAttributeId;
            productAttributeMapping.TextPrompt = model.TextPrompt;
            productAttributeMapping.IsRequired = model.IsRequired;
            productAttributeMapping.AttributeControlTypeId = model.AttributeControlTypeId;
            productAttributeMapping.DisplayOrder = model.DisplayOrder;
            productAttributeMapping.ValidationMinLength = model.ValidationMinLength;
            productAttributeMapping.ValidationMaxLength = model.ValidationMaxLength;
            productAttributeMapping.ValidationFileAllowedExtensions = model.ValidationFileAllowedExtensions;
            productAttributeMapping.ValidationFileMaximumSize = model.ValidationFileMaximumSize;
            productAttributeMapping.DefaultValue = model.DefaultValue;
            _productAttributeService.UpdateProductAttributeMapping(productAttributeMapping);

            UpdateLocales(productAttributeMapping, model);

            SaveConditionAttributes(productAttributeMapping, model.ConditionModel);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.Updated"));
            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("ProductAttributeMappingEdit", new {id = productAttributeMapping.Id});
            }

            SaveSelectedTabName("tab-product-attributes");
            return RedirectToAction("Edit", new {id = product.Id});
        }

        [HttpPost]
        public virtual IActionResult ProductAttributeMappingDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var productAttributeMapping = _productAttributeService.GetProductAttributeMappingById(id);
            if (productAttributeMapping == null)
                throw new ArgumentException("No product attribute mapping found with the specified id");

            var productId = productAttributeMapping.ProductId;
            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            _productAttributeService.DeleteProductAttributeMapping(productAttributeMapping);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.Deleted"));
            SaveSelectedTabName("tab-product-attributes");
            return RedirectToAction("Edit", new { id = productId });
        }
        
        [HttpPost]
        public virtual IActionResult ProductAttributeValueList(int productAttributeMappingId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            var productAttributeMapping = _productAttributeService.GetProductAttributeMappingById(productAttributeMappingId);
            if (productAttributeMapping == null)
                throw new ArgumentException("No product attribute mapping found with the specified id");

            var product = _productService.GetProductById(productAttributeMapping.ProductId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            var values = _productAttributeService.GetProductAttributeValues(productAttributeMappingId);
            var gridModel = new DataSourceResult
            {
                Data = values.Select(x =>
                {
                    Product associatedProduct = null;
                    if (x.AttributeValueType == AttributeValueType.AssociatedToProduct)
                    {
                        associatedProduct = _productService.GetProductById(x.AssociatedProductId);
                    }
                    var pictureThumbnailUrl = _pictureService.GetPictureUrl(x.PictureId, 75, false);
                    //little hack here. Grid is rendered wrong way with <img> without "src" attribute
                    if (string.IsNullOrEmpty(pictureThumbnailUrl))
                        pictureThumbnailUrl = _pictureService.GetPictureUrl(null, 1, true);
                    return new ProductModel.ProductAttributeValueModel
                    {
                        Id = x.Id,
                        ProductAttributeMappingId = x.ProductAttributeMappingId,
                        AttributeValueTypeId = x.AttributeValueTypeId,
                        AttributeValueTypeName = x.AttributeValueType.GetLocalizedEnum(_localizationService, _workContext),
                        AssociatedProductId = x.AssociatedProductId,
                        AssociatedProductName = associatedProduct != null ? associatedProduct.Name : "",
                        Name = x.ProductAttributeMapping.AttributeControlType != AttributeControlType.ColorSquares ? x.Name : $"{x.Name} - {x.ColorSquaresRgb}",
                        ColorSquaresRgb = x.ColorSquaresRgb,
                        ImageSquaresPictureId = x.ImageSquaresPictureId,
                        PriceAdjustment = x.PriceAdjustment,
                        PriceAdjustmentStr = x.AttributeValueType == AttributeValueType.Simple ? x.PriceAdjustment.ToString("G29") : "",
                        WeightAdjustment = x.WeightAdjustment,
                        WeightAdjustmentStr = x.AttributeValueType == AttributeValueType.Simple ? x.WeightAdjustment.ToString("G29") : "",
                        Cost = x.Cost,
                        CustomerEntersQty = x.CustomerEntersQty,
                        Quantity = x.Quantity,
                        IsPreSelected = x.IsPreSelected,
                        DisplayOrder = x.DisplayOrder,
                        PictureId = x.PictureId,
                        PictureThumbnailUrl = pictureThumbnailUrl
                    };
                }),
                Total = values.Count
            };

            return Json(gridModel);
        }

        public virtual IActionResult ProductAttributeValueCreatePopup(int productAttributeMappingId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var productAttributeMapping = _productAttributeService.GetProductAttributeMappingById(productAttributeMappingId);
            if (productAttributeMapping == null)
                throw new ArgumentException("No product attribute mapping found with the specified id");

            var product = _productService.GetProductById(productAttributeMapping.ProductId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");

            var model = new ProductModel.ProductAttributeValueModel
            {
                ProductAttributeMappingId = productAttributeMappingId,

                //color squares
                DisplayColorSquaresRgb = productAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares,
                //image squares
                DisplayImageSquaresPicture = productAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares,

                //default qantity for associated product
                Quantity = 1
            };

            //locales
            AddLocales(_languageService, model.Locales);

            //pictures
            model.ProductPictureModels = _productService.GetProductPicturesByProductId(product.Id)
                .Select(x => new ProductModel.ProductPictureModel
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    PictureId = x.PictureId,
                    PictureUrl = _pictureService.GetPictureUrl(x.PictureId),
                    DisplayOrder = x.DisplayOrder
                })
                .ToList();

            return View(model);
        }
        [HttpPost]
        public virtual IActionResult ProductAttributeValueCreatePopup(ProductModel.ProductAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var productAttributeMapping = _productAttributeService.GetProductAttributeMappingById(model.ProductAttributeMappingId);
            if (productAttributeMapping == null)
                //No product attribute found with the specified id
                return RedirectToAction("List", "Product");

            var product = _productService.GetProductById(productAttributeMapping.ProductId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");

            if (productAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares)
            {
                //ensure valid color is chosen/entered
                if (string.IsNullOrEmpty(model.ColorSquaresRgb))
                    ModelState.AddModelError("", "Color is required");
                try
                {
                    //ensure color is valid (can be instanciated)
                    System.Drawing.ColorTranslator.FromHtml(model.ColorSquaresRgb);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.Message);
                }
            }

            //ensure a picture is uploaded
            if (productAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares && model.ImageSquaresPictureId == 0)
            {
                ModelState.AddModelError("", "Image is required");
            }

            if (ModelState.IsValid)
            {
                var pav = new ProductAttributeValue
                {
                    ProductAttributeMappingId = model.ProductAttributeMappingId,
                    AttributeValueTypeId = model.AttributeValueTypeId,
                    AssociatedProductId = model.AssociatedProductId,
                    Name = model.Name,
                    ColorSquaresRgb = model.ColorSquaresRgb,
                    ImageSquaresPictureId = model.ImageSquaresPictureId,
                    PriceAdjustment = model.PriceAdjustment,
                    WeightAdjustment = model.WeightAdjustment,
                    Cost = model.Cost,
                    CustomerEntersQty = model.CustomerEntersQty,
                    Quantity = model.CustomerEntersQty ? 1 : model.Quantity,
                    IsPreSelected = model.IsPreSelected,
                    DisplayOrder = model.DisplayOrder,
                    PictureId = model.PictureId,
                };

                _productAttributeService.InsertProductAttributeValue(pav);
                UpdateLocales(pav, model);

                ViewBag.RefreshPage = true;
                return View(model);
            }

            //If we got this far, something failed, redisplay form

            //pictures
            model.ProductPictureModels = _productService.GetProductPicturesByProductId(product.Id)
                .Select(x => new ProductModel.ProductPictureModel
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    PictureId = x.PictureId,
                    PictureUrl = _pictureService.GetPictureUrl(x.PictureId),
                    DisplayOrder = x.DisplayOrder
                })
                .ToList();

            var associatedProduct = _productService.GetProductById(model.AssociatedProductId);
            model.AssociatedProductName = associatedProduct != null ? associatedProduct.Name : "";

            return View(model);
        }

        public virtual IActionResult ProductAttributeValueEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var pav = _productAttributeService.GetProductAttributeValueById(id);
            if (pav == null)
                //No attribute value found with the specified id
                return RedirectToAction("List", "Product");

            var product = _productService.GetProductById(pav.ProductAttributeMapping.ProductId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");

            var associatedProduct = _productService.GetProductById(pav.AssociatedProductId);

            var model = new ProductModel.ProductAttributeValueModel
            {
                ProductAttributeMappingId = pav.ProductAttributeMappingId,
                AttributeValueTypeId = pav.AttributeValueTypeId,
                AttributeValueTypeName = pav.AttributeValueType.GetLocalizedEnum(_localizationService, _workContext),
                AssociatedProductId = pav.AssociatedProductId,
                AssociatedProductName = associatedProduct != null ? associatedProduct.Name : "",
                Name = pav.Name,
                ColorSquaresRgb = pav.ColorSquaresRgb,
                DisplayColorSquaresRgb = pav.ProductAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares,
                ImageSquaresPictureId = pav.ImageSquaresPictureId,
                DisplayImageSquaresPicture = pav.ProductAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares,
                PriceAdjustment = pav.PriceAdjustment,
                WeightAdjustment = pav.WeightAdjustment,
                Cost = pav.Cost,
                CustomerEntersQty = pav.CustomerEntersQty,
                Quantity = pav.Quantity,
                IsPreSelected = pav.IsPreSelected,
                DisplayOrder = pav.DisplayOrder,
                PictureId = pav.PictureId
            };
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = pav.GetLocalized(x => x.Name, languageId, false, false);
            });
            //pictures
            model.ProductPictureModels = _productService.GetProductPicturesByProductId(product.Id)
                .Select(x => new ProductModel.ProductPictureModel
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    PictureId = x.PictureId,
                    PictureUrl = _pictureService.GetPictureUrl(x.PictureId),
                    DisplayOrder = x.DisplayOrder
                })
                .ToList();

            return View(model);
        }
        [HttpPost]
        public virtual IActionResult ProductAttributeValueEditPopup(ProductModel.ProductAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var pav = _productAttributeService.GetProductAttributeValueById(model.Id);
            if (pav == null)
                //No attribute value found with the specified id
                return RedirectToAction("List", "Product");

            var product = _productService.GetProductById(pav.ProductAttributeMapping.ProductId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");

            if (pav.ProductAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares)
            {
                //ensure valid color is chosen/entered
                if (string.IsNullOrEmpty(model.ColorSquaresRgb))
                    ModelState.AddModelError("", "Color is required");
                try
                {
                    //ensure color is valid (can be instanciated)
                    System.Drawing.ColorTranslator.FromHtml(model.ColorSquaresRgb);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.Message);
                }
            }

            //ensure a picture is uploaded
            if (pav.ProductAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares && model.ImageSquaresPictureId == 0)
            {
                ModelState.AddModelError("", "Image is required");
            }

            if (ModelState.IsValid)
            {
                pav.AttributeValueTypeId = model.AttributeValueTypeId;
                pav.AssociatedProductId = model.AssociatedProductId;
                pav.Name = model.Name;
                pav.ColorSquaresRgb = model.ColorSquaresRgb;
                pav.ImageSquaresPictureId = model.ImageSquaresPictureId;
                pav.PriceAdjustment = model.PriceAdjustment;
                pav.WeightAdjustment = model.WeightAdjustment;
                pav.Cost = model.Cost;
                pav.CustomerEntersQty = model.CustomerEntersQty;
                pav.Quantity = model.CustomerEntersQty ? 1 : model.Quantity;
                pav.IsPreSelected = model.IsPreSelected;
                pav.DisplayOrder = model.DisplayOrder;
                pav.PictureId = model.PictureId;
                _productAttributeService.UpdateProductAttributeValue(pav);

                UpdateLocales(pav, model);

                ViewBag.RefreshPage = true;
                return View(model);
            }

            //If we got this far, something failed, redisplay form

            //pictures
            model.ProductPictureModels = _productService.GetProductPicturesByProductId(product.Id)
                .Select(x => new ProductModel.ProductPictureModel
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    PictureId = x.PictureId,
                    PictureUrl = _pictureService.GetPictureUrl(x.PictureId),
                    DisplayOrder = x.DisplayOrder
                })
                .ToList();

            var associatedProduct = _productService.GetProductById(model.AssociatedProductId);
            model.AssociatedProductName = associatedProduct != null ? associatedProduct.Name : "";

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ProductAttributeValueDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var pav = _productAttributeService.GetProductAttributeValueById(id);
            if (pav == null)
                throw new ArgumentException("No product attribute value found with the specified id");

            var product = _productService.GetProductById(pav.ProductAttributeMapping.ProductId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            _productAttributeService.DeleteProductAttributeValue(pav);

            return new NullJsonResult();
        }
        
        public virtual IActionResult AssociateProductToAttributeValuePopup()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var model = new ProductModel.ProductAttributeValueModel.AssociateProductToAttributeValueModel
            {
                //a vendor should have access only to his products
                IsLoggedInAsVendor = _workContext.CurrentVendor != null
            };

            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var manufacturers = SelectListHelper.GetManufacturerList(_manufacturerService, _cacheManager, true);
            foreach (var m in manufacturers)
                model.AvailableManufacturers.Add(m);

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var vendors = SelectListHelper.GetVendorList(_vendorService, _cacheManager, true);
            foreach (var v in vendors)
                model.AvailableVendors.Add(v);

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult AssociateProductToAttributeValuePopupList(DataSourceRequest command,
            ProductModel.ProductAttributeValueModel.AssociateProductToAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.SearchVendorId = _workContext.CurrentVendor.Id;
            }

            var products = _productService.SearchProducts(
                categoryIds: new List<int> { model.SearchCategoryId },
                manufacturerId: model.SearchManufacturerId,
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true
                );
            var gridModel = new DataSourceResult
            {
                Data = products.Select(x => x.ToModel()),
                Total = products.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult AssociateProductToAttributeValuePopup(ProductModel.ProductAttributeValueModel.AssociateProductToAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var associatedProduct = _productService.GetProductById(model.AssociatedToProductId);
            if (associatedProduct == null)
                return Content("Cannot load a product");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && associatedProduct.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
            ViewBag.RefreshPage = true;
            ViewBag.productId = associatedProduct.Id;
            ViewBag.productName = associatedProduct.Name;
            return View(model);
        }

        //action displaying notification (warning) to a store owner when associating some product
        public virtual IActionResult AssociatedProductGetWarnings(int productId)
        {
            var associatedProduct = _productService.GetProductById(productId);
            if (associatedProduct != null)
            {
                //attributes
                if (associatedProduct.ProductAttributeMappings.Any())
                {
                    if (associatedProduct.ProductAttributeMappings.Any(attribute => attribute.IsRequired))
                        return Json(new { Result = _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.HasRequiredAttributes") });

                    return Json(new { Result = _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.HasAttributes") });
                }
                
                //gift card
                if (associatedProduct.IsGiftCard)
                {
                    return Json(new { Result = _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.GiftCard") });
                }

                //downloaable product
                if (associatedProduct.IsDownload)
                {
                    return Json(new { Result = _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.Downloadable") });
                }
            }

            return Json(new { Result = string.Empty });
        }

        #endregion

        #region Product attribute combinations

        [HttpPost]
        public virtual IActionResult ProductAttributeCombinationList(DataSourceRequest command, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            var combinations = _productAttributeService.GetAllProductAttributeCombinations(productId);
            var combinationsModel = combinations
                .Select(x =>
                {
                    var pacModel = new ProductModel.ProductAttributeCombinationModel
                    {
                        Id = x.Id,
                        ProductId = x.ProductId,
                        AttributesXml = _productAttributeFormatter.FormatAttributes(x.Product, x.AttributesXml, _workContext.CurrentCustomer, "<br />", true, true, true, false),
                        StockQuantity = x.StockQuantity,
                        AllowOutOfStockOrders = x.AllowOutOfStockOrders,
                        Sku = x.Sku,
                        ManufacturerPartNumber = x.ManufacturerPartNumber,
                        Gtin = x.Gtin,
                        OverriddenPrice = x.OverriddenPrice,
                        NotifyAdminForQuantityBelow = x.NotifyAdminForQuantityBelow
                    };
                    //warnings
                    var warnings = _shoppingCartService.GetShoppingCartItemAttributeWarnings(_workContext.CurrentCustomer,
                        ShoppingCartType.ShoppingCart, x.Product, 1, x.AttributesXml, true);
                    for (var i = 0; i < warnings.Count; i++)
                    {
                        pacModel.Warnings += warnings[i];
                        if (i != warnings.Count - 1)
                            pacModel.Warnings += "<br />";
                    }

                    return pacModel;
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = combinationsModel,
                Total = combinationsModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult ProductAttributeCombinationUpdate(ProductModel.ProductAttributeCombinationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var combination = _productAttributeService.GetProductAttributeCombinationById(model.Id);
            if (combination == null)
                throw new ArgumentException("No product attribute combination found with the specified id");

            var product = _productService.GetProductById(combination.ProductId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            var previousSrockQuantity = combination.StockQuantity;

            combination.StockQuantity = model.StockQuantity;
            combination.AllowOutOfStockOrders = model.AllowOutOfStockOrders;
            combination.Sku = model.Sku;
            combination.ManufacturerPartNumber = model.ManufacturerPartNumber;
            combination.Gtin = model.Gtin;
            combination.OverriddenPrice = model.OverriddenPrice;
            combination.NotifyAdminForQuantityBelow = model.NotifyAdminForQuantityBelow;
            _productAttributeService.UpdateProductAttributeCombination(combination);

            //quantity change history
            _productService.AddStockQuantityHistoryEntry(product, combination.StockQuantity - previousSrockQuantity, combination.StockQuantity,
                message: _localizationService.GetResource("Admin.StockQuantityHistory.Messages.Combination.Edit"), combinationId: combination.Id);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult ProductAttributeCombinationDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var combination = _productAttributeService.GetProductAttributeCombinationById(id);
            if (combination == null)
                throw new ArgumentException("No product attribute combination found with the specified id");

            var product = _productService.GetProductById(combination.ProductId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            _productAttributeService.DeleteProductAttributeCombination(combination);

            return new NullJsonResult();
        }

        public virtual IActionResult AddAttributeCombinationPopup(int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var product = _productService.GetProductById(productId);
            if (product == null)
                //No product found with the specified id
                return RedirectToAction("List", "Product");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");
            
            var model = new AddProductAttributeCombinationModel();
            PrepareAddProductAttributeCombinationModel(model, product);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult AddAttributeCombinationPopup(int productId, AddProductAttributeCombinationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var product = _productService.GetProductById(productId);
            if (product == null)
                //No product found with the specified id
                return RedirectToAction("List", "Product");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");
            
            //attributes
            var attributesXml = "";
            var form = model.Form;
            var warnings = new List<string>();

            //product attributes
            var attributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id)
                //ignore non-combinable attributes for combinations
                .Where(x => !x.IsNonCombinable()).ToList();
            foreach (var attribute in attributes)
            {
                var controlId = $"product_attribute_{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    {
                        var ctrlAttributes = form[controlId];
                        if (!string.IsNullOrEmpty(ctrlAttributes))
                        {
                            var selectedAttributeId = int.Parse(ctrlAttributes);
                            if (selectedAttributeId > 0)
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                        }
                    }
                        break;
                    case AttributeControlType.Checkboxes:
                    {
                        var cblAttributes = form[controlId].ToString();
                        if (!string.IsNullOrEmpty(cblAttributes))
                        {
                            foreach (var item in cblAttributes.Split(new[] {','},
                                StringSplitOptions.RemoveEmptyEntries))
                            {
                                var selectedAttributeId = int.Parse(item);
                                if (selectedAttributeId > 0)
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                    }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                    {
                        //load read-only (already server-side selected) values
                        var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                        foreach (var selectedAttributeId in attributeValues
                            .Where(v => v.IsPreSelected)
                            .Select(v => v.Id)
                            .ToList())
                        {
                            attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                attribute, selectedAttributeId.ToString());
                        }
                    }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                    {
                        var ctrlAttributes = form[controlId].ToString();
                        if (!string.IsNullOrEmpty(ctrlAttributes))
                        {
                            var enteredText = ctrlAttributes.Trim();
                            attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                attribute, enteredText);
                        }
                    }
                        break;
                    case AttributeControlType.Datepicker:
                    {
                        var date = form[controlId + "_day"];
                        var month = form[controlId + "_month"];
                        var year = form[controlId + "_year"];
                        DateTime? selectedDate = null;
                        try
                        {
                            selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(date));
                        }
                        catch
                        {
                        }
                        if (selectedDate.HasValue)
                        {
                            attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                attribute, selectedDate.Value.ToString("D"));
                        }
                    }
                        break;
                    case AttributeControlType.FileUpload:
                    {
                        var httpPostedFile = this.Request.Form.Files[controlId];
                        if ((httpPostedFile != null) && (!string.IsNullOrEmpty(httpPostedFile.FileName)))
                        {
                            var fileSizeOk = true;
                            if (attribute.ValidationFileMaximumSize.HasValue)
                            {
                                //compare in bytes
                                var maxFileSizeBytes = attribute.ValidationFileMaximumSize.Value * 1024;
                                if (httpPostedFile.Length > maxFileSizeBytes)
                                {
                                    warnings.Add(string.Format(
                                        _localizationService.GetResource("ShoppingCart.MaximumUploadedFileSize"),
                                        attribute.ValidationFileMaximumSize.Value));
                                    fileSizeOk = false;
                                }
                            }
                            if (fileSizeOk)
                            {
                                //save an uploaded file
                                var download = new Download
                                {
                                    DownloadGuid = Guid.NewGuid(),
                                    UseDownloadUrl = false,
                                    DownloadUrl = "",
                                    DownloadBinary = httpPostedFile.GetDownloadBits(),
                                    ContentType = httpPostedFile.ContentType,
                                    Filename = Path.GetFileNameWithoutExtension(httpPostedFile.FileName),
                                    Extension = Path.GetExtension(httpPostedFile.FileName),
                                    IsNew = true
                                };
                                _downloadService.InsertDownload(download);

                                //save attribute
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, download.DownloadGuid.ToString());
                            }
                        }
                    }
                        break;
                    default:
                        break;
                }
            }
            //validate conditional attributes (if specified)
            foreach (var attribute in attributes)
            {
                var conditionMet = _productAttributeParser.IsConditionMet(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                {
                    attributesXml = _productAttributeParser.RemoveProductAttribute(attributesXml, attribute);
                }
            }

            warnings.AddRange(_shoppingCartService.GetShoppingCartItemAttributeWarnings(_workContext.CurrentCustomer,
                ShoppingCartType.ShoppingCart, product, 1, attributesXml, true));

            //check whether the same attribute combination already exists
            var existingCombination = _productAttributeParser.FindProductAttributeCombination(product, attributesXml);
            if (existingCombination != null)
                warnings.Add(_localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.AlreadyExists"));

            if (!warnings.Any())
            {
                //save combination
                var combination = new ProductAttributeCombination
                {
                    ProductId = product.Id,
                    AttributesXml = attributesXml,
                    StockQuantity = model.StockQuantity,
                    AllowOutOfStockOrders = model.AllowOutOfStockOrders,
                    Sku = model.Sku,
                    ManufacturerPartNumber = model.ManufacturerPartNumber,
                    Gtin = model.Gtin,
                    OverriddenPrice = model.OverriddenPrice,
                    NotifyAdminForQuantityBelow = model.NotifyAdminForQuantityBelow,
                };
                _productAttributeService.InsertProductAttributeCombination(combination);

                //quantity change history
                _productService.AddStockQuantityHistoryEntry(product, combination.StockQuantity, combination.StockQuantity,
                    message: _localizationService.GetResource("Admin.StockQuantityHistory.Messages.Combination.Edit"), combinationId: combination.Id);

                ViewBag.RefreshPage = true;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            PrepareAddProductAttributeCombinationModel(model, product);
            model.Warnings = warnings;
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult GenerateAllAttributeCombinations(int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            var allAttributesXml = _productAttributeParser.GenerateAllCombinations(product, true);
            foreach (var attributesXml in allAttributesXml)
            {
                var existingCombination = _productAttributeParser.FindProductAttributeCombination(product, attributesXml);

                //already exists?
                if (existingCombination != null)
                    continue;

                //new one
                var warnings = new List<string>();
                warnings.AddRange(_shoppingCartService.GetShoppingCartItemAttributeWarnings(_workContext.CurrentCustomer,
                    ShoppingCartType.ShoppingCart, product, 1, attributesXml, true));
                if (warnings.Count != 0)
                    continue;

                //save combination
                var combination = new ProductAttributeCombination
                {
                    ProductId = product.Id,
                    AttributesXml = attributesXml,
                    StockQuantity = 0,
                    AllowOutOfStockOrders = false,
                    Sku = null,
                    ManufacturerPartNumber = null,
                    Gtin = null,
                    OverriddenPrice = null,
                    NotifyAdminForQuantityBelow = 1
                };
                _productAttributeService.InsertProductAttributeCombination(combination);
            }
            return Json(new { Success = true });
        }

        #endregion

        #region Product editor settings

        [HttpPost]
        public virtual IActionResult SaveProductEditorSettings(ProductModel model, string returnUrl = "")
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //vendors cannot manage these settings
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("List");
            
            var productEditorSettings = _settingService.LoadSetting<ProductEditorSettings>();
            productEditorSettings = model.ProductEditorSettingsModel.ToEntity(productEditorSettings);
            _settingService.SaveSetting(productEditorSettings);

            //product list
            if (string.IsNullOrEmpty(returnUrl))
                return RedirectToAction("List");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("List");

            return Redirect(returnUrl);
        }

        #endregion

        #region Stock quantity history

        [HttpPost]
        public virtual IActionResult StockQuantityHistory(DataSourceRequest command, int productId, int warehouseId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            var stockQuantityHistory = _productService.GetStockQuantityHistory(product, warehouseId, pageIndex: command.Page - 1, pageSize: command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = stockQuantityHistory.Select(historyEntry =>
                {
                    var warehouseName = _localizationService.GetResource("Admin.Catalog.Products.Fields.Warehouse.None");
                    if (historyEntry.WarehouseId.HasValue)
                    {
                        var warehouse = _shippingService.GetWarehouseById(historyEntry.WarehouseId.Value);
                        warehouseName = warehouse != null ? warehouse.Name : "Deleted";
                    }

                    var attributesXml = string.Empty;
                    if (historyEntry.CombinationId.HasValue)
                    {
                        var combination = _productAttributeService.GetProductAttributeCombinationById(historyEntry.CombinationId.Value);
                        attributesXml = combination == null ? string.Empty :
                            _productAttributeFormatter.FormatAttributes(historyEntry.Product, combination.AttributesXml, _workContext.CurrentCustomer, renderGiftCardAttributes: false);
                    }

                    return new ProductModel.StockQuantityHistoryModel
                    {
                        Id = historyEntry.Id,
                        QuantityAdjustment = historyEntry.QuantityAdjustment,
                        StockQuantity = historyEntry.StockQuantity,
                        Message = historyEntry.Message,
                        AttributeCombination = attributesXml,
                        WarehouseName = warehouseName,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(historyEntry.CreatedOnUtc, DateTimeKind.Utc)
                    };
                }),
                Total = stockQuantityHistory.TotalCount
            };

            return Json(gridModel);
        }

        #endregion

        #endregion
    }
}