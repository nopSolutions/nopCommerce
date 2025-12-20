using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Events;

namespace Nop.Web.Framework.Components;

/// <summary>
/// Base class for ViewComponent in nopCommerce that provides common functionality
/// and event publishing for view components.
/// </summary>
public abstract partial class NopViewComponent : ViewComponent
{
    private readonly IEventPublisher _eventPublisher;
    private IEventPublisher EventPublisher => _eventPublisher ?? EngineContext.Current.Resolve<IEventPublisher>();

    protected NopViewComponent()
    {
        // For backward compatibility
    }

    protected NopViewComponent(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    /// <summary>
    /// Returns a result which will render the partial view with the specified name and model.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <param name="viewName">The name of the partial view to render.</param>
    /// <param name="model">The model object for the view.</param>
    /// <returns>A <see cref="ViewViewComponentResult"/>.</returns>
    public new ViewViewComponentResult View<TModel>(string viewName, TModel model)
    {
        PublishModelPrepared(model);
        return base.View(viewName, model);
    }

    /// <summary>
    /// Returns a result which will render the partial view with the specified model.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <param name="model">The model object for the view.</param>
    /// <returns>A <see cref="ViewViewComponentResult"/>.</returns>
    public new ViewViewComponentResult View<TModel>(TModel model)
    {
        PublishModelPrepared(model);
        return base.View(model);
    }

    /// <summary>
    /// Returns a result which will render the partial view with the specified name.
    /// </summary>
    /// <param name="viewName">The name of the partial view to render.</param>
    /// <returns>A <see cref="ViewViewComponentResult"/>.</returns>
    public new ViewViewComponentResult View(string viewName)
    {
        return base.View(viewName);
    }

    /// <summary>
    /// Asynchronously returns a result which will render the partial view with the specified model.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <param name="model">The model object for the view.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="ViewViewComponentResult"/>.</returns>
    protected async Task<ViewViewComponentResult> ViewAsync<TModel>(TModel model)
    {
        await PublishModelPreparedAsync(model);
        return View(model);
    }

    /// <summary>
    /// Asynchronously returns a result which will render the partial view with the specified name and model.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <param name="viewName">The name of the partial view to render.</param>
    /// <param name="model">The model object for the view.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="ViewViewComponentResult"/>.</returns>
    protected async Task<ViewViewComponentResult> ViewAsync<TModel>(string viewName, TModel model)
    {
        await PublishModelPreparedAsync(model);
        return View(viewName, model);
    }

    private void PublishModelPrepared<TModel>(TModel model)
    {
        if (model != null)
        {
            EventPublisher.ModelPrepared(model);
        }
    }

    private async Task PublishModelPreparedAsync<TModel>(TModel model)
    {
        if (model != null && EventPublisher is IAsyncEventPublisher asyncPublisher)
        {
            await asyncPublisher.ModelPreparedAsync(model);
        }
        else if (model != null)
        {
            EventPublisher.ModelPrepared(model);
        }
    }
}