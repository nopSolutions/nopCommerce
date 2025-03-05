using Microsoft.AspNetCore.Mvc.Razor;

namespace Nop.Plugin.Payments.MercadoPagoPlugin.Infrastructure;
public class ViewLocationExpander : IViewLocationExpander
{
    /// <summary>
    /// Invoked by a <see cref="T:Microsoft.AspNetCore.Mvc.Razor.RazorViewEngine" /> to determine the values that would be consumed by this instance
    /// of <see cref="T:Microsoft.AspNetCore.Mvc.Razor.IViewLocationExpander" />. The calculated values are used to determine if the view location
    /// has changed since the last time it was located.
    /// </summary>
    /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Razor.ViewLocationExpanderContext" /> for the current view location
    /// expansion operation.</param>
    public void PopulateValues(ViewLocationExpanderContext context)
    {
    }

    /// <summary>
    /// Invoked by a <see cref="T:Microsoft.AspNetCore.Mvc.Razor.RazorViewEngine" /> to determine potential locations for a view.
    /// </summary>
    /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Razor.ViewLocationExpanderContext" /> for the current view location
    /// expansion operation.</param>
    /// <param name="viewLocations">The sequence of view locations to expand.</param>
    /// <returns>A list of expanded view locations.</returns>        
    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        if (context.AreaName == "Admin")
        {
            viewLocations = new[] { $"/Plugins/Nop.Plugin.Payments.MercadoPagoPlugin/Areas/Admin/Views/{context.ControllerName}/{context.ViewName}.cshtml" }.Concat(viewLocations);
        }
        else
        {
            viewLocations = new[] { $"/Plugins/Nop.Plugin.Payments.MercadoPagoPlugin/Views/{context.ControllerName}/{context.ViewName}.cshtml" }.Concat(viewLocations);
        }

        return viewLocations;
    }
}