<<<<<<< HEAD
<<<<<<< HEAD
﻿﻿using System.Threading.Tasks;
using Nop.Core.Domain.Shipping;
 using Nop.Core.Events;

namespace Nop.Services.Shipping
{
    /// <summary>
    /// Event publisher extensions
    /// </summary>
    public static class EventPublisherExtensions
    {
        /// <summary>
        /// Publishes the shipment sent event.
        /// </summary>
        /// <param name="eventPublisher">The event publisher.</param>
        /// <param name="shipment">The shipment.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task PublishShipmentSentAsync(this IEventPublisher eventPublisher, Shipment shipment)
        {
            await eventPublisher.PublishAsync(new ShipmentSentEvent(shipment));
        }

        /// <summary>
        /// Publishes the shipment ready for pickup event.
        /// </summary>
        /// <param name="eventPublisher">The event publisher.</param>
        /// <param name="shipment">The shipment.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task PublishShipmentReadyForPickupAsync(this IEventPublisher eventPublisher, Shipment shipment)
        {
            await eventPublisher.PublishAsync(new ShipmentReadyForPickupEvent(shipment));
        }

        /// <summary>
        /// Publishes the shipment delivered event.
        /// </summary>
        /// <param name="eventPublisher">The event publisher.</param>
        /// <param name="shipment">The shipment.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task PublishShipmentDeliveredAsync(this IEventPublisher eventPublisher, Shipment shipment)
        {
            await eventPublisher.PublishAsync(new ShipmentDeliveredEvent(shipment));
        }
    }
=======
=======
=======
<<<<<<< HEAD
﻿﻿using System.Threading.Tasks;
using Nop.Core.Domain.Shipping;
 using Nop.Core.Events;

namespace Nop.Services.Shipping
{
    /// <summary>
    /// Event publisher extensions
    /// </summary>
    public static class EventPublisherExtensions
    {
        /// <summary>
        /// Publishes the shipment sent event.
        /// </summary>
        /// <param name="eventPublisher">The event publisher.</param>
        /// <param name="shipment">The shipment.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task PublishShipmentSentAsync(this IEventPublisher eventPublisher, Shipment shipment)
        {
            await eventPublisher.PublishAsync(new ShipmentSentEvent(shipment));
        }

        /// <summary>
        /// Publishes the shipment ready for pickup event.
        /// </summary>
        /// <param name="eventPublisher">The event publisher.</param>
        /// <param name="shipment">The shipment.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task PublishShipmentReadyForPickupAsync(this IEventPublisher eventPublisher, Shipment shipment)
        {
            await eventPublisher.PublishAsync(new ShipmentReadyForPickupEvent(shipment));
        }

        /// <summary>
        /// Publishes the shipment delivered event.
        /// </summary>
        /// <param name="eventPublisher">The event publisher.</param>
        /// <param name="shipment">The shipment.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task PublishShipmentDeliveredAsync(this IEventPublisher eventPublisher, Shipment shipment)
        {
            await eventPublisher.PublishAsync(new ShipmentDeliveredEvent(shipment));
        }
    }
=======
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
﻿﻿using System.Threading.Tasks;
using Nop.Core.Domain.Shipping;
 using Nop.Core.Events;

namespace Nop.Services.Shipping
{
    /// <summary>
    /// Event publisher extensions
    /// </summary>
    public static class EventPublisherExtensions
    {
        /// <summary>
        /// Publishes the shipment sent event.
        /// </summary>
        /// <param name="eventPublisher">The event publisher.</param>
        /// <param name="shipment">The shipment.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task PublishShipmentSentAsync(this IEventPublisher eventPublisher, Shipment shipment)
        {
            await eventPublisher.PublishAsync(new ShipmentSentEvent(shipment));
        }

        /// <summary>
        /// Publishes the shipment ready for pickup event.
        /// </summary>
        /// <param name="eventPublisher">The event publisher.</param>
        /// <param name="shipment">The shipment.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task PublishShipmentReadyForPickupAsync(this IEventPublisher eventPublisher, Shipment shipment)
        {
            await eventPublisher.PublishAsync(new ShipmentReadyForPickupEvent(shipment));
        }

        /// <summary>
        /// Publishes the shipment delivered event.
        /// </summary>
        /// <param name="eventPublisher">The event publisher.</param>
        /// <param name="shipment">The shipment.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task PublishShipmentDeliveredAsync(this IEventPublisher eventPublisher, Shipment shipment)
        {
            await eventPublisher.PublishAsync(new ShipmentDeliveredEvent(shipment));
        }
    }
<<<<<<< HEAD
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
=======
<<<<<<< HEAD
=======
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
}