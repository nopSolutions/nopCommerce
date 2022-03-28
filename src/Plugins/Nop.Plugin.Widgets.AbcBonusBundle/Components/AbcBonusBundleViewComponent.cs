using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Services.Logging;
using Nop.Services.Messages;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Data;
using Nop.Web.Framework.Components;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Core.Domain.Security;
using Nop.Services.Customers;
using Nop.Services.Catalog;
using Nop.Plugin.Widgets.AbcBonusBundle.Services;
using Nop.Plugin.Widgets.AbcBonusBundle.Models;
using System.IO;
using Nop.Plugin.Misc.AbcCore;
using Nop.Web.Models.Catalog;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.AbcBonusBundle.Components
{
    [ViewComponent(Name = "AbcBonusBundle")]
    public class AbcBonusBundleViewComponent : NopViewComponent
    {
        private readonly AbcBonusBundleSettings _abcBonusBundleSettings;
        private readonly ILogger _logger;
        private readonly IProductAbcBundleService _productAbcBundleService;
        private readonly IProductService _productService;

        public AbcBonusBundleViewComponent(
            AbcBonusBundleSettings abcBonusBundleSettings,
            ILogger logger,
            IProductAbcBundleService productAbcBundleService,
            IProductService productService
        )
        {
            _abcBonusBundleSettings = abcBonusBundleSettings;
            _logger = logger;
            _productAbcBundleService = productAbcBundleService;
            _productService = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, ProductDetailsModel additionalData = null)
        {
            if (additionalData == null)
            {
                await _logger.ErrorAsync("ProductDetailsModel not passed to ABC Warehouse Bonus Bundle widget - skipping display of bonus bundle info.");
                return Content("");
            }

            int productId = additionalData.Id;
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null) { return Content(""); }

            var bundles = _productAbcBundleService.GetBundles(product.Sku);
            if (!bundles.Any()) { return Content(""); }

            var modelList = new List<AbcBonusBundleModel>();
            foreach (var bundle in bundles)
            {
                if (string.IsNullOrWhiteSpace(bundle.Comment)) { continue; }

                var imageUrl = await FindBundleImageAsync(bundle.MemoNumber);

                var model = new AbcBonusBundleModel
                {
                    BundleDescription = bundle.Comment,
                    PopupCommand = imageUrl != null ? bundle.GetPopupCommand(imageUrl) : null,
                    StoreName = _abcBonusBundleSettings.StoreName,
                    PhoneNumber = _abcBonusBundleSettings.PhoneNumber,
                    EndDate = bundle.GetEndDateTime().ToString("M/d/yy")
                };

                modelList.Add(model);
            }

            return View("~/Plugins/Widgets.AbcBonusBundle/Views/Display.cshtml", modelList);
        }

        private async Task<string> FindBundleImageAsync(string memoNumber)
        {
            var imageUrls = Directory.GetFiles(GetBonusBundlesImageFolder(), $"{memoNumber}.*");

            if (!imageUrls.Any()) { return null; }
            if (imageUrls.Length > 1)
            {
                await _logger.WarningAsync($"ABCWarehouse Bonus Bundles: Multiple images found for bonus bundle {memoNumber}, using first found.");
            }

            return $"/{imageUrls[0].Split('/').Last().Replace("\\", "/")}";
        }

        public static string GetBonusBundlesImageFolder()
        {
            var directoryPath = $"{CoreUtilities.WebRootPath()}/bundle_images";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            return directoryPath;
        }
    }
}
