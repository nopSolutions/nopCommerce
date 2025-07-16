using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Core.Events;
using Nop.Web.Framework.Models;

namespace Nop.Web.Framework.Events;

/// <summary>
/// Represents event publisher extensions
/// </summary>
public static class EventPublisherExtensions
{
    /// <summary>
    /// Publish ModelPrepared event
    /// </summary>
    /// <param name="eventPublisher">Event publisher</param>
    /// <param name="model">Model</param>
    public static void ModelPrepared(this IEventPublisher eventPublisher, object model)
    {
        ModelPreparedAsync(eventPublisher, model).Wait();
    }

    /// <summary>
    /// Publish ModelPrepared event
    /// </summary>
    /// <param name="eventPublisher">Event publisher</param>
    /// <param name="model">Model</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task ModelPreparedAsync(this IEventPublisher eventPublisher, object model)
    {
        switch (model)
        {
            case BaseNopModel nopModel:
                //we publish the ModelPrepared event for all models as the BaseNopModel, 
                //so you need to implement IConsumer<ModelPrepared<BaseNopModel>> interface to handle this event
                await eventPublisher.PublishAsync(new ModelPreparedEvent<BaseNopModel>(nopModel));
                break;
            case IEnumerable<BaseNopModel> modelCollection:
                //we publish the ModelPrepared event for collection as the IEnumerable<BaseNopModel>, 
                //so you need to implement IConsumer<ModelPrepared<IEnumerable<BaseNopModel>>> interface to handle this event
                await eventPublisher.PublishAsync(new ModelPreparedEvent<IEnumerable<BaseNopModel>>(modelCollection));
                break;
        }
    }

    /// <summary>
    /// Publish ModelReceived event
    /// </summary>
    /// <typeparam name="T">Type of the model</typeparam>
    /// <param name="eventPublisher">Event publisher</param>
    /// <param name="model">Model</param>
    /// <param name="modelState">Model state</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task ModelReceivedAsync<T>(this IEventPublisher eventPublisher, T model, ModelStateDictionary modelState)
    {
        await eventPublisher.PublishAsync(new ModelReceivedEvent<T>(model, modelState));
    }
}