using LinqToDB;
using LinqToDB.Data;
using Microsoft.AspNetCore.Mvc;
using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Delivery;
using Nop.Plugin.Misc.AbcCore.Models;
using Nop.Plugin.Misc.AbcCore.Services;
using Nop.Web.Framework.Controllers;
using System.Data;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcCore.Controllers
{
    public class CartSlideoutController : BasePluginController
    {
        private readonly IBackendStockService _backendStockService;
        private readonly IDeliveryService _deliveryService;
        private readonly INopDataProvider _nopDataProvider;

        public CartSlideoutController(
            IBackendStockService backendStockService,
            IDeliveryService deliveryService,
            INopDataProvider nopDataProvider
        ) {
            _backendStockService = backendStockService;
            _deliveryService = deliveryService;
            _nopDataProvider = nopDataProvider;
        }

        public async Task<IActionResult> GetDeliveryOptions(int? productId, int? zip)
        {
            if (zip == null || zip.ToString().Length != 5)
            {
                return BadRequest("Zip code must be a 5 digit number provided as a query parameter 'zip'.");
            }

            if (productId == null || productId == 0)
            {
                return BadRequest("Product ID must be provided.");
            }

            // pickup in store options
            StockResponse stockResponse = await _backendStockService.GetApiStockAsync(productId.Value);

            return Json(new {
                isDeliveryAvailable = await _deliveryService.CheckZipcodeAsync(zip.Value),
                pickupInStoreHtml = await RenderViewComponentToStringAsync(
                    "CartSlideoutPickupInStore",
                    new {
                        productStock = stockResponse.ProductStocks
                    })
            });
        }
    }
}
