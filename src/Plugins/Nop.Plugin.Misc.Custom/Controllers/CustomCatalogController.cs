using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Misc.Custom.Controllers
{
    public class CustomCatalogController : CatalogController
    {
        private readonly IWorkContext _workContext;

        public CustomCatalogController(CatalogSettings catalogSettings,
            IAclService aclService,
            ICatalogModelFactory catalogModelFactory, 
            ICategoryService categoryService, 
            ICustomerActivityService customerActivityService, 
            IGenericAttributeService genericAttributeService, 
            ILocalizationService localizationService, 
            IManufacturerService manufacturerService,
            INopUrlHelper nopUrlHelper,
            IPermissionService permissionService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IProductTagService productTagService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            VendorSettings vendorSettings) : base(catalogSettings, aclService, catalogModelFactory, categoryService, customerActivityService, genericAttributeService, localizationService, manufacturerService, nopUrlHelper, permissionService, productModelFactory, productService, productTagService, storeContext, storeMappingService, urlRecordService, vendorService, webHelper, workContext, mediaSettings, vendorSettings)
        {
            _workContext = workContext;
        }

        public async override Task<IActionResult> Category(int categoryId, CatalogProductsCommand command)
        {
            //customization : re direct guest users to login/register page when they try to visit any category (except pricing category) or profiles
            if ((await _workContext.GetCurrentCustomerAsync()).CustomerProfileTypeId == 0 && categoryId != 3)
            {
                //return RedirectToRoute("Login");
            }

            //customization : show opposite category products to logged in user
            //i.e for 'Support Takers' show 'Give Support' category profiles and vice versa
            //if (categoryId == 1)
            //    categoryId = 2;
            //else if (categoryId == 2)
            //    categoryId = 1;

            return await base.Category(categoryId, command);
        }

    }
}
