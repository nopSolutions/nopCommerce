using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Nop.Core.Infrastructure;
using Nop.Services.Logging.Events;

namespace Nop.Services.Events
{
    /// <summary>
    /// Represents the event publisher implementation
    /// </summary>
    public partial class EventPublisher : IEventPublisher
    {

        /// <summary>
        /// Publish event to consumers
        /// </summary>
        /// <typeparam name="TEvent">Type of event</typeparam>
        /// <param name="event">Event object</param>
        public virtual void Publish<TEvent>(TEvent @event)
        {
            //get all event consumers
            var consumers = EngineContext.Current.ResolveAll<IConsumer<TEvent>>().ToList();

            foreach (var consumer in consumers)
            {
                try
                {
                    //try to handle published event
                    consumer.HandleEvent(@event);
                }
                catch (Exception ex)
                {
                    //log error, we put in to nested try-catch to prevent possible cyclic (if some error occurs)
                    try
                    {
                        EngineContext.Current.Resolve<ILogger<EventPublisher>>()?.LogError(LoggingEvents.EventPublisherError, ex, "Error when trying to send to consumer");
                    }
                    catch { }
                }
            }
        }
    }
}