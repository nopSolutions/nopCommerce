using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Admin.Models;
using Nop.Admin.Models;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Tax;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Tax;
using Nop.Web.Framework.Controllers;
using Telerik.Web.Mvc;

namespace Nop.Admin.Controllers
{
	[AdminAuthorize]
    public class TaxProviderController : BaseNopController
	{
		#region Fields

        private readonly ITaxService _taxService;
        private readonly TaxSettings _taxSettings;
        private readonly ISettingService _settingService;

		#endregion Fields 

		#region Constructors

        public TaxProviderController(ITaxService taxService,
            TaxSettings taxSettings, ISettingService settingService)
		{
            this._taxService = taxService;
            this._taxSettings = taxSettings;
            this._settingService = settingService;
		}

		#endregion Constructors 

        #region List

        public ActionResult List(string systemName)
        {
            //mark as active tax provider (if selected)
            if (!String.IsNullOrEmpty(systemName))
            {
                var taxProvider = _taxService.LoadTaxProviderBySystemName(systemName);
                if (taxProvider != null)
                {
                    _taxSettings.ActiveTaxProviderSystemName = systemName;
                    _settingService.SaveSetting(_taxSettings);
                }
            }

            var taxProvidersModel = _taxService.LoadAllTaxProviders()
                .Select(x => x.ToModel()).ToList();
            foreach (var tpm in taxProvidersModel)
                tpm.IsPrimaryTaxProvider = tpm.SystemName.Equals(_taxSettings.ActiveTaxProviderSystemName, StringComparison.InvariantCultureIgnoreCase);
            var gridModel = new GridModel<TaxProviderModel>
            {
                Data = taxProvidersModel,
                Total = taxProvidersModel.Count()
            };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            var taxProvidersModel = _taxService.LoadAllTaxProviders()
                .Select(x => x.ToModel()).ToList();
            foreach (var tpm in taxProvidersModel)
                tpm.IsPrimaryTaxProvider = tpm.SystemName.Equals(_taxSettings.ActiveTaxProviderSystemName, StringComparison.InvariantCultureIgnoreCase);
            var gridModel = new GridModel<TaxProviderModel>
            {
                Data = taxProvidersModel,
                Total = taxProvidersModel.Count()
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        #endregion

        #region Edit

        public ActionResult Configure(string systemName)
        {
            var taxProvider = _taxService.LoadTaxProviderBySystemName(systemName);
            if (taxProvider == null) throw new ArgumentException("No tax provider found with the specified system name", "systemName");

            var model = taxProvider.ToModel();
            string actionName, controllerName;
            RouteValueDictionary routeValues;
            taxProvider.GetConfigurationRoute(out actionName, out controllerName, out routeValues);
            model.ConfigurationActionName = actionName;
            model.ConfigurationControllerName = controllerName;
            model.ConfigurationRouteValues = routeValues;
            return View(model);
        }

        #endregion
    }
}
