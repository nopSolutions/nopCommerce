using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace Nop.Web.Framework.Controllers;

public partial class NullView : IView
{
    public static readonly NullView Instance = new();

    public string Path => string.Empty;

    public virtual Task RenderAsync(ViewContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return Task.CompletedTask;
    }
}