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
    public class AbcPromoController : BaseAdminController
    {
        private readonly IAbcPromoService _abcPromoService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductAbcDescriptionService _productAbcDescriptionService;
        private readonly IUrlRecordService _urlRecordService;

        public AbcPromoController(
            IAbcPromoService abcPromoService,
            IManufacturerService manufacturerService,
            IProductAbcDescriptionService productAbcDescriptionService,
            IUrlRecordService urlRecordService
        )
        {
            _abcPromoService = abcPromoService;
            _manufacturerService = manufacturerService;
            _productAbcDescriptionService = productAbcDescriptionService;
            _urlRecordService = urlRecordService;
        }

        public IActionResult List()
        {
            return View(
                "~/Plugins/Misc.AbcCore/Areas/Admin/Views/AbcPromo/List.cshtml",
                new AbcPromoSearchModel()
            );
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(AbcPromoSearchModel searchModel)
        {
            var promos = (await _abcPromoService.GetAllPromosAsync()).ToPagedList(searchModel);
            var model = new AbcPromoListModel().PrepareToGrid(searchModel, promos, () =>
            {
                //fill in model values from the entity
                return promos.Select(promo =>
                {
                    //fill in model values from the entity
                    var manufacturerName =
                        promo.ManufacturerId.HasValue ?
                            _manufacturerService.GetManufacturerByIdAsync(promo.ManufacturerId.Value).GetAwaiter().GetResult().Name :
                            "";

                    
                    // should be asyncronous, but I'm not figuring out how to make this work.
                    var slug = _urlRecordService.GetActiveSlugAsync(promo.Id, "AbcPromo", 0).GetAwaiter().GetResult();
                    var products = _abcPromoService.GetProductsByPromoIdAsync(promo.Id).GetAwaiter().GetResult();
                    var productCount = products.Count();

                    var abcPromoModel = new AbcPromoModel()
                    {
                        Id = promo.Id,
                        Name = promo.Name,
                        Description = promo.Description,
                        StartDate = promo.StartDate,
                        EndDate = promo.EndDate,
                        IsActive = promo.IsActive(),
                        Manufacturer = manufacturerName,
                        Slug = slug,
                        ProductCount = productCount
                    };

                    return abcPromoModel;
                });
            });

            return Json(model);
        }

        public async Task<IActionResult> Products(int abcPromoId)
        {
            var abcPromo = await _abcPromoService.GetPromoByIdAsync(abcPromoId);
            var model = new AbcPromoProductSearchModel()
            {
                AbcPromoId = abcPromo.Id,
                AbcPromoName = abcPromo.Name
            };

            return View(
                "~/Plugins/Misc.AbcCore/Areas/Admin/Views/AbcPromo/Products.cshtml",
                model
            );
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductsAsync(AbcPromoProductSearchModel searchModel)
        {
            var products = await _abcPromoService.GetProductsByPromoIdAsync(searchModel.AbcPromoId);
            var productsPaged = products.ToPagedList(searchModel);

            var model = await new AbcPromoProductListModel().PrepareToGridAsync(searchModel, productsPaged, () =>
            {
                return products.SelectAwait(async product =>
                {
                    var productAbcDescription =
                        await _productAbcDescriptionService.GetProductAbcDescriptionByProductIdAsync(product.Id);
                    var abcItemNumber = productAbcDescription?.AbcItemNumber;

                    var abcPromoProductModel = new AbcPromoProductModel()
                    {
                        AbcItemNumber = abcItemNumber,
                        Name = product.Name,
                        Published = product.Published
                    };

                    return abcPromoProductModel;
                });
            });

            return Json(model);
        }
    }
}
