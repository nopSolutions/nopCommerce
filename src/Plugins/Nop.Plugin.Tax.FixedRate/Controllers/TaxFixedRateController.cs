using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Tax.FixedRate.Models;
using Nop.Services.Configuration;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Tax.FixedRate.Controllers
{
    [AdminAuthorize]
    public class TaxFixedRateController : BasePluginController
    {
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;

        public TaxFixedRateController(ITaxCategoryService taxCategoryService,
            ISettingService settingService,
            IPermissionService permissionService)
        {
            this._taxCategoryService = taxCategoryService;
            this._settingService = settingService;
            this._permissionService = permissionService;
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            //little hack here
            //always set culture to 'en-US' (Telerik has a bug related to editing decimal values in other cultures). Like currently it's done for admin area in Global.asax.cs
            CommonHelper.SetTelerikCulture();

            base.Initialize(requestContext);
        }

        [ChildActionOnly]
        public ActionResult Configure()
        {
            return View("~/Plugins/Tax.FixedRate/Views/TaxFixedRate/Configure.cshtml");
        }

        [HttpPost]
        public ActionResult Configure(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            var taxRateModels = new List<FixedTaxRateModel>();
            foreach (var taxCategory in _taxCategoryService.GetAllTaxCategories())
                taxRateModels.Add(new FixedTaxRateModel
                {
                    TaxCategoryId = taxCategory.Id,
                    TaxCategoryName = taxCategory.Name,
                    Rate = GetTaxRate(taxCategory.Id)
                });

            var gridModel = new DataSourceResult
            {
                Data = taxRateModels,
                Total = taxRateModels.Count
            };
            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult TaxRateUpdate(FixedTaxRateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            int taxCategoryId = model.TaxCategoryId;
            decimal rate = model.Rate;

            _settingService.SetSetting(string.Format("Tax.TaxProvider.FixedRate.TaxCategoryId{0}", taxCategoryId), rate);

            return new NullJsonResult();
        }

        [NonAction]
        protected decimal GetTaxRate(int taxCategoryId)
        {
            var rate = this._settingService.GetSettingByKey<decimal>(string.Format("Tax.TaxProvider.FixedRate.TaxCategoryId{0}", taxCategoryId));
            return rate;
        }
    }
}
