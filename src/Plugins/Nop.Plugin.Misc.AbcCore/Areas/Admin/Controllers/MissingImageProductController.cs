using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.AbcCore.Areas.Admin.Models;
using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Models.Extensions;
using System.Linq;
using Nop.Services.Catalog;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Services.Seo;
using Nop.Plugin.Misc.AbcCore.Services.Custom;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcCore.Areas.Admin.Controllers
{
    public class MissingImageProductController : BaseAdminController
    {
        private readonly ICustomProductService _customProductService;
        private readonly IProductAbcDescriptionService _productAbcDescriptionService;

        public MissingImageProductController(
            ICustomProductService customProductService,
            IProductAbcDescriptionService productAbcDescriptionService
        )
        {
            _customProductService = customProductService;
            _productAbcDescriptionService = productAbcDescriptionService;
        }

        public IActionResult List()
        {
            return View(
                "~/Plugins/Misc.AbcCore/Areas/Admin/Views/MissingImageProduct/List.cshtml",
                new MissingImageProductSearchModel()
            );
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(MissingImageProductSearchModel searchModel)
        {
            var productsWithoutImages = await _customProductService.GetProductsWithoutImagesAsync();
            var pagedList = productsWithoutImages.ToPagedList(searchModel);
            var model = await new MissingImageProductListModel().PrepareToGridAsync(searchModel, pagedList, () =>
            {
                //fill in model values from the entity
                return pagedList.SelectAwait(async product =>
                {
                    var pad = await _productAbcDescriptionService.GetProductAbcDescriptionByProductIdAsync(product.Id);
                    var itemNo = pad?.AbcItemNumber;

                    var missingImageProductModel = new MissingImageProductModel()
                    {
                        ItemNumber = itemNo,
                        Sku = product.Sku,
                        Name = product.Name
                    };

                    return missingImageProductModel;
                });
            });

            return Json(model);
        }
    }
}
