using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Web.Models.Cms;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the widget model factory
    /// </summary>
    public partial interface IWidgetModelFactory
    {
        /// <summary>
        /// Get render the widget models
        /// </summary>
        /// <param name="widgetZone">Name of widget zone</param>
        /// <param name="additionalData">Additional data object</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of the render widget models
        /// </returns>
        Task<List<RenderWidgetModel>> PrepareRenderWidgetModelAsync(string widgetZone, object additionalData = null);
    }
}