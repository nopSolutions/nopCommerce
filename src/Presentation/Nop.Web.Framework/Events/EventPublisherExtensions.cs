using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Services.Events;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Framework.Events
{
    public static class EventPublisherExtensions
    {
        public static void ModelPrepared<T>(this IEventPublisher eventPublisher, T model) where T : BaseNopModel
        {
            eventPublisher.Publish(new ModelPrepared<T>(model));
        }

        public static void ModelReceived<T>(this IEventPublisher eventPublisher, T model,
            ModelStateDictionary modelState) where T : BaseNopModel
        {
            eventPublisher.Publish(new ModelReceived<T>(model, modelState));
        }
    }
}