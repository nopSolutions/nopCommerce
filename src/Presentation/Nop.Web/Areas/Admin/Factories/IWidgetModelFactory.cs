using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Web.Areas.Admin.Models.Cms;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the widget model factory
    /// </summary>
    public partial interface IWidgetModelFactory
    {
        /// <summary>
        /// Prepare widget search model
        /// </summary>
        /// <param name="searchModel">Widget search model</param>
        /// <returns>Widget search model</returns>
        Task<WidgetSearchModel> PrepareWidgetSearchModelAsync(WidgetSearchModel searchModel);

        /// <summary>
        /// Prepare paged widget list model
        /// </summary>
        /// <param name="searchModel">Widget search model</param>
        /// <returns>Widget list model</returns>
        Task<WidgetListModel> PrepareWidgetListModelAsync(WidgetSearchModel searchModel);

        /// <summary>
        /// Prepare render widget models
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>List of render widget models</returns>
        Task<IList<RenderWidgetModel>> PrepareRenderWidgetModelsAsync(string widgetZone, object additionalData = null);
    }
}