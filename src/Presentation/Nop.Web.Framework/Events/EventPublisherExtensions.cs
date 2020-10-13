﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
 using Nop.Core.Events;

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
        public static async Task ModelPrepared<T>(this IEventPublisher eventPublisher, T model)
        {
            await eventPublisher.Publish(new ModelPreparedEvent<T>(model));
        }

        /// <summary>
        /// Publish ModelReceived event
        /// </summary>
        /// <typeparam name="T">Type of the model</typeparam>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="model">Model</param>
        /// <param name="modelState">Model state</param>
        public static async Task ModelReceived<T>(this IEventPublisher eventPublisher, T model, ModelStateDictionary modelState)
        {
            await eventPublisher.Publish(new ModelReceivedEvent<T>(model, modelState));
        }
    }
}