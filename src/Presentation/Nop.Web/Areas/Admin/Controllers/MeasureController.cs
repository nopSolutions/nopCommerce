using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Directory;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class MeasureController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IMeasureModelFactory _measureModelFactory;
        private readonly IMeasureService _measureService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly MeasureSettings _measureSettings;

        #endregion

        #region Ctor

        public MeasureController(ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IMeasureModelFactory measureModelFactory,
            IMeasureService measureService,
            IPermissionService permissionService,
            ISettingService settingService,
            MeasureSettings measureSettings)
        {
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _measureModelFactory = measureModelFactory;
            _measureService = measureService;
            _permissionService = permissionService;
            _settingService = settingService;
            _measureSettings = measureSettings;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = _measureModelFactory.PrepareMeasureSearchModel(new MeasureSearchModel());

            return View(model);
        }

        #region Weights

        [HttpPost]
        public virtual async Task<IActionResult> Weights(MeasureWeightSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _measureModelFactory.PrepareMeasureWeightListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> WeightUpdate(MeasureWeightModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var weight = await _measureService.GetMeasureWeightById(model.Id);
            weight = model.ToEntity(weight);
            await _measureService.UpdateMeasureWeight(weight);

            //activity log
            await _customerActivityService.InsertActivity("EditMeasureWeight",
                string.Format(await _localizationService.GetResource("ActivityLog.EditMeasureWeight"), weight.Id), weight);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> WeightAdd(MeasureWeightModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var weight = new MeasureWeight();
            weight = model.ToEntity(weight);
            await _measureService.InsertMeasureWeight(weight);

            //activity log
            await _customerActivityService.InsertActivity("AddNewMeasureWeight",
                string.Format(await _localizationService.GetResource("ActivityLog.AddNewMeasureWeight"), weight.Id), weight);

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> WeightDelete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a weight with the specified id
            var weight = await _measureService.GetMeasureWeightById(id)
                ?? throw new ArgumentException("No weight found with the specified id", nameof(id));

            if (weight.Id == _measureSettings.BaseWeightId)
            {
                return ErrorJson(await _localizationService.GetResource("Admin.Configuration.Shipping.Measures.Weights.CantDeletePrimary"));
            }

            await _measureService.DeleteMeasureWeight(weight);

            //activity log
            await _customerActivityService.InsertActivity("DeleteMeasureWeight",
                string.Format(await _localizationService.GetResource("ActivityLog.DeleteMeasureWeight"), weight.Id), weight);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> MarkAsPrimaryWeight(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a weight with the specified id
            var weight = _measureService.GetMeasureWeightById(id)
                ?? throw new ArgumentException("No weight found with the specified id", nameof(id));

            _measureSettings.BaseWeightId = weight.Id;
            await _settingService.SaveSetting(_measureSettings);

            return Json(new { result = true });
        }

        #endregion

        #region Dimensions

        [HttpPost]
        public virtual async Task<IActionResult> Dimensions(MeasureDimensionSearchModel searchModel)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _measureModelFactory.PrepareMeasureDimensionListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> DimensionUpdate(MeasureDimensionModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var dimension = await _measureService.GetMeasureDimensionById(model.Id);
            dimension = model.ToEntity(dimension);
            await _measureService.UpdateMeasureDimension(dimension);

            //activity log
            await _customerActivityService.InsertActivity("EditMeasureDimension",
                string.Format(await _localizationService.GetResource("ActivityLog.EditMeasureDimension"), dimension.Id), dimension);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> DimensionAdd(MeasureDimensionModel model)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var dimension = new MeasureDimension();
            dimension = model.ToEntity(dimension);
            await _measureService.InsertMeasureDimension(dimension);

            //activity log
            await _customerActivityService.InsertActivity("AddNewMeasureDimension",
                string.Format(await _localizationService.GetResource("ActivityLog.AddNewMeasureDimension"), dimension.Id), dimension);

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> DimensionDelete(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a dimension with the specified id
            var dimension = await _measureService.GetMeasureDimensionById(id)
                ?? throw new ArgumentException("No dimension found with the specified id", nameof(id));

            if (dimension.Id == _measureSettings.BaseDimensionId)
            {
                return ErrorJson(await _localizationService.GetResource("Admin.Configuration.Shipping.Measures.Dimensions.CantDeletePrimary"));
            }

            await _measureService.DeleteMeasureDimension(dimension);

            //activity log
            await _customerActivityService.InsertActivity("DeleteMeasureDimension",
                string.Format(await _localizationService.GetResource("ActivityLog.DeleteMeasureDimension"), dimension.Id), dimension);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> MarkAsPrimaryDimension(int id)
        {
            if (!await _permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a dimension with the specified id
            var dimension = _measureService.GetMeasureDimensionById(id)
                ?? throw new ArgumentException("No dimension found with the specified id", nameof(id));

            _measureSettings.BaseDimensionId = dimension.Id;
            await _settingService.SaveSetting(_measureSettings);

            return Json(new { result = true });
        }

        #endregion

        #endregion
    }
}