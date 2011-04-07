using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Admin.Models;
using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public class ShippingController : BaseNopController
	{
		#region Fields

        private readonly IShippingService _shippingService;
        private readonly ShippingSettings _shippingSettings;
        private ISettingService _settingService;

		#endregion Fields 

		#region Constructors

        public ShippingController(IShippingService shippingService, ShippingSettings shippingSettings,
            ISettingService settingService)
		{
            this._shippingService = shippingService;
            this._shippingSettings = shippingSettings;
            this._settingService = settingService;
		}

		#endregion Constructors 
        
        #region Shipping methods

        public ActionResult Methods()
        {
            var shippingMethodsModel = _shippingService.GetAllShippingMethods()
                .Select(x => x.ToModel())
                .ToList();
            var model = new GridModel<ShippingMethodModel>
            {
                Data = shippingMethodsModel,
                Total = shippingMethodsModel.Count
            };
            return View(model);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult Methods(GridCommand command)
        {
            var shippingMethodsModel = _shippingService.GetAllShippingMethods()
                .Select(x => x.ToModel())
                .ForCommand(command)
                .ToList();
            var model = new GridModel<ShippingMethodModel>
            {
                Data = shippingMethodsModel,
                Total = shippingMethodsModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult MethodUpdate(ShippingMethodModel model, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Methods");
            }

            var shippingMethod = _shippingService.GetShippingMethodById(model.Id);
            shippingMethod = model.ToEntity(shippingMethod);
            _shippingService.UpdateShippingMethod(shippingMethod);

            var shippingMethodsModel = _shippingService.GetAllShippingMethods()
                 .Select(x => x.ToModel())
                 .ForCommand(command)
                 .ToList();
            var gridModel = new GridModel<ShippingMethodModel>
            {
                Data = shippingMethodsModel,
                Total = shippingMethodsModel.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult MethodAdd([Bind(Exclude = "Id")] ShippingMethodModel model, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                //TODO:Find out how telerik handles errors
                return new JsonResult { Data = "error" };
            }

            var shippingMethod = new ShippingMethod();
            shippingMethod = model.ToEntity(shippingMethod);
            _shippingService.InsertShippingMethod(shippingMethod);

            var shippingMethodsModel = _shippingService.GetAllShippingMethods()
                 .Select(x => x.ToModel())
                 .ForCommand(command)
                 .ToList();
            var gridModel = new GridModel<ShippingMethodModel>
            {
                Data = shippingMethodsModel,
                Total = shippingMethodsModel.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult MethodDelete(int id, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                //TODO:Find out how telerik handles errors
                return new JsonResult { Data = "error" };
            }

            var shippingMethod = _shippingService.GetShippingMethodById(id);
            _shippingService.DeleteShippingMethod(shippingMethod);

            var shippingMethodsModel = _shippingService.GetAllShippingMethods()
                .Select(x => x.ToModel())
                .ForCommand(command)
                .ToList();
            var gridModel = new GridModel<ShippingMethodModel>
            {
                Data = shippingMethodsModel,
                Total = shippingMethodsModel.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        #endregion
    }
}
