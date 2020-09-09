using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
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
        private readonly INotificationService _notificationService;
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
        private readonly IStoreContext _storeContext;
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
            INotificationService notificationService,
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
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            VendorSettings vendorSettings)
        {
            _aclService = aclService;
            _backInStockSubscriptionService = backInStockSubscriptionService;
            _categoryService = categoryService;
            _copyProductService = copyProductService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _discountService = discountService;
            _downloadService = downloadService;
            _exportManager = exportManager;
            _importManager = importManager;
            _languageService = languageService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _manufacturerService = manufacturerService;
            _fileProvider = fileProvider;
            _notificationService = notificationService;
            _pdfService = pdfService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _productAttributeParser = productAttributeParser;
            _productAttributeService = productAttributeService;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _productTagService = productTagService;
            _settingService = settingService;
            _shippingService = shippingService;
            _shoppingCartService = shoppingCartService;
            _specificationAttributeService = specificationAttributeService;
            _storeContext = storeContext;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
            _vendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocales(Product product, ProductModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValue(product,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
                await _localizedEntityService.SaveLocalizedValue(product,
                    x => x.ShortDescription,
                    localized.ShortDescription,
                    localized.LanguageId);
                await _localizedEntityService.SaveLocalizedValue(product,
                    x => x.FullDescription,
                    localized.FullDescription,
                    localized.LanguageId);
                await _localizedEntityService.SaveLocalizedValue(product,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId);
                await _localizedEntityService.SaveLocalizedValue(product,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId);
                await _localizedEntityService.SaveLocalizedValue(product,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId);

                //search engine name
                var seName = await _urlRecordService.ValidateSeName(product, localized.SeName, localized.Name, false);
                await _urlRecordService.SaveSlug(product, seName, localized.LanguageId);
            }
        }

        protected virtual async Task UpdateLocales(ProductTag productTag, ProductTagModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValue(productTag,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                var seName = await _urlRecordService.ValidateSeName(productTag, string.Empty, localized.Name, false);
                await _urlRecordService.SaveSlug(productTag, seName, localized.LanguageId);
            }
        }

        protected virtual async Task UpdateLocales(ProductAttributeMapping pam, ProductAttributeMappingModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValue(pam,
                    x => x.TextPrompt,
                    localized.TextPrompt,
                    localized.LanguageId);
                await _localizedEntityService.SaveLocalizedValue(pam,
                    x => x.DefaultValue,
                    localized.DefaultValue,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateLocales(ProductAttributeValue pav, ProductAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValue(pav,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdatePictureSeoNames(Product product)
        {
            foreach (var pp in await _productService.GetProductPicturesByProductId(product.Id))
                await _pictureService.SetSeoFilename(pp.PictureId, await _pictureService.GetPictureSeName(product.Name));
        }

        protected virtual async Task SaveProductAcl(Product product, ProductModel model)
        {
            product.SubjectToAcl = model.SelectedCustomerRoleIds.Any();
            await _productService.UpdateProduct(product);

            var existingAclRecords = await _aclService.GetAclRecords(product);
            var allCustomerRoles = await _customerService.GetAllCustomerRoles(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        await _aclService.InsertAclRecord(product, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        await _aclService.DeleteAclRecord(aclRecordToDelete);
                }
            }
        }

        protected virtual async Task SaveCategoryMappings(Product product, ProductModel model)
        {
            var existingProductCategories = await _categoryService.GetProductCategoriesByProductId(product.Id, true);

            //delete categories
            foreach (var existingProductCategory in existingProductCategories)
                if (!model.SelectedCategoryIds.Contains(existingProductCategory.CategoryId))
                    await _categoryService.DeleteProductCategory(existingProductCategory);

            //add categories
            foreach (var categoryId in model.SelectedCategoryIds)
            {
                if (_categoryService.FindProductCategory(existingProductCategories, product.Id, categoryId) == null)
                {
                    //find next display order
                    var displayOrder = 1;
                    var existingCategoryMapping = await _categoryService.GetProductCategoriesByCategoryId(categoryId, showHidden: true);
                    if (existingCategoryMapping.Any())
                        displayOrder = existingCategoryMapping.Max(x => x.DisplayOrder) + 1;
                    await _categoryService.InsertProductCategory(new ProductCategory
                    {
                        ProductId = product.Id,
                        CategoryId = categoryId,
                        DisplayOrder = displayOrder
                    });
                }
            }
        }

        protected virtual async Task SaveManufacturerMappings(Product product, ProductModel model)
        {
            var existingProductManufacturers = await _manufacturerService.GetProductManufacturersByProductId(product.Id, true);

            //delete manufacturers
            foreach (var existingProductManufacturer in existingProductManufacturers)
                if (!model.SelectedManufacturerIds.Contains(existingProductManufacturer.ManufacturerId))
                    await _manufacturerService.DeleteProductManufacturer(existingProductManufacturer);

            //add manufacturers
            foreach (var manufacturerId in model.SelectedManufacturerIds)
            {
                if (_manufacturerService.FindProductManufacturer(existingProductManufacturers, product.Id, manufacturerId) == null)
                {
                    //find next display order
                    var displayOrder = 1;
                    var existingManufacturerMapping = await _manufacturerService.GetProductManufacturersByManufacturerId(manufacturerId, showHidden: true);
                    if (existingManufacturerMapping.Any())
                        displayOrder = existingManufacturerMapping.Max(x => x.DisplayOrder) + 1;
                    await _manufacturerService.InsertProductManufacturer(new ProductManufacturer
                    {
                        ProductId = product.Id,
                        ManufacturerId = manufacturerId,
                        DisplayOrder = displayOrder
                    });
                }
            }
        }

        protected virtual async Task SaveDiscountMappings(Product product, ProductModel model)
        {
            var allDiscounts = await _discountService.GetAllDiscounts(DiscountType.AssignedToSkus, showHidden: true);

            foreach (var discount in allDiscounts)
            {
                if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                {
                    //new discount
                    if (await _productService.GetDiscountAppliedToProduct(product.Id, discount.Id) is null)
                        await _productService.InsertDiscountProductMapping(new DiscountProductMapping { EntityId = product.Id, DiscountId = discount.Id });
                }
                else
                {
                    //remove discount
                    if (await _productService.GetDiscountAppliedToProduct(product.Id, discount.Id) is DiscountProductMapping discountProductMapping)
                        await _productService.DeleteDiscountProductMapping(discountProductMapping);
                }
            }

            await _productService.UpdateProduct(product);
            await _productService.UpdateHasDiscountsApplied(product);
        }

        protected virtual async Task<string> GetAttributesXmlForProductAttributeCombination(IFormCollection form, List<string> warnings, int productId)
        {
            var attributesXml = string.Empty;

            //get product attribute mappings (exclude non-combinable attributes)
            var attributes = (await _productAttributeService.GetProductAttributeMappingsByProductId(productId))
                .Where(productAttributeMapping => !productAttributeMapping.IsNonCombinable()).ToList();

            foreach (var attribute in attributes)
            {
                var controlId = $"{NopCatalogDefaults.ProductAttributePrefix}{attribute.Id}";
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
                        var attributeValues = await _productAttributeService.GetProductAttributeValues(attribute.Id);
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
                                        await _localizationService.GetResource("ShoppingCart.MaximumUploadedFileSize"),
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
                                    DownloadBinary = await _downloadService.GetDownloadBits(httpPostedFile),
                                    ContentType = httpPostedFile.ContentType,
                                    Filename = _fileProvider.GetFileNameWithoutExtension(httpPostedFile.FileName),
                                    Extension = _fileProvider.GetFileExtension(httpPostedFile.FileName),
                                    IsNew = true
                                };
                                await _downloadService.InsertDownload(download);

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
                var conditionMet = await _productAttributeParser.IsConditionMet(attribute, attributesXml);
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

        protected virtual async Task SaveProductWarehouseInventory(Product product, ProductModel model)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (model.ManageInventoryMethodId != (int)ManageInventoryMethod.ManageStock)
                return;

            if (!model.UseMultipleWarehouses)
                return;

            var warehouses = await _shippingService.GetAllWarehouses();

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
                var message = $"{await _localizationService.GetResource("Admin.StockQuantityHistory.Messages.MultipleWarehouses")} {await _localizationService.GetResource("Admin.StockQuantityHistory.Messages.Edit")}";

                var existingPwI = (await _productService.GetAllProductWarehouseInventoryRecords(product.Id)).FirstOrDefault(x => x.WarehouseId == warehouse.Id);
                if (existingPwI != null)
                {
                    if (used)
                    {
                        var previousStockQuantity = existingPwI.StockQuantity;

                        //update existing record
                        existingPwI.StockQuantity = stockQuantity;
                        existingPwI.ReservedQuantity = reservedQuantity;
                        await _productService.UpdateProductWarehouseInventory(existingPwI);

                        //quantity change history
                        await _productService.AddStockQuantityHistoryEntry(product, existingPwI.StockQuantity - previousStockQuantity, existingPwI.StockQuantity,
                            existingPwI.WarehouseId, message);
                    }
                    else
                    {
                        //delete. no need to store record for qty 0
                        await _productService.DeleteProductWarehouseInventory(existingPwI);

                        //quantity change history
                        await _productService.AddStockQuantityHistoryEntry(product, -existingPwI.StockQuantity, 0, existingPwI.WarehouseId, message);
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

                    await _productService.InsertProductWarehouseInventory(existingPwI);

                    //quantity change history
                    await _productService.AddStockQuantityHistoryEntry(product, existingPwI.StockQuantity, existingPwI.StockQuantity,
                        existingPwI.WarehouseId, message);
                }
            }
        }

        protected virtual async Task SaveConditionAttributes(ProductAttributeMapping productAttributeMapping,
            ProductAttributeConditionModel model, IFormCollection form)
        {
            string attributesXml = null;
            if (model.EnableCondition)
            {
                var attribute = await _productAttributeService.GetProductAttributeMappingById(model.SelectedProductAttributeId);
                if (attribute != null)
                {
                    var controlId = $"{NopCatalogDefaults.ProductAttributePrefix}{attribute.Id}";
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
            await _productAttributeService.UpdateProductAttributeMapping(productAttributeMapping);
        }

        protected virtual async Task GenerateAttributeCombinations(Product product, IList<int> allowedAttributeIds = null)
        {
            var allAttributesXml = await _productAttributeParser.GenerateAllCombinations(product, true, allowedAttributeIds);
            foreach (var attributesXml in allAttributesXml)
            {
                var existingCombination = _productAttributeParser.FindProductAttributeCombination(product, attributesXml);

                //already exists?
                if (existingCombination != null)
                    continue;

                //new one
                var warnings = new List<string>();
                warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarnings(await _workContext.GetCurrentCustomer(),
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
                await _productAttributeService.InsertProductAttributeCombination(combination);
            }
        }

        #endregion

        #region Methods

        #region Product list / create / edit / delete

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model = await _productModelFactory.PrepareProductSearchModel(new ProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductList(ProductSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _productModelFactory.PrepareProductListModel(searchModel);

            return Json(model);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-product-by-sku")]
        public virtual async Task<IActionResult> GoToSku(ProductSearchModel searchModel)
        {
            //try to load a product entity, if not found, then try to load a product attribute combination
            var productId = (await _productService.GetProductBySku(searchModel.GoDirectlyToSku))?.Id
                ?? (await _productAttributeService.GetProductAttributeCombinationBySku(searchModel.GoDirectlyToSku))?.ProductId;

            if (productId != null)
                return RedirectToAction("Edit", "Product", new { id = productId });

            //not found
            return await List();
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //validate maximum number of products per vendor
            if (_vendorSettings.MaximumProductNumber > 0 && await _workContext.GetCurrentVendor() != null
                && await _productService.GetNumberOfProductsByVendorId((await _workContext.GetCurrentVendor()).Id) >= _vendorSettings.MaximumProductNumber)
            {
                _notificationService.ErrorNotification(string.Format(await _localizationService.GetResource("Admin.Catalog.Products.ExceededMaximumNumber"),
                    _vendorSettings.MaximumProductNumber));
                return RedirectToAction("List");
            }

            //prepare model
            var model = await _productModelFactory.PrepareProductModel(new ProductModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(ProductModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //validate maximum number of products per vendor
            if (_vendorSettings.MaximumProductNumber > 0 && await _workContext.GetCurrentVendor() != null
                && await _productService.GetNumberOfProductsByVendorId((await _workContext.GetCurrentVendor()).Id) >= _vendorSettings.MaximumProductNumber)
            {
                _notificationService.ErrorNotification(string.Format(await _localizationService.GetResource("Admin.Catalog.Products.ExceededMaximumNumber"),
                    _vendorSettings.MaximumProductNumber));
                return RedirectToAction("List");
            }

            if (ModelState.IsValid)
            {
                //a vendor should have access only to his products
                if (await _workContext.GetCurrentVendor() != null)
                    model.VendorId = (await _workContext.GetCurrentVendor()).Id;

                //vendors cannot edit "Show on home page" property
                if (await _workContext.GetCurrentVendor() != null && model.ShowOnHomepage)
                    model.ShowOnHomepage = false;

                //product
                var product = model.ToEntity<Product>();
                product.CreatedOnUtc = DateTime.UtcNow;
                product.UpdatedOnUtc = DateTime.UtcNow;
                await _productService.InsertProduct(product);

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeName(product, model.SeName, product.Name, true);
                await _urlRecordService.SaveSlug(product, model.SeName, 0);

                //locales
                await UpdateLocales(product, model);

                //categories
                await SaveCategoryMappings(product, model);

                //manufacturers
                await SaveManufacturerMappings(product, model);

                //ACL (customer roles)
                await SaveProductAcl(product, model);

                //stores
                await _productService.UpdateProductStoreMappings(product, model.SelectedStoreIds);

                //discounts
                await SaveDiscountMappings(product, model);

                //tags
                await _productTagService.UpdateProductTags(product, ParseProductTags(model.ProductTags));

                //warehouses
                await SaveProductWarehouseInventory(product, model);

                //quantity change history
                await _productService.AddStockQuantityHistoryEntry(product, product.StockQuantity, product.StockQuantity, product.WarehouseId,
                    await _localizationService.GetResource("Admin.StockQuantityHistory.Messages.Edit"));

                //activity log
                await _customerActivityService.InsertActivity("AddNewProduct",
                    string.Format(await _localizationService.GetResource("ActivityLog.AddNewProduct"), product.Name), product);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Catalog.Products.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = product.Id });
            }

            //prepare model
            model = await _productModelFactory.PrepareProductModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await _productService.GetProductById(id);
            if (product == null || product.Deleted)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return RedirectToAction("List");

            //prepare model
            var model = await _productModelFactory.PrepareProductModel(null, product);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(ProductModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await _productService.GetProductById(model.Id);
            if (product == null || product.Deleted)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return RedirectToAction("List");

            //check if the product quantity has been changed while we were editing the product
            //and if it has been changed then we show error notification
            //and redirect on the editing page without data saving
            if (product.StockQuantity != model.LastStockQuantity)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResource("Admin.Catalog.Products.Fields.StockQuantity.ChangedWarning"));
                return RedirectToAction("Edit", new { id = product.Id });
            }

            if (ModelState.IsValid)
            {
                //a vendor should have access only to his products
                if (await _workContext.GetCurrentVendor() != null)
                    model.VendorId = (await _workContext.GetCurrentVendor()).Id;

                //we do not validate maximum number of products per vendor when editing existing products (only during creation of new products)
                //vendors cannot edit "Show on home page" property
                if (await _workContext.GetCurrentVendor() != null && model.ShowOnHomepage != product.ShowOnHomepage)
                    model.ShowOnHomepage = product.ShowOnHomepage;

                //some previously used values
                var prevTotalStockQuantity = await _productService.GetTotalStockQuantity(product);
                var prevDownloadId = product.DownloadId;
                var prevSampleDownloadId = product.SampleDownloadId;
                var previousStockQuantity = product.StockQuantity;
                var previousWarehouseId = product.WarehouseId;
                var previousProductType = product.ProductType;

                //product
                product = model.ToEntity(product);

                product.UpdatedOnUtc = DateTime.UtcNow;
                await _productService.UpdateProduct(product);

                //remove associated products
                if (previousProductType == ProductType.GroupedProduct && product.ProductType == ProductType.SimpleProduct)
                {
                    var storeId = (await _storeContext.GetCurrentStore())?.Id ?? 0;
                    var vendorId = (await _workContext.GetCurrentVendor())?.Id ?? 0;

                    var associatedProducts = await _productService.GetAssociatedProducts(product.Id, storeId, vendorId);
                    foreach (var associatedProduct in associatedProducts)
                    {
                        associatedProduct.ParentGroupedProductId = 0;
                        await _productService.UpdateProduct(associatedProduct);
                    }
                }

                //search engine name
                model.SeName = await _urlRecordService.ValidateSeName(product, model.SeName, product.Name, true);
                await _urlRecordService.SaveSlug(product, model.SeName, 0);

                //locales
                await UpdateLocales(product, model);

                //tags
                await _productTagService.UpdateProductTags(product, ParseProductTags(model.ProductTags));

                //warehouses
                await SaveProductWarehouseInventory(product, model);

                //categories
                await SaveCategoryMappings(product, model);

                //manufacturers
                await SaveManufacturerMappings(product, model);

                //ACL (customer roles)
                await SaveProductAcl(product, model);

                //stores
                await _productService.UpdateProductStoreMappings(product, model.SelectedStoreIds);

                //discounts
                await SaveDiscountMappings(product, model);

                //picture seo names
                await UpdatePictureSeoNames(product);

                //back in stock notifications
                if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                    product.BackorderMode == BackorderMode.NoBackorders &&
                    product.AllowBackInStockSubscriptions &&
                    await _productService.GetTotalStockQuantity(product) > 0 &&
                    prevTotalStockQuantity <= 0 &&
                    product.Published &&
                    !product.Deleted)
                {
                    await _backInStockSubscriptionService.SendNotificationsToSubscribers(product);
                }

                //delete an old "download" file (if deleted or updated)
                if (prevDownloadId > 0 && prevDownloadId != product.DownloadId)
                {
                    var prevDownload = await _downloadService.GetDownloadById(prevDownloadId);
                    if (prevDownload != null)
                        await _downloadService.DeleteDownload(prevDownload);
                }

                //delete an old "sample download" file (if deleted or updated)
                if (prevSampleDownloadId > 0 && prevSampleDownloadId != product.SampleDownloadId)
                {
                    var prevSampleDownload = await _downloadService.GetDownloadById(prevSampleDownloadId);
                    if (prevSampleDownload != null)
                        await _downloadService.DeleteDownload(prevSampleDownload);
                }

                //quantity change history
                if (previousWarehouseId != product.WarehouseId)
                {
                    //warehouse is changed 
                    //compose a message
                    var oldWarehouseMessage = string.Empty;
                    if (previousWarehouseId > 0)
                    {
                        var oldWarehouse = await _shippingService.GetWarehouseById(previousWarehouseId);
                        if (oldWarehouse != null)
                            oldWarehouseMessage = string.Format(await _localizationService.GetResource("Admin.StockQuantityHistory.Messages.EditWarehouse.Old"), oldWarehouse.Name);
                    }

                    var newWarehouseMessage = string.Empty;
                    if (product.WarehouseId > 0)
                    {
                        var newWarehouse = await _shippingService.GetWarehouseById(product.WarehouseId);
                        if (newWarehouse != null)
                            newWarehouseMessage = string.Format(await _localizationService.GetResource("Admin.StockQuantityHistory.Messages.EditWarehouse.New"), newWarehouse.Name);
                    }

                    var message = string.Format(await _localizationService.GetResource("Admin.StockQuantityHistory.Messages.EditWarehouse"), oldWarehouseMessage, newWarehouseMessage);

                    //record history
                    await _productService.AddStockQuantityHistoryEntry(product, -previousStockQuantity, 0, previousWarehouseId, message);
                    await _productService.AddStockQuantityHistoryEntry(product, product.StockQuantity, product.StockQuantity, product.WarehouseId, message);
                }
                else
                {
                    await _productService.AddStockQuantityHistoryEntry(product, product.StockQuantity - previousStockQuantity, product.StockQuantity,
                        product.WarehouseId, await _localizationService.GetResource("Admin.StockQuantityHistory.Messages.Edit"));
                }

                //activity log
                await _customerActivityService.InsertActivity("EditProduct",
                    string.Format(await _localizationService.GetResource("ActivityLog.EditProduct"), product.Name), product);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Catalog.Products.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = product.Id });
            }

            //prepare model
            model = await _productModelFactory.PrepareProductModel(model, product, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await _productService.GetProductById(id);
            if (product == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return RedirectToAction("List");

            await _productService.DeleteProduct(product);

            //activity log
            await _customerActivityService.InsertActivity("DeleteProduct",
                string.Format(await _localizationService.GetResource("ActivityLog.DeleteProduct"), product.Name), product);

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Catalog.Products.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                await _productService.DeleteProducts((await _productService.GetProductsByIds(selectedIds.ToArray())).Where(p => _workContext.GetCurrentVendor().Result == null || p.VendorId == _workContext.GetCurrentVendor().Result.Id).ToList());
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> CopyProduct(ProductModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var copyModel = model.CopyProductModel;
            try
            {
                var originalProduct = await _productService.GetProductById(copyModel.Id);

                //a vendor should have access only to his products
                if (await _workContext.GetCurrentVendor() != null && originalProduct.VendorId != (await _workContext.GetCurrentVendor()).Id)
                    return RedirectToAction("List");

                var newProduct = _copyProductService.CopyProduct(originalProduct, copyModel.Name, copyModel.Published, copyModel.CopyImages);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Catalog.Products.Copied"));

                return RedirectToAction("Edit", new { id = newProduct.Id });
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = copyModel.Id });
            }
        }

        //action displaying notification (warning) to a store owner that entered SKU already exists
        public virtual async Task<IActionResult> SkuReservedWarning(int productId, string sku)
        {
            string message;

            //check whether product with passed SKU already exists
            var productBySku = await _productService.GetProductBySku(sku);
            if (productBySku != null)
            {
                if (productBySku.Id == productId)
                    return Json(new { Result = string.Empty });

                message = string.Format(await _localizationService.GetResource("Admin.Catalog.Products.Fields.Sku.Reserved"), productBySku.Name);
                return Json(new { Result = message });
            }

            //check whether combination with passed SKU already exists
            var combinationBySku = await _productAttributeService.GetProductAttributeCombinationBySku(sku);
            if (combinationBySku == null)
                return Json(new { Result = string.Empty });

            message = string.Format(await _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.Sku.Reserved"),
                (await _productService.GetProductById(combinationBySku.ProductId))?.Name);

            return Json(new { Result = message });
        }

        #endregion

        #region Required products

        [HttpPost]
        public virtual async Task<IActionResult> LoadProductFriendlyNames(string productIds)
        {
            var result = string.Empty;

            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
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

            var products = await _productService.GetProductsByIds(ids.ToArray());
            for (var i = 0; i <= products.Count - 1; i++)
            {
                result += products[i].Name;
                if (i != products.Count - 1)
                    result += ", ";
            }

            return Json(new { Text = result });
        }

        public virtual async Task<IActionResult> RequiredProductAddPopup()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model = await _productModelFactory.PrepareAddRequiredProductSearchModel(new AddRequiredProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> RequiredProductAddPopupList(AddRequiredProductSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _productModelFactory.PrepareAddRequiredProductListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Related products

        [HttpPost]
        public virtual async Task<IActionResult> RelatedProductList(RelatedProductSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedDataTablesJson();

            //try to get a product with the specified id
            var product = await _productService.GetProductById(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return Content("This is not your product");

            //prepare model
            var model = await _productModelFactory.PrepareRelatedProductListModel(searchModel, product);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> RelatedProductUpdate(RelatedProductModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a related product with the specified id
            var relatedProduct = await _productService.GetRelatedProductById(model.Id)
                ?? throw new ArgumentException("No related product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null)
            {
                var product = await _productService.GetProductById(relatedProduct.ProductId1);
                if (product != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                    return Content("This is not your product");
            }

            relatedProduct.DisplayOrder = model.DisplayOrder;
            await _productService.UpdateRelatedProduct(relatedProduct);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> RelatedProductDelete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a related product with the specified id
            var relatedProduct = await _productService.GetRelatedProductById(id)
                ?? throw new ArgumentException("No related product found with the specified id");

            var productId = relatedProduct.ProductId1;

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null)
            {
                var product = await _productService.GetProductById(productId);
                if (product != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                    return Content("This is not your product");
            }

            await _productService.DeleteRelatedProduct(relatedProduct);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> RelatedProductAddPopup(int productId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model = await _productModelFactory.PrepareAddRelatedProductSearchModel(new AddRelatedProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> RelatedProductAddPopupList(AddRelatedProductSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _productModelFactory.PrepareAddRelatedProductListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> RelatedProductAddPopup(AddRelatedProductModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var selectedProducts = await _productService.GetProductsByIds(model.SelectedProductIds.ToArray());
            if (selectedProducts.Any())
            {
                var existingRelatedProducts = await _productService.GetRelatedProductsByProductId1(model.ProductId, showHidden: true);
                foreach (var product in selectedProducts)
                {
                    //a vendor should have access only to his products
                    if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                        continue;

                    if (_productService.FindRelatedProduct(existingRelatedProducts, model.ProductId, product.Id) != null)
                        continue;

                    await _productService.InsertRelatedProduct(new RelatedProduct
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
        public virtual async Task<IActionResult> CrossSellProductList(CrossSellProductSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedDataTablesJson();

            //try to get a product with the specified id
            var product = await _productService.GetProductById(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return Content("This is not your product");

            //prepare model
            var model = await _productModelFactory.PrepareCrossSellProductListModel(searchModel, product);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CrossSellProductDelete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a cross-sell product with the specified id
            var crossSellProduct = await _productService.GetCrossSellProductById(id)
                ?? throw new ArgumentException("No cross-sell product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null)
            {
                var product = await _productService.GetProductById(crossSellProduct.ProductId1);
                if (product != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                    return Content("This is not your product");
            }

            await _productService.DeleteCrossSellProduct(crossSellProduct);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> CrossSellProductAddPopup(int productId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model = await _productModelFactory.PrepareAddCrossSellProductSearchModel(new AddCrossSellProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CrossSellProductAddPopupList(AddCrossSellProductSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _productModelFactory.PrepareAddCrossSellProductListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> CrossSellProductAddPopup(AddCrossSellProductModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var selectedProducts = await _productService.GetProductsByIds(model.SelectedProductIds.ToArray());
            if (selectedProducts.Any())
            {
                var existingCrossSellProducts = await _productService.GetCrossSellProductsByProductId1(model.ProductId, showHidden: true);
                foreach (var product in selectedProducts)
                {
                    //a vendor should have access only to his products
                    if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                        continue;

                    if (_productService.FindCrossSellProduct(existingCrossSellProducts, model.ProductId, product.Id) != null)
                        continue;

                    await _productService.InsertCrossSellProduct(new CrossSellProduct
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
        public virtual async Task<IActionResult> AssociatedProductList(AssociatedProductSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedDataTablesJson();

            //try to get a product with the specified id
            var product = await _productService.GetProductById(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return Content("This is not your product");

            //prepare model
            var model = await _productModelFactory.PrepareAssociatedProductListModel(searchModel, product);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AssociatedProductUpdate(AssociatedProductModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get an associated product with the specified id
            var associatedProduct = await _productService.GetProductById(model.Id)
                ?? throw new ArgumentException("No associated product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && associatedProduct.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return Content("This is not your product");

            associatedProduct.DisplayOrder = model.DisplayOrder;
            await _productService.UpdateProduct(associatedProduct);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> AssociatedProductDelete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get an associated product with the specified id
            var product = await _productService.GetProductById(id)
                ?? throw new ArgumentException("No associated product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return Content("This is not your product");

            product.ParentGroupedProductId = 0;
            await _productService.UpdateProduct(product);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> AssociatedProductAddPopup(int productId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model = await _productModelFactory.PrepareAddAssociatedProductSearchModel(new AddAssociatedProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AssociatedProductAddPopupList(AddAssociatedProductSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _productModelFactory.PrepareAddAssociatedProductListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> AssociatedProductAddPopup(AddAssociatedProductModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var selectedProducts = await _productService.GetProductsByIds(model.SelectedProductIds.ToArray());

            var tryToAddSelfGroupedProduct = selectedProducts
                .Select(p => p.Id)
                .Contains(model.ProductId);

            if (selectedProducts.Any())
            {
                foreach (var product in selectedProducts)
                {
                    if (product.Id == model.ProductId)
                        continue;

                    //a vendor should have access only to his products
                    if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                        continue;

                    product.ParentGroupedProductId = model.ProductId;
                    await _productService.UpdateProduct(product);
                }
            }

            if (tryToAddSelfGroupedProduct)
            {
                _notificationService.WarningNotification(await _localizationService.GetResource("Admin.Catalog.Products.AssociatedProducts.TryToAddSelfGroupedProduct"));

                var addAssociatedProductSearchModel = await _productModelFactory.PrepareAddAssociatedProductSearchModel(new AddAssociatedProductSearchModel());
                //set current product id
                addAssociatedProductSearchModel.ProductId = model.ProductId;

                ViewBag.RefreshPage = true;

                return View(addAssociatedProductSearchModel);
            }

            ViewBag.RefreshPage = true;

            ViewBag.ClosePage = true;

            return View(new AddAssociatedProductSearchModel());
        }

        #endregion

        #region Product pictures

        public virtual async Task<IActionResult> ProductPictureAdd(int pictureId, int displayOrder,
            string overrideAltAttribute, string overrideTitleAttribute, int productId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (pictureId == 0)
                throw new ArgumentException();

            //try to get a product with the specified id
            var product = await _productService.GetProductById(productId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return RedirectToAction("List");

            if ((await _productService.GetProductPicturesByProductId(productId)).Any(p => p.PictureId == pictureId))
                return Json(new { Result = false });

            //try to get a picture with the specified id
            var picture = await _pictureService.GetPictureById(pictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            await _pictureService.UpdatePicture(picture.Id,
                await _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                overrideAltAttribute,
                overrideTitleAttribute);

            await _pictureService.SetSeoFilename(pictureId, await _pictureService.GetPictureSeName(product.Name));

            await _productService.InsertProductPicture(new ProductPicture
            {
                PictureId = pictureId,
                ProductId = productId,
                DisplayOrder = displayOrder
            });

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductPictureList(ProductPictureSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedDataTablesJson();

            //try to get a product with the specified id
            var product = await _productService.GetProductById(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return Content("This is not your product");

            //prepare model
            var model = await _productModelFactory.PrepareProductPictureListModel(searchModel, product);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductPictureUpdate(ProductPictureModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product picture with the specified id
            var productPicture = await _productService.GetProductPictureById(model.Id)
                ?? throw new ArgumentException("No product picture found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null)
            {
                var product = await _productService.GetProductById(productPicture.ProductId);
                if (product != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                    return Content("This is not your product");
            }

            //try to get a picture with the specified id
            var picture = await _pictureService.GetPictureById(productPicture.PictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            await _pictureService.UpdatePicture(picture.Id,
                await _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                model.OverrideAltAttribute,
                model.OverrideTitleAttribute);

            productPicture.DisplayOrder = model.DisplayOrder;
            await _productService.UpdateProductPicture(productPicture);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductPictureDelete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product picture with the specified id
            var productPicture = await _productService.GetProductPictureById(id)
                ?? throw new ArgumentException("No product picture found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null)
            {
                var product = await _productService.GetProductById(productPicture.ProductId);
                if (product != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                    return Content("This is not your product");
            }

            var pictureId = productPicture.PictureId;
            await _productService.DeleteProductPicture(productPicture);

            //try to get a picture with the specified id
            var picture = await _pictureService.GetPictureById(pictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            await _pictureService.DeletePicture(picture);

            return new NullJsonResult();
        }

        #endregion

        #region Product specification attributes

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> ProductSpecificationAttributeAdd(AddSpecificationAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var product = await _productService.GetProductById(model.ProductId);
            if (product == null)
            {
                _notificationService.ErrorNotification("No product found with the specified id");
                return RedirectToAction("List");
            }

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
            {
                return RedirectToAction("List");
            }

            //we allow filtering only for "Option" attribute type
            if (model.AttributeTypeId != (int)SpecificationAttributeType.Option)
                model.AllowFiltering = false;

            //we don't allow CustomValue for "Option" attribute type
            if (model.AttributeTypeId == (int)SpecificationAttributeType.Option)
                model.ValueRaw = null;

            //store raw html if field allow this
            if (model.AttributeTypeId == (int)SpecificationAttributeType.CustomText
                || model.AttributeTypeId == (int)SpecificationAttributeType.Hyperlink)
                model.ValueRaw = model.Value;

            var psa = model.ToEntity<ProductSpecificationAttribute>();
            psa.CustomValue = model.ValueRaw;
            await _specificationAttributeService.InsertProductSpecificationAttribute(psa);

            switch (psa.AttributeType)
            {
                case SpecificationAttributeType.CustomText:
                    foreach (var localized in model.Locales)
                    {
                        await _localizedEntityService.SaveLocalizedValue(psa,
                            x => x.CustomValue,
                            localized.Value,
                            localized.LanguageId);
                    }

                    break;
                case SpecificationAttributeType.CustomHtmlText:
                    foreach (var localized in model.Locales)
                    {
                        await _localizedEntityService.SaveLocalizedValue(psa,
                            x => x.CustomValue,
                            localized.ValueRaw,
                            localized.LanguageId);
                    }

                    break;
                case SpecificationAttributeType.Option:
                    break;
                case SpecificationAttributeType.Hyperlink:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (continueEditing)
                return RedirectToAction("ProductSpecAttributeAddOrEdit",
                    new { productId = psa.ProductId, specificationId = psa.Id });

            //select an appropriate panel
            SaveSelectedPanelName("product-specification-attributes");
            return RedirectToAction("Edit", new { id = model.ProductId });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductSpecAttrList(ProductSpecificationAttributeSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedDataTablesJson();

            //try to get a product with the specified id
            var product = await _productService.GetProductById(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return Content("This is not your product");

            //prepare model
            var model = await _productModelFactory.PrepareProductSpecificationAttributeListModel(searchModel, product);

            return Json(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> ProductSpecAttrUpdate(AddSpecificationAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product specification attribute with the specified id
            var psa = await _specificationAttributeService.GetProductSpecificationAttributeById(model.SpecificationId);
            if (psa == null)
            {
                //select an appropriate panel
                SaveSelectedPanelName("product-specification-attributes");
                _notificationService.ErrorNotification("No product specification attribute found with the specified id");

                return RedirectToAction("Edit", new { id = model.ProductId });
            }

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null
                && (await _productService.GetProductById(psa.ProductId)).VendorId != (await _workContext.GetCurrentVendor()).Id)
            {
                _notificationService.ErrorNotification("This is not your product");

                return RedirectToAction("List");
            }

            //we allow filtering and change option only for "Option" attribute type
            //save localized values for CustomHtmlText and CustomText
            switch (model.AttributeTypeId)
            {
                case (int)SpecificationAttributeType.Option:
                    psa.AllowFiltering = model.AllowFiltering;
                    psa.SpecificationAttributeOptionId = model.SpecificationAttributeOptionId;

                    break;
                case (int)SpecificationAttributeType.CustomHtmlText:
                    psa.CustomValue = model.ValueRaw;
                    foreach (var localized in model.Locales)
                    {
                        await _localizedEntityService.SaveLocalizedValue(psa,
                            x => x.CustomValue,
                            localized.ValueRaw,
                            localized.LanguageId);
                    }

                    break;
                case (int)SpecificationAttributeType.CustomText:
                    psa.CustomValue = model.Value;
                    foreach (var localized in model.Locales)
                    {
                        await _localizedEntityService.SaveLocalizedValue(psa,
                            x => x.CustomValue,
                            localized.ValueRaw,
                            localized.LanguageId);
                    }

                    break;
                default:
                    psa.CustomValue = model.Value;

                    break;
            }

            psa.ShowOnProductPage = model.ShowOnProductPage;
            psa.DisplayOrder = model.DisplayOrder;
            await _specificationAttributeService.UpdateProductSpecificationAttribute(psa);

            if (continueEditing)
            {
                return RedirectToAction("ProductSpecAttributeAddOrEdit",
                    new { productId = psa.ProductId, specificationId = model.SpecificationId });
            }

            //select an appropriate panel
            SaveSelectedPanelName("product-specification-attributes");

            return RedirectToAction("Edit", new { id = psa.ProductId });
        }

        public virtual async Task<IActionResult> ProductSpecAttributeAddOrEdit(int productId, int? specificationId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (_productService.GetProductById(productId) == null)
            {
                _notificationService.ErrorNotification("No product found with the specified id");
                return RedirectToAction("List");
            }

            //try to get a product specification attribute with the specified id
            try
            {
                var model = await _productModelFactory.PrepareAddSpecificationAttributeModel(productId, specificationId);
                return View(model);
            }
            catch (Exception ex)
            {
                _notificationService.ErrorNotification(ex);

                //select an appropriate panel
                SaveSelectedPanelName("product-specification-attributes");
                return RedirectToAction("Edit", new { id = productId });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductSpecAttrDelete(AddSpecificationAttributeModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product specification attribute with the specified id
            var psa = await _specificationAttributeService.GetProductSpecificationAttributeById(model.SpecificationId);
            if (psa == null)
            {
                //select an appropriate panel
                SaveSelectedPanelName("product-specification-attributes");
                _notificationService.ErrorNotification("No product specification attribute found with the specified id");
                return RedirectToAction("Edit", new { id = model.ProductId });
            }

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && (await _productService.GetProductById(psa.ProductId)).VendorId != (await _workContext.GetCurrentVendor()).Id)
            {
                _notificationService.ErrorNotification("This is not your product");
                return RedirectToAction("List", new { id = model.ProductId });
            }

            await _specificationAttributeService.DeleteProductSpecificationAttribute(psa);

            //select an appropriate panel
            SaveSelectedPanelName("product-specification-attributes");

            return RedirectToAction("Edit", new { id = psa.ProductId });
        }

        #endregion

        #region Product tags

        public virtual async Task<IActionResult> ProductTags()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedView();

            //prepare model
            var model = await _productModelFactory.PrepareProductTagSearchModel(new ProductTagSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductTags(ProductTagSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _productModelFactory.PrepareProductTagListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductTagDelete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedView();

            //try to get a product tag with the specified id
            var tag = await _productTagService.GetProductTagById(id)
                ?? throw new ArgumentException("No product tag found with the specified id");

            await _productTagService.DeleteProductTag(tag);

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Catalog.ProductTags.Deleted"));

            return RedirectToAction("ProductTags");
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductTagsDelete(ICollection<int> selectedIds)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                var tags = await _productTagService.GetProductTagsByIds(selectedIds.ToArray());
                await _productTagService.DeleteProductTags(tags);
            }

            return Json(new { Result = true });
        }

        public virtual async Task<IActionResult> EditProductTag(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedView();

            //try to get a product tag with the specified id
            var productTag = await _productTagService.GetProductTagById(id);
            if (productTag == null)
                return RedirectToAction("List");

            //prepare tag model
            var model = await _productModelFactory.PrepareProductTagModel(null, productTag);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> EditProductTag(ProductTagModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedView();

            //try to get a product tag with the specified id
            var productTag = await _productTagService.GetProductTagById(model.Id);
            if (productTag == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                productTag.Name = model.Name;
                await _productTagService.UpdateProductTag(productTag);

                //locales
                await UpdateLocales(productTag, model);

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Catalog.ProductTags.Updated"));

                return continueEditing ? RedirectToAction("EditProductTag", new { id = productTag.Id }) : RedirectToAction("ProductTags");
            }

            //prepare model
            model = await _productModelFactory.PrepareProductTagModel(model, productTag, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Purchased with order

        [HttpPost]
        public virtual async Task<IActionResult> PurchasedWithOrders(ProductOrderSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedDataTablesJson();

            //try to get a product with the specified id
            var product = await _productService.GetProductById(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return Content("This is not your product");

            //prepare model
            var model = await _productModelFactory.PrepareProductOrderListModel(searchModel, product);

            return Json(model);
        }

        #endregion

        #region Export / Import

        [HttpPost, ActionName("DownloadCatalogPDF")]
        [FormValueRequired("download-catalog-pdf")]
        public virtual async Task<IActionResult> DownloadCatalogAsPdf(ProductSearchModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null)
            {
                model.SearchVendorId = (await _workContext.GetCurrentVendor()).Id;
            }

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                categoryIds.AddRange(await _categoryService.GetChildCategoryIds(parentCategoryId: model.SearchCategoryId, showHidden: true));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var products = await _productService.SearchProducts(0,
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
                await using (var stream = new MemoryStream())
                {
                    await _pdfService.PrintProductsToPdf(stream, products);
                    bytes = stream.ToArray();
                }

                return File(bytes, MimeTypes.ApplicationPdf, "pdfcatalog.pdf");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost, ActionName("ExportToXml")]
        [FormValueRequired("exportxml-all")]
        public virtual async Task<IActionResult> ExportXmlAll(ProductSearchModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null)
            {
                model.SearchVendorId = (await _workContext.GetCurrentVendor()).Id;
            }

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                categoryIds.AddRange(await _categoryService.GetChildCategoryIds(parentCategoryId: model.SearchCategoryId, showHidden: true));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var products = await _productService.SearchProducts(0,
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
                var xml = await _exportManager.ExportProductsToXml(products);

                return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "products.xml");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ExportXmlSelected(string selectedIds)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var products = new List<Product>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                products.AddRange(await _productService.GetProductsByIds(ids));
            }
            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null)
            {
                products = products.Where(p => p.VendorId == _workContext.GetCurrentVendor().Result.Id).ToList();
            }

            try
            {
                var xml = await _exportManager.ExportProductsToXml(products);
                return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "products.xml");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost, ActionName("ExportToExcel")]
        [FormValueRequired("exportexcel-all")]
        public virtual async Task<IActionResult> ExportExcelAll(ProductSearchModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null)
            {
                model.SearchVendorId = (await _workContext.GetCurrentVendor()).Id;
            }

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                categoryIds.AddRange(await _categoryService.GetChildCategoryIds(parentCategoryId: model.SearchCategoryId, showHidden: true));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var products = await _productService.SearchProducts(0,
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
                var bytes = await _exportManager.ExportProductsToXlsx(products);

                return File(bytes, MimeTypes.TextXlsx, "products.xlsx");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);

                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ExportExcelSelected(string selectedIds)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var products = new List<Product>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                products.AddRange(await _productService.GetProductsByIds(ids));
            }
            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null)
            {
                products = products.Where(p => p.VendorId == _workContext.GetCurrentVendor().Result.Id).ToList();
            }

            try
            {
                var bytes = await _exportManager.ExportProductsToXlsx(products);

                return File(bytes, MimeTypes.TextXlsx, "products.xlsx");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportExcel(IFormFile importexcelfile)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (await _workContext.GetCurrentVendor() != null && !_vendorSettings.AllowVendorsToImportProducts)
                //a vendor can not import products
                return AccessDeniedView();

            try
            {
                if (importexcelfile != null && importexcelfile.Length > 0)
                {
                    await _importManager.ImportProductsFromXlsx(importexcelfile.OpenReadStream());
                }
                else
                {
                    _notificationService.ErrorNotification(await _localizationService.GetResource("Admin.Common.UploadFile"));
                    
                    return RedirectToAction("List");
                }

                _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Catalog.Products.Imported"));
                
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                
                return RedirectToAction("List");
            }
        }

        #endregion

        #region Tier prices

        [HttpPost]
        public virtual async Task<IActionResult> TierPriceList(TierPriceSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedDataTablesJson();

            //try to get a product with the specified id
            var product = await _productService.GetProductById(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return Content("This is not your product");

            //prepare model
            var model = await _productModelFactory.PrepareTierPriceListModel(searchModel, product);

            return Json(model);
        }

        public virtual async Task<IActionResult> TierPriceCreatePopup(int productId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await _productService.GetProductById(productId)
                ?? throw new ArgumentException("No product found with the specified id");

            //prepare model
            var model = await _productModelFactory.PrepareTierPriceModel(new TierPriceModel(), product, null);

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> TierPriceCreatePopup(TierPriceModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await _productService.GetProductById(model.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return RedirectToAction("List", "Product");

            if (ModelState.IsValid)
            {
                //fill entity from model
                var tierPrice = model.ToEntity<TierPrice>();
                tierPrice.ProductId = product.Id;
                tierPrice.CustomerRoleId = model.CustomerRoleId > 0 ? model.CustomerRoleId : (int?)null;

                await _productService.InsertTierPrice(tierPrice);

                //update "HasTierPrices" property
                await _productService.UpdateHasTierPricesProperty(product);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _productModelFactory.PrepareTierPriceModel(model, product, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> TierPriceEditPopup(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a tier price with the specified id
            var tierPrice = await _productService.GetTierPriceById(id);
            if (tierPrice == null)
                return RedirectToAction("List", "Product");

            //try to get a product with the specified id
            var product = await _productService.GetProductById(tierPrice.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return RedirectToAction("List", "Product");

            //prepare model
            var model = await _productModelFactory.PrepareTierPriceModel(null, product, tierPrice);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TierPriceEditPopup(TierPriceModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a tier price with the specified id
            var tierPrice = await _productService.GetTierPriceById(model.Id);
            if (tierPrice == null)
                return RedirectToAction("List", "Product");

            //try to get a product with the specified id
            var product = await _productService.GetProductById(tierPrice.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return RedirectToAction("List", "Product");

            if (ModelState.IsValid)
            {
                //fill entity from model
                tierPrice = model.ToEntity(tierPrice);
                tierPrice.CustomerRoleId = model.CustomerRoleId > 0 ? model.CustomerRoleId : (int?)null;
                await _productService.UpdateTierPrice(tierPrice);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _productModelFactory.PrepareTierPriceModel(model, product, tierPrice, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TierPriceDelete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a tier price with the specified id
            var tierPrice = await _productService.GetTierPriceById(id)
                ?? throw new ArgumentException("No tier price found with the specified id");

            //try to get a product with the specified id
            var product = await _productService.GetProductById(tierPrice.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return Content("This is not your product");

            await _productService.DeleteTierPrice(tierPrice);

            //update "HasTierPrices" property
            await _productService.UpdateHasTierPricesProperty(product);

            return new NullJsonResult();
        }

        #endregion

        #region Product attributes

        [HttpPost]
        public virtual async Task<IActionResult> ProductAttributeMappingList(ProductAttributeMappingSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedDataTablesJson();

            //try to get a product with the specified id
            var product = await _productService.GetProductById(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return Content("This is not your product");

            //prepare model
            var model = await _productModelFactory.PrepareProductAttributeMappingListModel(searchModel, product);

            return Json(model);
        }

        public virtual async Task<IActionResult> ProductAttributeMappingCreate(int productId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await _productService.GetProductById(productId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResource("This is not your product"));
                return RedirectToAction("List");
            }

            //prepare model
            var model = await _productModelFactory.PrepareProductAttributeMappingModel(new ProductAttributeMappingModel(), product, null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> ProductAttributeMappingCreate(ProductAttributeMappingModel model, bool continueEditing)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await _productService.GetProductById(model.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResource("This is not your product"));
                return RedirectToAction("List");
            }

            //ensure this attribute is not mapped yet
            if ((await _productAttributeService.GetProductAttributeMappingsByProductId(product.Id))
                .Any(x => x.ProductAttributeId == model.ProductAttributeId))
            {
                //redisplay form
                _notificationService.ErrorNotification(await _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.AlreadyExists"));

                model = await _productModelFactory.PrepareProductAttributeMappingModel(model, product, null, true);

                return View(model);
            }

            //insert mapping
            var productAttributeMapping = model.ToEntity<ProductAttributeMapping>();

            await _productAttributeService.InsertProductAttributeMapping(productAttributeMapping);
            await UpdateLocales(productAttributeMapping, model);

            //predefined values
            var predefinedValues = await _productAttributeService.GetPredefinedProductAttributeValues(model.ProductAttributeId);
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
                await _productAttributeService.InsertProductAttributeValue(pav);

                //locales
                var languages = await _languageService.GetAllLanguages(true);

                //localization
                foreach (var lang in languages)
                {
                    var name = await _localizationService.GetLocalized(predefinedValue, x => x.Name, lang.Id, false, false);
                    if (!string.IsNullOrEmpty(name))
                        await _localizedEntityService.SaveLocalizedValue(pav, x => x.Name, name, lang.Id);
                }
            }

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.Added"));

            if (!continueEditing)
            {
                //select an appropriate panel
                SaveSelectedPanelName("product-product-attributes");
                return RedirectToAction("Edit", new { id = product.Id });
            }

            return RedirectToAction("ProductAttributeMappingEdit", new { id = productAttributeMapping.Id });
        }

        public virtual async Task<IActionResult> ProductAttributeMappingEdit(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingById(id)
                ?? throw new ArgumentException("No product attribute mapping found with the specified id");

            //try to get a product with the specified id
            var product = await _productService.GetProductById(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResource("This is not your product"));
                return RedirectToAction("List");
            }

            //prepare model
            var model = await _productModelFactory.PrepareProductAttributeMappingModel(null, product, productAttributeMapping);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> ProductAttributeMappingEdit(ProductAttributeMappingModel model, bool continueEditing, IFormCollection form)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingById(model.Id)
                ?? throw new ArgumentException("No product attribute mapping found with the specified id");

            //try to get a product with the specified id
            var product = await _productService.GetProductById(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResource("This is not your product"));
                return RedirectToAction("List");
            }

            //ensure this attribute is not mapped yet
            if ((await _productAttributeService.GetProductAttributeMappingsByProductId(product.Id))
                .Any(x => x.ProductAttributeId == model.ProductAttributeId && x.Id != productAttributeMapping.Id))
            {
                //redisplay form
                _notificationService.ErrorNotification(await _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.AlreadyExists"));

                model = await _productModelFactory.PrepareProductAttributeMappingModel(model, product, productAttributeMapping, true);

                return View(model);
            }

            //fill entity from model
            productAttributeMapping = model.ToEntity(productAttributeMapping);
            await _productAttributeService.UpdateProductAttributeMapping(productAttributeMapping);

            await UpdateLocales(productAttributeMapping, model);

            await SaveConditionAttributes(productAttributeMapping, model.ConditionModel, form);

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.Updated"));

            if (!continueEditing)
            {
                //select an appropriate panel
                SaveSelectedPanelName("product-product-attributes");
                return RedirectToAction("Edit", new { id = product.Id });
            }

            return RedirectToAction("ProductAttributeMappingEdit", new { id = productAttributeMapping.Id });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAttributeMappingDelete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingById(id)
                ?? throw new ArgumentException("No product attribute mapping found with the specified id");

            //try to get a product with the specified id
            var product = await _productService.GetProductById(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return Content("This is not your product");

            await _productAttributeService.DeleteProductAttributeMapping(productAttributeMapping);

            _notificationService.SuccessNotification(await _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.Deleted"));

            //select an appropriate panel
            SaveSelectedPanelName("product-product-attributes");
            return RedirectToAction("Edit", new { id = productAttributeMapping.ProductId });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAttributeValueList(ProductAttributeValueSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedDataTablesJson();

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingById(searchModel.ProductAttributeMappingId)
                ?? throw new ArgumentException("No product attribute mapping found with the specified id");

            //try to get a product with the specified id
            var product = await _productService.GetProductById(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return Content("This is not your product");

            //prepare model
            var model = await _productModelFactory.PrepareProductAttributeValueListModel(searchModel, productAttributeMapping);

            return Json(model);
        }

        public virtual async Task<IActionResult> ProductAttributeValueCreatePopup(int productAttributeMappingId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingById(productAttributeMappingId)
                ?? throw new ArgumentException("No product attribute mapping found with the specified id");

            //try to get a product with the specified id
            var product = await _productService.GetProductById(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return RedirectToAction("List", "Product");

            //prepare model
            var model = await _productModelFactory.PrepareProductAttributeValueModel(new ProductAttributeValueModel(), productAttributeMapping, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAttributeValueCreatePopup(ProductAttributeValueModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingById(model.ProductAttributeMappingId);
            if (productAttributeMapping == null)
                return RedirectToAction("List", "Product");

            //try to get a product with the specified id
            var product = await _productService.GetProductById(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
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
                //fill entity from model
                var pav = model.ToEntity<ProductAttributeValue>();

                pav.Quantity = model.CustomerEntersQty ? 1 : model.Quantity;

                await _productAttributeService.InsertProductAttributeValue(pav);
                await UpdateLocales(pav, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _productModelFactory.PrepareProductAttributeValueModel(model, productAttributeMapping, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> ProductAttributeValueEditPopup(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute value with the specified id
            var productAttributeValue = await _productAttributeService.GetProductAttributeValueById(id);
            if (productAttributeValue == null)
                return RedirectToAction("List", "Product");

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingById(productAttributeValue.ProductAttributeMappingId);
            if (productAttributeMapping == null)
                return RedirectToAction("List", "Product");

            //try to get a product with the specified id
            var product = await _productService.GetProductById(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return RedirectToAction("List", "Product");

            //prepare model
            var model = await _productModelFactory.PrepareProductAttributeValueModel(null, productAttributeMapping, productAttributeValue);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAttributeValueEditPopup(ProductAttributeValueModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute value with the specified id
            var productAttributeValue = await _productAttributeService.GetProductAttributeValueById(model.Id);
            if (productAttributeValue == null)
                return RedirectToAction("List", "Product");

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingById(productAttributeValue.ProductAttributeMappingId);
            if (productAttributeMapping == null)
                return RedirectToAction("List", "Product");

            //try to get a product with the specified id
            var product = await _productService.GetProductById(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
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
                //fill entity from model
                productAttributeValue = model.ToEntity(productAttributeValue);
                productAttributeValue.Quantity = model.CustomerEntersQty ? 1 : model.Quantity;
                await _productAttributeService.UpdateProductAttributeValue(productAttributeValue);

                await UpdateLocales(productAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _productModelFactory.PrepareProductAttributeValueModel(model, productAttributeMapping, productAttributeValue, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAttributeValueDelete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute value with the specified id
            var productAttributeValue = await _productAttributeService.GetProductAttributeValueById(id)
                ?? throw new ArgumentException("No product attribute value found with the specified id");

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = await _productAttributeService.GetProductAttributeMappingById(productAttributeValue.ProductAttributeMappingId)
                ?? throw new ArgumentException("No product attribute mapping found with the specified id");

            //try to get a product with the specified id
            var product = await _productService.GetProductById(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return Content("This is not your product");

            await _productAttributeService.DeleteProductAttributeValue(productAttributeValue);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> AssociateProductToAttributeValuePopup()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model = await _productModelFactory.PrepareAssociateProductToAttributeValueSearchModel(new AssociateProductToAttributeValueSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AssociateProductToAttributeValuePopupList(AssociateProductToAttributeValueSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = await _productModelFactory.PrepareAssociateProductToAttributeValueListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> AssociateProductToAttributeValuePopup([Bind(Prefix = nameof(AssociateProductToAttributeValueModel))] AssociateProductToAttributeValueModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var associatedProduct = await _productService.GetProductById(model.AssociatedToProductId);
            if (associatedProduct == null)
                return Content("Cannot load a product");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && associatedProduct.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return Content("This is not your product");

            ViewBag.RefreshPage = true;
            ViewBag.productId = associatedProduct.Id;
            ViewBag.productName = associatedProduct.Name;

            return View(new AssociateProductToAttributeValueSearchModel());
        }

        //action displaying notification (warning) to a store owner when associating some product
        public virtual async Task<IActionResult> AssociatedProductGetWarnings(int productId)
        {
            var associatedProduct = await _productService.GetProductById(productId);
            if (associatedProduct == null)
                return Json(new { Result = string.Empty });

            //attributes
            if (await _productAttributeService.GetProductAttributeMappingsByProductId(associatedProduct.Id) is IList<ProductAttributeMapping> mapping && mapping.Any())
            {
                if (mapping.Any(attribute => attribute.IsRequired))
                    return Json(new { Result = await _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.HasRequiredAttributes") });

                return Json(new { Result = await _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.HasAttributes") });
            }

            //gift card
            if (associatedProduct.IsGiftCard)
            {
                return Json(new { Result = await _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.GiftCard") });
            }

            //downloadable product
            if (associatedProduct.IsDownload)
            {
                return Json(new { Result = await _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.Downloadable") });
            }

            return Json(new { Result = string.Empty });
        }

        #endregion

        #region Product attribute combinations

        [HttpPost]
        public virtual async Task<IActionResult> ProductAttributeCombinationList(ProductAttributeCombinationSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedDataTablesJson();

            //try to get a product with the specified id
            var product = await _productService.GetProductById(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return Content("This is not your product");

            //prepare model
            var model = await _productModelFactory.PrepareProductAttributeCombinationListModel(searchModel, product);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAttributeCombinationDelete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a combination with the specified id
            var combination = await _productAttributeService.GetProductAttributeCombinationById(id)
                ?? throw new ArgumentException("No product attribute combination found with the specified id");

            //try to get a product with the specified id
            var product = await _productService.GetProductById(combination.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return Content("This is not your product");

            await _productAttributeService.DeleteProductAttributeCombination(combination);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> ProductAttributeCombinationCreatePopup(int productId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await _productService.GetProductById(productId);
            if (product == null)
                return RedirectToAction("List", "Product");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return RedirectToAction("List", "Product");

            //prepare model
            var model = await _productModelFactory.PrepareProductAttributeCombinationModel(new ProductAttributeCombinationModel(), product, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAttributeCombinationCreatePopup(int productId, ProductAttributeCombinationModel model, IFormCollection form)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await _productService.GetProductById(productId);
            if (product == null)
                return RedirectToAction("List", "Product");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return RedirectToAction("List", "Product");

            //attributes
            var warnings = new List<string>();
            var attributesXml = await GetAttributesXmlForProductAttributeCombination(form, warnings, product.Id);

            //check whether the attribute value is specified
            if (string.IsNullOrEmpty(attributesXml))
                warnings.Add(await _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Alert.FailedValue"));

            warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarnings(await _workContext.GetCurrentCustomer(),
                ShoppingCartType.ShoppingCart, product, 1, attributesXml, true));

            //check whether the same attribute combination already exists
            var existingCombination = _productAttributeParser.FindProductAttributeCombination(product, attributesXml);
            if (existingCombination != null)
                warnings.Add(await _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.AlreadyExists"));

            if (!warnings.Any())
            {
                //save combination
                var combination = model.ToEntity<ProductAttributeCombination>();

                //fill attributes
                combination.AttributesXml = attributesXml;

                await _productAttributeService.InsertProductAttributeCombination(combination);

                //quantity change history
                await _productService.AddStockQuantityHistoryEntry(product, combination.StockQuantity, combination.StockQuantity,
                    message: await _localizationService.GetResource("Admin.StockQuantityHistory.Messages.Combination.Edit"), combinationId: combination.Id);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _productModelFactory.PrepareProductAttributeCombinationModel(model, product, null, true);
            model.Warnings = warnings;

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> ProductAttributeCombinationGeneratePopup(int productId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await _productService.GetProductById(productId);
            if (product == null)
                return RedirectToAction("List", "Product");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return RedirectToAction("List", "Product");

            //prepare model
            var model = await _productModelFactory.PrepareProductAttributeCombinationModel(new ProductAttributeCombinationModel(), product, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAttributeCombinationGeneratePopup(IFormCollection form, ProductAttributeCombinationModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await _productService.GetProductById(model.ProductId);
            if (product == null)
                return RedirectToAction("List", "Product");

            var allowedAttributeIds = form.Keys.Where(key => key.Contains("attribute_value_"))
                .Select(key => int.TryParse(form[key], out var id) ? id : 0).Where(id => id > 0).ToList();

            var requiredAttributeNames = (await _productAttributeService.GetProductAttributeMappingsByProductId(product.Id))
                .Where(pam => pam.IsRequired)
                .Where(pam => !pam.IsNonCombinable())
                .Where(pam => !_productAttributeService.GetProductAttributeValues(pam.Id).Result.Any(v => allowedAttributeIds.Any(id => id == v.Id)))
                .Select(pam => _productAttributeService.GetProductAttributeById(pam.ProductAttributeId).Result.Name).ToList();

            if (requiredAttributeNames.Any())
            {
                model = await _productModelFactory.PrepareProductAttributeCombinationModel(model, product, null, true);
                model.ProductAttributes.SelectMany(pa => pa.Values)
                    .Where(v => allowedAttributeIds.Any(id => id == v.Id))
                    .ToList().ForEach(v => v.Checked = "checked");

                model.Warnings.Add(string.Format(await _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.SelectRequiredAttributes"), string.Join(", ", requiredAttributeNames)));

                return View(model);
            }

            await GenerateAttributeCombinations(product, allowedAttributeIds);

            ViewBag.RefreshPage = true;

            return View(new ProductAttributeCombinationModel());
        }

        public virtual async Task<IActionResult> ProductAttributeCombinationEditPopup(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a combination with the specified id
            var combination = await _productAttributeService.GetProductAttributeCombinationById(id);
            if (combination == null)
                return RedirectToAction("List", "Product");

            //try to get a product with the specified id
            var product = await _productService.GetProductById(combination.ProductId);
            if (product == null)
                return RedirectToAction("List", "Product");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return RedirectToAction("List", "Product");

            //prepare model
            var model = await _productModelFactory.PrepareProductAttributeCombinationModel(null, product, combination);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAttributeCombinationEditPopup(ProductAttributeCombinationModel model, IFormCollection form)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a combination with the specified id
            var combination = await _productAttributeService.GetProductAttributeCombinationById(model.Id);
            if (combination == null)
                return RedirectToAction("List", "Product");

            //try to get a product with the specified id
            var product = await _productService.GetProductById(combination.ProductId);
            if (product == null)
                return RedirectToAction("List", "Product");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return RedirectToAction("List", "Product");

            //attributes
            var warnings = new List<string>();
            var attributesXml = await GetAttributesXmlForProductAttributeCombination(form, warnings, product.Id);

            //check whether the attribute value is specified
            if (string.IsNullOrEmpty(attributesXml))
                warnings.Add(await _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Alert.FailedValue"));

            warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarnings(await _workContext.GetCurrentCustomer(),
                ShoppingCartType.ShoppingCart, product, 1, attributesXml, true));

            //check whether the same attribute combination already exists
            var existingCombination = await _productAttributeParser.FindProductAttributeCombination(product, attributesXml);
            if (existingCombination != null && existingCombination.Id != model.Id && existingCombination.AttributesXml.Equals(attributesXml))
                warnings.Add(await _localizationService.GetResource("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.AlreadyExists"));

            if (!warnings.Any() && ModelState.IsValid)
            {
                var previousStockQuantity = combination.StockQuantity;

                //save combination
                //fill entity from model
                combination = model.ToEntity(combination);
                combination.AttributesXml = attributesXml;

                await _productAttributeService.UpdateProductAttributeCombination(combination);

                //quantity change history
                await _productService.AddStockQuantityHistoryEntry(product, combination.StockQuantity - previousStockQuantity, combination.StockQuantity,
                    message: await _localizationService.GetResource("Admin.StockQuantityHistory.Messages.Combination.Edit"), combinationId: combination.Id);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _productModelFactory.PrepareProductAttributeCombinationModel(model, product, combination, true);
            model.Warnings = warnings;

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GenerateAllAttributeCombinations(int productId)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await _productService.GetProductById(productId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return Content("This is not your product");

            await GenerateAttributeCombinations(product);

            return Json(new { Success = true });
        }

        #endregion

        #region Product editor settings

        [HttpPost]
        public virtual async Task<IActionResult> SaveProductEditorSettings(ProductModel model, string returnUrl = "")
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //vendors cannot manage these settings
            if (await _workContext.GetCurrentVendor() != null)
                return RedirectToAction("List");

            var productEditorSettings = await _settingService.LoadSetting<ProductEditorSettings>();
            productEditorSettings = model.ProductEditorSettingsModel.ToSettings(productEditorSettings);
            await _settingService.SaveSetting(productEditorSettings);

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
        public virtual async Task<IActionResult> StockQuantityHistory(StockQuantityHistorySearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedDataTablesJson();

            var product = await _productService.GetProductById(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            if (await _workContext.GetCurrentVendor() != null && product.VendorId != (await _workContext.GetCurrentVendor()).Id)
                return Content("This is not your product");

            //prepare model
            var model = await _productModelFactory.PrepareStockQuantityHistoryListModel(searchModel, product);

            return Json(model);
        }

        #endregion

        #endregion
    }
}