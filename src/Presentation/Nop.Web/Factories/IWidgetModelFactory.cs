using System.Collections.Generic;
using Nop.Web.Models.Cms;

namespace Nop.Web.Factories
{
    /// <summary>
    /// 约定一个部件模型工厂类应该实现的接口
    /// Represents the interface of the widget model factory
    /// </summary>
    public partial interface IWidgetModelFactory
    {
        /// <summary>
        /// Get render the widget models
        /// </summary>
        /// <param name="widgetZone">Name of widget zone</param>
        /// <param name="additionalData">Additional data object</param>
        /// <returns>List of the render widget models</returns>
        List<RenderWidgetModel> GetRenderWidgetModels(string widgetZone, object additionalData = null);
    }
}
