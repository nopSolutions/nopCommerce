using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Nop.Plugin.HaifaGold.InvoicePlugin.Infrastructure;
public class ViewLocationExpander : IViewLocationExpander
{
    public void PopulateValues(ViewLocationExpanderContext context)
    {
    }

    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        if (context.AreaName == "Admin")
        {
            viewLocations = new[] { $"/Plugins/Nop.Plugin.HaifaGold.InvoicePlugin/Areas/Admin/Views/{context.ControllerName}/{context.ViewName}.cshtml" }.Concat(viewLocations);
        }
        else
        {
            viewLocations = new[] { $"/Plugins/Nop.Plugin.HaifaGold.InvoicePlugin/Views/{context.ControllerName}/{context.ViewName}.cshtml" }.Concat(viewLocations);
        }

        return viewLocations;
    }
}
