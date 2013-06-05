using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Nop.Plugin.Shipping.FixedRateShipping.Models;
using Nop.Services.Configuration;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Plugin.Shipping.FixedRateShipping.Controllers
{
    [AdminAuthorize]
    public class ShippingFixedRateController : Controller
    {
        private readonly IShippingService _shippingService;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;

        public ShippingFixedRateController(IShippingService shippingServicee,
            ISettingService settingService, 
            IPermissionService permissionService)
        {
            this._shippingService = shippingServicee;
            this._settingService = settingService;
            this._permissionService = permissionService;
        }
        
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            //little hack here
            //always set culture to 'en-US' (Telerik has a bug related to editing decimal values in other cultures). Like currently it's done for admin area in Global.asax.cs
            var culture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            base.Initialize(requestContext);
        }

        [ChildActionOnly]
        public ActionResult Configure()
        {
            return View("Nop.Plugin.Shipping.FixedRateShipping.Views.ShippingFixedRate.Configure");
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult Configure(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            var tmp = new List<FixedShippingRateModel>();
            foreach (var shippingMethod in _shippingService.GetAllShippingMethods())
                tmp.Add(new FixedShippingRateModel()
                {
                    ShippingMethodId = shippingMethod.Id,
                    ShippingMethodName = shippingMethod.Name,
                    Rate = GetShippingRate(shippingMethod.Id)
                });

            var tmp2 = tmp.ForCommand(command);
            var gridModel = new GridModel<FixedShippingRateModel>
            {
                Data = tmp2,
                Total = tmp2.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult ShippingRateUpdate(FixedShippingRateModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return Content("Access denied");

            int shippingMethodId = model.ShippingMethodId;
            decimal rate = model.Rate;

            _settingService.SetSetting(string.Format("ShippingRateComputationMethod.FixedRate.Rate.ShippingMethodId{0}", shippingMethodId), rate);

            return Configure(command);
        }

        [NonAction]
        protected decimal GetShippingRate(int shippingMethodId)
        {
            decimal rate = this._settingService.GetSettingByKey<decimal>(string.Format("ShippingRateComputationMethod.FixedRate.Rate.ShippingMethodId{0}", shippingMethodId));
            return rate;
        }
    }
}
