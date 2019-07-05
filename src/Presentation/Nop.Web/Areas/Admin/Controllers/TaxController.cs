using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Tax;
using Nop.Services.Configuration;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Tax;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class TaxController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly ITaxModelFactory _taxModelFactory;
        private readonly ITaxPluginManager _taxPluginManager;
        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public TaxController(IPermissionService permissionService,
            ISettingService settingService,
            ITaxCategoryService taxCategoryService,
            ITaxModelFactory taxModelFactory,
            ITaxPluginManager taxPluginManager,
            TaxSettings taxSettings)
        {
            _permissionService = permissionService;
            _settingService = settingService;
            _taxCategoryService = taxCategoryService;
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

        public virtual IActionResult Providers()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            //prepare model
            var model = _taxModelFactory.PrepareTaxProviderSearchModel(new TaxProviderSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Providers(TaxProviderSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _taxModelFactory.PrepareTaxProviderListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult MarkAsPrimaryProvider(string systemName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            if (string.IsNullOrEmpty(systemName))
                return RedirectToAction("Providers");

            var taxProvider = _taxPluginManager.LoadPluginBySystemName(systemName);
            if (taxProvider == null)
                return RedirectToAction("Providers");

            _taxSettings.ActiveTaxProviderSystemName = systemName;
            _settingService.SaveSetting(_taxSettings);

            return RedirectToAction("Providers");
        }

        #endregion

        #region Tax Categories

        public virtual IActionResult Categories()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            //prepare model
            var model = _taxModelFactory.PrepareTaxCategorySearchModel(new TaxCategorySearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Categories(TaxCategorySearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _taxModelFactory.PrepareTaxCategoryListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult CategoryUpdate(TaxCategoryModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

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
                return ErrorJson(ModelState.SerializeErrors());

            var taxCategory = new TaxCategory();
            taxCategory = model.ToEntity(taxCategory);
            _taxCategoryService.InsertTaxCategory(taxCategory);

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult CategoryDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            //try to get a tax category with the specified id
            var taxCategory = _taxCategoryService.GetTaxCategoryById(id)
                ?? throw new ArgumentException("No tax category found with the specified id", nameof(id));

            _taxCategoryService.DeleteTaxCategory(taxCategory);

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}