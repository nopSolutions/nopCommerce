using System;
using System.Collections.Generic;
using System.Linq;
using Avalara.AvaTax.RestClient;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Plugin.Tax.Avalara.Services;
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
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;

namespace Nop.Plugin.Tax.Avalara.Controllers
{
    public class OverriddenProductController : ProductController
    {
        #region Fields

        private readonly AvalaraTaxManager _avalaraTaxManager;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly ITaxPluginManager _taxPluginManager;

        #endregion

        #region Ctor

        public OverriddenProductController(AvalaraTaxManager avalaraTaxManager,
            IAclService aclService,
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
            ITaxCategoryService taxCategoryService,
            ITaxPluginManager taxPluginManager,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            VendorSettings vendorSettings) : base(aclService,
                backInStockSubscriptionService,
                categoryService,
                copyProductService,
                customerActivityService,
                customerService,
                discountService,
                downloadService,
                exportManager,
                importManager,
                languageService,
                localizationService,
                localizedEntityService,
                manufacturerService,
                fileProvider,
                notificationService,
                pdfService,
                permissionService,
                pictureService,
                productAttributeParser,
                productAttributeService,
                productModelFactory,
                productService,
                productTagService,
                settingService,
                shippingService,
                shoppingCartService,
                specificationAttributeService,
                urlRecordService,
                workContext,
                vendorSettings)
        {
            _avalaraTaxManager = avalaraTaxManager;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _productAttributeService = productAttributeService;
            _productService = productService;
            _taxCategoryService = taxCategoryService;
            _taxPluginManager = taxPluginManager;
        }

        #endregion

        #region Methods

        [HttpPost]
        public IActionResult ExportProducts(string selectedIds)
        {
            //ensure that Avalara tax provider is active
            if (!_taxPluginManager.IsPluginActive(AvalaraTaxDefaults.SystemName))
                return RedirectToAction("List", "Product");

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            //prepare exported items
            var productIds = selectedIds?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(id => Convert.ToInt32(id)).ToArray();
            var exportedItems = new List<ItemModel>();
            foreach (var product in _productService.GetProductsByIds(productIds))
            {
                //find product combinations
                var combinations = _productAttributeService.GetAllProductAttributeCombinations(product.Id)
                    .Where(combination => !string.IsNullOrEmpty(combination.Sku));

                //export items with specified SKU only
                if (string.IsNullOrEmpty(product.Sku) && !combinations.Any())
                    continue;

                //prepare common properties
                var taxCategory = _taxCategoryService.GetTaxCategoryById(product.TaxCategoryId);
                var taxCode = CommonHelper.EnsureMaximumLength(taxCategory?.Name, 25);
                var description = CommonHelper.EnsureMaximumLength(product.ShortDescription ?? product.Name, 255);

                //add the product as exported item
                if (!string.IsNullOrEmpty(product.Sku))
                {
                    exportedItems.Add(new ItemModel
                    {
                        createdDate = DateTime.UtcNow,
                        description = description,
                        itemCode = CommonHelper.EnsureMaximumLength(product.Sku, 50),
                        taxCode = taxCode
                    });
                }

                //add product combinations
                exportedItems.AddRange(combinations.Select(combination => new ItemModel
                {
                    createdDate = DateTime.UtcNow,
                    description = description,
                    itemCode = CommonHelper.EnsureMaximumLength(combination.Sku, 50),
                    taxCode = taxCode
                }));
            }

            //get existing items
            var existingItemCodes = _avalaraTaxManager.GetItems()?.Select(item => item.itemCode).ToList() ?? new List<string>();

            //remove duplicates
            exportedItems = exportedItems.Where(item => !existingItemCodes.Contains(item.itemCode)).Distinct().ToList();

            //export items
            if (exportedItems.Any())
            {
                //create items and get the result
                var result = _avalaraTaxManager.CreateItems(exportedItems)?.Count;

                //display results
                if (result.HasValue && result > 0)
                    _notificationService.SuccessNotification(string.Format(_localizationService.GetResource("Plugins.Tax.Avalara.Items.Export.Success"), result));
                else
                    _notificationService.ErrorNotification(_localizationService.GetResource("PPlugins.Tax.Avalara.Items.Export.Error"));
            }
            else
                _notificationService.SuccessNotification(_localizationService.GetResource("Plugins.Tax.Avalara.Items.Export.AlreadyExported"));

            return RedirectToAction("List", "Product");
        }

        #endregion
    }
}