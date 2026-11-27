using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Search.Lucene.Admin.Controllers;

public class LuceneController : BasePluginAdminController
{
    #region Methods

    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public IActionResult Configure()
    {
        return View("~/Plugins/Search.Lucene/Views/Configure.cshtml");
    }

    #endregion
}
