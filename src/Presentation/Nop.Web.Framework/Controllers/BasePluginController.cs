using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Framework.Controllers
{
    /// <summary>
    /// Base controller for plugins
    /// </summary>
    [NotNullValidationMessage]
    public abstract class BasePluginController : BaseController
    {
    }
}
