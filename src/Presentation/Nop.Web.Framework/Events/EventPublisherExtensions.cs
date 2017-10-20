using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Services.Events;

namespace Nop.Web.Framework.Events
{
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
        public static void ModelPrepared<T>(this IEventPublisher eventPublisher, T model)
        {
            eventPublisher.Publish(new ModelPrepared<T>(model));
        }

        /// <summary>
        /// Publish ModelReceived event
        /// </summary>
        /// <typeparam name="T">Type of the model</typeparam>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="model">Model</param>
        /// <param name="modelState">Model state</param>
        public static void ModelReceived<T>(this IEventPublisher eventPublisher, T model, ModelStateDictionary modelState)
        {
            eventPublisher.Publish(new ModelReceived<T>(model, modelState));
        }
    }
}