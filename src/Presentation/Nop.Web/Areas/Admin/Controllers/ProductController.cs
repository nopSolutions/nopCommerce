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
using Nop.Core.Domain.Customers;
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

        protected IAclService AclService { get; }
        protected IBackInStockSubscriptionService BackInStockSubscriptionService { get; }
        protected ICategoryService CategoryService { get; }
        protected ICopyProductService CopyProductService { get; }
        protected ICustomerActivityService CustomerActivityService { get; }
        protected ICustomerService CustomerService { get; }
        protected IDiscountService DiscountService { get; }
        protected IDownloadService DownloadService { get; }
        protected IExportManager ExportManager { get; }
        protected IImportManager ImportManager { get; }
        protected ILanguageService LanguageService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected ILocalizedEntityService LocalizedEntityService { get; }
        protected IManufacturerService ManufacturerService { get; }
        protected INopFileProvider FileProvider { get; }
        protected INotificationService NotificationService { get; }
        protected IPdfService PdfService { get; }
        protected IPermissionService PermissionService { get; }
        protected IPictureService PictureService { get; }
        protected IProductAttributeParser ProductAttributeParser { get; }
        protected IProductAttributeService ProductAttributeService { get; }
        protected IProductAttributeFormatter ProductAttributeFormatter { get; }
        protected IProductModelFactory ProductModelFactory { get; }
        protected IProductService ProductService { get; }
        protected IProductTagService ProductTagService { get; }
        protected ISettingService SettingService { get; }
        protected IShippingService ShippingService { get; }
        protected IShoppingCartService ShoppingCartService { get; }
        protected ISpecificationAttributeService SpecificationAttributeService { get; }
        protected IStoreContext StoreContext { get; }
        protected IUrlRecordService UrlRecordService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected IWorkContext WorkContext { get; }
        protected VendorSettings VendorSettings { get; }

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
            IProductAttributeFormatter productAttributeFormatter,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IProductTagService productTagService,
            ISettingService settingService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            ISpecificationAttributeService specificationAttributeService,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext,
            VendorSettings vendorSettings)
        {
            AclService = aclService;
            BackInStockSubscriptionService = backInStockSubscriptionService;
            CategoryService = categoryService;
            CopyProductService = copyProductService;
            CustomerActivityService = customerActivityService;
            CustomerService = customerService;
            DiscountService = discountService;
            DownloadService = downloadService;
            ExportManager = exportManager;
            ImportManager = importManager;
            LanguageService = languageService;
            LocalizationService = localizationService;
            LocalizedEntityService = localizedEntityService;
            ManufacturerService = manufacturerService;
            FileProvider = fileProvider;
            NotificationService = notificationService;
            PdfService = pdfService;
            PermissionService = permissionService;
            PictureService = pictureService;
            ProductAttributeParser = productAttributeParser;
            ProductAttributeService = productAttributeService;
            ProductAttributeFormatter = productAttributeFormatter;
            ProductModelFactory = productModelFactory;
            ProductService = productService;
            ProductTagService = productTagService;
            SettingService = settingService;
            ShippingService = shippingService;
            ShoppingCartService = shoppingCartService;
            SpecificationAttributeService = specificationAttributeService;
            StoreContext = storeContext;
            UrlRecordService = urlRecordService;
            GenericAttributeService = genericAttributeService;
            WorkContext = workContext;
            VendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(Product product, ProductModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(product,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
                await LocalizedEntityService.SaveLocalizedValueAsync(product,
                    x => x.ShortDescription,
                    localized.ShortDescription,
                    localized.LanguageId);
                await LocalizedEntityService.SaveLocalizedValueAsync(product,
                    x => x.FullDescription,
                    localized.FullDescription,
                    localized.LanguageId);
                await LocalizedEntityService.SaveLocalizedValueAsync(product,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId);
                await LocalizedEntityService.SaveLocalizedValueAsync(product,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId);
                await LocalizedEntityService.SaveLocalizedValueAsync(product,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId);

                //search engine name
                var seName = await UrlRecordService.ValidateSeNameAsync(product, localized.SeName, localized.Name, false);
                await UrlRecordService.SaveSlugAsync(product, seName, localized.LanguageId);
            }
        }

        protected virtual async Task UpdateLocalesAsync(ProductTag productTag, ProductTagModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(productTag,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                var seName = await UrlRecordService.ValidateSeNameAsync(productTag, string.Empty, localized.Name, false);
                await UrlRecordService.SaveSlugAsync(productTag, seName, localized.LanguageId);
            }
        }

        protected virtual async Task UpdateLocalesAsync(ProductAttributeMapping pam, ProductAttributeMappingModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(pam,
                    x => x.TextPrompt,
                    localized.TextPrompt,
                    localized.LanguageId);
                await LocalizedEntityService.SaveLocalizedValueAsync(pam,
                    x => x.DefaultValue,
                    localized.DefaultValue,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateLocalesAsync(ProductAttributeValue pav, ProductAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                await LocalizedEntityService.SaveLocalizedValueAsync(pav,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdatePictureSeoNamesAsync(Product product)
        {
            foreach (var pp in await ProductService.GetProductPicturesByProductIdAsync(product.Id))
                await PictureService.SetSeoFilenameAsync(pp.PictureId, await PictureService.GetPictureSeNameAsync(product.Name));
        }

        protected virtual async Task SaveProductAclAsync(Product product, ProductModel model)
        {
            product.SubjectToAcl = model.SelectedCustomerRoleIds.Any();
            await ProductService.UpdateProductAsync(product);

            var existingAclRecords = await AclService.GetAclRecordsAsync(product);
            var allCustomerRoles = await CustomerService.GetAllCustomerRolesAsync(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (!existingAclRecords.Any(acl => acl.CustomerRoleId == customerRole.Id))
                        await AclService.InsertAclRecordAsync(product, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        await AclService.DeleteAclRecordAsync(aclRecordToDelete);
                }
            }
        }

        protected virtual async Task SaveCategoryMappingsAsync(Product product, ProductModel model)
        {
            var existingProductCategories = await CategoryService.GetProductCategoriesByProductIdAsync(product.Id, true);

            //delete categories
            foreach (var existingProductCategory in existingProductCategories)
                if (!model.SelectedCategoryIds.Contains(existingProductCategory.CategoryId))
                    await CategoryService.DeleteProductCategoryAsync(existingProductCategory);

            //add categories
            foreach (var categoryId in model.SelectedCategoryIds)
            {
                if (CategoryService.FindProductCategory(existingProductCategories, product.Id, categoryId) == null)
                {
                    //find next display order
                    var displayOrder = 1;
                    var existingCategoryMapping = await CategoryService.GetProductCategoriesByCategoryIdAsync(categoryId, showHidden: true);
                    if (existingCategoryMapping.Any())
                        displayOrder = existingCategoryMapping.Max(x => x.DisplayOrder) + 1;
                    await CategoryService.InsertProductCategoryAsync(new ProductCategory
                    {
                        ProductId = product.Id,
                        CategoryId = categoryId,
                        DisplayOrder = displayOrder
                    });
                }
            }
        }

        protected virtual async Task SaveManufacturerMappingsAsync(Product product, ProductModel model)
        {
            var existingProductManufacturers = await ManufacturerService.GetProductManufacturersByProductIdAsync(product.Id, true);

            //delete manufacturers
            foreach (var existingProductManufacturer in existingProductManufacturers)
                if (!model.SelectedManufacturerIds.Contains(existingProductManufacturer.ManufacturerId))
                    await ManufacturerService.DeleteProductManufacturerAsync(existingProductManufacturer);

            //add manufacturers
            foreach (var manufacturerId in model.SelectedManufacturerIds)
            {
                if (ManufacturerService.FindProductManufacturer(existingProductManufacturers, product.Id, manufacturerId) == null)
                {
                    //find next display order
                    var displayOrder = 1;
                    var existingManufacturerMapping = await ManufacturerService.GetProductManufacturersByManufacturerIdAsync(manufacturerId, showHidden: true);
                    if (existingManufacturerMapping.Any())
                        displayOrder = existingManufacturerMapping.Max(x => x.DisplayOrder) + 1;
                    await ManufacturerService.InsertProductManufacturerAsync(new ProductManufacturer
                    {
                        ProductId = product.Id,
                        ManufacturerId = manufacturerId,
                        DisplayOrder = displayOrder
                    });
                }
            }
        }

        protected virtual async Task SaveDiscountMappingsAsync(Product product, ProductModel model)
        {
            var allDiscounts = await DiscountService.GetAllDiscountsAsync(DiscountType.AssignedToSkus, showHidden: true);

            foreach (var discount in allDiscounts)
            {
                if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                {
                    //new discount
                    if (await ProductService.GetDiscountAppliedToProductAsync(product.Id, discount.Id) is null)
                        await ProductService.InsertDiscountProductMappingAsync(new DiscountProductMapping { EntityId = product.Id, DiscountId = discount.Id });
                }
                else
                {
                    //remove discount
                    if (await ProductService.GetDiscountAppliedToProductAsync(product.Id, discount.Id) is DiscountProductMapping discountProductMapping)
                        await ProductService.DeleteDiscountProductMappingAsync(discountProductMapping);
                }
            }

            await ProductService.UpdateProductAsync(product);
            await ProductService.UpdateHasDiscountsAppliedAsync(product);
        }

        protected virtual async Task<string> GetAttributesXmlForProductAttributeCombinationAsync(IFormCollection form, List<string> warnings, int productId)
        {
            var attributesXml = string.Empty;

            //get product attribute mappings (exclude non-combinable attributes)
            var attributes = (await ProductAttributeService.GetProductAttributeMappingsByProductIdAsync(productId))
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
                                attributesXml = ProductAttributeParser.AddProductAttribute(attributesXml,
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
                                    attributesXml = ProductAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }

                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        //load read-only (already server-side selected) values
                        var attributeValues = await ProductAttributeService.GetProductAttributeValuesAsync(attribute.Id);
                        foreach (var selectedAttributeId in attributeValues
                            .Where(v => v.IsPreSelected)
                            .Select(v => v.Id)
                            .ToList())
                        {
                            attributesXml = ProductAttributeParser.AddProductAttribute(attributesXml,
                                attribute, selectedAttributeId.ToString());
                        }

                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        ctrlAttributes = form[controlId];
                        if (!string.IsNullOrEmpty(ctrlAttributes))
                        {
                            var enteredText = ctrlAttributes.ToString().Trim();
                            attributesXml = ProductAttributeParser.AddProductAttribute(attributesXml,
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
                            attributesXml = ProductAttributeParser.AddProductAttribute(attributesXml,
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
                                        await LocalizationService.GetResourceAsync("ShoppingCart.MaximumUploadedFileSize"),
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
                                    DownloadBinary = await DownloadService.GetDownloadBitsAsync(httpPostedFile),
                                    ContentType = httpPostedFile.ContentType,
                                    Filename = FileProvider.GetFileNameWithoutExtension(httpPostedFile.FileName),
                                    Extension = FileProvider.GetFileExtension(httpPostedFile.FileName),
                                    IsNew = true
                                };
                                await DownloadService.InsertDownloadAsync(download);

                                //save attribute
                                attributesXml = ProductAttributeParser.AddProductAttribute(attributesXml,
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
                var conditionMet = await ProductAttributeParser.IsConditionMetAsync(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                {
                    attributesXml = ProductAttributeParser.RemoveProductAttribute(attributesXml, attribute);
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

        protected virtual async Task SaveProductWarehouseInventoryAsync(Product product, ProductModel model)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (model.ManageInventoryMethodId != (int)ManageInventoryMethod.ManageStock)
                return;

            if (!model.UseMultipleWarehouses)
                return;

            var warehouses = await ShippingService.GetAllWarehousesAsync();

            var formData = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());

            foreach (var warehouse in warehouses)
            {
                //parse stock quantity
                var stockQuantity = 0;
                foreach (var formKey in formData.Keys)
                {
                    if (!formKey.Equals($"warehouse_qty_{warehouse.Id}", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    _ = int.TryParse(formData[formKey], out stockQuantity);
                    break;
                }

                //parse reserved quantity
                var reservedQuantity = 0;
                foreach (var formKey in formData.Keys)
                    if (formKey.Equals($"warehouse_reserved_{warehouse.Id}", StringComparison.InvariantCultureIgnoreCase))
                    {
                        _ = int.TryParse(formData[formKey], out reservedQuantity);
                        break;
                    }

                //parse "used" field
                var used = false;
                foreach (var formKey in formData.Keys)
                    if (formKey.Equals($"warehouse_used_{warehouse.Id}", StringComparison.InvariantCultureIgnoreCase))
                    {
                        _ = int.TryParse(formData[formKey], out var tmp);
                        used = tmp == warehouse.Id;
                        break;
                    }

                //quantity change history message
                var message = $"{await LocalizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.MultipleWarehouses")} {await LocalizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.Edit")}";

                var existingPwI = (await ProductService.GetAllProductWarehouseInventoryRecordsAsync(product.Id)).FirstOrDefault(x => x.WarehouseId == warehouse.Id);
                if (existingPwI != null)
                {
                    if (used)
                    {
                        var previousStockQuantity = existingPwI.StockQuantity;

                        //update existing record
                        existingPwI.StockQuantity = stockQuantity;
                        existingPwI.ReservedQuantity = reservedQuantity;
                        await ProductService.UpdateProductWarehouseInventoryAsync(existingPwI);

                        //quantity change history
                        await ProductService.AddStockQuantityHistoryEntryAsync(product, existingPwI.StockQuantity - previousStockQuantity, existingPwI.StockQuantity,
                            existingPwI.WarehouseId, message);
                    }
                    else
                    {
                        //delete. no need to store record for qty 0
                        await ProductService.DeleteProductWarehouseInventoryAsync(existingPwI);

                        //quantity change history
                        await ProductService.AddStockQuantityHistoryEntryAsync(product, -existingPwI.StockQuantity, 0, existingPwI.WarehouseId, message);
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

                    await ProductService.InsertProductWarehouseInventoryAsync(existingPwI);

                    //quantity change history
                    await ProductService.AddStockQuantityHistoryEntryAsync(product, existingPwI.StockQuantity, existingPwI.StockQuantity,
                        existingPwI.WarehouseId, message);
                }
            }
        }

        protected virtual async Task SaveConditionAttributesAsync(ProductAttributeMapping productAttributeMapping,
            ProductAttributeConditionModel model, IFormCollection form)
        {
            string attributesXml = null;
            if (model.EnableCondition)
            {
                var attribute = await ProductAttributeService.GetProductAttributeMappingByIdAsync(model.SelectedProductAttributeId);
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
                                attributesXml = ProductAttributeParser.AddProductAttribute(null, attribute,
                                    selectedAttributeId > 0 ? selectedAttributeId.ToString() : string.Empty);
                            }
                            else
                            {
                                //for conditions we should empty values save even when nothing is selected
                                //otherwise "attributesXml" will be empty
                                //hence we won't be able to find a selected attribute
                                attributesXml = ProductAttributeParser.AddProductAttribute(null,
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

                                    attributesXml = ProductAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                                    anyValueSelected = true;
                                }

                                if (!anyValueSelected)
                                {
                                    //for conditions we should save empty values even when nothing is selected
                                    //otherwise "attributesXml" will be empty
                                    //hence we won't be able to find a selected attribute
                                    attributesXml = ProductAttributeParser.AddProductAttribute(null,
                                        attribute, string.Empty);
                                }
                            }
                            else
                            {
                                //for conditions we should save empty values even when nothing is selected
                                //otherwise "attributesXml" will be empty
                                //hence we won't be able to find a selected attribute
                                attributesXml = ProductAttributeParser.AddProductAttribute(null,
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
            await ProductAttributeService.UpdateProductAttributeMappingAsync(productAttributeMapping);
        }

        protected virtual async Task GenerateAttributeCombinationsAsync(Product product, IList<int> allowedAttributeIds = null)
        {
            var allAttributesXml = await ProductAttributeParser.GenerateAllCombinationsAsync(product, true, allowedAttributeIds);
            foreach (var attributesXml in allAttributesXml)
            {
                var existingCombination = await ProductAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);

                //already exists?
                if (existingCombination != null)
                    continue;

                //new one
                var warnings = new List<string>();
                warnings.AddRange(await ShoppingCartService.GetShoppingCartItemAttributeWarningsAsync(await WorkContext.GetCurrentCustomerAsync(),
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
                await ProductAttributeService.InsertProductAttributeCombinationAsync(combination);
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
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model = await ProductModelFactory.PrepareProductSearchModelAsync(new ProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductList(ProductSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ProductModelFactory.PrepareProductListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-product-by-sku")]
        public virtual async Task<IActionResult> GoToSku(ProductSearchModel searchModel)
        {
            //try to load a product entity, if not found, then try to load a product attribute combination
            var productId = (await ProductService.GetProductBySkuAsync(searchModel.GoDirectlyToSku))?.Id
                ?? (await ProductAttributeService.GetProductAttributeCombinationBySkuAsync(searchModel.GoDirectlyToSku))?.ProductId;

            if (productId != null)
                return RedirectToAction("Edit", "Product", new { id = productId });

            //not found
            return await List();
        }

        public virtual async Task<IActionResult> Create(bool showtour = false)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //validate maximum number of products per vendor
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (VendorSettings.MaximumProductNumber > 0 && currentVendor != null
                && await ProductService.GetNumberOfProductsByVendorIdAsync(currentVendor.Id) >= VendorSettings.MaximumProductNumber)
            {
                NotificationService.ErrorNotification(string.Format(await LocalizationService.GetResourceAsync("Admin.Catalog.Products.ExceededMaximumNumber"),
                    VendorSettings.MaximumProductNumber));
                return RedirectToAction("List");
            }

            //prepare model
            var model = await ProductModelFactory.PrepareProductModelAsync(new ProductModel(), null);

            //show configuration tour
            if (showtour)
            {
                var customer = await WorkContext.GetCurrentCustomerAsync();
                var hideCard = await GenericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.HideConfigurationStepsAttribute);
                var closeCard = await GenericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.CloseConfigurationStepsAttribute);

                if (!hideCard && !closeCard)
                    ViewBag.ShowTour = true;
            }

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(ProductModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //validate maximum number of products per vendor
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (VendorSettings.MaximumProductNumber > 0 && currentVendor != null
                && await ProductService.GetNumberOfProductsByVendorIdAsync(currentVendor.Id) >= VendorSettings.MaximumProductNumber)
            {
                NotificationService.ErrorNotification(string.Format(await LocalizationService.GetResourceAsync("Admin.Catalog.Products.ExceededMaximumNumber"),
                    VendorSettings.MaximumProductNumber));
                return RedirectToAction("List");
            }

            if (ModelState.IsValid)
            {
                //a vendor should have access only to his products
                if (currentVendor != null)
                    model.VendorId = currentVendor.Id;

                //vendors cannot edit "Show on home page" property
                if (currentVendor != null && model.ShowOnHomepage)
                    model.ShowOnHomepage = false;

                //product
                var product = model.ToEntity<Product>();
                product.CreatedOnUtc = DateTime.UtcNow;
                product.UpdatedOnUtc = DateTime.UtcNow;
                await ProductService.InsertProductAsync(product);

                //search engine name
                model.SeName = await UrlRecordService.ValidateSeNameAsync(product, model.SeName, product.Name, true);
                await UrlRecordService.SaveSlugAsync(product, model.SeName, 0);

                //locales
                await UpdateLocalesAsync(product, model);

                //categories
                await SaveCategoryMappingsAsync(product, model);

                //manufacturers
                await SaveManufacturerMappingsAsync(product, model);

                //ACL (customer roles)
                await SaveProductAclAsync(product, model);

                //stores
                await ProductService.UpdateProductStoreMappingsAsync(product, model.SelectedStoreIds);

                //discounts
                await SaveDiscountMappingsAsync(product, model);

                //tags
                await ProductTagService.UpdateProductTagsAsync(product, ParseProductTags(model.ProductTags));

                //warehouses
                await SaveProductWarehouseInventoryAsync(product, model);

                //quantity change history
                await ProductService.AddStockQuantityHistoryEntryAsync(product, product.StockQuantity, product.StockQuantity, product.WarehouseId,
                    await LocalizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.Edit"));

                //activity log
                await CustomerActivityService.InsertActivityAsync("AddNewProduct",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewProduct"), product.Name), product);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Products.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = product.Id });
            }

            //prepare model
            model = await ProductModelFactory.PrepareProductModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(id);
            if (product == null || product.Deleted)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return RedirectToAction("List");

            //prepare model
            var model = await ProductModelFactory.PrepareProductModelAsync(null, product);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(ProductModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(model.Id);
            if (product == null || product.Deleted)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return RedirectToAction("List");

            //check if the product quantity has been changed while we were editing the product
            //and if it has been changed then we show error notification
            //and redirect on the editing page without data saving
            if (product.StockQuantity != model.LastStockQuantity)
            {
                NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Products.Fields.StockQuantity.ChangedWarning"));
                return RedirectToAction("Edit", new { id = product.Id });
            }

            if (ModelState.IsValid)
            {
                //a vendor should have access only to his products
                if (currentVendor != null)
                    model.VendorId = currentVendor.Id;

                //we do not validate maximum number of products per vendor when editing existing products (only during creation of new products)
                //vendors cannot edit "Show on home page" property
                if (currentVendor != null && model.ShowOnHomepage != product.ShowOnHomepage)
                    model.ShowOnHomepage = product.ShowOnHomepage;

                //some previously used values
                var prevTotalStockQuantity = await ProductService.GetTotalStockQuantityAsync(product);
                var prevDownloadId = product.DownloadId;
                var prevSampleDownloadId = product.SampleDownloadId;
                var previousStockQuantity = product.StockQuantity;
                var previousWarehouseId = product.WarehouseId;
                var previousProductType = product.ProductType;

                //product
                product = model.ToEntity(product);

                product.UpdatedOnUtc = DateTime.UtcNow;
                await ProductService.UpdateProductAsync(product);

                //remove associated products
                if (previousProductType == ProductType.GroupedProduct && product.ProductType == ProductType.SimpleProduct)
                {
                    var store = await StoreContext.GetCurrentStoreAsync();
                    var storeId = store?.Id ?? 0;
                    var vendorId = currentVendor?.Id ?? 0;

                    var associatedProducts = await ProductService.GetAssociatedProductsAsync(product.Id, storeId, vendorId);
                    foreach (var associatedProduct in associatedProducts)
                    {
                        associatedProduct.ParentGroupedProductId = 0;
                        await ProductService.UpdateProductAsync(associatedProduct);
                    }
                }

                //search engine name
                model.SeName = await UrlRecordService.ValidateSeNameAsync(product, model.SeName, product.Name, true);
                await UrlRecordService.SaveSlugAsync(product, model.SeName, 0);

                //locales
                await UpdateLocalesAsync(product, model);

                //tags
                await ProductTagService.UpdateProductTagsAsync(product, ParseProductTags(model.ProductTags));

                //warehouses
                await SaveProductWarehouseInventoryAsync(product, model);

                //categories
                await SaveCategoryMappingsAsync(product, model);

                //manufacturers
                await SaveManufacturerMappingsAsync(product, model);

                //ACL (customer roles)
                await SaveProductAclAsync(product, model);

                //stores
                await ProductService.UpdateProductStoreMappingsAsync(product, model.SelectedStoreIds);

                //discounts
                await SaveDiscountMappingsAsync(product, model);

                //picture seo names
                await UpdatePictureSeoNamesAsync(product);

                //back in stock notifications
                if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                    product.BackorderMode == BackorderMode.NoBackorders &&
                    product.AllowBackInStockSubscriptions &&
                    await ProductService.GetTotalStockQuantityAsync(product) > 0 &&
                    prevTotalStockQuantity <= 0 &&
                    product.Published &&
                    !product.Deleted)
                {
                    await BackInStockSubscriptionService.SendNotificationsToSubscribersAsync(product);
                }

                //delete an old "download" file (if deleted or updated)
                if (prevDownloadId > 0 && prevDownloadId != product.DownloadId)
                {
                    var prevDownload = await DownloadService.GetDownloadByIdAsync(prevDownloadId);
                    if (prevDownload != null)
                        await DownloadService.DeleteDownloadAsync(prevDownload);
                }

                //delete an old "sample download" file (if deleted or updated)
                if (prevSampleDownloadId > 0 && prevSampleDownloadId != product.SampleDownloadId)
                {
                    var prevSampleDownload = await DownloadService.GetDownloadByIdAsync(prevSampleDownloadId);
                    if (prevSampleDownload != null)
                        await DownloadService.DeleteDownloadAsync(prevSampleDownload);
                }

                //quantity change history
                if (previousWarehouseId != product.WarehouseId)
                {
                    //warehouse is changed 
                    //compose a message
                    var oldWarehouseMessage = string.Empty;
                    if (previousWarehouseId > 0)
                    {
                        var oldWarehouse = await ShippingService.GetWarehouseByIdAsync(previousWarehouseId);
                        if (oldWarehouse != null)
                            oldWarehouseMessage = string.Format(await LocalizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditWarehouse.Old"), oldWarehouse.Name);
                    }

                    var newWarehouseMessage = string.Empty;
                    if (product.WarehouseId > 0)
                    {
                        var newWarehouse = await ShippingService.GetWarehouseByIdAsync(product.WarehouseId);
                        if (newWarehouse != null)
                            newWarehouseMessage = string.Format(await LocalizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditWarehouse.New"), newWarehouse.Name);
                    }

                    var message = string.Format(await LocalizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.EditWarehouse"), oldWarehouseMessage, newWarehouseMessage);

                    //record history
                    await ProductService.AddStockQuantityHistoryEntryAsync(product, -previousStockQuantity, 0, previousWarehouseId, message);
                    await ProductService.AddStockQuantityHistoryEntryAsync(product, product.StockQuantity, product.StockQuantity, product.WarehouseId, message);
                }
                else
                {
                    await ProductService.AddStockQuantityHistoryEntryAsync(product, product.StockQuantity - previousStockQuantity, product.StockQuantity,
                        product.WarehouseId, await LocalizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.Edit"));
                }

                //activity log
                await CustomerActivityService.InsertActivityAsync("EditProduct",
                    string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditProduct"), product.Name), product);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Products.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = product.Id });
            }

            //prepare model
            model = await ProductModelFactory.PrepareProductModelAsync(model, product, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(id);
            if (product == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return RedirectToAction("List");

            await ProductService.DeleteProductAsync(product);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteProduct",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteProduct"), product.Name), product);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Products.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            await ProductService.DeleteProductsAsync((await ProductService.GetProductsByIdsAsync(selectedIds.ToArray()))
                .Where(p => currentVendor == null || p.VendorId == currentVendor.Id).ToList());

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> CopyProduct(ProductModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var copyModel = model.CopyProductModel;
            try
            {
                var originalProduct = await ProductService.GetProductByIdAsync(copyModel.Id);

                //a vendor should have access only to his products
                var currentVendor = await WorkContext.GetCurrentVendorAsync();
                if (currentVendor != null && originalProduct.VendorId != currentVendor.Id)
                    return RedirectToAction("List");

                var newProduct = await CopyProductService.CopyProductAsync(originalProduct, copyModel.Name, copyModel.Published, copyModel.CopyImages);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Products.Copied"));

                return RedirectToAction("Edit", new { id = newProduct.Id });
            }
            catch (Exception exc)
            {
                NotificationService.ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = copyModel.Id });
            }
        }

        //action displaying notification (warning) to a store owner that entered SKU already exists
        public virtual async Task<IActionResult> SkuReservedWarning(int productId, string sku)
        {
            string message;

            //check whether product with passed SKU already exists
            var productBySku = await ProductService.GetProductBySkuAsync(sku);
            if (productBySku != null)
            {
                if (productBySku.Id == productId)
                    return Json(new { Result = string.Empty });

                message = string.Format(await LocalizationService.GetResourceAsync("Admin.Catalog.Products.Fields.Sku.Reserved"), productBySku.Name);
                return Json(new { Result = message });
            }

            //check whether combination with passed SKU already exists
            var combinationBySku = await ProductAttributeService.GetProductAttributeCombinationBySkuAsync(sku);
            if (combinationBySku == null)
                return Json(new { Result = string.Empty });

            message = string.Format(await LocalizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.Sku.Reserved"),
                (await ProductService.GetProductByIdAsync(combinationBySku.ProductId))?.Name);

            return Json(new { Result = message });
        }

        #endregion

        #region Required products

        [HttpPost]
        public virtual async Task<IActionResult> LoadProductFriendlyNames(string productIds)
        {
            var result = string.Empty;

            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
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

            var products = await ProductService.GetProductsByIdsAsync(ids.ToArray());
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
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model = await ProductModelFactory.PrepareAddRequiredProductSearchModelAsync(new AddRequiredProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> RequiredProductAddPopupList(AddRequiredProductSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ProductModelFactory.PrepareAddRequiredProductListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #region Related products

        [HttpPost]
        public virtual async Task<IActionResult> RelatedProductList(RelatedProductSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return Content("This is not your product");

            //prepare model
            var model = await ProductModelFactory.PrepareRelatedProductListModelAsync(searchModel, product);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> RelatedProductUpdate(RelatedProductModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a related product with the specified id
            var relatedProduct = await ProductService.GetRelatedProductByIdAsync(model.Id)
                ?? throw new ArgumentException("No related product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                var product = await ProductService.GetProductByIdAsync(relatedProduct.ProductId1);
                if (product != null && product.VendorId != currentVendor.Id)
                    return Content("This is not your product");
            }

            relatedProduct.DisplayOrder = model.DisplayOrder;
            await ProductService.UpdateRelatedProductAsync(relatedProduct);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> RelatedProductDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a related product with the specified id
            var relatedProduct = await ProductService.GetRelatedProductByIdAsync(id)
                ?? throw new ArgumentException("No related product found with the specified id");

            var productId = relatedProduct.ProductId1;

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                var product = await ProductService.GetProductByIdAsync(productId);
                if (product != null && product.VendorId != currentVendor.Id)
                    return Content("This is not your product");
            }

            await ProductService.DeleteRelatedProductAsync(relatedProduct);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> RelatedProductAddPopup(int productId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model = await ProductModelFactory.PrepareAddRelatedProductSearchModelAsync(new AddRelatedProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> RelatedProductAddPopupList(AddRelatedProductSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ProductModelFactory.PrepareAddRelatedProductListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> RelatedProductAddPopup(AddRelatedProductModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var selectedProducts = await ProductService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());
            if (selectedProducts.Any())
            {
                var existingRelatedProducts = await ProductService.GetRelatedProductsByProductId1Async(model.ProductId, showHidden: true);
                var currentVendor = await WorkContext.GetCurrentVendorAsync();
                foreach (var product in selectedProducts)
                {
                    //a vendor should have access only to his products
                    if (currentVendor != null && product.VendorId != currentVendor.Id)
                        continue;

                    if (ProductService.FindRelatedProduct(existingRelatedProducts, model.ProductId, product.Id) != null)
                        continue;

                    await ProductService.InsertRelatedProductAsync(new RelatedProduct
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
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return Content("This is not your product");

            //prepare model
            var model = await ProductModelFactory.PrepareCrossSellProductListModelAsync(searchModel, product);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CrossSellProductDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a cross-sell product with the specified id
            var crossSellProduct = await ProductService.GetCrossSellProductByIdAsync(id)
                ?? throw new ArgumentException("No cross-sell product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                var product = await ProductService.GetProductByIdAsync(crossSellProduct.ProductId1);
                if (product != null && product.VendorId != currentVendor.Id)
                    return Content("This is not your product");
            }

            await ProductService.DeleteCrossSellProductAsync(crossSellProduct);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> CrossSellProductAddPopup(int productId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model = await ProductModelFactory.PrepareAddCrossSellProductSearchModelAsync(new AddCrossSellProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CrossSellProductAddPopupList(AddCrossSellProductSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ProductModelFactory.PrepareAddCrossSellProductListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> CrossSellProductAddPopup(AddCrossSellProductModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var selectedProducts = await ProductService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());
            if (selectedProducts.Any())
            {
                var existingCrossSellProducts = await ProductService.GetCrossSellProductsByProductId1Async(model.ProductId, showHidden: true);
                var currentVendor = await WorkContext.GetCurrentVendorAsync();
                foreach (var product in selectedProducts)
                {
                    //a vendor should have access only to his products
                    if (currentVendor != null && product.VendorId != currentVendor.Id)
                        continue;

                    if (ProductService.FindCrossSellProduct(existingCrossSellProducts, model.ProductId, product.Id) != null)
                        continue;

                    await ProductService.InsertCrossSellProductAsync(new CrossSellProduct
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
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return Content("This is not your product");

            //prepare model
            var model = await ProductModelFactory.PrepareAssociatedProductListModelAsync(searchModel, product);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AssociatedProductUpdate(AssociatedProductModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get an associated product with the specified id
            var associatedProduct = await ProductService.GetProductByIdAsync(model.Id)
                ?? throw new ArgumentException("No associated product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && associatedProduct.VendorId != currentVendor.Id)
                return Content("This is not your product");

            associatedProduct.DisplayOrder = model.DisplayOrder;
            await ProductService.UpdateProductAsync(associatedProduct);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> AssociatedProductDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get an associated product with the specified id
            var product = await ProductService.GetProductByIdAsync(id)
                ?? throw new ArgumentException("No associated product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return Content("This is not your product");

            product.ParentGroupedProductId = 0;
            await ProductService.UpdateProductAsync(product);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> AssociatedProductAddPopup(int productId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model = await ProductModelFactory.PrepareAddAssociatedProductSearchModelAsync(new AddAssociatedProductSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AssociatedProductAddPopupList(AddAssociatedProductSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ProductModelFactory.PrepareAddAssociatedProductListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> AssociatedProductAddPopup(AddAssociatedProductModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var selectedProducts = await ProductService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());

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
                    var currentVendor = await WorkContext.GetCurrentVendorAsync();
                    if (currentVendor != null && product.VendorId != currentVendor.Id)
                        continue;

                    product.ParentGroupedProductId = model.ProductId;
                    await ProductService.UpdateProductAsync(product);
                }
            }

            if (tryToAddSelfGroupedProduct)
            {
                NotificationService.WarningNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Products.AssociatedProducts.TryToAddSelfGroupedProduct"));

                var addAssociatedProductSearchModel = await ProductModelFactory.PrepareAddAssociatedProductSearchModelAsync(new AddAssociatedProductSearchModel());
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
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (pictureId == 0)
                throw new ArgumentException();

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(productId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return RedirectToAction("List");

            if ((await ProductService.GetProductPicturesByProductIdAsync(productId)).Any(p => p.PictureId == pictureId))
                return Json(new { Result = false });

            //try to get a picture with the specified id
            var picture = await PictureService.GetPictureByIdAsync(pictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            await PictureService.UpdatePictureAsync(picture.Id,
                await PictureService.LoadPictureBinaryAsync(picture),
                picture.MimeType,
                picture.SeoFilename,
                overrideAltAttribute,
                overrideTitleAttribute);

            await PictureService.SetSeoFilenameAsync(pictureId, await PictureService.GetPictureSeNameAsync(product.Name));

            await ProductService.InsertProductPictureAsync(new ProductPicture
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
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return Content("This is not your product");

            //prepare model
            var model = await ProductModelFactory.PrepareProductPictureListModelAsync(searchModel, product);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductPictureUpdate(ProductPictureModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product picture with the specified id
            var productPicture = await ProductService.GetProductPictureByIdAsync(model.Id)
                ?? throw new ArgumentException("No product picture found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                var product = await ProductService.GetProductByIdAsync(productPicture.ProductId);
                if (product != null && product.VendorId != currentVendor.Id)
                    return Content("This is not your product");
            }

            //try to get a picture with the specified id
            var picture = await PictureService.GetPictureByIdAsync(productPicture.PictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            await PictureService.UpdatePictureAsync(picture.Id,
                await PictureService.LoadPictureBinaryAsync(picture),
                picture.MimeType,
                picture.SeoFilename,
                model.OverrideAltAttribute,
                model.OverrideTitleAttribute);

            productPicture.DisplayOrder = model.DisplayOrder;
            await ProductService.UpdateProductPictureAsync(productPicture);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductPictureDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product picture with the specified id
            var productPicture = await ProductService.GetProductPictureByIdAsync(id)
                ?? throw new ArgumentException("No product picture found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                var product = await ProductService.GetProductByIdAsync(productPicture.ProductId);
                if (product != null && product.VendorId != currentVendor.Id)
                    return Content("This is not your product");
            }

            var pictureId = productPicture.PictureId;
            await ProductService.DeleteProductPictureAsync(productPicture);

            //try to get a picture with the specified id
            var picture = await PictureService.GetPictureByIdAsync(pictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            await PictureService.DeletePictureAsync(picture);

            return new NullJsonResult();
        }

        #endregion

        #region Product specification attributes

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> ProductSpecificationAttributeAdd(AddSpecificationAttributeModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var product = await ProductService.GetProductByIdAsync(model.ProductId);
            if (product == null)
            {
                NotificationService.ErrorNotification("No product found with the specified id");
                return RedirectToAction("List");
            }

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
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
            await SpecificationAttributeService.InsertProductSpecificationAttributeAsync(psa);

            switch (psa.AttributeType)
            {
                case SpecificationAttributeType.CustomText:
                    foreach (var localized in model.Locales)
                    {
                        await LocalizedEntityService.SaveLocalizedValueAsync(psa,
                            x => x.CustomValue,
                            localized.Value,
                            localized.LanguageId);
                    }

                    break;
                case SpecificationAttributeType.CustomHtmlText:
                    foreach (var localized in model.Locales)
                    {
                        await LocalizedEntityService.SaveLocalizedValueAsync(psa,
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

            //select an appropriate card
            SaveSelectedCardName("product-specification-attributes");
            return RedirectToAction("Edit", new { id = model.ProductId });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductSpecAttrList(ProductSpecificationAttributeSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return Content("This is not your product");

            //prepare model
            var model = await ProductModelFactory.PrepareProductSpecificationAttributeListModelAsync(searchModel, product);

            return Json(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> ProductSpecAttrUpdate(AddSpecificationAttributeModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product specification attribute with the specified id
            var psa = await SpecificationAttributeService.GetProductSpecificationAttributeByIdAsync(model.SpecificationId);
            if (psa == null)
            {
                //select an appropriate card
                SaveSelectedCardName("product-specification-attributes");
                NotificationService.ErrorNotification("No product specification attribute found with the specified id");

                return RedirectToAction("Edit", new { id = model.ProductId });
            }

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null
                && (await ProductService.GetProductByIdAsync(psa.ProductId)).VendorId != currentVendor.Id)
            {
                NotificationService.ErrorNotification("This is not your product");

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
                        await LocalizedEntityService.SaveLocalizedValueAsync(psa,
                            x => x.CustomValue,
                            localized.ValueRaw,
                            localized.LanguageId);
                    }

                    break;
                case (int)SpecificationAttributeType.CustomText:
                    psa.CustomValue = model.Value;
                    foreach (var localized in model.Locales)
                    {
                        await LocalizedEntityService.SaveLocalizedValueAsync(psa,
                            x => x.CustomValue,
                            localized.Value,
                            localized.LanguageId);
                    }

                    break;
                default:
                    psa.CustomValue = model.Value;

                    break;
            }

            psa.ShowOnProductPage = model.ShowOnProductPage;
            psa.DisplayOrder = model.DisplayOrder;
            await SpecificationAttributeService.UpdateProductSpecificationAttributeAsync(psa);

            if (continueEditing)
            {
                return RedirectToAction("ProductSpecAttributeAddOrEdit",
                    new { productId = psa.ProductId, specificationId = model.SpecificationId });
            }

            //select an appropriate card
            SaveSelectedCardName("product-specification-attributes");

            return RedirectToAction("Edit", new { id = psa.ProductId });
        }

        public virtual async Task<IActionResult> ProductSpecAttributeAddOrEdit(int productId, int? specificationId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (await ProductService.GetProductByIdAsync(productId) == null)
            {
                NotificationService.ErrorNotification("No product found with the specified id");
                return RedirectToAction("List");
            }

            //try to get a product specification attribute with the specified id
            try
            {
                var model = await ProductModelFactory.PrepareAddSpecificationAttributeModelAsync(productId, specificationId);
                return View(model);
            }
            catch (Exception ex)
            {
                await NotificationService.ErrorNotificationAsync(ex);

                //select an appropriate card
                SaveSelectedCardName("product-specification-attributes");
                return RedirectToAction("Edit", new { id = productId });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductSpecAttrDelete(AddSpecificationAttributeModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product specification attribute with the specified id
            var psa = await SpecificationAttributeService.GetProductSpecificationAttributeByIdAsync(model.SpecificationId);
            if (psa == null)
            {
                //select an appropriate card
                SaveSelectedCardName("product-specification-attributes");
                NotificationService.ErrorNotification("No product specification attribute found with the specified id");
                return RedirectToAction("Edit", new { id = model.ProductId });
            }

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && (await ProductService.GetProductByIdAsync(psa.ProductId)).VendorId != currentVendor.Id)
            {
                NotificationService.ErrorNotification("This is not your product");
                return RedirectToAction("List", new { id = model.ProductId });
            }

            await SpecificationAttributeService.DeleteProductSpecificationAttributeAsync(psa);

            //select an appropriate card
            SaveSelectedCardName("product-specification-attributes");

            return RedirectToAction("Edit", new { id = psa.ProductId });
        }

        #endregion

        #region Product tags

        public virtual async Task<IActionResult> ProductTags()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedView();

            //prepare model
            var model = await ProductModelFactory.PrepareProductTagSearchModelAsync(new ProductTagSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductTags(ProductTagSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductTags))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ProductModelFactory.PrepareProductTagListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductTagDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedView();

            //try to get a product tag with the specified id
            var tag = await ProductTagService.GetProductTagByIdAsync(id)
                ?? throw new ArgumentException("No product tag found with the specified id");

            await ProductTagService.DeleteProductTagAsync(tag);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.ProductTags.Deleted"));

            return RedirectToAction("ProductTags");
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductTagsDelete(ICollection<int> selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            var tags = await ProductTagService.GetProductTagsByIdsAsync(selectedIds.ToArray());
            await ProductTagService.DeleteProductTagsAsync(tags);

            return Json(new { Result = true });
        }

        public virtual async Task<IActionResult> EditProductTag(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedView();

            //try to get a product tag with the specified id
            var productTag = await ProductTagService.GetProductTagByIdAsync(id);
            if (productTag == null)
                return RedirectToAction("List");

            //prepare tag model
            var model = await ProductModelFactory.PrepareProductTagModelAsync(null, productTag);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> EditProductTag(ProductTagModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProductTags))
                return AccessDeniedView();

            //try to get a product tag with the specified id
            var productTag = await ProductTagService.GetProductTagByIdAsync(model.Id);
            if (productTag == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                productTag.Name = model.Name;
                await ProductTagService.UpdateProductTagAsync(productTag);

                //locales
                await UpdateLocalesAsync(productTag, model);

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.ProductTags.Updated"));

                return continueEditing ? RedirectToAction("EditProductTag", new { id = productTag.Id }) : RedirectToAction("ProductTags");
            }

            //prepare model
            model = await ProductModelFactory.PrepareProductTagModelAsync(model, productTag, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Purchased with order

        [HttpPost]
        public virtual async Task<IActionResult> PurchasedWithOrders(ProductOrderSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return Content("This is not your product");

            //prepare model
            var model = await ProductModelFactory.PrepareProductOrderListModelAsync(searchModel, product);

            return Json(model);
        }

        #endregion

        #region Export / Import

        [HttpPost, ActionName("DownloadCatalogPDF")]
        [FormValueRequired("download-catalog-pdf")]
        public virtual async Task<IActionResult> DownloadCatalogAsPdf(ProductSearchModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                model.SearchVendorId = currentVendor.Id;
            }

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                categoryIds.AddRange(await CategoryService.GetChildCategoryIdsAsync(parentCategoryId: model.SearchCategoryId, showHidden: true));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var products = await ProductService.SearchProductsAsync(0,
                categoryIds: categoryIds,
                manufacturerIds: new List<int> { model.SearchManufacturerId },
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
                    await PdfService.PrintProductsToPdfAsync(stream, products);
                    bytes = stream.ToArray();
                }

                return File(bytes, MimeTypes.ApplicationPdf, "pdfcatalog.pdf");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost, ActionName("ExportToXml")]
        [FormValueRequired("exportxml-all")]
        public virtual async Task<IActionResult> ExportXmlAll(ProductSearchModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                model.SearchVendorId = currentVendor.Id;
            }

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                categoryIds.AddRange(await CategoryService.GetChildCategoryIdsAsync(parentCategoryId: model.SearchCategoryId, showHidden: true));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var products = await ProductService.SearchProductsAsync(0,
                categoryIds: categoryIds,
                manufacturerIds: new List<int> { model.SearchManufacturerId },
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                warehouseId: model.SearchWarehouseId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                showHidden: true,
                overridePublished: overridePublished);

            try
            {
                var xml = await ExportManager.ExportProductsToXmlAsync(products);

                return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "products.xml");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ExportXmlSelected(string selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var products = new List<Product>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                products.AddRange(await ProductService.GetProductsByIdsAsync(ids));
            }
            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                products = products.Where(p => p.VendorId == currentVendor.Id).ToList();
            }

            try
            {
                var xml = await ExportManager.ExportProductsToXmlAsync(products);
                return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "products.xml");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost, ActionName("ExportToExcel")]
        [FormValueRequired("exportexcel-all")]
        public virtual async Task<IActionResult> ExportExcelAll(ProductSearchModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                model.SearchVendorId = currentVendor.Id;
            }

            var categoryIds = new List<int> { model.SearchCategoryId };
            //include subcategories
            if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                categoryIds.AddRange(await CategoryService.GetChildCategoryIdsAsync(parentCategoryId: model.SearchCategoryId, showHidden: true));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? overridePublished = null;
            if (model.SearchPublishedId == 1)
                overridePublished = true;
            else if (model.SearchPublishedId == 2)
                overridePublished = false;

            var products = await ProductService.SearchProductsAsync(0,
                categoryIds: categoryIds,
                manufacturerIds: new List<int> { model.SearchManufacturerId },
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                warehouseId: model.SearchWarehouseId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                showHidden: true,
                overridePublished: overridePublished);

            try
            {
                var bytes = await ExportManager.ExportProductsToXlsxAsync(products);

                return File(bytes, MimeTypes.TextXlsx, "products.xlsx");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);

                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ExportExcelSelected(string selectedIds)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var products = new List<Product>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                products.AddRange(await ProductService.GetProductsByIdsAsync(ids));
            }
            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                products = products.Where(p => p.VendorId == currentVendor.Id).ToList();
            }

            try
            {
                var bytes = await ExportManager.ExportProductsToXlsxAsync(products);

                return File(bytes, MimeTypes.TextXlsx, "products.xlsx");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportExcel(IFormFile importexcelfile)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (await WorkContext.GetCurrentVendorAsync() != null && !VendorSettings.AllowVendorsToImportProducts)
                //a vendor can not import products
                return AccessDeniedView();

            try
            {
                if (importexcelfile != null && importexcelfile.Length > 0)
                {
                    await ImportManager.ImportProductsFromXlsxAsync(importexcelfile.OpenReadStream());
                }
                else
                {
                    NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Common.UploadFile"));
                    
                    return RedirectToAction("List");
                }

                NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Products.Imported"));
                
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                await NotificationService.ErrorNotificationAsync(exc);
                
                return RedirectToAction("List");
            }
        }

        #endregion

        #region Tier prices

        [HttpPost]
        public virtual async Task<IActionResult> TierPriceList(TierPriceSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return Content("This is not your product");

            //prepare model
            var model = await ProductModelFactory.PrepareTierPriceListModelAsync(searchModel, product);

            return Json(model);
        }

        public virtual async Task<IActionResult> TierPriceCreatePopup(int productId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(productId)
                ?? throw new ArgumentException("No product found with the specified id");

            //prepare model
            var model = await ProductModelFactory.PrepareTierPriceModelAsync(new TierPriceModel(), product, null);

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> TierPriceCreatePopup(TierPriceModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(model.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return RedirectToAction("List", "Product");

            if (ModelState.IsValid)
            {
                //fill entity from model
                var tierPrice = model.ToEntity<TierPrice>();
                tierPrice.ProductId = product.Id;
                tierPrice.CustomerRoleId = model.CustomerRoleId > 0 ? model.CustomerRoleId : (int?)null;

                await ProductService.InsertTierPriceAsync(tierPrice);

                //update "HasTierPrices" property
                await ProductService.UpdateHasTierPricesPropertyAsync(product);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await ProductModelFactory.PrepareTierPriceModelAsync(model, product, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> TierPriceEditPopup(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a tier price with the specified id
            var tierPrice = await ProductService.GetTierPriceByIdAsync(id);
            if (tierPrice == null)
                return RedirectToAction("List", "Product");

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(tierPrice.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return RedirectToAction("List", "Product");

            //prepare model
            var model = await ProductModelFactory.PrepareTierPriceModelAsync(null, product, tierPrice);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TierPriceEditPopup(TierPriceModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a tier price with the specified id
            var tierPrice = await ProductService.GetTierPriceByIdAsync(model.Id);
            if (tierPrice == null)
                return RedirectToAction("List", "Product");

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(tierPrice.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return RedirectToAction("List", "Product");

            if (ModelState.IsValid)
            {
                //fill entity from model
                tierPrice = model.ToEntity(tierPrice);
                tierPrice.CustomerRoleId = model.CustomerRoleId > 0 ? model.CustomerRoleId : (int?)null;
                await ProductService.UpdateTierPriceAsync(tierPrice);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await ProductModelFactory.PrepareTierPriceModelAsync(model, product, tierPrice, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TierPriceDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a tier price with the specified id
            var tierPrice = await ProductService.GetTierPriceByIdAsync(id)
                ?? throw new ArgumentException("No tier price found with the specified id");

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(tierPrice.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return Content("This is not your product");

            await ProductService.DeleteTierPriceAsync(tierPrice);

            //update "HasTierPrices" property
            await ProductService.UpdateHasTierPricesPropertyAsync(product);

            return new NullJsonResult();
        }

        #endregion

        #region Product attributes

        [HttpPost]
        public virtual async Task<IActionResult> ProductAttributeMappingList(ProductAttributeMappingSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return Content("This is not your product");

            //prepare model
            var model = await ProductModelFactory.PrepareProductAttributeMappingListModelAsync(searchModel, product);

            return Json(model);
        }

        public virtual async Task<IActionResult> ProductAttributeMappingCreate(int productId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(productId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
            {
                NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("This is not your product"));
                return RedirectToAction("List");
            }

            //prepare model
            var model = await ProductModelFactory.PrepareProductAttributeMappingModelAsync(new ProductAttributeMappingModel(), product, null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> ProductAttributeMappingCreate(ProductAttributeMappingModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(model.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
            {
                NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("This is not your product"));
                return RedirectToAction("List");
            }

            //ensure this attribute is not mapped yet
            if ((await ProductAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id))
                .Any(x => x.ProductAttributeId == model.ProductAttributeId))
            {
                //redisplay form
                NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.AlreadyExists"));

                model = await ProductModelFactory.PrepareProductAttributeMappingModelAsync(model, product, null, true);

                return View(model);
            }

            //insert mapping
            var productAttributeMapping = model.ToEntity<ProductAttributeMapping>();

            await ProductAttributeService.InsertProductAttributeMappingAsync(productAttributeMapping);
            await UpdateLocalesAsync(productAttributeMapping, model);

            //predefined values
            var predefinedValues = await ProductAttributeService.GetPredefinedProductAttributeValuesAsync(model.ProductAttributeId);
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
                await ProductAttributeService.InsertProductAttributeValueAsync(pav);

                //locales
                var languages = await LanguageService.GetAllLanguagesAsync(true);

                //localization
                foreach (var lang in languages)
                {
                    var name = await LocalizationService.GetLocalizedAsync(predefinedValue, x => x.Name, lang.Id, false, false);
                    if (!string.IsNullOrEmpty(name))
                        await LocalizedEntityService.SaveLocalizedValueAsync(pav, x => x.Name, name, lang.Id);
                }
            }

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.Added"));

            if (!continueEditing)
            {
                //select an appropriate card
                SaveSelectedCardName("product-product-attributes");
                return RedirectToAction("Edit", new { id = product.Id });
            }

            return RedirectToAction("ProductAttributeMappingEdit", new { id = productAttributeMapping.Id });
        }

        public virtual async Task<IActionResult> ProductAttributeMappingEdit(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = await ProductAttributeService.GetProductAttributeMappingByIdAsync(id)
                ?? throw new ArgumentException("No product attribute mapping found with the specified id");

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
            {
                NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("This is not your product"));
                return RedirectToAction("List");
            }

            //prepare model
            var model = await ProductModelFactory.PrepareProductAttributeMappingModelAsync(null, product, productAttributeMapping);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> ProductAttributeMappingEdit(ProductAttributeMappingModel model, bool continueEditing)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = await ProductAttributeService.GetProductAttributeMappingByIdAsync(model.Id)
                ?? throw new ArgumentException("No product attribute mapping found with the specified id");

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
            {
                NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("This is not your product"));
                return RedirectToAction("List");
            }

            //ensure this attribute is not mapped yet
            if ((await ProductAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id))
                .Any(x => x.ProductAttributeId == model.ProductAttributeId && x.Id != productAttributeMapping.Id))
            {
                //redisplay form
                NotificationService.ErrorNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.AlreadyExists"));

                model = await ProductModelFactory.PrepareProductAttributeMappingModelAsync(model, product, productAttributeMapping, true);

                return View(model);
            }

            //fill entity from model
            productAttributeMapping = model.ToEntity(productAttributeMapping);
            await ProductAttributeService.UpdateProductAttributeMappingAsync(productAttributeMapping);

            await UpdateLocalesAsync(productAttributeMapping, model);

            await SaveConditionAttributesAsync(productAttributeMapping, model.ConditionModel, model.Form);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.Updated"));

            if (!continueEditing)
            {
                //select an appropriate card
                SaveSelectedCardName("product-product-attributes");
                return RedirectToAction("Edit", new { id = product.Id });
            }

            return RedirectToAction("ProductAttributeMappingEdit", new { id = productAttributeMapping.Id });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAttributeMappingDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = await ProductAttributeService.GetProductAttributeMappingByIdAsync(id)
                ?? throw new ArgumentException("No product attribute mapping found with the specified id");

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return Content("This is not your product");

            //check if existed combinations contains the specified attribute
            var existedCombinations = await ProductAttributeService.GetAllProductAttributeCombinationsAsync(product.Id);
            if (existedCombinations?.Any() == true)
            {
                foreach (var combination in existedCombinations)
                {
                    var mappings = await ProductAttributeParser
                        .ParseProductAttributeMappingsAsync(combination.AttributesXml);
                    
                    if (mappings?.Any(m => m.Id == productAttributeMapping.Id) == true)
                    {
                        NotificationService.ErrorNotification(
                            string.Format(await LocalizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.AlreadyExistsInCombination"),
                                await ProductAttributeFormatter.FormatAttributesAsync(product, combination.AttributesXml, await WorkContext.GetCurrentCustomerAsync(), ", ")));

                        return RedirectToAction("ProductAttributeMappingEdit", new { id = productAttributeMapping.Id });
                    }
                }
            }

            await ProductAttributeService.DeleteProductAttributeMappingAsync(productAttributeMapping);

            NotificationService.SuccessNotification(await LocalizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.Deleted"));

            //select an appropriate card
            SaveSelectedCardName("product-product-attributes");
            return RedirectToAction("Edit", new { id = productAttributeMapping.ProductId });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAttributeValueList(ProductAttributeValueSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = await ProductAttributeService.GetProductAttributeMappingByIdAsync(searchModel.ProductAttributeMappingId)
                ?? throw new ArgumentException("No product attribute mapping found with the specified id");

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return Content("This is not your product");

            //prepare model
            var model = await ProductModelFactory.PrepareProductAttributeValueListModelAsync(searchModel, productAttributeMapping);

            return Json(model);
        }

        public virtual async Task<IActionResult> ProductAttributeValueCreatePopup(int productAttributeMappingId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = await ProductAttributeService.GetProductAttributeMappingByIdAsync(productAttributeMappingId)
                ?? throw new ArgumentException("No product attribute mapping found with the specified id");

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return RedirectToAction("List", "Product");

            //prepare model
            var model = await ProductModelFactory.PrepareProductAttributeValueModelAsync(new ProductAttributeValueModel(), productAttributeMapping, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAttributeValueCreatePopup(ProductAttributeValueModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = await ProductAttributeService.GetProductAttributeMappingByIdAsync(model.ProductAttributeMappingId);
            if (productAttributeMapping == null)
                return RedirectToAction("List", "Product");

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
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

                await ProductAttributeService.InsertProductAttributeValueAsync(pav);
                await UpdateLocalesAsync(pav, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await ProductModelFactory.PrepareProductAttributeValueModelAsync(model, productAttributeMapping, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> ProductAttributeValueEditPopup(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute value with the specified id
            var productAttributeValue = await ProductAttributeService.GetProductAttributeValueByIdAsync(id);
            if (productAttributeValue == null)
                return RedirectToAction("List", "Product");

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = await ProductAttributeService.GetProductAttributeMappingByIdAsync(productAttributeValue.ProductAttributeMappingId);
            if (productAttributeMapping == null)
                return RedirectToAction("List", "Product");

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return RedirectToAction("List", "Product");

            //prepare model
            var model = await ProductModelFactory.PrepareProductAttributeValueModelAsync(null, productAttributeMapping, productAttributeValue);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAttributeValueEditPopup(ProductAttributeValueModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute value with the specified id
            var productAttributeValue = await ProductAttributeService.GetProductAttributeValueByIdAsync(model.Id);
            if (productAttributeValue == null)
                return RedirectToAction("List", "Product");

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = await ProductAttributeService.GetProductAttributeMappingByIdAsync(productAttributeValue.ProductAttributeMappingId);
            if (productAttributeMapping == null)
                return RedirectToAction("List", "Product");

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
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
                await ProductAttributeService.UpdateProductAttributeValueAsync(productAttributeValue);

                await UpdateLocalesAsync(productAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await ProductModelFactory.PrepareProductAttributeValueModelAsync(model, productAttributeMapping, productAttributeValue, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAttributeValueDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product attribute value with the specified id
            var productAttributeValue = await ProductAttributeService.GetProductAttributeValueByIdAsync(id)
                ?? throw new ArgumentException("No product attribute value found with the specified id");

            //try to get a product attribute mapping with the specified id
            var productAttributeMapping = await ProductAttributeService.GetProductAttributeMappingByIdAsync(productAttributeValue.ProductAttributeMappingId)
                ?? throw new ArgumentException("No product attribute mapping found with the specified id");

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(productAttributeMapping.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return Content("This is not your product");

            //check if existed combinations contains the specified attribute value
            var existedCombinations = await ProductAttributeService.GetAllProductAttributeCombinationsAsync(product.Id);
            if (existedCombinations?.Any() == true)
            {
                foreach (var combination in existedCombinations)
                {
                    var attributeValues = await ProductAttributeParser.ParseProductAttributeValuesAsync(combination.AttributesXml);
                    
                    if (attributeValues.Where(attribute => attribute.Id == id).Any())
                    {
                        return Conflict(string.Format(await LocalizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.Values.AlreadyExistsInCombination"),
                            await ProductAttributeFormatter.FormatAttributesAsync(product, combination.AttributesXml, await WorkContext.GetCurrentCustomerAsync(), ", ")));
                    }
                }
            }

            await ProductAttributeService.DeleteProductAttributeValueAsync(productAttributeValue);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> AssociateProductToAttributeValuePopup()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //prepare model
            var model = await ProductModelFactory.PrepareAssociateProductToAttributeValueSearchModelAsync(new AssociateProductToAttributeValueSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AssociateProductToAttributeValuePopupList(AssociateProductToAttributeValueSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await ProductModelFactory.PrepareAssociateProductToAttributeValueListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> AssociateProductToAttributeValuePopup([Bind(Prefix = nameof(AssociateProductToAttributeValueModel))] AssociateProductToAttributeValueModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var associatedProduct = await ProductService.GetProductByIdAsync(model.AssociatedToProductId);
            if (associatedProduct == null)
                return Content("Cannot load a product");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && associatedProduct.VendorId != currentVendor.Id)
                return Content("This is not your product");

            ViewBag.RefreshPage = true;
            ViewBag.productId = associatedProduct.Id;
            ViewBag.productName = associatedProduct.Name;

            return View(new AssociateProductToAttributeValueSearchModel());
        }

        //action displaying notification (warning) to a store owner when associating some product
        public virtual async Task<IActionResult> AssociatedProductGetWarnings(int productId)
        {
            var associatedProduct = await ProductService.GetProductByIdAsync(productId);
            if (associatedProduct == null)
                return Json(new { Result = string.Empty });

            //attributes
            if (await ProductAttributeService.GetProductAttributeMappingsByProductIdAsync(associatedProduct.Id) is IList<ProductAttributeMapping> mapping && mapping.Any())
            {
                if (mapping.Any(attribute => attribute.IsRequired))
                    return Json(new { Result = await LocalizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.HasRequiredAttributes") });

                return Json(new { Result = await LocalizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.HasAttributes") });
            }

            //gift card
            if (associatedProduct.IsGiftCard)
            {
                return Json(new { Result = await LocalizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.GiftCard") });
            }

            //downloadable product
            if (associatedProduct.IsDownload)
            {
                return Json(new { Result = await LocalizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.Attributes.Values.Fields.AssociatedProduct.Downloadable") });
            }

            return Json(new { Result = string.Empty });
        }

        #endregion

        #region Product attribute combinations

        [HttpPost]
        public virtual async Task<IActionResult> ProductAttributeCombinationList(ProductAttributeCombinationSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return Content("This is not your product");

            //prepare model
            var model = await ProductModelFactory.PrepareProductAttributeCombinationListModelAsync(searchModel, product);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAttributeCombinationDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a combination with the specified id
            var combination = await ProductAttributeService.GetProductAttributeCombinationByIdAsync(id)
                ?? throw new ArgumentException("No product attribute combination found with the specified id");

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(combination.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return Content("This is not your product");

            await ProductAttributeService.DeleteProductAttributeCombinationAsync(combination);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> ProductAttributeCombinationCreatePopup(int productId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(productId);
            if (product == null)
                return RedirectToAction("List", "Product");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return RedirectToAction("List", "Product");

            //prepare model
            var model = await ProductModelFactory.PrepareProductAttributeCombinationModelAsync(new ProductAttributeCombinationModel(), product, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAttributeCombinationCreatePopup(int productId, ProductAttributeCombinationModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(productId);
            if (product == null)
                return RedirectToAction("List", "Product");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return RedirectToAction("List", "Product");

            //attributes
            var warnings = new List<string>();
            var attributesXml = await GetAttributesXmlForProductAttributeCombinationAsync(model.Form, warnings, product.Id);

            //check whether the attribute value is specified
            if (string.IsNullOrEmpty(attributesXml))
                warnings.Add(await LocalizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Alert.FailedValue"));

            warnings.AddRange(await ShoppingCartService.GetShoppingCartItemAttributeWarningsAsync(await WorkContext.GetCurrentCustomerAsync(),
                ShoppingCartType.ShoppingCart, product, 1, attributesXml, true));

            //check whether the same attribute combination already exists
            var existingCombination = await ProductAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
            if (existingCombination != null)
                warnings.Add(await LocalizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.AlreadyExists"));

            if (!warnings.Any())
            {
                //save combination
                var combination = model.ToEntity<ProductAttributeCombination>();

                //fill attributes
                combination.AttributesXml = attributesXml;

                await ProductAttributeService.InsertProductAttributeCombinationAsync(combination);

                //quantity change history
                await ProductService.AddStockQuantityHistoryEntryAsync(product, combination.StockQuantity, combination.StockQuantity,
                    message: await LocalizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.Combination.Edit"), combinationId: combination.Id);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await ProductModelFactory.PrepareProductAttributeCombinationModelAsync(model, product, null, true);
            model.Warnings = warnings;

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> ProductAttributeCombinationGeneratePopup(int productId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(productId);
            if (product == null)
                return RedirectToAction("List", "Product");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return RedirectToAction("List", "Product");

            //prepare model
            var model = await ProductModelFactory.PrepareProductAttributeCombinationModelAsync(new ProductAttributeCombinationModel(), product, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAttributeCombinationGeneratePopup(IFormCollection form, ProductAttributeCombinationModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(model.ProductId);
            if (product == null)
                return RedirectToAction("List", "Product");

            var allowedAttributeIds = form.Keys.Where(key => key.Contains("attribute_value_"))
                .Select(key => int.TryParse(form[key], out var id) ? id : 0).Where(id => id > 0).ToList();

            var requiredAttributeNames = await (await ProductAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id))
                .Where(pam => pam.IsRequired)
                .Where(pam => !pam.IsNonCombinable())
                .WhereAwait(async pam => !(await ProductAttributeService.GetProductAttributeValuesAsync(pam.Id)).Any(v => allowedAttributeIds.Any(id => id == v.Id)))
                .SelectAwait(async pam => (await ProductAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId)).Name).ToListAsync();

            if (requiredAttributeNames.Any())
            {
                model = await ProductModelFactory.PrepareProductAttributeCombinationModelAsync(model, product, null, true);
                model.ProductAttributes.SelectMany(pa => pa.Values)
                    .Where(v => allowedAttributeIds.Any(id => id == v.Id))
                    .ToList().ForEach(v => v.Checked = "checked");

                model.Warnings.Add(string.Format(await LocalizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.SelectRequiredAttributes"), string.Join(", ", requiredAttributeNames)));

                return View(model);
            }

            await GenerateAttributeCombinationsAsync(product, allowedAttributeIds);

            ViewBag.RefreshPage = true;

            return View(new ProductAttributeCombinationModel());
        }

        public virtual async Task<IActionResult> ProductAttributeCombinationEditPopup(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a combination with the specified id
            var combination = await ProductAttributeService.GetProductAttributeCombinationByIdAsync(id);
            if (combination == null)
                return RedirectToAction("List", "Product");

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(combination.ProductId);
            if (product == null)
                return RedirectToAction("List", "Product");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return RedirectToAction("List", "Product");

            //prepare model
            var model = await ProductModelFactory.PrepareProductAttributeCombinationModelAsync(null, product, combination);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductAttributeCombinationEditPopup(ProductAttributeCombinationModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a combination with the specified id
            var combination = await ProductAttributeService.GetProductAttributeCombinationByIdAsync(model.Id);
            if (combination == null)
                return RedirectToAction("List", "Product");

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(combination.ProductId);
            if (product == null)
                return RedirectToAction("List", "Product");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return RedirectToAction("List", "Product");

            //attributes
            var warnings = new List<string>();
            var attributesXml = await GetAttributesXmlForProductAttributeCombinationAsync(model.Form, warnings, product.Id);

            //check whether the attribute value is specified
            if (string.IsNullOrEmpty(attributesXml))
                warnings.Add(await LocalizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Alert.FailedValue"));

            warnings.AddRange(await ShoppingCartService.GetShoppingCartItemAttributeWarningsAsync(await WorkContext.GetCurrentCustomerAsync(),
                ShoppingCartType.ShoppingCart, product, 1, attributesXml, true));

            //check whether the same attribute combination already exists
            var existingCombination = await ProductAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
            if (existingCombination != null && existingCombination.Id != model.Id && existingCombination.AttributesXml.Equals(attributesXml))
                warnings.Add(await LocalizationService.GetResourceAsync("Admin.Catalog.Products.ProductAttributes.AttributeCombinations.AlreadyExists"));

            if (!warnings.Any() && ModelState.IsValid)
            {
                var previousStockQuantity = combination.StockQuantity;

                //save combination
                //fill entity from model
                combination = model.ToEntity(combination);
                combination.AttributesXml = attributesXml;

                await ProductAttributeService.UpdateProductAttributeCombinationAsync(combination);

                //quantity change history
                await ProductService.AddStockQuantityHistoryEntryAsync(product, combination.StockQuantity - previousStockQuantity, combination.StockQuantity,
                    message: await LocalizationService.GetResourceAsync("Admin.StockQuantityHistory.Messages.Combination.Edit"), combinationId: combination.Id);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await ProductModelFactory.PrepareProductAttributeCombinationModelAsync(model, product, combination, true);
            model.Warnings = warnings;

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GenerateAllAttributeCombinations(int productId)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product with the specified id
            var product = await ProductService.GetProductByIdAsync(productId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return Content("This is not your product");

            await GenerateAttributeCombinationsAsync(product);

            return Json(new { Success = true });
        }

        #endregion

        #region Product editor settings

        [HttpPost]
        public virtual async Task<IActionResult> SaveProductEditorSettings(ProductModel model, string returnUrl = "")
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //vendors cannot manage these settings
            if (await WorkContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("List");

            var productEditorSettings = await SettingService.LoadSettingAsync<ProductEditorSettings>();
            productEditorSettings = model.ProductEditorSettingsModel.ToSettings(productEditorSettings);
            await SettingService.SaveSettingAsync(productEditorSettings);

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
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
                return await AccessDeniedDataTablesJson();

            var product = await ProductService.GetProductByIdAsync(searchModel.ProductId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor should have access only to his products
            var currentVendor = await WorkContext.GetCurrentVendorAsync();
            if (currentVendor != null && product.VendorId != currentVendor.Id)
                return Content("This is not your product");

            //prepare model
            var model = await ProductModelFactory.PrepareStockQuantityHistoryListModelAsync(searchModel, product);

            return Json(model);
        }

        #endregion

        #endregion
    }
}