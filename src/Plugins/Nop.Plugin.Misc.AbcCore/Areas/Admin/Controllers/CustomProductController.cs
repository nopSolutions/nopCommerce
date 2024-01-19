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
using Nop.Web.Areas.Admin.Controllers;

namespace Nop.Plugin.Misc.AbcCore.Areas.Admin.Controllers
{
    public class CustomProductController : ProductController
    {
        private readonly IProductService _productService;
        private readonly IGenericAttributeService _genericAttributeService;

        public CustomProductController(
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
            VendorSettings vendorSettings

        ) : base(aclService, backInStockSubscriptionService, categoryService,
            copyProductService, customerActivityService, customerService,
            discountService, downloadService, exportManager, importManager,
            languageService, localizationService, localizedEntityService,
            manufacturerService, fileProvider, notificationService,
            pdfService, permissionService, pictureService, productAttributeParser,
            productAttributeService, productAttributeFormatter, productModelFactory,
            productService, productTagService, settingService, shippingService,
            shoppingCartService, specificationAttributeService, storeContext,
            urlRecordService, genericAttributeService, workContext, vendorSettings)
        {
            _genericAttributeService = genericAttributeService;
            _productService = productService;
        }

        public override async Task<IActionResult> Edit(int id)
        {
            return await base.Edit(id);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public override async Task<IActionResult> Edit(ProductModel model, bool continueEditing)
        {
            // Saves PLP description
            var plpDescription = Request.Form["PLPDescription"].ToString();
            var product = await _productService.GetProductByIdAsync(model.Id);
            await _genericAttributeService.SaveAttributeAsync<string>(
                product, "PLPDescription", plpDescription
            );

            return await base.Edit(model, continueEditing);
        }
    }
}
