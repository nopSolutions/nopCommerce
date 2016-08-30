using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Directory;
using Nop.Core.Domain.Directory;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Controllers
{
    public partial class MeasureController : BaseAdminController
	{
		#region Fields

        private readonly IMeasureService _measureService;
        private readonly MeasureSettings _measureSettings;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;

		#endregion

		#region Constructors

        public MeasureController(IMeasureService measureService,
            MeasureSettings measureSettings, ISettingService settingService,
            IPermissionService permissionService, ILocalizationService localizationService)
		{
            this._measureService = measureService;
            this._measureSettings = measureSettings;
            this._settingService = settingService;
            this._permissionService = permissionService;
            this._localizationService = localizationService;
		}

        #endregion

        #region Methods

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            return View();
        }

        #region Weights


        [HttpPost]
        public ActionResult Weights(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var weightsModel = _measureService.GetAllMeasureWeights()
                .Select(x => x.ToModel())
                .ToList();
            foreach (var wm in weightsModel)
                wm.IsPrimaryWeight = wm.Id == _measureSettings.BaseWeightId;
            var gridModel = new DataSourceResult
            {
                Data = weightsModel,
                Total = weightsModel.Count
            };

            return Json(gridModel);
		}

        [HttpPost]
        public ActionResult WeightUpdate(MeasureWeightModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();
            
            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var weight = _measureService.GetMeasureWeightById(model.Id);
            weight = model.ToEntity(weight);
            _measureService.UpdateMeasureWeight(weight);

            return new NullJsonResult();
        }
        
        [HttpPost]
        public ActionResult WeightAdd([Bind(Exclude="Id")] MeasureWeightModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult {Errors = ModelState.SerializeErrors()});
            }

            var weight = new MeasureWeight();
            weight = model.ToEntity(weight);
            _measureService.InsertMeasureWeight(weight);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult WeightDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var weight = _measureService.GetMeasureWeightById(id);
            if (weight == null)
                throw new ArgumentException("No weight found with the specified id");

            if (weight.Id == _measureSettings.BaseWeightId)
            {
                return Json(new DataSourceResult { Errors = _localizationService.GetResource("Admin.Configuration.Shipping.Measures.Weights.CantDeletePrimary") });
            }

            _measureService.DeleteMeasureWeight(weight);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult MarkAsPrimaryWeight(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var primaryWeight = _measureService.GetMeasureWeightById(id);
            if (primaryWeight != null)
            {
                _measureSettings.BaseWeightId = primaryWeight.Id;
                _settingService.SaveSetting(_measureSettings);
            }

            return Json(new { result = true });
        }

        #endregion

        #region Dimensions

        [HttpPost]
        public ActionResult Dimensions(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var dimensionsModel = _measureService.GetAllMeasureDimensions()
                .Select(x => x.ToModel())
                .ToList();
            foreach (var wm in dimensionsModel)
                wm.IsPrimaryDimension = wm.Id == _measureSettings.BaseDimensionId;
            var gridModel = new DataSourceResult
            {
                Data = dimensionsModel,
                Total = dimensionsModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult DimensionUpdate(MeasureDimensionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var dimension = _measureService.GetMeasureDimensionById(model.Id);
            dimension = model.ToEntity(dimension);
            _measureService.UpdateMeasureDimension(dimension);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult DimensionAdd([Bind(Exclude = "Id")] MeasureDimensionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var dimension = new MeasureDimension();
            dimension = model.ToEntity(dimension);
            _measureService.InsertMeasureDimension(dimension);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult DimensionDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var dimension = _measureService.GetMeasureDimensionById(id);
            if (dimension == null)
                throw new ArgumentException("No dimension found with the specified id");

            if (dimension.Id == _measureSettings.BaseDimensionId)
            {
                return Json(new DataSourceResult { Errors = _localizationService.GetResource("Admin.Configuration.Shipping.Measures.Dimensions.CantDeletePrimary") });
            }

            _measureService.DeleteMeasureDimension(dimension);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult MarkAsPrimaryDimension(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var primaryDimension = _measureService.GetMeasureDimensionById(id);
            if (primaryDimension != null)
            {
                _measureSettings.BaseDimensionId = id;
                _settingService.SaveSetting(_measureSettings);
            }

            return Json(new { result = true });
        }

        #endregion

        #endregion
    }
}
