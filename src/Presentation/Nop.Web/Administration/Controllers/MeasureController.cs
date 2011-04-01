using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using Nop.Admin.Models;
using Telerik.Web.Mvc;
using Telerik.Web.Mvc.UI;
using Nop.Web.Framework;
using Telerik.Web.Mvc.Extensions;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public class MeasureController : BaseNopController
	{
		#region Fields

		private readonly IMeasureService _measureService;

		#endregion Fields 

		#region Constructors

        public MeasureController(IMeasureService measureService)
		{
            this._measureService = measureService;
		}

		#endregion Constructors 

		#region Methods

		public ActionResult Weights()
		{
            var weights = _measureService.GetAllMeasureWeights();
            var gridModel = new GridModel<MeasureWeightModel>
			{
				Data = weights.Select(x => x.ToModel()),
                Total = weights.Count
			};
			return View(gridModel);
		}

		[HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult Weights(GridCommand command)
        {
            var weights = _measureService.GetAllMeasureWeights()
                .Select(x => x.ToModel())
                .ForCommand(command);

            var model = new GridModel<MeasureWeightModel>
                            {
                                Data = weights,
                                Total = weights.Count()
                            };
		    return new JsonResult
			{
				Data = model
			};
		}

        [GridAction(EnableCustomBinding=true)]
        public ActionResult WeightUpdate(MeasureWeightModel model, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Weights");
            }

            var weight = _measureService.GetMeasureWeightById(model.Id);
            weight = model.ToEntity(weight);
            _measureService.UpdateMeasureWeight(weight);


            var weights = _measureService.GetAllMeasureWeights()
                .Select(x => x.ToModel())
                .ForCommand(command);
            var gridModel = new GridModel<MeasureWeightModel>
                                {
                                    Data = weights,
                                    Total = weights.Count()
                                };
            return new JsonResult
            {
                Data = gridModel
            };
        }
        
        [GridAction(EnableCustomBinding = true)]
        public ActionResult WeightAdd(MeasureWeightModel weightModel, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                //TODO:Find out how telerik handles errors
                return new JsonResult {Data = "error"};
            }

            var weight = new MeasureWeight();
            weight = weightModel.ToEntity(weight);
            _measureService.InsertMeasureWeight(weight);

            var weights = _measureService.GetAllMeasureWeights()
                .Select(x => x.ToModel())
                .ForCommand(command);
            var gridModel = new GridModel<MeasureWeightModel>
            {
                Data = weights,
                Total = weights.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult WeightDelete(int id,  GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                //TODO:Find out how telerik handles errors
                return new JsonResult { Data = "error" };
            }

            var weight = _measureService.GetMeasureWeightById(id);
            _measureService.DeleteMeasureWeight(weight);

            var weights = _measureService.GetAllMeasureWeights()
                .Select(x => x.ToModel())
                .ForCommand(command);
            var gridModel = new GridModel<MeasureWeightModel>
            {
                Data = weights,
                Total = weights.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        #endregion
    }
}
