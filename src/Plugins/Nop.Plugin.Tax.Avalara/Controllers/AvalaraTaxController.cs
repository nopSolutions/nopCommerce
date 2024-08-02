using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Tax;
using Nop.Plugin.Tax.Avalara.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Tax;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Tax.Avalara.Controllers;

public class AvalaraTaxController : TaxController
{
    #region Fields

    protected readonly AvalaraTaxManager _avalaraTaxManager;
    protected readonly ILocalizationService _localizationService;
    protected readonly INotificationService _notificationService;
    protected readonly IStaticCacheManager _cacheManager;

    #endregion

    #region Ctor

    public AvalaraTaxController(AvalaraTaxManager avalaraTaxManager,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        ISettingService settingService,
        IStaticCacheManager cacheManager,
        ITaxCategoryService taxCategoryService,
        ITaxModelFactory taxModelFactory,
        ITaxPluginManager taxPluginManager,
        IWorkContext workContext,
        TaxSettings taxSettings) : base(permissionService,
        settingService,
        taxCategoryService,
        genericAttributeService,
        workContext,
        taxModelFactory,
        taxPluginManager,
        taxSettings)
    {
        _avalaraTaxManager = avalaraTaxManager;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _cacheManager = cacheManager;
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public override async Task<IActionResult> Categories()
    {
        //ensure that Avalara tax provider is active
        if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
        {
            //if isn't active return base action result
            RouteData.Values["controller"] = "Tax";
            return await base.Categories();
        }

        //prepare model
        var model = new Models.Tax.TaxCategorySearchModel();
        var cacheKey = _cacheManager.PrepareKeyForDefaultCache(AvalaraTaxDefaults.TaxCodeTypesCacheKey);
        var taxCodeTypes = await _cacheManager.GetAsync(cacheKey, async () => await _avalaraTaxManager.GetTaxCodeTypesAsync());

        if (taxCodeTypes != null)
            model.AvailableTypes = taxCodeTypes.Select(type => new SelectListItem(type.Value, type.Key)).ToList();
        model.SetGridPageSize();

        //use overridden view
        return View("~/Plugins/Tax.Avalara/Views/Tax/Categories.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public override async Task<IActionResult> Categories(TaxCategorySearchModel searchModel)
    {
        //ensure that Avalara tax provider is active
        if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
            return await base.Categories(searchModel);
        
        //get tax categories
        var taxCategories = (await _taxCategoryService.GetAllTaxCategoriesAsync()).ToPagedList(searchModel);

        //get tax types and define the default value
        var cacheKey = _cacheManager.PrepareKeyForDefaultCache(AvalaraTaxDefaults.TaxCodeTypesCacheKey);
        var taxTypes = (await _cacheManager.GetAsync(cacheKey, async () => await _avalaraTaxManager.GetTaxCodeTypesAsync()))
            ?.Select(taxType => new { Id = taxType.Key, Name = taxType.Value });
        var defaultType = taxTypes
            ?.FirstOrDefault(taxType => taxType.Name.Equals("Unknown", StringComparison.InvariantCultureIgnoreCase))
            ?? taxTypes?.FirstOrDefault();

        //prepare grid model
        var model = await new Models.Tax.TaxCategoryListModel().PrepareToGridAsync(searchModel, taxCategories, () =>
        {
            //fill in model values from the entity
            return taxCategories.SelectAwait(async taxCategory =>
            {
                //fill in model values from the entity
                var taxCategoryModel = new Models.Tax.TaxCategoryModel
                {
                    Id = taxCategory.Id,
                    Name = taxCategory.Name,
                    DisplayOrder = taxCategory.DisplayOrder
                };

                //try to get previously saved tax code type and description
                var taxCodeType = (await taxTypes?.FirstOrDefaultAwaitAsync(async type =>
                        type.Id.Equals((await _genericAttributeService.GetAttributeAsync<string>(taxCategory, AvalaraTaxDefaults.TaxCodeTypeAttribute)) ?? string.Empty)))
                    ?? defaultType;
                taxCategoryModel.Type = taxCodeType?.Name ?? string.Empty;
                taxCategoryModel.TypeId = taxCodeType?.Id ?? Guid.Empty.ToString();
                taxCategoryModel.Description = (await _genericAttributeService
                    .GetAttributeAsync<string>(taxCategory, AvalaraTaxDefaults.TaxCodeDescriptionAttribute)) ?? string.Empty;

                return taxCategoryModel;
            });
        });

        return Json(model);
    }

    [HttpPost]
    public async Task<IActionResult> TaxCategoryUpdate(Models.Tax.TaxCategoryModel model)
    {
        return await base.CategoryUpdate(model);
    }

    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    [HttpPost]
    public async Task<IActionResult> TaxCategoryAdd(Models.Tax.TaxCategoryModel model)
    {
        //ensure that Avalara tax provider is active
        if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
            return new NullJsonResult();
        
        if (!ModelState.IsValid)
            return ErrorJson(ModelState.SerializeErrors());

        var taxCategory = new TaxCategory();
        taxCategory = model.ToEntity(taxCategory);
        await _taxCategoryService.InsertTaxCategoryAsync(taxCategory);

        //save tax code type as generic attribute
        if (!string.IsNullOrEmpty(model.TypeId) && !model.TypeId.Equals(Guid.Empty.ToString()))
            await _genericAttributeService.SaveAttributeAsync(taxCategory, AvalaraTaxDefaults.TaxCodeTypeAttribute, model.TypeId);

        return Json(new { Result = true });
    }

    [HttpPost]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public override async Task<IActionResult> CategoryDelete(int id)
    {
        //ensure that Avalara tax provider is active
        if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
            return new NullJsonResult();
        
        //try to get a tax category with the specified id
        var taxCategory = await _taxCategoryService.GetTaxCategoryByIdAsync(id)
            ?? throw new ArgumentException("No tax category found with the specified id");

        //delete generic attributes 
        await _genericAttributeService.SaveAttributeAsync<string>(taxCategory, AvalaraTaxDefaults.TaxCodeDescriptionAttribute, null);
        await _genericAttributeService.SaveAttributeAsync<string>(taxCategory, AvalaraTaxDefaults.TaxCodeTypeAttribute, null);

        await _taxCategoryService.DeleteTaxCategoryAsync(taxCategory);

        return new NullJsonResult();
    }

    [HttpPost, ActionName("Categories")]
    [FormValueRequired("importTaxCodes")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> ImportTaxCodes()
    {
        //ensure that Avalara tax provider is active
        if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
            return await Categories();
        
        //import tax caodes
        var importedTaxCodesNumber = await _avalaraTaxManager.ImportTaxCodesAsync();
        if (importedTaxCodesNumber.HasValue)
        {
            //successfully imported
            var successMessage = await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.TaxCodes.Import.Success");
            _notificationService.SuccessNotification(string.Format(successMessage, importedTaxCodesNumber));
        }
        else
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.TaxCodes.Import.Error"));

        return await Categories();
    }

    [HttpPost, ActionName("Categories")]
    [FormValueRequired("exportTaxCodes")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> ExportTaxCodes()
    {
        //ensure that Avalara tax provider is active
        if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
            return await Categories();
        
        //export tax codes
        var exportedTaxCodes = await _avalaraTaxManager.ExportTaxCodesAsync();
        if (exportedTaxCodes.HasValue)
        {
            if (exportedTaxCodes > 0)
                _notificationService.SuccessNotification(string.Format(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.TaxCodes.Export.Success"), exportedTaxCodes));
            else
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.TaxCodes.Export.AlreadyExported"));
        }
        else
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.TaxCodes.Export.Error"));

        return await Categories();
    }

    [HttpPost, ActionName("Categories")]
    [FormValueRequired("deleteTaxCodes")]
    [CheckPermission(StandardPermission.Configuration.MANAGE_TAX_SETTINGS)]
    public async Task<IActionResult> DeleteSystemTaxCodes()
    {
        //ensure that Avalara tax provider is active
        if (!await _taxPluginManager.IsPluginActiveAsync(AvalaraTaxDefaults.SystemName))
            return await Categories();
        
        var deleted = await _avalaraTaxManager.DeleteSystemTaxCodesAsync();
        if (deleted)
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.TaxCodes.Delete.Success"));
        else
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Tax.Avalara.TaxCodes.Delete.Error"));

        return await Categories();
    }

    #endregion
}