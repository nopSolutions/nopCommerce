using Nop.Core.Domain.Orders;
using Nop.Services.Events;

namespace Nop.Plugin.Shipping.CourierGuy.Services;

public interface ICourierShipmentService
{

    Task HandleOrderPaidEventWithCourierGuy(OrderPaidEvent eventMessage);

    Task SendPushoverNotification(string message, string title);

}