using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ProductController : BaseAdminController
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
        private readonly ICategoryService _categoryService;
        private readonly ICopyProductService _copyProductService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IDownloadService _downloadService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IManufacturerService _manufacturerService;
        private readonly INopFileProvider _fileProvider;
        private readonly IPdfService _pdfService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IProductTagService _productTagService;
        private readonly ISettingService _settingService;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public ProductController(IAclService aclService,
            IBackInStockSubscriptionService backInStockSubscriptionService,
            ICategoryService categoryService,
            ICopyProductService copyProductService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDiscountService discountService,
            IDownloadService downloadService,
            IExportManager exportManager,
            IImportManager importManager,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IManufacturerService manufacturerService,
            INopFileProvider fileProvider,
            IPdfService pdfService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IProductTagService productTagService,
            ISettingService settingService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            ISpecificationAttributeService specificationAttributeService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            VendorSettings vendorSettings)
        {
            this._aclService = aclService;
            this._backInStockSubscriptionService = backInStockSubscriptionService;
            this._categoryService = categoryService;
            this._copyProductService = copyProductService;
            this._customerActivityService = customerActivityService;
            this._customerService = customerService;
            this._discountService = discountService;
            this._downloadService = downloadService;
            this._exportManager = exportManager;
            this._importManager = importManager;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._manufacturerService = manufacturerService;
            this._fileProvider = fileProvider;
            this._pdfService = pdfService;
            this._permissionService = permissionService;
            this._pictureService = pictureService;
            this._productAttributeParser = productAttributeParser;
            this._productAttributeService = productAttributeService;
            this._productModelFactory = productModelFactory;
            this._productService = productService;
            this._productTagService = productTagService;
            this._settingService = settingService;
            this._shippingService = shippingService;
            this._shoppingCartService = shoppingCartService;
            this._specificationAttributeService = specificationAttributeService;
            this._storeMappingService = storeMappingService;
            this._storeService = storeService;
            this._urlRecordService = urlRecordService;
            this._workContext = workContext;
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
                var seName = _urlRecordService.ValidateSeName(product, localized.SeName, localized.Name, false);
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

                var seName = _urlRecordService.ValidateSeName(productTag, string.Empty, localized.Name, false);
                _urlRecordService.SaveSlug(productTag, seName, localized.LanguageId);
            }
        }

        protected virtual void UpdateLocales(ProductAttributeMapping pam, ProductAttributeMappingModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(pam,
                    x => x.TextPrompt,
                    localized.TextPrompt,
                    localized.LanguageId);
            }
        }

        protected virtual void UpdateLocales(ProductAttributeValue pav, ProductAttributeValueModel model)
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

        protected virtual void SaveCategoryMappings(Product product, ProductModel model)
        {
            var existingProductCategories = _categoryService.GetProductCategoriesByProductId(product.Id, true);

            //delete categories
            foreach (var existingProductCategory in existingProductCategories)
                if (!model.SelectedCategoryIds.Contains(existingProductCategory.CategoryId))
                    _categoryService.DeleteProductCategory(existingProductCategory);

            //add categories
            foreach (var categoryId in model.SelectedCategoryIds)
            {
                if (_categoryService.FindProductCategory(existingProductCategories, product.Id, categoryId) == null)
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
            {
                if (_manufacturerService.FindProductManufacturer(existingProductManufacturers, product.Id, manufacturerId) == null)
                {
                    //find next display order
                    var displayOrder = 1;
                    var existingManufacturerMapping = _manufacturerService.GetProductManufacturersByManufacturerId(manufacturerId, showHidden: true);
                    if (existingManufacturerMapping.Any())
                        displayOrder = existingManufacturerMapping.Max(x => x.DisplayOrder) + 1;
                    _manufacturerService.InsertProductManufacturer(new ProductManufacturer
                    {
                        ProductId = product.Id,
                        ManufacturerId = manufacturerId,
                        DisplayOrder = displayOrder
                    });
                }
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
                    if (product.DiscountProductMappings.Count(mapping => mapping.DiscountId == discount.Id) == 0)
                        product.DiscountProductMappings.Add(new DiscountProductMapping { Discount = discount });
                }
                else
                {
                    //remove discount
                    if (product.DiscountProductMappings.Count(mapping => mapping.DiscountId == discount.Id) > 0)
                    {
                        product.DiscountProductMappings
                            .Remove(product.DiscountProductMappings.FirstOrDefault(mapping => mapping.DiscountId == discount.Id));
                    }
                }
            }

            _productService.UpdateProduct(product);
            _productService.UpdateHasDiscountsApplied(product);
        }

        protected virtual string GetAttributesXmlForProductAttributeCombination(IFormCollection form, List<string> warnings, int productId)
        {
            var attributesXml = string.Empty;

            //get product attribute mappings (exclude non-combinable attributes)
            var attributes = _productAttributeService.GetProductAttributeMappingsByProductId(productId)
                .Where(productAttributeMapping => !productAttributeMapping.IsNonCombinable()).ToList();

            foreach (var attribute in attributes)
            {
                var controlId = $"product_attribute_{attribute.Id}";
                StringValues ctrlAttributes;

                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        ctrlAttributes = form[controlId];
                        if (!string.IsNullOrEmpty(ctrlAttributes))
                        {
                            var selectedAttributeId = int.Parse(ctrlAttributes);
                            if (selectedAttributeId > 0)
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                        }

                        break;
                    case AttributeControlType.Checkboxes:
                        var cblAttributes = form[controlId].ToString();
                        if (!string.IsNullOrEmpty(cblAttributes))
                        {
                            foreach (var item in cblAttributes.Split(new[] { ',' },
                                StringSplitOptions.RemoveEmptyEntries))
                            {
                                var selectedAttributeId = int.Parse(item);
                                if (selectedAttributeId > 0)
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }

                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
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

                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        ctrlAttributes = form[controlId];
                        if (!string.IsNullOrEmpty(ctrlAttributes))
                        {
                            var enteredText = ctrlAttributes.ToString().Trim();
                            attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                attribute, enteredText);
                        }

                        break;
                    case AttributeControlType.Datepicker:
                        var date = form[controlId + "_day"];
                        var month = form[controlId + "_month"];
                        var year = form[controlId + "_year"];
                        DateTime? selectedDate = null;
                        try
                        {
                            selectedDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(date));
                        }
                        catch
                        {
                            //ignore any exception
                        }

                        if (selectedDate.HasValue)
                        {
                            attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                attribute, selectedDate.Value.ToString("D"));
                        }

                        break;
                    case AttributeControlType.FileUpload:
                        var httpPostedFile = Request.Form.Files[controlId];
                        if (!string.IsNullOrEmpty(httpPostedFile?.FileName))
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
                                    DownloadUrl = string.Empty,
                                    DownloadBinary = _downloadService.GetDownloadBits(httpPostedFile),
                                    ContentType = httpPostedFile.ContentType,
                                    Filename = _fileProvider.GetFileNameWithoutExtension(httpPostedFile.FileName),
                                    Extension = _fileProvider.GetFileExtension(httpPostedFile.FileName),
                                    IsNew = true
                                };
                                _downloadService.InsertDownload(download);

                                //save attribute
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, download.DownloadGuid.ToString());
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

            return attributesXml;
        }

        protected virtual string[] ParseProductTags(string productTags)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(productTags))
                return result.ToArray();

            var values = productTags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var val in values)
                if (!string.IsNullOrEmpty(val.Trim()))
                    result.Add(val.Trim());

            return result.ToArray();
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

            var formData = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());

            foreach (var warehouse in warehouses)
            {
                //parse stock quantity
                var stockQuantity = 0;
                foreach (var formKey in formData.Keys)
                {
                    if (!formKey.Equals($"warehouse_qty_{warehouse.Id}", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    int.TryParse(formData[formKey], out stockQuantity);
                    break;
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
                        int.TryParse(formData[formKey], out var tmp);
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
                    if (!used)
                        continue;

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
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                //for conditions we should empty values save even when nothing is selected
                                //otherwise "attributesXml" will be empty
                                //hence we won't be able to find a selected attribute
                                attributesXml = _productAttributeParser.AddProductAttribute(null, attribute,
                                    selectedAttributeId > 0 ? selectedAttributeId.ToString() : string.Empty);
                            }
                            else
                            {
                                //for conditions we should empty values save even when nothing is selected
                                //otherwise "attributesXml" will be empty
                                //hence we won't be able to find a selected attribute
                                attributesXml = _productAttributeParser.AddProductAttribute(null,
                                    attribute, string.Empty);
                            }

                            break;
                        case AttributeControlType.Checkboxes:
                            var cblAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(cblAttributes))
                            {
                                var anyValueSelected = false;
                                foreach (var item in cblAttributes.ToString()
                                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId <= 0)
                                        continue;

                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                                    anyValueSelected = true;
                                }

                                if (!anyValueSelected)
                                {
                                    //for conditions we should save empty values even when nothing is selected
                                    //otherwise "attributesXml" will be empty
                                    //hence we won't be able to find a selected attribute
                                    attributesXml = _productAttributeParser.AddProductAttribute(null,
                                        attribute, string.Empty);
                                }
                            }
                            else
                            {
                                //for conditions we should save empty values even when nothing is selected
                                //otherwise "attributesXml" will be empty
                                //hence we won't be able to find a selected attribute
                                attributesXml = _productAttributeParser.AddProductAttribute(null,
                                    attribute, string.Empty);
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

        protected virtual void GenerateAttributeCombinations(Product product, IList<int> allowedAttributeIds = null)
        {
            var allAttributesXml = _productAttributeParser.GenerateAllCombinations(product, true, allowedAttributeIds);
            foreach (var attributesXml in allAttributesXml)
            {
                var existingCombination = _productAttributeParser.FindProductAttributeCombination(product, attributesXml);

                //already exists?
                if (existingCombination != null)
                    continue;

                //new one
                var warnings = new List<string>();
                warnings.AddRange(_shoppingCartService.GetShoppingCartItemAttributeWarnings(_workContext.CurrentCustomer,
                    ShoppingCartType.ShoppingCart, product, 1, attributesXml, true, true));
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
                    NotifyAdminForQuantityBelow = 1,
                    PictureId = 0
                };
                _productAttributeService.InsertProductAttributeCombination(combination);
            }
        }

        #endregion

        #region Methods

        #region Product list / create / edit / delete

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model = _productModelFactory.PrepareProductSearchModel(new ProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ProductList(ProductSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _productModelFactory.PrepareProductListModel(searchModel);

            return Json(model);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-product-by-sku")]
        public virtual IActionResult GoToSku(ProductSearchModel searchModel)
        {
            //try to load a product entity, if not found, then try to load a product attribute combination
            var product = _productService.GetProductBySku(searchModel.GoDirectlyToSku)
                ?? _productAttributeService.GetProductAttributeCombinationBySku(searchModel.GoDirectlyToSku)?.Product;

            if (product != null)
                return RedirectToAction("Edit", "Product", new { id = product.Id });

            //not found
            return List();
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //validate maximum number of products per vendor
            if (_vendorSettings.MaximumProductNumber > 0 && _workContext.CurrentVendor != null
                && _productService.GetNumberOfProductsByVendorId(_workContext.CurrentVendor.Id) >= _vendorSettings.MaximumProductNumber)
            {
                ErrorNotification(string.Format(_localizationService.GetResource("Admin.Catalog.Products.ExceededMaximumNumber"),
                    _vendorSettings.MaximumProductNumber));
                return RedirectToAction("List");
            }

            //prepare model
            var model = _productModelFactory.PrepareProductModel(new ProductModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(ProductModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //validate maximum number of products per vendor
            if (_vendorSettings.MaximumProductNumber > 0 && _workContext.CurrentVendor != null
                && _productService.GetNumberOfProductsByVendorId(_workContext.CurrentVendor.Id) >= _vendorSettings.MaximumProductNumber)
            {
                ErrorNotification(string.Format(_localizationService.GetResource("Admin.Catalog.Products.ExceededMaximumNumber"),
                    _vendorSettings.MaximumProductNumber));
                return RedirectToAction("List");
            }

            if (ModelState.IsValid)
            {
                //a vendor should have access only to his products
                if (_workContext.CurrentVendor != null)
                    model.VendorId = _workContext.CurrentVendor.Id;

                //vendors cannot edit "Show on home page" property
                if (_workContext.CurrentVendor != null && model.ShowOnHomePage)
                    model.ShowOnHomePage = false;

                //product
                var product = model.ToEntity<Product>();
                product.CreatedOnUtc = DateTime.UtcNow;
                product.UpdatedOnUtc = DateTime.UtcNow;
                _productService.InsertProduct(product);

                //search engine name
                model.SeName = _urlRecordService.ValidateSeName(product, model.SeName, product.Name, true);
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
                _customerActivityService.InsertActivity("AddNewProduct",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewProduct"), product.Name), product);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = product.Id });
            }

            //prepare model
            model = _productModelFactory.PrepareProductModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = _productService.GetProductById(id);
            if (product == null || product.Deleted)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List");

            //prepare model
            var model = _productModelFactory.PrepareProductModel(null, product);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(ProductModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = _productService.GetProductById(model.Id);
            if (product == null || product.Deleted)
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
                    model.VendorId = _workContext.CurrentVendor.Id;

                //we do not validate maximum number of products per vendor when editing existing products (only during creation of new products)
                //vendors cannot edit "Show on home page" property
                if (_workContext.CurrentVendor != null && model.ShowOnHomePage != product.ShowOnHomePage)
                    model.ShowOnHomePage = product.ShowOnHomePage;

                //some previously used values
                var prevTotalStockQuantity = _productService.GetTotalStockQuantity(product);
                var prevDownloadId = product.DownloadId;
                var prevSampleDownloadId = product.SampleDownloadId;
                var previousStockQuantity = product.StockQuantity;
                var previousWarehouseId = product.WarehouseId;

                //product
                product = model.ToEntity(product);

                product.UpdatedOnUtc = DateTime.UtcNow;
                _productService.UpdateProduct(product);

                //search engine name
                model.SeName = _urlRecordService.ValidateSeName(product, model.SeName, product.Name, true);
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
                    _productService.GetTotalStockQuantity(product) > 0 &&
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
                _customerActivityService.InsertActivity("EditProduct",
                    string.Format(_localizationService.GetResource("ActivityLog.EditProduct"), product.Name), product);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = product.Id });
            }

            //prepare model
            model = _productModelFactory.PrepareProductModel(model, product, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = _productService.GetProductById(id);
            if (product == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List");

            _productService.DeleteProduct(product);

            //activity log
            _customerActivityService.InsertActivity("DeleteProduct",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteProduct"), product.Name), product);

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

                var newProduct = _copyProductService.CopyProduct(originalProduct, copyModel.Name, copyModel.Published, copyModel.CopyImages);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.Copied"));

                return RedirectToAction("Edit", new { id = newProduct.Id });
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = copyModel.Id });
            }
        }

        //action displaying notification (warning) to a store owner that entered SKU already exists
        public virtual IActionResult SkuReservedWarning(int productId, string sku)
        {
            string message;

            //check whether product with passed SKU already exists
            var productBySku = _productService.GetProductBySku(sku);
            if (productBySku != null)
            {
                if (productBySku.Id == productId)
                    return Json(new { Result = string.Empty });

                message = string.Format(_localizationService.GetResource("Admin.Catalog.Products.Fields.Sku.Reserved"), productBySku.Name);
                return Json(new { Result = message });
            }

            //check whether combination with passed SKU already exists
            var combinationBySku = _productAttributeService.GetProductAttributeCombinationBySku(sku);
            if (combinationBySku == null)
                return Json(new { Result = string.Empty });

            message = string.Format(_localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.Sku.Reserved"), combinationBySku.Product.Name);
            return Json(new { Result = message });
        }

        #endregion

        #region Required products

        [HttpPost]
        public virtual IActionResult LoadProductFriendlyNames(string productIds)
        {
            var result = string.Empty;

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return Json(new { Text = result });

            if (string.IsNullOrWhiteSpace(productIds))
                return Json(new { Text = result });

            var ids = new List<int>();
            var rangeArray = productIds
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

            foreach (var str1 in rangeArray)
            {
                if (int.TryParse(str1, out var tmp1))
                    ids.Add(tmp1);
            }

            var products = _productService.GetProductsByIds(ids.ToArray());
            for (var i = 0; i <= products.Count - 1; i++)
            {
                result += products[i].Name;
                if (i != products.Count - 1)
                    result += ", ";
            }

            return Json(new { Text = result });
        }

        public virtual IActionResult RequiredProductAddPopup()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model = _productModelFactory.PrepareAddRequiredProductSearchModel(new AddRequiredProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult RequiredProductAddPopupList(AddRequiredProductSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _productModelFactory.PrepareAddRequiredProductListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Related products

        [HttpPost]
        public virtual IActionResult RelatedProductList(RelatedProductSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //try to get a product with the specified id
            var product = _productService.GetProductById(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            //prepare model
            var model = _productModelFactory.PrepareRelatedProductListModel(searchModel, product);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult RelatedProductUpdate(RelatedProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a related product with the specified id
            var relatedProduct = _productService.GetRelatedProductById(model.Id)
                ?? throw new ArgumentException("No related product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                var product = _productService.GetProductById(relatedProduct.ProductId1);
                if (product != null && product.VendorId != _workContext.CurrentVendor.Id)
                    return Content("This is not your product");
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

            //try to get a related product with the specified id
            var relatedProduct = _productService.GetRelatedProductById(id)
                ?? throw new ArgumentException("No related product found with the specified id");

            var productId = relatedProduct.ProductId1;

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                var product = _productService.GetProductById(productId);
                if (product != null && product.VendorId != _workContext.CurrentVendor.Id)
                    return Content("This is not your product");
            }

            _productService.DeleteRelatedProduct(relatedProduct);

            return new NullJsonResult();
        }

        public virtual IActionResult RelatedProductAddPopup(int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model = _productModelFactory.PrepareAddRelatedProductSearchModel(new AddRelatedProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult RelatedProductAddPopupList(AddRelatedProductSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _productModelFactory.PrepareAddRelatedProductListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult RelatedProductAddPopup(AddRelatedProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var selectedProducts = _productService.GetProductsByIds(model.SelectedProductIds.ToArray());
            if (selectedProducts.Any())
            {
                var existingRelatedProducts = _productService.GetRelatedProductsByProductId1(model.ProductId);
                foreach (var product in selectedProducts)
                {
                    //a vendor should have access only to his products
                    if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                        continue;

                    if (_productService.FindRelatedProduct(existingRelatedProducts, model.ProductId, product.Id) != null)
                        continue;

                    _productService.InsertRelatedProduct(new RelatedProduct
                    {
                        ProductId1 = model.ProductId,
                        ProductId2 = product.Id,
                        DisplayOrder = 1
                    });
                }
            }

            ViewBag.RefreshPage = true;

            return View(new AddRelatedProductSearchModel());
        }

        #endregion

        #region Cross-sell products

        [HttpPost]
        public virtual IActionResult CrossSellProductList(CrossSellProductSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //try to get a product with the specified id
            var product = _productService.GetProductById(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            //prepare model
            var model = _productModelFactory.PrepareCrossSellProductListModel(searchModel, product);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult CrossSellProductDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a cross-sell product with the specified id
            var crossSellProduct = _productService.GetCrossSellProductById(id)
                ?? throw new ArgumentException("No cross-sell product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                var product = _productService.GetProductById(crossSellProduct.ProductId1);
                if (product != null && product.VendorId != _workContext.CurrentVendor.Id)
                    return Content("This is not your product");
            }

            _productService.DeleteCrossSellProduct(crossSellProduct);

            return new NullJsonResult();
        }

        public virtual IActionResult CrossSellProductAddPopup(int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model = _productModelFactory.PrepareAddCrossSellProductSearchModel(new AddCrossSellProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult CrossSellProductAddPopupList(AddCrossSellProductSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _productModelFactory.PrepareAddCrossSellProductListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult CrossSellProductAddPopup(AddCrossSellProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var selectedProducts = _productService.GetProductsByIds(model.SelectedProductIds.ToArray());
            if (selectedProducts.Any())
            {
                var existingCrossSellProducts = _productService.GetCrossSellProductsByProductId1(model.ProductId);
                foreach (var product in selectedProducts)
                {
                    //a vendor should have access only to his products
                    if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                        continue;

                    if (_productService.FindCrossSellProduct(existingCrossSellProducts, model.ProductId, product.Id) != null)
                        continue;

                    _productService.InsertCrossSellProduct(new CrossSellProduct
                    {
                        ProductId1 = model.ProductId,
                        ProductId2 = product.Id
                    });
                }
            }

            ViewBag.RefreshPage = true;

            return View(new AddCrossSellProductSearchModel());
        }

        #endregion

        #region Associated products

        [HttpPost]
        public virtual IActionResult AssociatedProductList(AssociatedProductSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //try to get a product with the specified id
            var product = _productService.GetProductById(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            //prepare model
            var model = _productModelFactory.PrepareAssociatedProductListModel(searchModel, product);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult AssociatedProductUpdate(AssociatedProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get an associated product with the specified id
            var associatedProduct = _productService.GetProductById(model.Id)
                ?? throw new ArgumentException("No associated product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && associatedProduct.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            associatedProduct.DisplayOrder = model.DisplayOrder;
            _productService.UpdateProduct(associatedProduct);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult AssociatedProductDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get an associated product with the specified id
            var product = _productService.GetProductById(id)
                ?? throw new ArgumentException("No associated product found with the specified id");

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

            //prepare model
            var model = _productModelFactory.PrepareAddAssociatedProductSearchModel(new AddAssociatedProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult AssociatedProductAddPopupList(AddAssociatedProductSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _productModelFactory.PrepareAddAssociatedProductListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult AssociatedProductAddPopup(AddAssociatedProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var selectedProducts = _productService.GetProductsByIds(model.SelectedProductIds.ToArray());
            if (selectedProducts.Any())
            {
                foreach (var product in selectedProducts)
                {
                    //a vendor should have access only to his products
                    if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                        continue;

                    product.ParentGroupedProductId = model.ProductId;
                    _productService.UpdateProduct(product);
                }
            }

            ViewBag.RefreshPage = true;

            return View(new AddAssociatedProductSearchModel());
        }

        #endregion

        #region Product pictures

        public virtual IActionResult ProductPictureAdd(int pictureId, int displayOrder,
            string overrideAltAttribute, string overrideTitleAttribute, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (pictureId == 0)
                throw new ArgumentException();

            //try to get a product with the specified id
            var product = _productService.GetProductById(productId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List");

            if (_productService.GetProductPicturesByProductId(productId).Any(p => p.PictureId == pictureId))
                return Json(new { Result = false });

            //try to get a picture with the specified id
            var picture = _pictureService.GetPictureById(pictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

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
                DisplayOrder = displayOrder
            });

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult ProductPictureList(ProductPictureSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //try to get a product with the specified id
            var product = _productService.GetProductById(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            //prepare model
            var model = _productModelFactory.PrepareProductPictureListModel(searchModel, product);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult ProductPictureUpdate(ProductPictureModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product picture with the specified id
            var productPicture = _productService.GetProductPictureById(model.Id)
                ?? throw new ArgumentException("No product picture found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                var product = _productService.GetProductById(productPicture.ProductId);
                if (product != null && product.VendorId != _workContext.CurrentVendor.Id)
                    return Content("This is not your product");
            }

            //try to get a picture with the specified id
            var picture = _pictureService.GetPictureById(productPicture.PictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

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

            //try to get a product picture with the specified id
            var productPicture = _productService.GetProductPictureById(id)
                ?? throw new ArgumentException("No product picture found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                var product = _productService.GetProductById(productPicture.ProductId);
                if (product != null && product.VendorId != _workContext.CurrentVendor.Id)
                    return Content("This is not your product");
            }

            var pictureId = productPicture.PictureId;
            _productService.DeleteProductPicture(productPicture);

            //try to get a picture with the specified id
            var picture = _pictureService.GetPictureById(pictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            _pictureService.DeletePicture(picture);

            return new NullJsonResult();
        }

        #endregion

        #region Product specification attributes

        public virtual IActionResult ProductSpecificationAttributeAdd(int attributeTypeId, int specificationAttributeOptionId,
            string customValue, bool allowFiltering, bool showOnProductPage, int displayOrder, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                var product = _productService.GetProductById(productId);
                if (product != null && product.VendorId != _workContext.CurrentVendor.Id)
                    return RedirectToAction("List");
            }

            //we allow filtering only for "Option" attribute type
            if (attributeTypeId != (int)SpecificationAttributeType.Option)
                allowFiltering = false;

            //we don't allow CustomValue for "Option" attribute type
            if (attributeTypeId == (int)SpecificationAttributeType.Option)
                customValue = null;

            var psa = new ProductSpecificationAttribute
            {
                AttributeTypeId = attributeTypeId,
                SpecificationAttributeOptionId = specificationAttributeOptionId,
                ProductId = productId,
                CustomValue = customValue,
                AllowFiltering = allowFiltering,
                ShowOnProductPage = showOnProductPage,
                DisplayOrder = displayOrder
            };
            _specificationAttributeService.InsertProductSpecificationAttribute(psa);

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult ProductSpecAttrList(ProductSpecificationAttributeSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //try to get a product with the specified id
            var product = _productService.GetProductById(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            //prepare model
            var model = _productModelFactory.PrepareProductSpecificationAttributeListModel(searchModel, product);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult ProductSpecAttrUpdate(ProductSpecificationAttributeModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product specification attribute with the specified id
            var psa = _specificationAttributeService.GetProductSpecificationAttributeById(model.Id);
            if (psa == null)
                return Content("No product specification attribute found with the specified id");

            var productId = psa.Product.Id;

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                var product = _productService.GetProductById(productId);
                if (product != null && product.VendorId != _workContext.CurrentVendor.Id)
                    return Content("This is not your product");
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

            //try to get a product specification attribute with the specified id
            var psa = _specificationAttributeService.GetProductSpecificationAttributeById(id)
                ?? throw new ArgumentException("No specification attribute found with the specified id");

            var productId = psa.ProductId;

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                var product = _productService.GetProductById(productId);
                if (product != null && product.VendorId != _workContext.CurrentVendor.Id)
                    return Content("This is not your product");
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

            //prepare model
            var model = _productModelFactory.PrepareProductTagSearchModel(new ProductTagSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ProductTags(ProductTagSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _productModelFactory.PrepareProductTagListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult ProductTagDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedView();

            //try to get a product tag with the specified id
            var tag = _productTagService.GetProductTagById(id)
                ?? throw new ArgumentException("No product tag found with the specified id");

            _productTagService.DeleteProductTag(tag);

            return new NullJsonResult();
        }

        public virtual IActionResult EditProductTag(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedView();

            //try to get a product tag with the specified id
            var productTag = _productTagService.GetProductTagById(id);
            if (productTag == null)
                return RedirectToAction("List");

            //prepare tag model
            var model = _productModelFactory.PrepareProductTagModel(null, productTag);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult EditProductTag(ProductTagModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedView();

            //try to get a product tag with the specified id
            var productTag = _productTagService.GetProductTagById(model.Id);
            if (productTag == null)
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

            //prepare model
            model = _productModelFactory.PrepareProductTagModel(model, productTag, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Purchased with order

        [HttpPost]
        public virtual IActionResult PurchasedWithOrders(ProductOrderSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //try to get a product with the specified id
            var product = _productService.GetProductById(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            //prepare model
            var model = _productModelFactory.PrepareProductOrderListModel(searchModel, product);

            return Json(model);
        }

        #endregion

        #region Export / Import

        [HttpPost, ActionName("List")]
        [FormValueRequired("download-catalog-pdf")]
        public virtual IActionResult DownloadCatalogAsPdf(ProductSearchModel model)
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
                categoryIds.AddRange(_categoryService.GetChildCategoryIds(parentCategoryId: model.SearchCategoryId, showHidden: true));

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
                overridePublished: overridePublished);

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
        public virtual IActionResult ExportXmlAll(ProductSearchModel model)
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
                categoryIds.AddRange(_categoryService.GetChildCategoryIds(parentCategoryId: model.SearchCategoryId, showHidden: true));

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
                overridePublished: overridePublished);

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
        public virtual IActionResult ExportExcelAll(ProductSearchModel model)
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
                categoryIds.AddRange(_categoryService.GetChildCategoryIds(parentCategoryId: model.SearchCategoryId, showHidden: true));

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
                overridePublished: overridePublished);

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

        #region Bulk editing

        public virtual IActionResult BulkEdit()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model = _productModelFactory.PrepareBulkEditProductSearchModel(new BulkEditProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult BulkEditSelect(BulkEditProductSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _productModelFactory.PrepareBulkEditProductListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult BulkEditUpdate(IEnumerable<BulkEditProductModel> products)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (products == null)
                return new NullJsonResult();

            foreach (var pModel in products)
            {
                //update
                var product = _productService.GetProductById(pModel.Id);
                if (product == null)
                    continue;

                //a vendor should have access only to his products
                if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                    continue;

                var prevTotalStockQuantity = _productService.GetTotalStockQuantity(product);
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
                    _productService.GetTotalStockQuantity(product) > 0 &&
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

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult BulkEditDelete(IEnumerable<BulkEditProductModel> products)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (products == null)
                return new NullJsonResult();

            foreach (var pModel in products)
            {
                //delete
                var product = _productService.GetProductById(pModel.Id);
                if (product == null)
                    continue;

                //a vendor should have access only to his products
                if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                    continue;

                _productService.DeleteProduct(product);
            }

            return new NullJsonResult();
        }

        #endregion

        #region Tier prices

        [HttpPost]
        public virtual IActionResult TierPriceList(TierPriceSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //try to get a product with the specified id
            var product = _productService.GetProductById(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            //prepare model
            var model = _productModelFactory.PrepareTierPriceListModel(searchModel, product);

            return Json(model);
        }

        public virtual IActionResult TierPriceCreatePopup(int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = _productService.GetProductById(productId)
                ?? throw new ArgumentException("No product found with the specified id");

            //prepare model
            var model = _productModelFactory.PrepareTierPriceModel(new TierPriceModel(), product, null);

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult TierPriceCreatePopup(TierPriceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = _productService.GetProductById(model.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

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

            //prepare model
            model = _productModelFactory.PrepareTierPriceModel(model, product, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult TierPriceEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a tier price with the specified id
            var tierPrice = _productService.GetTierPriceById(id);
            if (tierPrice == null)
                return RedirectToAction("List", "Product");

            //try to get a product with the specified id
            var product = _productService.GetProductById(tierPrice.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");

            //prepare model
            var model = _productModelFactory.PrepareTierPriceModel(null, product, tierPrice);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult TierPriceEditPopup(TierPriceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a tier price with the specified id
            var tierPrice = _productService.GetTierPriceById(model.Id);
            if (tierPrice == null)
                return RedirectToAction("List", "Product");

            //try to get a product with the specified id
            var product = _productService.GetProductById(tierPrice.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

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

            //prepare model
            model = _productModelFactory.PrepareTierPriceModel(model, product, tierPrice, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult TierPriceDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a tier price with the specified id
            var tierPrice = _productService.GetTierPriceById(id)
                ?? throw new ArgumentException("No tier price found with the specified id");

            //try to get a product with the specified id
            var product = _productService.GetProductById(tierPrice.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

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
        public virtual IActionResult ProductAttributeMappingList(ProductAttributeMappingSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //try to get a product with the specified id
            var product = _productService.GetProductById(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            //prepare model
            var model = _productModelFactory.PrepareProductAttributeMappingListModel(searchModel, product);

            return Json(model);
        }

        public virtual IActionResult ProductAttributeMappingCreate(int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = _productService.GetProductById(productId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
            {
                ErrorNotification(_localizationService.GetResource("This is not your product"));
                return RedirectToAction("List");
            }

            //prepare model
            var model = _productModelFactory.PrepareProductAttributeMappingModel(new ProductAttributeMappingModel(), product, null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult ProductAttributeMappingCreate(ProductAttributeMappingModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = _productService.GetProductById(model.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

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

                model = _productModelFactory.PrepareProductAttributeMappingModel(model, product, null, true);

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
                    PriceAdjustmentUsePercentage = predefinedValue.PriceAdjustmentUsePercentage,
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
                    var name = _localizationService.GetLocalized(predefinedValue, x => x.Name, lang.Id, false, false);
                    if (!string.IsNullOrEmpty(name))
                        _localizedEntityService.SaveLocalizedValue(pav, x => x.Name, name, lang.Id);
                }
            }

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.Added"));

            if (!continueEditing)
            {
                SaveSelectedTabName("tab-product-attributes");

                return RedirectToAction("Edit", new { id = product.Id });
            }

            //selected tab
            SaveSelectedTabName();

            return RedirectToAction("ProductAttributeMappingEdit", new { id = productAttributeMapping.Id });
        }

        public virtual IActionResult ProductAttributeMappingEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = _productAttributeService.GetProductAttributeMappingById(id)
                ?? throw new ArgumentException("No product attribute mapping found with the specified id");

            //try to get a product with the specified id
            var product = _productService.GetProductById(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
            {
                ErrorNotification(_localizationService.GetResource("This is not your product"));
                return RedirectToAction("List");
            }

            //prepare model
            var model = _productModelFactory.PrepareProductAttributeMappingModel(null, product, productAttributeMapping);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult ProductAttributeMappingEdit(ProductAttributeMappingModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = _productAttributeService.GetProductAttributeMappingById(model.Id)
                ?? throw new ArgumentException("No product attribute mapping found with the specified id");

            //try to get a product with the specified id
            var product = _productService.GetProductById(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

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

                model = _productModelFactory.PrepareProductAttributeMappingModel(model, product, productAttributeMapping, true);

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

            if (!continueEditing)
            {
                SaveSelectedTabName("tab-product-attributes");

                return RedirectToAction("Edit", new { id = product.Id });
            }

            //selected tab
            SaveSelectedTabName();

            return RedirectToAction("ProductAttributeMappingEdit", new { id = productAttributeMapping.Id });
        }

        [HttpPost]
        public virtual IActionResult ProductAttributeMappingDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = _productAttributeService.GetProductAttributeMappingById(id)
                ?? throw new ArgumentException("No product attribute mapping found with the specified id");

            //try to get a product with the specified id
            var product = _productService.GetProductById(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            _productAttributeService.DeleteProductAttributeMapping(productAttributeMapping);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.Deleted"));

            SaveSelectedTabName("tab-product-attributes");

            return RedirectToAction("Edit", new { id = productAttributeMapping.ProductId });
        }

        [HttpPost]
        public virtual IActionResult ProductAttributeValueList(ProductAttributeValueSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = _productAttributeService.GetProductAttributeMappingById(searchModel.ProductAttributeMappingId)
                ?? throw new ArgumentException("No product attribute mapping found with the specified id");

            //try to get a product with the specified id
            var product = _productService.GetProductById(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            //prepare model
            var model = _productModelFactory.PrepareProductAttributeValueListModel(searchModel, productAttributeMapping);

            return Json(model);
        }

        public virtual IActionResult ProductAttributeValueCreatePopup(int productAttributeMappingId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = _productAttributeService.GetProductAttributeMappingById(productAttributeMappingId)
                ?? throw new ArgumentException("No product attribute mapping found with the specified id");

            //try to get a product with the specified id
            var product = _productService.GetProductById(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");

            //prepare model
            var model = _productModelFactory.PrepareProductAttributeValueModel(new ProductAttributeValueModel(), productAttributeMapping, null);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ProductAttributeValueCreatePopup(ProductAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = _productAttributeService.GetProductAttributeMappingById(model.ProductAttributeMappingId);
            if (productAttributeMapping == null)
                return RedirectToAction("List", "Product");

            //try to get a product with the specified id
            var product = _productService.GetProductById(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");

            if (productAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares)
            {
                //ensure valid color is chosen/entered
                if (string.IsNullOrEmpty(model.ColorSquaresRgb))
                    ModelState.AddModelError(string.Empty, "Color is required");
                try
                {
                    //ensure color is valid (can be instantiated)
                    System.Drawing.ColorTranslator.FromHtml(model.ColorSquaresRgb);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.Message);
                }
            }

            //ensure a picture is uploaded
            if (productAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares && model.ImageSquaresPictureId == 0)
            {
                ModelState.AddModelError(string.Empty, "Image is required");
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
                    PriceAdjustmentUsePercentage = model.PriceAdjustmentUsePercentage,
                    WeightAdjustment = model.WeightAdjustment,
                    Cost = model.Cost,
                    CustomerEntersQty = model.CustomerEntersQty,
                    Quantity = model.CustomerEntersQty ? 1 : model.Quantity,
                    IsPreSelected = model.IsPreSelected,
                    DisplayOrder = model.DisplayOrder,
                    PictureId = model.PictureId
                };

                _productAttributeService.InsertProductAttributeValue(pav);
                UpdateLocales(pav, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = _productModelFactory.PrepareProductAttributeValueModel(model, productAttributeMapping, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult ProductAttributeValueEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute value with the specified id
            var productAttributeValue = _productAttributeService.GetProductAttributeValueById(id);
            if (productAttributeValue == null)
                return RedirectToAction("List", "Product");

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = _productAttributeService.GetProductAttributeMappingById(productAttributeValue.ProductAttributeMappingId);
            if (productAttributeMapping == null)
                return RedirectToAction("List", "Product");

            //try to get a product with the specified id
            var product = _productService.GetProductById(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");

            //prepare model
            var model = _productModelFactory.PrepareProductAttributeValueModel(null, productAttributeMapping, productAttributeValue);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ProductAttributeValueEditPopup(ProductAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute value with the specified id
            var productAttributeValue = _productAttributeService.GetProductAttributeValueById(model.Id);
            if (productAttributeValue == null)
                return RedirectToAction("List", "Product");

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = _productAttributeService.GetProductAttributeMappingById(productAttributeValue.ProductAttributeMappingId);
            if (productAttributeMapping == null)
                return RedirectToAction("List", "Product");

            //try to get a product with the specified id
            var product = _productService.GetProductById(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");

            if (productAttributeValue.ProductAttributeMapping.AttributeControlType == AttributeControlType.ColorSquares)
            {
                //ensure valid color is chosen/entered
                if (string.IsNullOrEmpty(model.ColorSquaresRgb))
                    ModelState.AddModelError(string.Empty, "Color is required");
                try
                {
                    //ensure color is valid (can be instantiated)
                    System.Drawing.ColorTranslator.FromHtml(model.ColorSquaresRgb);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(string.Empty, exc.Message);
                }
            }

            //ensure a picture is uploaded
            if (productAttributeValue.ProductAttributeMapping.AttributeControlType == AttributeControlType.ImageSquares && model.ImageSquaresPictureId == 0)
            {
                ModelState.AddModelError(string.Empty, "Image is required");
            }

            if (ModelState.IsValid)
            {
                productAttributeValue.AttributeValueTypeId = model.AttributeValueTypeId;
                productAttributeValue.AssociatedProductId = model.AssociatedProductId;
                productAttributeValue.Name = model.Name;
                productAttributeValue.ColorSquaresRgb = model.ColorSquaresRgb;
                productAttributeValue.ImageSquaresPictureId = model.ImageSquaresPictureId;
                productAttributeValue.PriceAdjustment = model.PriceAdjustment;
                productAttributeValue.PriceAdjustmentUsePercentage = model.PriceAdjustmentUsePercentage;
                productAttributeValue.WeightAdjustment = model.WeightAdjustment;
                productAttributeValue.Cost = model.Cost;
                productAttributeValue.CustomerEntersQty = model.CustomerEntersQty;
                productAttributeValue.Quantity = model.CustomerEntersQty ? 1 : model.Quantity;
                productAttributeValue.IsPreSelected = model.IsPreSelected;
                productAttributeValue.DisplayOrder = model.DisplayOrder;
                productAttributeValue.PictureId = model.PictureId;
                _productAttributeService.UpdateProductAttributeValue(productAttributeValue);

                UpdateLocales(productAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = _productModelFactory.PrepareProductAttributeValueModel(model, productAttributeMapping, productAttributeValue, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ProductAttributeValueDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute value with the specified id
            var productAttributeValue = _productAttributeService.GetProductAttributeValueById(id)
                ?? throw new ArgumentException("No product attribute value found with the specified id");

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = _productAttributeService.GetProductAttributeMappingById(productAttributeValue.ProductAttributeMappingId)
                ?? throw new ArgumentException("No product attribute mapping found with the specified id");

            //try to get a product with the specified id
            var product = _productService.GetProductById(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            _productAttributeService.DeleteProductAttributeValue(productAttributeValue);

            return new NullJsonResult();
        }

        public virtual IActionResult AssociateProductToAttributeValuePopup()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model = _productModelFactory.PrepareAssociateProductToAttributeValueSearchModel(new AssociateProductToAttributeValueSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult AssociateProductToAttributeValuePopupList(AssociateProductToAttributeValueSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _productModelFactory.PrepareAssociateProductToAttributeValueListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult AssociateProductToAttributeValuePopup([Bind(Prefix = nameof(AssociateProductToAttributeValueModel))] AssociateProductToAttributeValueModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var associatedProduct = _productService.GetProductById(model.AssociatedToProductId);
            if (associatedProduct == null)
                return Content("Cannot load a product");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && associatedProduct.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            ViewBag.RefreshPage = true;
            ViewBag.productId = associatedProduct.Id;
            ViewBag.productName = associatedProduct.Name;

            return View(new AssociateProductToAttributeValueSearchModel());
        }

        //action displaying notification (warning) to a store owner when associating some product
        public virtual IActionResult AssociatedProductGetWarnings(int productId)
        {
            var associatedProduct = _productService.GetProductById(productId);
            if (associatedProduct == null)
                return Json(new { Result = string.Empty });

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

            //downloadable product
            if (associatedProduct.IsDownload)
            {
                return Json(new { Result = _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.Downloadable") });
            }

            return Json(new { Result = string.Empty });
        }

        #endregion

        #region Product attribute combinations

        [HttpPost]
        public virtual IActionResult ProductAttributeCombinationList(ProductAttributeCombinationSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            //try to get a product with the specified id
            var product = _productService.GetProductById(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            //prepare model
            var model = _productModelFactory.PrepareProductAttributeCombinationListModel(searchModel, product);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult ProductAttributeCombinationDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a combination with the specified id
            var combination = _productAttributeService.GetProductAttributeCombinationById(id)
                ?? throw new ArgumentException("No product attribute combination found with the specified id");

            //try to get a product with the specified id
            var product = _productService.GetProductById(combination.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            _productAttributeService.DeleteProductAttributeCombination(combination);

            return new NullJsonResult();
        }

        public virtual IActionResult ProductAttributeCombinationCreatePopup(int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = _productService.GetProductById(productId);
            if (product == null)
                return RedirectToAction("List", "Product");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");

            //prepare model
            var model = _productModelFactory.PrepareProductAttributeCombinationModel(new ProductAttributeCombinationModel(), product, null);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ProductAttributeCombinationCreatePopup(int productId, ProductAttributeCombinationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = _productService.GetProductById(productId);
            if (product == null)
                return RedirectToAction("List", "Product");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");

            //attributes
            var warnings = new List<string>();
            var attributesXml = GetAttributesXmlForProductAttributeCombination(model.Form, warnings, product.Id);

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
                    PictureId = model.PictureId
                };
                _productAttributeService.InsertProductAttributeCombination(combination);

                //quantity change history
                _productService.AddStockQuantityHistoryEntry(product, combination.StockQuantity, combination.StockQuantity,
                    message: _localizationService.GetResource("Admin.StockQuantityHistory.Messages.Combination.Edit"), combinationId: combination.Id);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = _productModelFactory.PrepareProductAttributeCombinationModel(model, product, null, true);
            model.Warnings = warnings;

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult ProductAttributeCombinationGeneratePopup(int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = _productService.GetProductById(productId);
            if (product == null)
                return RedirectToAction("List", "Product");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");

            //prepare model
            var model = _productModelFactory.PrepareProductAttributeCombinationModel(new ProductAttributeCombinationModel(), product, null);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ProductAttributeCombinationGeneratePopup(IFormCollection form, ProductAttributeCombinationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = _productService.GetProductById(model.ProductId);
            if (product == null)
                return RedirectToAction("List", "Product");

            var allowedAttributeIds = form.Keys.Where(key => key.Contains("attribute_value_"))
                .Select(key => int.TryParse(form[key], out var id) ? id : 0).Where(id => id > 0).ToList();

            var requiredAttributeNames = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id)
                .Where(pam => pam.IsRequired)
                .Where(pam => !pam.IsNonCombinable())
                .Where(pam => !pam.ProductAttributeValues.Any(v => allowedAttributeIds.Any(id => id == v.Id)))
                .Select(pam => pam.ProductAttribute.Name).ToList();

            if (requiredAttributeNames.Any())
            {
                model = _productModelFactory.PrepareProductAttributeCombinationModel(model, product, null, true);
                model.ProductAttributes.SelectMany(pa => pa.Values)
                    .Where(v => allowedAttributeIds.Any(id => id == v.Id))
                    .ToList().ForEach(v => v.Checked = "checked");

                model.Warnings.Add(string.Format(_localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.SelectRequiredAttributes"), string.Join(", ", requiredAttributeNames)));

                return View(model);
            }

            GenerateAttributeCombinations(product, allowedAttributeIds);

            ViewBag.RefreshPage = true;

            return View(new ProductAttributeCombinationModel());
        }

        public virtual IActionResult ProductAttributeCombinationEditPopup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a combination with the specified id
            var combination = _productAttributeService.GetProductAttributeCombinationById(id);
            if (combination == null)
                return RedirectToAction("List", "Product");

            //try to get a product with the specified id
            var product = _productService.GetProductById(combination.ProductId);
            if (product == null)
                return RedirectToAction("List", "Product");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");

            //prepare model
            var model = _productModelFactory.PrepareProductAttributeCombinationModel(null, product, combination);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ProductAttributeCombinationEditPopup(ProductAttributeCombinationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a combination with the specified id
            var combination = _productAttributeService.GetProductAttributeCombinationById(model.Id);
            if (combination == null)
                return RedirectToAction("List", "Product");

            //try to get a product with the specified id
            var product = _productService.GetProductById(combination.ProductId);
            if (product == null)
                return RedirectToAction("List", "Product");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return RedirectToAction("List", "Product");

            if (ModelState.IsValid)
            {
                var previousStockQuantity = combination.StockQuantity;

                //save combination
                combination.AllowOutOfStockOrders = model.AllowOutOfStockOrders;
                combination.Gtin = model.Gtin;
                combination.ManufacturerPartNumber = model.ManufacturerPartNumber;
                combination.NotifyAdminForQuantityBelow = model.NotifyAdminForQuantityBelow;
                combination.OverriddenPrice = model.OverriddenPrice;
                combination.PictureId = model.PictureId;
                combination.Sku = model.Sku;
                combination.StockQuantity = model.StockQuantity;
                _productAttributeService.UpdateProductAttributeCombination(combination);

                //quantity change history
                _productService.AddStockQuantityHistoryEntry(product, combination.StockQuantity - previousStockQuantity, combination.StockQuantity,
                    message: _localizationService.GetResource("Admin.StockQuantityHistory.Messages.Combination.Edit"), combinationId: combination.Id);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = _productModelFactory.PrepareProductAttributeCombinationModel(model, product, combination, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult GenerateAllAttributeCombinations(int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = _productService.GetProductById(productId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            GenerateAttributeCombinations(product);

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
            productEditorSettings = model.ProductEditorSettingsModel.ToSettings(productEditorSettings);
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
        public virtual IActionResult StockQuantityHistory(StockQuantityHistorySearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();

            var product = _productService.GetProductById(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && product.VendorId != _workContext.CurrentVendor.Id)
                return Content("This is not your product");

            //prepare model
            var model = _productModelFactory.PrepareStockQuantityHistoryListModel(searchModel, product);

            return Json(model);
        }

        #endregion

        #endregion
    }
}