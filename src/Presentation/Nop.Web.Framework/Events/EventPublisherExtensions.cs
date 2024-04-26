using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Core.Events;

namespace Nop.Web.Framework.Events;

/// <summary>
/// Represents event publisher extensions
/// </summary>
public static class EventPublisherExtensions
{
    /// <summary>
    /// Publish ModelPrepared event
    /// </summary>
    /// <typeparam name="T">Type of the model</typeparam>
    /// <param name="eventPublisher">Event publisher</param>
    /// <param name="model">Model</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public static async Task ModelPreparedAsync<T>(this IEventPublisher eventPublisher, T model)
    {
        await eventPublisher.PublishAsync(new ModelPreparedEvent<T>(model));
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