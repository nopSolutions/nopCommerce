using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Common;
using Nop.Plugin.Api.Rest.Models;
using Nop.Services.Helpers;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Api.Rest.Controllers;

/// <summary>
/// Represents the plugin configuration controller
/// </summary>
[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[AutoValidateAntiforgeryToken]
public class ApiRestController : BasePluginController
{
    #region Fields

    private readonly IWebHelper _webHelper;

    #endregion

    #region Ctor

    public ApiRestController(IWebHelper webHelper)
    {
        _webHelper = webHelper;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Configure the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    [CheckPermission(StandardPermission.Configuration.MANAGE_PLUGINS)]
    public IActionResult Configure()
    {
        var storeLocation = _webHelper.GetStoreLocation();

        var model = new ConfigurationModel
        {
            SwaggerUiUrl = $"{storeLocation}{ApiRestDefaults.SwaggerUiPath}",
            SwaggerJsonUrl = $"{storeLocation}{ApiRestDefaults.SwaggerJsonPath}"
        };

        //the plugin view is not discoverable via the standard area locations, so pass its path explicitly
        var viewPath = $"~/{NopPluginDefaults.PathName}/{ApiRestDefaults.OutputFolderName}/Views/{nameof(Configure)}.cshtml";

        return View(viewPath, model);
    }

    #endregion
}
