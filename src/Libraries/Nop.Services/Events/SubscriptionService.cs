using System.Collections.Generic;
using System.Linq;
using Nop.Core.Infrastructure;

namespace Nop.Services.Events
{
    /// <summary>
    /// Event subscription service
    /// </summary>
    public class SubscriptionService : ISubscriptionService
    {
        /// <summary>
        /// Get subscriptions
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Event consumers</returns>
        public IList<IConsumer<T>> GetSubscriptions<T>()
        {
            var typeFinder = EngineContext.Current.Resolve<ITypeFinder>();
            var consumers = typeFinder.FindClassesOfType(typeof(IConsumer<T>)).ToList();
            var result = new List<IConsumer<T>>();
            foreach (var c in consumers)
            {
                var inst = EngineContext.Current.ResolveUnregistered(c) as IConsumer<T>;
                result.Add(inst);
            }
            return result;
        }
    }
}
