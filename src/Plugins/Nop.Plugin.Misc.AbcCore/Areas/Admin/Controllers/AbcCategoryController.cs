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
using Nop.Core.Caching;
using Nop.Services.Stores;

namespace Nop.Plugin.Misc.AbcCore.Areas.Admin.Controllers
{
    public class AbcCategoryController : CategoryController
    {
        private readonly ICategoryService _categoryService;
        private readonly IGenericAttributeService _genericAttributeService;

        public AbcCategoryController(
            IAclService aclService,
            ICategoryModelFactory categoryModelFactory,
            ICategoryService categoryService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDiscountService discountService,
            IExportManager exportManager,
            IImportManager importManager,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IProductService productService,
            IStaticCacheManager staticCacheManager,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            // custom
            IGenericAttributeService genericAttributeService
        ) : base(
            aclService,
            categoryModelFactory,
            categoryService,
            customerActivityService,
            customerService,
            discountService,
            exportManager,
            importManager,
            localizationService,
            localizedEntityService,
            notificationService,
            permissionService,
            pictureService,
            productService,
            staticCacheManager,
            storeMappingService,
            storeService,
            urlRecordService,
            workContext
        )
        {
            _categoryService = categoryService;
            _genericAttributeService = genericAttributeService;
        }

        public override async Task<IActionResult> Edit(int id)
        {
            return await base.Edit(id);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public override async Task<IActionResult> Edit(CategoryModel model, bool continueEditing)
        {
            await SaveHawthorneIntDataAsync(model.Id, "HawthornePictureId");
            await SaveHawthorneStringDataAsync(model.Id, "HawthorneMetaTitle");
            await SaveHawthorneStringDataAsync(model.Id, "HawthorneMetaDescription");

            return await base.Edit(model, continueEditing);
        }

        private async Task SaveHawthorneIntDataAsync(int categoryId, string key)
        {
            var value = Convert.ToInt32(Request.Form[key].ToString());
            if (value != 0)
            {
                var category = await _categoryService.GetCategoryByIdAsync(categoryId);
                await _genericAttributeService.SaveAttributeAsync<int>(
                    category, key, value
                );
            }
        }
        
        private async Task SaveHawthorneStringDataAsync(int categoryId, string key)
        {
            var value = Request.Form[key].ToString();
            if (!string.IsNullOrWhiteSpace(value))
            {
                var category = await _categoryService.GetCategoryByIdAsync(categoryId);
                await _genericAttributeService.SaveAttributeAsync<string>(
                    category, key, value
                );
            }
        }
    }
}
