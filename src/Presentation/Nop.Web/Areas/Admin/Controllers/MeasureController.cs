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

        protected ICustomerActivityService CustomerActivityService { get; }
        protected ILocalizationService LocalizationService { get; }
        protected IMeasureModelFactory MeasureModelFactory { get; }
        protected IMeasureService MeasureService { get; }
        protected IPermissionService PermissionService { get; }
        protected ISettingService SettingService { get; }
        protected MeasureSettings MeasureSettings { get; }

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
            CustomerActivityService = customerActivityService;
            LocalizationService = localizationService;
            MeasureModelFactory = measureModelFactory;
            MeasureService = measureService;
            PermissionService = permissionService;
            SettingService = settingService;
            MeasureSettings = measureSettings;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> List()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //prepare model
            var model = await MeasureModelFactory.PrepareMeasureSearchModelAsync(new MeasureSearchModel());

            return View(model);
        }

        #region Weights

        [HttpPost]
        public virtual async Task<IActionResult> Weights(MeasureWeightSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await MeasureModelFactory.PrepareMeasureWeightListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> WeightUpdate(MeasureWeightModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var weight = await MeasureService.GetMeasureWeightByIdAsync(model.Id);
            weight = model.ToEntity(weight);
            await MeasureService.UpdateMeasureWeightAsync(weight);

            //activity log
            await CustomerActivityService.InsertActivityAsync("EditMeasureWeight",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditMeasureWeight"), weight.Id), weight);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> WeightAdd(MeasureWeightModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var weight = new MeasureWeight();
            weight = model.ToEntity(weight);
            await MeasureService.InsertMeasureWeightAsync(weight);

            //activity log
            await CustomerActivityService.InsertActivityAsync("AddNewMeasureWeight",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewMeasureWeight"), weight.Id), weight);

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> WeightDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a weight with the specified id
            var weight = await MeasureService.GetMeasureWeightByIdAsync(id)
                ?? throw new ArgumentException("No weight found with the specified id", nameof(id));

            if (weight.Id == MeasureSettings.BaseWeightId)
            {
                return ErrorJson(await LocalizationService.GetResourceAsync("Admin.Configuration.Shipping.Measures.Weights.CantDeletePrimary"));
            }

            await MeasureService.DeleteMeasureWeightAsync(weight);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteMeasureWeight",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteMeasureWeight"), weight.Id), weight);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> MarkAsPrimaryWeight(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a weight with the specified id
            var weight = await MeasureService.GetMeasureWeightByIdAsync(id)
                ?? throw new ArgumentException("No weight found with the specified id", nameof(id));

            MeasureSettings.BaseWeightId = weight.Id;
            await SettingService.SaveSettingAsync(MeasureSettings);

            return Json(new { result = true });
        }

        #endregion

        #region Dimensions

        [HttpPost]
        public virtual async Task<IActionResult> Dimensions(MeasureDimensionSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await MeasureModelFactory.PrepareMeasureDimensionListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> DimensionUpdate(MeasureDimensionModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var dimension = await MeasureService.GetMeasureDimensionByIdAsync(model.Id);
            dimension = model.ToEntity(dimension);
            await MeasureService.UpdateMeasureDimensionAsync(dimension);

            //activity log
            await CustomerActivityService.InsertActivityAsync("EditMeasureDimension",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.EditMeasureDimension"), dimension.Id), dimension);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> DimensionAdd(MeasureDimensionModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var dimension = new MeasureDimension();
            dimension = model.ToEntity(dimension);
            await MeasureService.InsertMeasureDimensionAsync(dimension);

            //activity log
            await CustomerActivityService.InsertActivityAsync("AddNewMeasureDimension",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.AddNewMeasureDimension"), dimension.Id), dimension);

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> DimensionDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a dimension with the specified id
            var dimension = await MeasureService.GetMeasureDimensionByIdAsync(id)
                ?? throw new ArgumentException("No dimension found with the specified id", nameof(id));

            if (dimension.Id == MeasureSettings.BaseDimensionId)
            {
                return ErrorJson(await LocalizationService.GetResourceAsync("Admin.Configuration.Shipping.Measures.Dimensions.CantDeletePrimary"));
            }

            await MeasureService.DeleteMeasureDimensionAsync(dimension);

            //activity log
            await CustomerActivityService.InsertActivityAsync("DeleteMeasureDimension",
                string.Format(await LocalizationService.GetResourceAsync("ActivityLog.DeleteMeasureDimension"), dimension.Id), dimension);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> MarkAsPrimaryDimension(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            //try to get a dimension with the specified id
            var dimension = await MeasureService.GetMeasureDimensionByIdAsync(id)
                ?? throw new ArgumentException("No dimension found with the specified id", nameof(id));

            MeasureSettings.BaseDimensionId = dimension.Id;
            await SettingService.SaveSettingAsync(MeasureSettings);

            return Json(new { result = true });
        }

        #endregion

        #endregion
    }
}