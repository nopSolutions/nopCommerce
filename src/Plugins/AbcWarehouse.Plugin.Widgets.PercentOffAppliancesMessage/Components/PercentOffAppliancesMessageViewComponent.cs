using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.AbcCore.Extensions;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using System.Threading.Tasks;
using Nop.Plugin.Misc.AbcCore.Nop;
using System.Linq;
using Nop.Core;

namespace AbcWarehouse.Plugin.Widgets.PercentOffAppliancesMessageViewComponent.Components
{
    [ViewComponent(Name = "PercentOffAppliancesMessage")]
    public class PercentOffAppliancesMessageViewComponent : NopViewComponent
    {
        private readonly ILogger _logger;
        private readonly IAbcCategoryService  _abcCategoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;

        public PercentOffAppliancesMessageViewComponent(
            ILogger logger,
            IAbcCategoryService abcCategoryService,
            IManufacturerService manufacturerService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IProductService productService,
            IStoreContext storeContext
        )
        {
            _logger = logger;
            _abcCategoryService = abcCategoryService;
            _manufacturerService = manufacturerService;
            _priceCalculationService = priceCalculationService;
            _priceFormatter = priceFormatter;
            _productService = productService;
            _storeContext = storeContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, int additionalData)
        {
            // Don't display for Hawthorne
            var isHawthorne = (await _storeContext.GetCurrentStoreAsync()).Id == 8;
            if (isHawthorne)
            {
                return Content("");
            }

            int productId = additionalData;
            Product product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                await _logger.WarningAsync($"Percent Off Appliances Message Widget: Unable to find product with ID '{productId}', was the product ID passed in?");
                return Content("");
            }

            // Don't display for call for pricing products
            if (product.CallForPrice)
            {
                return Content("");
            }

            // Don't display for Add to cart to see price
            if (await product.IsAddToCartToSeePriceAsync())
            {
                return Content("");
            }


            // exclude specific brands
            var exceptionBrands = new string[] {
                "LG SIGNATURE",
                "LG STUDIO",
                "LG XBOOM",
                "\"C\" BY GE",
                "GE CAFE",
                "GE MONOGRAM",
                "GE PROFILE",
            };
            var pms = await _manufacturerService.GetProductManufacturersByProductIdAsync(productId);
            foreach (var pm in pms)
            {
                var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(pm.ManufacturerId);
                if (manufacturer.Published && exceptionBrands.Contains(manufacturer.Name))
                {
                    return Content("");
                }
            }

            var pcs = await _abcCategoryService.GetProductCategoriesByProductIdAsync(productId);
            var showMessage = false;
            foreach (var pc in pcs)
            {
                if (!showMessage)
                {
                    showMessage = await _abcCategoryService.HasApplianceTopLevelCategoryAsync(pc.CategoryId);
                }
            }

            var discountedPrice = await _priceCalculationService.RoundPriceAsync(product.Price - (product.Price * 0.10M));
            var formattedDiscountPrice = await _priceFormatter.FormatPriceAsync(discountedPrice);

            return showMessage ?
                View("~/Plugins/Widgets.PercentOffAppliancesMessage/Views/Message.cshtml", formattedDiscountPrice) :
                Content("");
        }
    }
}
