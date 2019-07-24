using Nop.Core.Domain.Shipping;
using Nop.Services.Events;

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
        public static void PublishShipmentSent(this IEventPublisher eventPublisher, Shipment shipment)
        {
            eventPublisher.Publish(new ShipmentSentEvent(shipment));
        }

        /// <summary>
        /// Publishes the shipment delivered event.
        /// </summary>
        /// <param name="eventPublisher">The event publisher.</param>
        /// <param name="shipment">The shipment.</param>
        public static void PublishShipmentDelivered(this IEventPublisher eventPublisher, Shipment shipment)
        {
            eventPublisher.Publish(new ShipmentDeliveredEvent(shipment));
        }
    }
}