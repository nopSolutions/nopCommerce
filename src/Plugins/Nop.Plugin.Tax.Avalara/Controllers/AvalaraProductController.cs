using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Tax.Avalara.Services;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Controllers;

namespace Nop.Plugin.Tax.Avalara.Controllers
{
    public class AvalaraProductController : BaseAdminController
    {
        #region Fields

        private readonly AvalaraTaxManager _avalaraTaxManager;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ITaxPluginManager _taxPluginManager;

        #endregion

        #region Ctor

        public AvalaraProductController(AvalaraTaxManager avalaraTaxManager,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ITaxPluginManager taxPluginManager)
        {
            _avalaraTaxManager = avalaraTaxManager;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _taxPluginManager = taxPluginManager;
        }

        #endregion

        #region Methods

        [HttpPost]
        public IActionResult ExportProducts(string selectedIds)
        {
            //ensure that Avalara tax provider is active
            if (!_taxPluginManager.IsPluginActive(AvalaraTaxDefaults.SystemName))
                return RedirectToAction("List", "Product");

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTaxSettings))
                return AccessDeniedView();

            //export items
            var exportedItems = _avalaraTaxManager.ExportProducts(selectedIds);
            if (exportedItems.HasValue)
            {
                if (exportedItems > 0)
                    _notificationService.SuccessNotification(string.Format(_localizationService.GetResource("Plugins.Tax.Avalara.Items.Export.Success"), exportedItems));
                else
                    _notificationService.SuccessNotification(_localizationService.GetResource("Plugins.Tax.Avalara.Items.Export.AlreadyExported"));
            }
            else
                _notificationService.ErrorNotification(_localizationService.GetResource("Plugins.Tax.Avalara.Items.Export.Error"));

            return RedirectToAction("List", "Product");
        }

        #endregion
    }
}