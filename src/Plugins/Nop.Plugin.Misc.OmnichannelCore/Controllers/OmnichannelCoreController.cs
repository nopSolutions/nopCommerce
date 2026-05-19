using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.OmnichannelCore.Models;
using Nop.Plugin.Misc.OmnichannelCore.Services;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.OmnichannelCore.Controllers;

[AutoValidateAntiforgeryToken]
[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
public class OmnichannelCoreController : BasePluginController
{
    #region Fields

    private readonly OmnichannelCoreService _omnichannelCoreService;

    #endregion

    #region Ctor

    public OmnichannelCoreController(OmnichannelCoreService omnichannelCoreService)
    {
        _omnichannelCoreService = omnichannelCoreService;
    }

    #endregion

    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public virtual async Task<IActionResult> Configure()
    {
        var model = new ConfigurationModel
        {
            OutboxMessages = await _omnichannelCoreService.GetOutboxMessageCountAsync(),
            InboxMessages = await _omnichannelCoreService.GetInboxMessageCountAsync(),
            FulfillmentRecords = await _omnichannelCoreService.GetFulfillmentRecordCountAsync(),
            StockProjectionRecords = await _omnichannelCoreService.GetStockProjectionRecordCountAsync()
        };

        return View("~/Plugins/Misc.OmnichannelCore/Views/Configure.cshtml", model);
    }

    #endregion
}
