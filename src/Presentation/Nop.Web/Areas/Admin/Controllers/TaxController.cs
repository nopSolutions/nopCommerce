using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Tax;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Tax;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Controllers;

public partial class TaxController : BaseAdminController
{
    #region Fields

    protected readonly IPermissionService _permissionService;
    protected readonly ISettingService _settingService;
    protected readonly ITaxCategoryService _taxCategoryService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IWorkContext _workContext;
    protected readonly ITaxModelFactory _taxModelFactory;
    protected readonly ITaxPluginManager _taxPluginManager;
    protected readonly TaxSettings _taxSettings;

    #endregion

    #region Ctor

    public TaxController(IPermissionService permissionService,
        ISettingService settingService,
        ITaxCategoryService taxCategoryService,
        IGenericAttributeService genericAttributeService,
        IWorkContext workContext,
        ITaxModelFactory taxModelFactory,
        ITaxPluginManager taxPluginManager,
        TaxSettings taxSettings)
    {
        _permissionService = permissionService;
        _settingService = settingService;
        _taxCategoryService = taxCategoryService;
        _genericAttributeService = genericAttributeService;
        _workContext = workContext;
        _taxModelFactory = taxModelFactory;
        _taxPluginManager = taxPluginManager;
        _taxSettings = taxSettings;
    }

    #endregion

    #region Methods

    #region Tax Providers

    public virtual IActionResult List()
    {
        return RedirectToAction("Providers");
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public virtual async Task<IActionResult> Providers(bool showtour = false)
    {
        //prepare model
        var model = await _taxModelFactory.PrepareTaxProviderSearchModelAsync(new TaxProviderSearchModel());

        //show configuration tour
        if (showtour)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.HideConfigurationStepsAttribute);
            var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.CloseConfigurationStepsAttribute);

            if (!hideCard && !closeCard)
                ViewBag.ShowTour = true;
        }

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public virtual async Task<IActionResult> Providers(TaxProviderSearchModel searchModel)
    {
        //prepare model
        var model = await _taxModelFactory.PrepareTaxProviderListModelAsync(searchModel);

        return Json(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public virtual async Task<IActionResult> MarkAsPrimaryProvider(string systemName)
    {
        if (string.IsNullOrEmpty(systemName))
            return RedirectToAction("Providers");

        var taxProvider = await _taxPluginManager.LoadPluginBySystemNameAsync(systemName);
        if (taxProvider == null)
            return RedirectToAction("Providers");

        _taxSettings.ActiveTaxProviderSystemName = systemName;
        await _settingService.SaveSettingAsync(_taxSettings);

        return RedirectToAction("Providers");
    }

    #endregion

    #region Tax Categories

    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public virtual async Task<IActionResult> Categories()
    {
        //prepare model
        var model = await _taxModelFactory.PrepareTaxCategorySearchModelAsync(new TaxCategorySearchModel());

        return View(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public virtual async Task<IActionResult> Categories(TaxCategorySearchModel searchModel)
    {
        //prepare model
        var model = await _taxModelFactory.PrepareTaxCategoryListModelAsync(searchModel);

        return Json(model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public virtual async Task<IActionResult> CategoryUpdate(TaxCategoryModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        var taxCategory = await _taxCategoryService.GetTaxCategoryByIdAsync(model.Id);
        taxCategory = model.ToEntity(taxCategory);
        await _taxCategoryService.UpdateTaxCategoryAsync(taxCategory);

        return new NullJsonResult();
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public virtual async Task<IActionResult> CategoryAdd(TaxCategoryModel model)
    {
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        var taxCategory = new TaxCategory();
        taxCategory = model.ToEntity(taxCategory);
        await _taxCategoryService.InsertTaxCategoryAsync(taxCategory);

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public virtual async Task<IActionResult> CategoryDelete(int id)
    {
        //try to get a tax category with the specified id
        var taxCategory = await _taxCategoryService.GetTaxCategoryByIdAsync(id)
            ?? throw new ArgumentException("No tax category found with the specified id", nameof(id));

        await _taxCategoryService.DeleteTaxCategoryAsync(taxCategory);

        return new NullJsonResult();
    }

    #endregion

    #endregion
}