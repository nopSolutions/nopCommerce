using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.Components;

/// <summary>
/// Base class for ViewComponent in nopCommerce
/// </summary>
public abstract partial class NopViewComponent : ViewComponent
{
    #region Utilities

    /// <summary>
    /// Publish model prepared event
    /// </summary>
    /// <param name="model">The model</param>
    protected virtual void PublishModelPrepared<TModel>(TModel model)
    {
        //Components are not part of the controller life cycle.
        //Hence, we could no longer use Action Filters to intercept the Models being returned
        //as we do in the /Nop.Web.Framework/Mvc/Filters/PublishModelEventsAttribute.cs for controllers

        switch (model)
        {
            //model prepared event
            case BaseNopModel nopModel:
            {
                var eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();

                //we publish the ModelPrepared event for all models as the BaseNopModel, 
                //so you need to implement IConsumer<ModelPrepared<BaseNopModel>> interface to handle this event
                eventPublisher.ModelPrepared(nopModel);
                break;
            }
            case IEnumerable<BaseNopModel> modelCollection:
            {
                var eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();

                //we publish the ModelPrepared event for collection as the IEnumerable<BaseNopModel>, 
                //so you need to implement IConsumer<ModelPrepared<IEnumerable<BaseNopModel>>> interface to handle this event
                eventPublisher.ModelPrepared(modelCollection);
                break;
            }
        }
    }

    /// <summary>
    /// Publish model prepared event
    /// </summary>
    /// <param name="model">The model</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task PublishModelPreparedAsync<TModel>(TModel model)
    {
        //Components are not part of the controller life cycle.
        //Hence, we could no longer use Action Filters to intercept the Models being returned
        //as we do in the /Nop.Web.Framework/Mvc/Filters/PublishModelEventsAttribute.cs for controllers

        switch (model)
        {
            //model prepared event
            case BaseNopModel nopModel:
            {
                var eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();

                //we publish the ModelPrepared event for all models as the BaseNopModel, 
                //so you need to implement IConsumer<ModelPrepared<BaseNopModel>> interface to handle this event
                await eventPublisher.ModelPreparedAsync(nopModel);
                break;
            }
            case IEnumerable<BaseNopModel> modelCollection:
            {
                var eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();

                //we publish the ModelPrepared event for collection as the IEnumerable<BaseNopModel>, 
                //so you need to implement IConsumer<ModelPrepared<IEnumerable<BaseNopModel>>> interface to handle this event
                await eventPublisher.ModelPreparedAsync(modelCollection);
                break;
            }
        }
    }

    /// <summary>
    /// Returns a result which will render the partial view with name <paramref name="viewName"/>.
    /// </summary>
    /// <param name="viewName">The name of the partial view to render.</param>
    /// <param name="model">The model object for the view.</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the <see cref="ViewViewComponentResult"/>.
    /// </returns>
    protected virtual async Task<ViewViewComponentResult> ViewAsync<TModel>(string viewName, TModel model)
    {
        await PublishModelPreparedAsync(model);

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
    protected virtual async Task<ViewViewComponentResult> ViewAsync<TModel>(TModel model)
    {
        await PublishModelPreparedAsync(model);

        //invoke the base method
        return View(model);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Returns a result which will render the partial view with name <paramref name="viewName"/>.
    /// </summary>
    /// <param name="viewName">The name of the partial view to render.</param>
    /// <param name="model">The model object for the view.</param>
    /// <returns>A <see cref="ViewViewComponentResult"/>.</returns>
    public new ViewViewComponentResult View<TModel>(string viewName, TModel model)
    {
        PublishModelPrepared(model);

        //invoke the base method
        return base.View(viewName, model);
    }

    /// <summary>
    /// Returns a result which will render the partial view
    /// </summary>
    /// <param name="model">The model object for the view.</param>
    /// <returns>A <see cref="ViewViewComponentResult"/>.</returns>
    public new ViewViewComponentResult View<TModel>(TModel model)
    {
        PublishModelPrepared(model);

        //invoke the base method
        return base.View(model);
    }

    #endregion
}