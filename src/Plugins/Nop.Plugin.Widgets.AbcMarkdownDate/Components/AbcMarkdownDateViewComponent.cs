using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.AbcCore.Extensions;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.AbcHomeDeliveryStatus.Components
{
    [ViewComponent(Name = "AbcMarkdownDate")]
    public class AbcMarkdownDateViewComponent : NopViewComponent
    {
        private readonly ILogger _logger;
        private readonly IProductService _productService;

        public AbcMarkdownDateViewComponent(
            ILogger logger,
            IProductService productService
        )
        {
            _logger = logger;
            _productService = productService;
        }
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, ProductDetailsModel additionalData = null)
        {
            int productId = additionalData.Id;
            Product product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                await _logger.WarningAsync($"ABC Markdown Date Widget: Unable to find product with ID '{productId}', was the product ID passed in?");
            }

            var specialPriceEndDate = await product.GetSpecialPriceEndDateAsync();
            if (specialPriceEndDate == null)
            {
                return Content("");
            }

            switch (widgetZone)
            {
                case "productdetails_before_addtocart":
                    return View(
                        "~/Plugins/Widgets.AbcMarkdownDate/Views/MarkdownMessage.cshtml",
                        model: specialPriceEndDate.Value.ToShortDateString());
            }

            await _logger.WarningAsync("ABC Markdown Date Widget: Did not match with any passed widgets, skipping display");
            return Content("");
        }
    }
}
