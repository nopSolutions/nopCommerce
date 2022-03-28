using LinqToDB;
using LinqToDB.Data;
using Microsoft.AspNetCore.Mvc;
using Nop.Data;
using Nop.Plugin.Misc.AbcCore.Delivery;
using Nop.Web.Framework.Controllers;
using System.Data;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AbcCore.Controllers
{
    public class CartSlideoutController : BasePluginController
    {
        private readonly IDeliveryService _deliveryService;
        private readonly INopDataProvider _nopDataProvider;

        public CartSlideoutController(
            IDeliveryService deliveryService,
            INopDataProvider nopDataProvider
        ) {
            _deliveryService = deliveryService;
            _nopDataProvider = nopDataProvider;
        }

        public async Task<IActionResult> GetDeliveryOptions(int? productId, int? zip)
        {
            if (zip == null || zip.ToString().Length != 5)
            {
                return BadRequest("Zip code must be a 5 digit number provided as a query parameter 'zip'.");
            }

            return Json(new {
                isDeliveryAvailable = await _deliveryService.CheckZipcodeAsync(zip.Value)
            });
        }
    }
}
