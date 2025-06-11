using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Plugin.Shipping.CourierGuy.Domain.Webhooks;
using Nop.Plugin.Shipping.CourierGuy.Services;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Core.Domain.Shipping;
using System.Text;
using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;

namespace Nop.Plugin.Shipping.CourierGuy.Controllers
{
    public class CourierGuyWebhookController : Controller
    {
        private readonly CourierGuySettings _settings;
        private readonly ILogger _logger;
        private readonly CurrencySettings _currencySettings;
        private readonly ICourierShipmentService _courierShipmentService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IShipmentService _shipmentService;
        private readonly IOrderService _orderService;

        #region Ctor

        public CourierGuyWebhookController(
            ILogger logger,
            CurrencySettings currencySettings,
            CourierGuySettings settings,
            ICourierShipmentService courierShipmentService, 
            IEventPublisher eventPublisher,
            IShipmentService shipmentService,
            IOrderService orderService)
        {
            _logger = logger;
            _settings = settings;
            _courierShipmentService = courierShipmentService;
            _eventPublisher = eventPublisher;
            _currencySettings = currencySettings;
            _shipmentService = shipmentService;
            _orderService = orderService;
        }

        #endregion

        [HttpPost]
        [AllowAnonymous]
        [ActionName("HandleTrackingEvent")]
        public async Task<IActionResult> HandleTrackingEventAsync()
        {
            await using var payload = HttpContext.Request.Body;
            using var reader = new StreamReader(payload);
            var json = await reader.ReadToEndAsync();

            CourierShipmentTrackingWebhookEvent tracking;
            try { tracking = JsonConvert.DeserializeObject<CourierShipmentTrackingWebhookEvent>(json); }
            catch (Exception ex) { await _logger.ErrorAsync("CourierGuy tracking webhook: failed to deserialize payload", ex, null); return BadRequest("Invalid payload"); }
            if (tracking == null) return BadRequest("Empty payload");

            // Build candidate tracking numbers to locate shipment
            var candidates = new List<string>();
            if (!string.IsNullOrWhiteSpace(tracking.CustomTrackingReference)) candidates.Add(tracking.CustomTrackingReference.Trim());
            if (!string.IsNullOrWhiteSpace(tracking.ShortTrackingReference)) candidates.Add(tracking.ShortTrackingReference.Trim());
            if (tracking.ParcelTrackingReferences?.Any() == true)
            {
                foreach (var p in tracking.ParcelTrackingReferences)
                {
                    if (string.IsNullOrWhiteSpace(p)) continue;
                    candidates.Add(p.Trim());
                    // base before slash
                    var baseRef = p.Split('/')[0];
                    if (!string.IsNullOrWhiteSpace(baseRef)) candidates.Add(baseRef.Trim());
                }
            }
            // distinct
            candidates = candidates.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

            Shipment matchedShipment = null;
            foreach (var c in candidates)
            {
                try
                {
                    var page = await _shipmentService.GetAllShipmentsAsync(trackingNumber: c, pageSize: 1);
                    if (page?.Any() == true) { matchedShipment = page.First(); break; }
                }
                catch (Exception ex) { await _logger.ErrorAsync($"CourierGuy tracking webhook: error searching shipment for tracking '{c}'", ex, null); }
            }
            if (matchedShipment == null)
            {   await _logger.WarningAsync($"CourierGuy tracking webhook: no matching shipment found for candidates [{string.Join(", ", candidates)}], shipment_id={tracking.ShipmentId}");
                await _courierShipmentService.SendPushoverNotification("Tracking webhook received but no shipment matched. Payload: " + json, "CourierGuy Tracking (unmatched)");
                return NotFound(); }

            bool shipmentChanged = false;
            // set tracking number if empty
            if (string.IsNullOrWhiteSpace(matchedShipment.TrackingNumber))
            {
                var firstRef = tracking.CustomTrackingReference ?? tracking.ShortTrackingReference ?? candidates.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(firstRef)) { matchedShipment.TrackingNumber = firstRef; shipmentChanged = true; }
            }

