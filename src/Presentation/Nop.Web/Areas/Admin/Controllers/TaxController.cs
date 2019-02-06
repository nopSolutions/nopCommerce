using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Tax;
using Nop.Services.Configuration;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Tax;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class TaxController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly ITaxModelFactory _taxModelFactory;
        private readonly ITaxService _taxService;
        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public TaxController(IPermissionService permissionService,
            ISettingService settingService,
            ITaxCategoryService taxCategoryService,
            ITaxModelFactory taxModelFactory,
            ITaxService taxService,
            TaxSettings taxSettings)
        {
            this._permissionService = permissionService;
            this._settingService = settingService;
            this._taxCategoryService = taxCategoryService;
            this._taxModelFactory = taxModelFactory;
            this._taxService = taxService;
            this._taxSettings = taxSettings;
        }

        #endregion

        #region Methods

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            //prepare model
            var model = _taxModelFactory.PrepareTaxConfigurationModel(new TaxConfigurationModel());

            return View(model);
        }

        #region Tax Providers

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
                return AccessDeniedKendoGridJson();

            //prepare model
            var model = _taxModelFactory.PrepareTaxProviderListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult MarkAsPrimaryProvider(string systemName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            if (string.IsNullOrEmpty(systemName))
                return RedirectToAction("List");

            var taxProvider = _taxService.LoadTaxProviderBySystemName(systemName);
            if (taxProvider == null)
                return RedirectToAction("List");

            _taxSettings.ActiveTaxProviderSystemName = systemName;
            _settingService.SaveSetting(_taxSettings);

            return RedirectToAction("List");
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
                return AccessDeniedKendoGridJson();

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
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });

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
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });

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