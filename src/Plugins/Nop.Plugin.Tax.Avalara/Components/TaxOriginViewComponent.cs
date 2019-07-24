using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Tax.Avalara.Domain;
using Nop.Plugin.Tax.Avalara.Models.Settings;
using Nop.Services;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Framework.Components;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Tax.Avalara.Components
{
    /// <summary>
    /// Represents a view component to render an additional field on a tax settings view
    /// </summary>
    [ViewComponent(Name = AvalaraTaxDefaults.TAX_ORIGIN_VIEW_COMPONENT_NAME)]
    public class TaxOriginViewComponent : NopViewComponent
    {
        #region Fields

        private readonly AvalaraTaxSettings _avalaraTaxSettings;
        private readonly IPermissionService _permissionService;
        private readonly ITaxPluginManager _taxPluginManager;

        #endregion

        #region Ctor

        public TaxOriginViewComponent(AvalaraTaxSettings avalaraTaxSettings,
            IPermissionService permissionService,
            ITaxPluginManager taxPluginManager)
        {
            _avalaraTaxSettings = avalaraTaxSettings;
            _permissionService = permissionService;
            _taxPluginManager = taxPluginManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke the widget view component
        /// </summary>
        /// <param name="widgetZone">Widget zone</param>
        /// <param name="additionalData">Additional parameters</param>
        /// <returns>View component result</returns>
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return Content(string.Empty);

            //ensure that Avalara tax provider is active
            if (!_taxPluginManager.IsPluginActive(AvalaraTaxDefaults.SystemName))
                return Content(string.Empty);

            //ensure that it's a proper widget zone
            if (!widgetZone.Equals(AdminWidgetZones.TaxSettingsDetailsBlock))
                return Content(string.Empty);

            //prepare model
            var model = new TaxOriginAddressTypeModel
            {
                PrecedingElementId = nameof(TaxSettingsModel.TaxBasedOn),
                AvalaraTaxOriginAddressType = (int)_avalaraTaxSettings.TaxOriginAddressType,
                TaxOriginAddressTypes = TaxOriginAddressType.DefaultTaxAddress.ToSelectList(false)
                    .Select(type => new SelectListItem(type.Text, type.Value)).ToList(),
            };

            return View("~/Plugins/Tax.Avalara/Views/Settings/TaxOriginAddressType.cshtml", model);
        }

        #endregion
    }
}