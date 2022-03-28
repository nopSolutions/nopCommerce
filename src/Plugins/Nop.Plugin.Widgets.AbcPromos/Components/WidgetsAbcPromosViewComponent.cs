using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.AbcCore;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Plugin.Misc.AbcCore.Extensions;
using Nop.Plugin.Misc.AbcCore.Infrastructure;
using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Plugin.Widgets.AbcPromos.Models;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Models.Catalog;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.AbcPromos.Components
{
    public class WidgetsAbcPromosViewComponent : NopViewComponent
    {
        private readonly ILogger _logger;
        private readonly IAbcPromoService _abcPromoService;

        private readonly string DirectoryName = "promo_banners";
        private readonly string DirectoryPath;

        public WidgetsAbcPromosViewComponent(
            ILogger logger,
            IAbcPromoService abcPromoService
        ) {
            _logger = logger;
            _abcPromoService = abcPromoService;

            DirectoryPath = $"{CoreUtilities.WebRootPath()}/{DirectoryName}";
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {
            if (widgetZone == CustomPublicWidgetZones.ProductBoxAddinfoReviews)
            {
                return await ProductBoxPromo(additionalData);
            }

            if (widgetZone == CustomPublicWidgetZones.ProductDetailsAfterPrice ||
                widgetZone == CustomPublicWidgetZones.OrderSummaryAfterProductMiniDescription)
            {
                return await ProductDetailPromos(int.Parse(additionalData.ToString()));
            }

            if (widgetZone == PublicWidgetZones.ProductDetailsAfterBreadcrumb)
            {
                return await DisplayBanner(additionalData);
            }

            await _logger.ErrorAsync($"Widgets.AbcPromos: Invalid Widget Zone passed ({widgetZone}).");
            return Content("");
        }

        private async Task<IViewComponentResult> ProductBoxPromo(object additionalData)
        {
            if (additionalData == null || !(additionalData is ProductOverviewModel))
            {
                await _logger.ErrorAsync("ProductOverviewModel not passed to Widgets.AbcPromos - skipping display of product box promo.");
                return Content("");
            }

            var productId = (additionalData as ProductOverviewModel).Id;
            var promos = (await _abcPromoService.GetActivePromosByProductIdAsync(productId)).Take(2);

            var promosArray = new AbcPromo[]
            {
                promos.Count() > 0 ? promos.ElementAt(0) : null,
                promos.Count() > 1 ? promos.ElementAt(1) : null,
            };

            return View("~/Plugins/Widgets.AbcPromos/Views/ProductBoxPromos.cshtml", promosArray);
        }

        private async Task<IViewComponentResult> ProductDetailPromos(int productId)
        {
            var promos = await _abcPromoService.GetActivePromosByProductIdAsync(productId);

            return View("~/Plugins/Widgets.AbcPromos/Views/ProductDetailPromos.cshtml", promos);
        }

        private async Task<IViewComponentResult> DisplayBanner(object additionalData)
        {
            if (additionalData == null || !(additionalData is ProductDetailsModel))
            {
                await _logger.ErrorAsync("ProductDetailsModel not passed to Widgets.AbcPromos - skipping display of promo banner.");
                return Content("");
            }

            int productId = (additionalData as ProductDetailsModel).Id;
            var promos = await _abcPromoService.GetActivePromosByProductIdAsync(productId);
            if (!promos.Any()) { return Content(""); }

            var banners = new List<BannerModel>();
            foreach (var promo in promos)
            {
                var bannerImage = await promo.GetPromoBannerUrlAsync();

                if (bannerImage != null)
                {
                    var bannerModel = new BannerModel
                    {
                        AltText = promo.Name,
                        BannerImageUrl = bannerImage,
                        PromoFormPopup = promo.GetPopupCommand()
                    };
                    banners.Add(bannerModel);
                }
            }

            return View("~/Plugins/Widgets.AbcPromos/Views/DisplayBanner.cshtml", banners);
        }
    }
}
