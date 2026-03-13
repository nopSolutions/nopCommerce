using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Events;
using Task = System.Threading.Tasks.Task;

namespace Nop.Web.Framework.Components;

/// <summary>
/// Base class for ViewComponent in nopCommerce
/// </summary>
public abstract partial class NopViewComponent : ViewComponent
{
    /// <summary>
    /// Returns a result which will render the partial view with name <paramref name="viewName"/>.
    /// </summary>
    /// <param name="viewName">The name of the partial view to render.</param>
    /// <param name="model">The model object for the view.</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the <see cref="ViewViewComponentResult"/>.
    /// </returns>
    public async Task<ViewViewComponentResult> ViewAsync<TModel>(string viewName, TModel model)
    {
        var eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();
        await eventPublisher.ModelPreparedAsync(model);

        //invoke the base method
        return View(viewName, model);
    }

    /// <summary>
    /// Returns a result which will render the partial view
    /// </summary>
    /// <param name="model">The model object for the view.</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the <see cref="ViewViewComponentResult"/>.
    /// </returns>
    public async Task<ViewViewComponentResult> ViewAsync<TModel>(TModel model)
    {
        var eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();
        await eventPublisher.ModelPreparedAsync(model);

        //invoke the base method
        return View(model);
    }

    /// <summary>
    ///  Returns a result which will render the partial view with name viewName
    /// </summary>
    /// <param name="viewName">The name of the partial view to render.</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the <see cref="ViewViewComponentResult"/>.
    /// </returns>
    public Task<ViewViewComponentResult> ViewAsync(string viewName)
    {
        //invoke the base method
        return Task.FromResult(View(viewName));
    }

    /// <summary>
    ///  Returns a result which will render the partial view
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the <see cref="ViewViewComponentResult"/>.
    /// </returns>
    public Task<ViewViewComponentResult> ViewAsync()
    {
        //invoke the base method
        return Task.FromResult(View());
    }
}