            if (tracking.ShipmentCollectedDate.HasValue)
            {
                var collectedUtc = tracking.ShipmentCollectedDate.Value.UtcDateTime;
                if (!matchedShipment.ShippedDateUtc.HasValue || matchedShipment.ShippedDateUtc.Value != collectedUtc)
                { matchedShipment.ShippedDateUtc = collectedUtc; shipmentChanged = true; }
            }
            if (tracking.ShipmentDeliveredDate.HasValue)
            {
                var deliveredUtc = tracking.ShipmentDeliveredDate.Value.UtcDateTime;
                if (!matchedShipment.DeliveryDateUtc.HasValue || matchedShipment.DeliveryDateUtc.Value != deliveredUtc)
                { matchedShipment.DeliveryDateUtc = deliveredUtc; shipmentChanged = true; }
            }

            var latestEvent = tracking.TrackingEvents?.OrderByDescending(e => e.Date).FirstOrDefault();
            if (latestEvent != null)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"[EventId:{latestEvent.Id}] CourierGuy status: {latestEvent.Status} at {latestEvent.Date:O}");
                if (!string.IsNullOrWhiteSpace(latestEvent.Location)) sb.AppendLine("Location: " + latestEvent.Location);
                if (!string.IsNullOrWhiteSpace(latestEvent.Message)) sb.AppendLine("Message: " + latestEvent.Message);
                sb.AppendLine($"External shipment id: {tracking.ShipmentId}");
                var summary = sb.ToString();
                var eventMarker = $"[EventId:{latestEvent.Id}]";
                if (string.IsNullOrWhiteSpace(matchedShipment.AdminComment)) matchedShipment.AdminComment = summary;
                else if (!matchedShipment.AdminComment.Contains(eventMarker)) matchedShipment.AdminComment += "\n" + summary;
                shipmentChanged = true;
            }

            if (shipmentChanged) await _shipmentService.UpdateShipmentAsync(matchedShipment);

            var order = await _orderService.GetOrderByIdAsync(matchedShipment.OrderId);
            if (order != null)
            {
                var originalStatus = order.ShippingStatus;
                if (tracking.ShipmentDeliveredDate.HasValue) order.ShippingStatus = ShippingStatus.Delivered;
                else if (tracking.ShipmentCollectedDate.HasValue) order.ShippingStatus = ShippingStatus.Shipped;
                if (order.ShippingStatus != originalStatus) await _orderService.UpdateOrderAsync(order);

                try
                {
                    var noteBuilder = new StringBuilder();
                    noteBuilder.AppendLine("CourierGuy tracking update received.");
                    if (latestEvent != null)
                        noteBuilder.AppendLine($"Latest event: {latestEvent.Status} ({latestEvent.Message}) at {latestEvent.Date:O} {latestEvent.Location} [EventId:{latestEvent.Id}]");
                    noteBuilder.AppendLine($"Shipment status: {tracking.Status}");
                    noteBuilder.AppendLine($"Tracking refs: {string.Join(", ", candidates)}");
                    var note = new OrderNote { OrderId = order.Id, Note = noteBuilder.ToString(), DisplayToCustomer = false, CreatedOnUtc = DateTime.UtcNow };
                    await _orderService.InsertOrderNoteAsync(note);
                }
                catch (Exception ex) { await _logger.ErrorAsync("CourierGuy tracking webhook: error adding order note", ex, order.CustomerId); }
            }

            await _courierShipmentService.SendPushoverNotification("Tracking webhook processed successfully for shipment " + matchedShipment.Id, "CourierGuy Tracking");
            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [ActionName("HandleShipmentNote")]
        public async Task<IActionResult> HandleShipmentNoteAsync()
        {
            
            await using var payload = HttpContext.Request.Body;
            using var reader = new StreamReader(payload);
            var payloadJson = await reader.ReadToEndAsync();
            var heading = "Received shipment note from CourierGuy";
            var message = "Payload: " + payloadJson;
            await _courierShipmentService.SendPushoverNotification(message, heading);
            return Ok();
        }
    }
}
