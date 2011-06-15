using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models;
using Nop.Admin.Models.Directory;
using Nop.Core.Domain.Directory;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public class MeasureController : BaseNopController
	{
		#region Fields

        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;
        private readonly ISettingService _settingService;

		#endregion

		#region Constructors

        public MeasureController(IMeasureService measureService,
            MeasureSettings measureSettings, ISettingService settingService)
		{
            this._measureService = measureService;
            this._measureSettings = measureSettings;
            this._settingService = settingService;
		}

		#endregion 

		#region Methods
        
        #region Weights

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
                return new JsonResult { Data = "error" };
            }

            var weight = _measureService.GetMeasureWeightById(model.Id);
            weight = model.ToEntity(weight);
            _measureService.UpdateMeasureWeight(weight);
            
            return Weights(command);
        }
        
        [GridAction(EnableCustomBinding = true)]
        public ActionResult WeightAdd([Bind(Exclude="Id")] MeasureWeightModel model, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult {Data = "error"};
            }

            var weight = new MeasureWeight();
            weight = model.ToEntity(weight);
            _measureService.InsertMeasureWeight(weight);
            
            return Weights(command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult WeightDelete(int id,  GridCommand command)
        {
            var weight = _measureService.GetMeasureWeightById(id);
            _measureService.DeleteMeasureWeight(weight);

            return Weights(command);
        }

        #endregion

        #region Dimensions

        public ActionResult Dimensions(string id)
        {
            //mark as primary dimension (if selected)
            if (!String.IsNullOrEmpty(id))
            {
                int primaryDimensionId = Convert.ToInt32(id);
                var primaryDimension = _measureService.GetMeasureDimensionById(primaryDimensionId);
                if (primaryDimension != null)
                {
                    _measureSettings.BaseDimensionId = primaryDimensionId;
                    _settingService.SaveSetting(_measureSettings);
                }
            }

            var dimensionsModel = _measureService.GetAllMeasureDimensions()
                .Select(x => x.ToModel())
                .ToList();
            foreach (var wm in dimensionsModel)
                wm.IsPrimaryDimension = wm.Id == _measureSettings.BaseDimensionId;
            var model = new GridModel<MeasureDimensionModel>
            {
                Data = dimensionsModel,
                Total = dimensionsModel.Count
            };
            return View(model);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult Dimensions(GridCommand command)
        {
            var dimensionsModel = _measureService.GetAllMeasureDimensions()
                .Select(x => x.ToModel())
                .ForCommand(command)
                .ToList();
            foreach (var wm in dimensionsModel)
                wm.IsPrimaryDimension = wm.Id == _measureSettings.BaseDimensionId;
            var model = new GridModel<MeasureDimensionModel>
            {
                Data = dimensionsModel,
                Total = dimensionsModel.Count
            };

            return new JsonResult
            {
                Data = model
            };
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult DimensionUpdate(MeasureDimensionModel model, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult { Data = "error" };
            }

            var dimension = _measureService.GetMeasureDimensionById(model.Id);
            dimension = model.ToEntity(dimension);
            _measureService.UpdateMeasureDimension(dimension);
            
            return Dimensions(command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult DimensionAdd([Bind(Exclude="Id")] MeasureDimensionModel model, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult { Data = "error" };
            }

            var dimension = new MeasureDimension();
            dimension = model.ToEntity(dimension);
            _measureService.InsertMeasureDimension(dimension);

            return Dimensions(command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult DimensionDelete(int id, GridCommand command)
        {
            var dimension = _measureService.GetMeasureDimensionById(id);
            _measureService.DeleteMeasureDimension(dimension);

            return Dimensions(command);
        }

        #endregion

        #endregion
    }
}
