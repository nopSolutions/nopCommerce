using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.InfigoProductProvider.Models;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.InfigoProductProvider.Controllers;

[AutoValidateAntiforgeryToken]
public class InfigoProductProviderController : BasePluginController
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public async Task<IActionResult> Configure()
    {
        var model = new ConfigurationModel();

        return View("~/Plugins/Misc.InfigoProductProvider/Views/Configure.cshtml", model);
    }
}