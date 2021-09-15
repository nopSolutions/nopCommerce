using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Nop.Services.Plugins;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.MrPoly
{
    public class MrPolyPlugin : BasePlugin, IAdminMenuPlugin
    {
        public Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            // Create custom PolyCommerce menu item
            var menuItem = new SiteMapNode()
            {
                SystemName = "MrPolyPlugin",
                Title = "Mr. Poly",
                IconClass = "fa fa-male",
                Url = "/Admin/MrPoly/Index",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", null } },
            };

            // Add Mr Poly menu item if it does not already exist.
            if (!rootNode.ChildNodes.Any(x => string.Equals(x.SystemName, "MrPolyPlugin")))
            {
                rootNode.ChildNodes.Add(menuItem);
            }

            return Task.FromResult(0);

        }
    }
}
