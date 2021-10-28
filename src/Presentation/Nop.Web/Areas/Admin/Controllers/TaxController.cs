using System;
using System.Threading.Tasks;
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
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class TaxController : BaseAdminController
    {
        #region Fields

        protected IPermissionService PermissionService { get; }
        protected ISettingService SettingService { get; }
        protected ITaxCategoryService TaxCategoryService { get; }
        protected IGenericAttributeService GenericAttributeService { get; }
        protected IWorkContext WorkContext { get; }
        protected ITaxModelFactory TaxModelFactory { get; }
        protected ITaxPluginManager TaxPluginManager { get; }
        protected TaxSettings TaxSettings { get; }

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
            PermissionService = permissionService;
            SettingService = settingService;
            TaxCategoryService = taxCategoryService;
            GenericAttributeService = genericAttributeService;
            WorkContext = workContext;
            TaxModelFactory = taxModelFactory;
            TaxPluginManager = taxPluginManager;
            TaxSettings = taxSettings;
        }

        #endregion

        #region Methods

        #region Tax Providers

        public virtual IActionResult List()
        {
            return RedirectToAction("Providers");
        }

        public virtual async Task<IActionResult> Providers(bool showtour = false)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            //prepare model
            var model = await TaxModelFactory.PrepareTaxProviderSearchModelAsync(new TaxProviderSearchModel());

            //show configuration tour
            if (showtour)
            {
                var customer = await WorkContext.GetCurrentCustomerAsync();
                var hideCard = await GenericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.HideConfigurationStepsAttribute);
                var closeCard = await GenericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.CloseConfigurationStepsAttribute);

                if (!hideCard && !closeCard)
                    ViewBag.ShowTour = true;
            }

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Providers(TaxProviderSearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await TaxModelFactory.PrepareTaxProviderListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> MarkAsPrimaryProvider(string systemName)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            if (string.IsNullOrEmpty(systemName))
                return RedirectToAction("Providers");

            var taxProvider = await TaxPluginManager.LoadPluginBySystemNameAsync(systemName);
            if (taxProvider == null)
                return RedirectToAction("Providers");

            TaxSettings.ActiveTaxProviderSystemName = systemName;
            await SettingService.SaveSettingAsync(TaxSettings);

            return RedirectToAction("Providers");
        }

        #endregion

        #region Tax Categories

        public virtual async Task<IActionResult> Categories()
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            //prepare model
            var model = await TaxModelFactory.PrepareTaxCategorySearchModelAsync(new TaxCategorySearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Categories(TaxCategorySearchModel searchModel)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await TaxModelFactory.PrepareTaxCategoryListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CategoryUpdate(TaxCategoryModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var taxCategory = await TaxCategoryService.GetTaxCategoryByIdAsync(model.Id);
            taxCategory = model.ToEntity(taxCategory);
            await TaxCategoryService.UpdateTaxCategoryAsync(taxCategory);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> CategoryAdd(TaxCategoryModel model)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var taxCategory = new TaxCategory();
            taxCategory = model.ToEntity(taxCategory);
            await TaxCategoryService.InsertTaxCategoryAsync(taxCategory);

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> CategoryDelete(int id)
        {
            if (!await PermissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            //try to get a tax category with the specified id
            var taxCategory = await TaxCategoryService.GetTaxCategoryByIdAsync(id)
                ?? throw new ArgumentException("No tax category found with the specified id", nameof(id));

            await TaxCategoryService.DeleteTaxCategoryAsync(taxCategory);

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}