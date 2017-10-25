using System.Web.Routing;

namespace Nop.Web.Framework.Mvc.Routes
{
    /// <summary>
    /// 路由发布者接口
    /// Route publisher
    /// </summary>
    public interface IRoutePublisher
    {
        /// <summary>
        /// 注册路由
        /// Register routes
        /// </summary>
        /// <param name="routes">Routes</param>
        void RegisterRoutes(RouteCollection routes);
    }
}
