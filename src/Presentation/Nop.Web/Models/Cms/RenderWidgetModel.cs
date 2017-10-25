using System.Web.Routing;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Cms
{
    /// <summary>
    /// 用于渲染的部件模型
    /// </summary>
    public partial class RenderWidgetModel : BaseNopModel
    {
        /// <summary>
        /// 动作名
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// 操作器名
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// 路由数据
        /// </summary>
        public RouteValueDictionary RouteValues { get; set; }
    }
}