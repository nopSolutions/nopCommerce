using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Framework.Controllers;

/// <summary>
/// Base admin controller for plugins
/// </summary>
[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
[ValidateIpAddress]
[AutoValidateAntiforgeryToken]
public abstract partial class BasePluginAdminController : BasePluginController;