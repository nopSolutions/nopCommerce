using Nop.Web.Areas.Admin.Models.Cms;

namespace Nop.Web.Areas.Admin.Factories;

/// <summary>
/// Represents the widget model factory
/// </summary>
public partial interface IWidgetModelFactory
{
    /// <summary>
    /// Prepare widget search model
    /// </summary>
    /// <param name="searchModel">Widget search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the widget search model
    /// </returns>
    Task<WidgetSearchModel> PrepareWidgetSearchModelAsync(WidgetSearchModel searchModel);

    /// <summary>
    /// Prepare paged widget list model
    /// </summary>
    /// <param name="searchModel">Widget search model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the widget list model
    /// </returns>
    Task<WidgetListModel> PrepareWidgetListModelAsync(WidgetSearchModel searchModel);
}