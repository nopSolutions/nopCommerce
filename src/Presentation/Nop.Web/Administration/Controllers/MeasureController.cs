using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
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
        private readonly MeasureSettings _measureSettings;
        private readonly ISettingService _settingService;

		#endregion Fields 

		#region Constructors

        public MeasureController(IMeasureService measureService,
            MeasureSettings measureSettings, ISettingService settingService)
		{
            this._measureService = measureService;
            this._measureSettings = measureSettings;
            this._settingService = settingService;
		}

		#endregion Constructors 

		#region Methods

		public ActionResult Weights(string id)
		{
            //mark as primary weight (if selected)
            if (!String.IsNullOrEmpty(id))
            {
                int primaryWeightId = Convert.ToInt32(id);
                var primaryWeight = _measureService.GetMeasureWeightById(primaryWeightId);
                if (primaryWeight != null)
                {
                    _measureSettings.BaseWeightId = primaryWeightId;
                    _settingService.SaveSetting(_measureSettings);
                }
            }

            var weightsModel = _measureService.GetAllMeasureWeights()
                .Select(x => x.ToModel())
                .ToList();
            foreach (var wm in weightsModel)
                wm.IsPrimaryWeight = wm.Id == _measureSettings.BaseWeightId;
            var model = new GridModel<MeasureWeightModel>
			{
                Data = weightsModel,
                Total = weightsModel.Count
			};
            return View(model);
		}

		[HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult Weights(GridCommand command)
        {
            var weightsModel = _measureService.GetAllMeasureWeights()
                .Select(x => x.ToModel())
                .ForCommand(command)
                .ToList();
            foreach (var wm in weightsModel)
                wm.IsPrimaryWeight = wm.Id == _measureSettings.BaseWeightId;
            var model = new GridModel<MeasureWeightModel>
            {
                Data = weightsModel,
                Total = weightsModel.Count
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


            var weightsModel = _measureService.GetAllMeasureWeights()
                .Select(x => x.ToModel())
                .ForCommand(command)
                .ToList();
            foreach (var wm in weightsModel)
                wm.IsPrimaryWeight = wm.Id == _measureSettings.BaseWeightId;
            var gridModel = new GridModel<MeasureWeightModel>
            {
                Data = weightsModel,
                Total = weightsModel.Count
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

            var weightsModel = _measureService.GetAllMeasureWeights()
                .Select(x => x.ToModel())
                .ForCommand(command)
                .ToList();
            foreach (var wm in weightsModel)
                wm.IsPrimaryWeight = wm.Id == _measureSettings.BaseWeightId;
            var gridModel = new GridModel<MeasureWeightModel>
            {
                Data = weightsModel,
                Total = weightsModel.Count
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

            var weightsModel = _measureService.GetAllMeasureWeights()
                .Select(x => x.ToModel())
                .ForCommand(command)
                .ToList();
            foreach (var wm in weightsModel)
                wm.IsPrimaryWeight = wm.Id == _measureSettings.BaseWeightId;
            var gridModel = new GridModel<MeasureWeightModel>
            {
                Data = weightsModel,
                Total = weightsModel.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }
        
        #endregion
    }
}
