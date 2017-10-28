using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Tax;
using Nop.Core.Domain.Tax;
using Nop.Services.Configuration;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class TaxController : BaseAdminController
	{
		#region Fields

        private readonly ITaxService _taxService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly TaxSettings _taxSettings;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public TaxController(ITaxService taxService,
            ITaxCategoryService taxCategoryService,
            TaxSettings taxSettings,
            ISettingService settingService,
            IPermissionService permissionService)
		{
            this._taxService = taxService;
            this._taxCategoryService = taxCategoryService;
            this._taxSettings = taxSettings;
            this._settingService = settingService;
            this._permissionService = permissionService;
		}

		#endregion 

        #region Tax Providers

        public virtual IActionResult Providers()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();
            
            return View();
        }

        [HttpPost]
        public virtual IActionResult Providers(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedKendoGridJson();

            var taxProvidersModel = new List<TaxProviderModel>();
            var taxProviders = _taxService.LoadAllTaxProviders();
            foreach (var taxProvider in taxProviders)
            {
                var tmp1 = taxProvider.ToModel();
                tmp1.IsPrimaryTaxProvider = tmp1.SystemName.Equals(_taxSettings.ActiveTaxProviderSystemName, StringComparison.InvariantCultureIgnoreCase);
                tmp1.ConfigurationUrl = taxProvider.GetConfigurationPageUrl();
                taxProvidersModel.Add(tmp1);
            }
                
            var gridModel = new DataSourceResult
            {
                Data = taxProvidersModel,
                Total = taxProvidersModel.Count()
            };

            return Json(gridModel);
        }
        
        public virtual IActionResult MarkAsPrimaryProvider(string systemName)
        {
            if (string.IsNullOrEmpty(systemName))
            {
                return RedirectToAction("Providers");
            }

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            var taxProvider = _taxService.LoadTaxProviderBySystemName(systemName);
            if (taxProvider != null)
            {
                _taxSettings.ActiveTaxProviderSystemName = systemName;
                _settingService.SaveSetting(_taxSettings);
            }

            return RedirectToAction("Providers");
        }

        #endregion

        #region Tax Categories

        public virtual IActionResult Categories()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual IActionResult Categories(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedKendoGridJson();

            var categoriesModel = _taxCategoryService.GetAllTaxCategories()
                .Select(x => x.ToModel())
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = categoriesModel,
                Total = categoriesModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult CategoryUpdate(TaxCategoryModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var taxCategory = _taxCategoryService.GetTaxCategoryById(model.Id);
            taxCategory = model.ToEntity(taxCategory);
            _taxCategoryService.UpdateTaxCategory(taxCategory);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult CategoryAdd(TaxCategoryModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var taxCategory = new TaxCategory();
            taxCategory = model.ToEntity(taxCategory);
            _taxCategoryService.InsertTaxCategory(taxCategory);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult CategoryDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            var taxCategory = _taxCategoryService.GetTaxCategoryById(id);
            if (taxCategory == null)
                throw new ArgumentException("No tax category found with the specified id");
            _taxCategoryService.DeleteTaxCategory(taxCategory);

            return new NullJsonResult();
        }

        #endregion
    }
}