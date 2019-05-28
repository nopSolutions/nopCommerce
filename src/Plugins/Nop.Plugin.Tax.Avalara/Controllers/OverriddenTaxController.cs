using System;
using System.Collections.Generic;
using System.Linq;
using Avalara.AvaTax.RestClient;
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
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Tax.Avalara.Controllers
{
    public class OverriddenTaxController : TaxController
    {
        #region Fields

        private readonly AvalaraTaxManager _avalaraTaxManager;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly ITaxPluginManager _taxPluginManager;

        #endregion

        #region Ctor

        public OverriddenTaxController(AvalaraTaxManager avalaraTaxManager,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStaticCacheManager cacheManager,
            ITaxCategoryService taxCategoryService,
            ITaxModelFactory taxModelFactory,
            ITaxPluginManager taxPluginManager,
            TaxSettings taxSettings) : base(permissionService,
                settingService,
                taxCategoryService,
                taxModelFactory,
                taxPluginManager,
                taxSettings)
        {
            _avalaraTaxManager = avalaraTaxManager;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _cacheManager = cacheManager;
            _taxCategoryService = taxCategoryService;
            _taxPluginManager = taxPluginManager;
        }

        #endregion

        #region Methods

        public override IActionResult Categories()
        {
            //ensure that Avalara tax provider is active
            if (!_taxPluginManager.IsPluginActive(AvalaraTaxDefaults.SystemName))
            {
                //if isn't active return base action result
                RouteData.Values["controller"] = "Tax";
                return base.Categories();
            }

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            //prepare model
            var model = new Models.Tax.TaxCategorySearchModel();
            var taxCodeTypes = _cacheManager.Get(AvalaraTaxDefaults.TaxCodeTypesCacheKey, () => _avalaraTaxManager.GetTaxCodeTypes());
            if (taxCodeTypes != null)
                model.AvailableTypes = taxCodeTypes.Select(type => new SelectListItem(type.Value, type.Key)).ToList();
            model.SetGridPageSize();

            //use overridden view
            return View("~/Plugins/Tax.Avalara/Views/Tax/Categories.cshtml", model);
        }

        [HttpPost]
        public IActionResult TaxCategoryUpdate(Models.Tax.TaxCategoryModel model)
        {
            return base.CategoryUpdate(model);
        }

        [HttpPost]
        public IActionResult TaxCategoryAdd(Models.Tax.TaxCategoryModel model)
        {
            //ensure that Avalara tax provider is active
            if (!_taxPluginManager.IsPluginActive(AvalaraTaxDefaults.SystemName))
                return new NullJsonResult();

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var taxCategory = new TaxCategory();
            taxCategory = model.ToEntity(taxCategory);
            _taxCategoryService.InsertTaxCategory(taxCategory);

            //save tax code type as generic attribute
            if (!string.IsNullOrEmpty(model.TypeId) && !model.TypeId.Equals(Guid.Empty.ToString()))
                _genericAttributeService.SaveAttribute(taxCategory, AvalaraTaxDefaults.TaxCodeTypeAttribute, model.TypeId);

            return Json(new { Result = true });
        }

        [HttpPost]
        public override IActionResult CategoryDelete(int id)
        {
            //ensure that Avalara tax provider is active
            if (!_taxPluginManager.IsPluginActive(AvalaraTaxDefaults.SystemName))
                return new NullJsonResult();

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            //try to get a tax category with the specified id
            var taxCategory = _taxCategoryService.GetTaxCategoryById(id);
            if (taxCategory == null)
                throw new ArgumentException("No tax category found with the specified id");

            //delete generic attributes 
            _genericAttributeService.SaveAttribute<string>(taxCategory, AvalaraTaxDefaults.TaxCodeDescriptionAttribute, null);
            _genericAttributeService.SaveAttribute<string>(taxCategory, AvalaraTaxDefaults.TaxCodeTypeAttribute, null);

            _taxCategoryService.DeleteTaxCategory(taxCategory);

            return new NullJsonResult();
        }

        [HttpPost, ActionName("Categories")]
        [FormValueRequired("importTaxCodes")]
        public IActionResult ImportTaxCodes()
        {
            //ensure that Avalara tax provider is active
            if (!_taxPluginManager.IsPluginActive(AvalaraTaxDefaults.SystemName))
                return Categories();

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            //get Avalara pre-defined system tax codes (only active)
            var systemTaxCodes = _avalaraTaxManager.GetSystemTaxCodes(true);
            if (!systemTaxCodes?.Any() ?? true)
            {
                _notificationService.ErrorNotification(_localizationService.GetResource("Plugins.Tax.Avalara.TaxCodes.Import.Error"));
                return Categories();
            }

            //get existing tax categories
            var existingTaxCategories = _taxCategoryService.GetAllTaxCategories().Select(taxCategory => taxCategory.Name).ToList();

            //remove duplicates
            var taxCodesToImport = systemTaxCodes.Where(taxCode => !existingTaxCategories.Contains(taxCode.taxCode)).ToList();

            var importedTaxCodesNumber = 0;
            foreach (var taxCode in taxCodesToImport)
            {
                if (string.IsNullOrEmpty(taxCode?.taxCode))
                    continue;

                //create new tax category
                var taxCategory = new TaxCategory { Name = taxCode.taxCode };
                _taxCategoryService.InsertTaxCategory(taxCategory);

                //save description and type
                if (!string.IsNullOrEmpty(taxCode.description))
                    _genericAttributeService.SaveAttribute(taxCategory, AvalaraTaxDefaults.TaxCodeDescriptionAttribute, taxCode.description);
                if (!string.IsNullOrEmpty(taxCode.taxCodeTypeId))
                    _genericAttributeService.SaveAttribute(taxCategory, AvalaraTaxDefaults.TaxCodeTypeAttribute, taxCode.taxCodeTypeId);

                importedTaxCodesNumber++;
            }

            //successfully imported
            var successMessage = _localizationService.GetResource("Plugins.Tax.Avalara.TaxCodes.Import.Success");
            _notificationService.SuccessNotification(string.Format(successMessage, importedTaxCodesNumber));

            return Categories();
        }

        [HttpPost, ActionName("Categories")]
        [FormValueRequired("exportTaxCodes")]
        public IActionResult ExportTaxCodes()
        {
            //ensure that Avalara tax provider is active
            if (!_taxPluginManager.IsPluginActive(AvalaraTaxDefaults.SystemName))
                return Categories();

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            //prepare tax codes to export
            var taxCodesToExport = _taxCategoryService.GetAllTaxCategories().Select(taxCategory => new TaxCodeModel
            {
                createdDate = DateTime.UtcNow,
                description = CommonHelper.EnsureMaximumLength(taxCategory.Name, 255),
                isActive = true,
                taxCode = CommonHelper.EnsureMaximumLength(taxCategory.Name, 25),
                taxCodeTypeId = CommonHelper.EnsureMaximumLength(_genericAttributeService
                    .GetAttribute<string>(taxCategory, AvalaraTaxDefaults.TaxCodeTypeAttribute) ?? "P", 2)
            }).Where(taxCode => !string.IsNullOrEmpty(taxCode.taxCode)).ToList();

            //get existing tax codes (only active)
            var existingTaxCodes = _avalaraTaxManager.GetTaxCodes(true)?.Select(taxCode => taxCode.taxCode).ToList() ?? new List<string>();

            //add Avalara pre-defined system tax codes
            var systemTaxCodes = _avalaraTaxManager.GetSystemTaxCodes(true)?.Select(taxCode => taxCode.taxCode).ToList() ?? new List<string>();
            existingTaxCodes.AddRange(systemTaxCodes);

            //remove duplicates
            taxCodesToExport = taxCodesToExport.Where(taxCode => !existingTaxCodes.Contains(taxCode.taxCode)).Distinct().ToList();

            //export tax codes
            if (taxCodesToExport.Any())
            {
                //create tax codes and get the result
                var result = _avalaraTaxManager.CreateTaxCodes(taxCodesToExport)?.Count;

                //display results
                if (result.HasValue && result > 0)
                    _notificationService.SuccessNotification(string.Format(_localizationService.GetResource("Plugins.Tax.Avalara.TaxCodes.Export.Success"), result));
                else
                    _notificationService.ErrorNotification(_localizationService.GetResource("Plugins.Tax.Avalara.TaxCodes.Export.Error"));
            }
            else
                _notificationService.SuccessNotification(_localizationService.GetResource("Plugins.Tax.Avalara.TaxCodes.Export.AlreadyExported"));

            return Categories();
        }

        [HttpPost, ActionName("Categories")]
        [FormValueRequired("deleteTaxCodes")]
        public IActionResult DeleteSystemTaxCodes()
        {
            //ensure that Avalara tax provider is active
            if (!_taxPluginManager.IsPluginActive(AvalaraTaxDefaults.SystemName))
                return Categories();

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            //get Avalara pre-defined system tax codes (only active)
            var systemTaxCodes = _avalaraTaxManager.GetSystemTaxCodes(true)?.Select(taxCode => taxCode.taxCode).ToList();
            if (!systemTaxCodes?.Any() ?? true)
            {
                _notificationService.ErrorNotification(_localizationService.GetResource("Plugins.Tax.Avalara.TaxCodes.Delete.Error"));
                return Categories();
            }

            //prepare tax categories to delete
            var taxCategoriesToDelete = _taxCategoryService.GetAllTaxCategories()
                .Where(taxCategory => systemTaxCodes.Contains(taxCategory.Name)).ToList();

            foreach (var taxCategory in taxCategoriesToDelete)
            {
                //delete generic attributes
                _genericAttributeService.SaveAttribute<string>(taxCategory, AvalaraTaxDefaults.TaxCodeDescriptionAttribute, null);
                _genericAttributeService.SaveAttribute<string>(taxCategory, AvalaraTaxDefaults.TaxCodeTypeAttribute, null);

                //delete tax categories
                _taxCategoryService.DeleteTaxCategory(taxCategory);
            }

            _notificationService.SuccessNotification(_localizationService.GetResource("Plugins.Tax.Avalara.TaxCodes.Delete.Success"));

            return Categories();
        }
        #endregion
    }
}