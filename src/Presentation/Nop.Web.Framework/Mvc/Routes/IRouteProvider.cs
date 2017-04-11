#if NET451
using System.Web.Routing;
#endif

namespace Nop.Web.Framework.Mvc.Routes
{
    public interface IRouteProvider
    {
#if NET451
        void RegisterRoutes(RouteCollection routes);
#endif

        int Priority { get; }
    }
}
