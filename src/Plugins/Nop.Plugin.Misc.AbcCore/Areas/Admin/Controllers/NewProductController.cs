using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Models.Extensions;
using System.Linq;
using Nop.Services.Catalog;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Services.Seo;
using System.Threading.Tasks;
using Nop.Plugin.Misc.AbcCore.Areas.Admin.Models;
using Nop.Plugin.Misc.AbcCore.Services.Custom;
using Nop.Plugin.Misc.AbcCore.Services;

namespace Nop.Plugin.Misc.AbcCore.Areas.Admin.Controllers
{
    public class NewProductController : BaseAdminController
    {
        private readonly IAbcProductService _abcProductService;
        private readonly IProductAbcDescriptionService _productAbcDescriptionService;

        public NewProductController(
            IAbcProductService abcProductService,
            IProductAbcDescriptionService productAbcDescriptionService
        )
        {
            _abcProductService = abcProductService;
            _productAbcDescriptionService = productAbcDescriptionService;
        }

        public IActionResult List()
        {
            return View(
                "~/Plugins/Misc.AbcCore/Areas/Admin/Views/NewProduct/List.cshtml",
                new NewProductSearchModel()
            );
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(NewProductSearchModel searchModel)
        {
            var products = await _abcProductService.GetNewProductsAsync();
            var pagedList = products.ToPagedList(searchModel);
            var model = await new NewProductListModel().PrepareToGridAsync(searchModel, pagedList, () =>
            {
                //fill in model values from the entity
                return pagedList.SelectAwait(async product =>
                {
                    var pad = await _productAbcDescriptionService.GetProductAbcDescriptionByProductIdAsync(product.Id);
                    var itemNo = pad?.AbcItemNumber;

                    var newProductModel = new NewProductModel()
                    {
                        ItemNo = itemNo,
                        Sku = product.Sku,
                        Name = product.Name
                    };

                    return newProductModel;
                });
            });

            return Json(model);
        }
    }
}
