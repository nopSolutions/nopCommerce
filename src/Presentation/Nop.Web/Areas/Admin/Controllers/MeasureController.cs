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
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class MeasureController : BaseAdminController
{
    #region Fields

    protected readonly ICustomerActivityService _customerActivityService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IMeasureModelFactory _measureModelFactory;
    protected readonly IMeasureService _measureService;
    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly MeasureSettings _measureSettings;

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

    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> List()
    {
        //prepare model
        var model = await _measureModelFactory.PrepareMeasureSearchModelAsync(new MeasureSearchModel());

        return View(model);
    }

    #region Weights

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> Weights(MeasureWeightSearchModel searchModel)
    {
        //prepare model
        var model = await _measureModelFactory.PrepareMeasureWeightListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> WeightUpdate(MeasureWeightModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        var weight = await _measureService.GetMeasureWeightByIdAsync(model.Id);
        weight = model.ToEntity(weight);
        await _measureService.UpdateMeasureWeightAsync(weight);

        //activity log
        await _customerActivityService.InsertActivityAsync("EditMeasureWeight",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditMeasureWeight"), weight.Id), weight);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> WeightAdd(MeasureWeightModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        var weight = new MeasureWeight();
        weight = model.ToEntity(weight);
        await _measureService.InsertMeasureWeightAsync(weight);

        //activity log
        await _customerActivityService.InsertActivityAsync("AddNewMeasureWeight",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewMeasureWeight"), weight.Id), weight);

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> WeightDelete(int id)
    {
        //try to get a weight with the specified id
        var weight = await _measureService.GetMeasureWeightByIdAsync(id)
            ?? throw new ArgumentException("No weight found with the specified id", nameof(id));

        if (weight.Id == _measureSettings.BaseWeightId)
        {
            return ErrorJson(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Measures.Weights.CantDeletePrimary"));
        }

        await _measureService.DeleteMeasureWeightAsync(weight);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteMeasureWeight",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteMeasureWeight"), weight.Id), weight);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> MarkAsPrimaryWeight(int id)
    {
        //try to get a weight with the specified id
        var weight = await _measureService.GetMeasureWeightByIdAsync(id)
            ?? throw new ArgumentException("No weight found with the specified id", nameof(id));

        _measureSettings.BaseWeightId = weight.Id;
        await _settingService.SaveSettingAsync(_measureSettings);

        return Json(new { result = true });
    }

    #endregion

    #region Dimensions

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> Dimensions(MeasureDimensionSearchModel searchModel)
    {
        //prepare model
        var model = await _measureModelFactory.PrepareMeasureDimensionListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> DimensionUpdate(MeasureDimensionModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        var dimension = await _measureService.GetMeasureDimensionByIdAsync(model.Id);
        dimension = model.ToEntity(dimension);
        await _measureService.UpdateMeasureDimensionAsync(dimension);

        //activity log
        await _customerActivityService.InsertActivityAsync("EditMeasureDimension",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditMeasureDimension"), dimension.Id), dimension);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> DimensionAdd(MeasureDimensionModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        var dimension = new MeasureDimension();
        dimension = model.ToEntity(dimension);
        await _measureService.InsertMeasureDimensionAsync(dimension);

        //activity log
        await _customerActivityService.InsertActivityAsync("AddNewMeasureDimension",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewMeasureDimension"), dimension.Id), dimension);

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> DimensionDelete(int id)
    {
        //try to get a dimension with the specified id
        var dimension = await _measureService.GetMeasureDimensionByIdAsync(id)
            ?? throw new ArgumentException("No dimension found with the specified id", nameof(id));

        if (dimension.Id == _measureSettings.BaseDimensionId)
        {
            return ErrorJson(await _localizationService.GetResourceAsync("Admin.Configuration.Shipping.Measures.Dimensions.CantDeletePrimary"));
        }

        await _measureService.DeleteMeasureDimensionAsync(dimension);

        //activity log
        await _customerActivityService.InsertActivityAsync("DeleteMeasureDimension",
            string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteMeasureDimension"), dimension.Id), dimension);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_SHIPPING_SETTINGS)]
    public virtual async Task<IActionResult> MarkAsPrimaryDimension(int id)
    {
        //try to get a dimension with the specified id
        var dimension = await _measureService.GetMeasureDimensionByIdAsync(id)
            ?? throw new ArgumentException("No dimension found with the specified id", nameof(id));

        _measureSettings.BaseDimensionId = dimension.Id;
        await _settingService.SaveSettingAsync(_measureSettings);

        return Json(new { result = true });
    }

    #endregion

    #endregion
}