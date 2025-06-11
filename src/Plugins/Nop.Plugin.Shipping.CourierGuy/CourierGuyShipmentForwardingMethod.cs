using System.Text;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Shipping.CourierGuy.Domain;
using Nop.Plugin.Shipping.CourierGuy.Domain.NopEntityMappers;
using Nop.Plugin.Shipping.CourierGuy.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using NUglify.Helpers;

namespace Nop.Plugin.Shipping.CourierGuy;

public class CourierGuyShippingPaymentEventConsumer
    : IConsumer<OrderPaidEvent>
{
    private readonly ICourierShipmentService _courierShipmentService;
    private readonly IEmailSender _emailSender;
    private readonly IEmailAccountService _emailAccountService;
    private readonly IMessageTemplateService _messageTemplateService;


    public CourierGuyShippingPaymentEventConsumer(ICourierShipmentService courierShipmentService, IEmailSender emailSender, IEmailAccountService emailAccountService, IMessageTemplateService messageTemplateService)
    {
        _courierShipmentService = courierShipmentService;
        _emailSender = emailSender;
        _emailAccountService = emailAccountService;
        _messageTemplateService = messageTemplateService;
    }

    public async Task HandleEventAsync(OrderPaidEvent eventMessage)
    {
        await _courierShipmentService.HandleOrderPaidEventWithCourierGuy(eventMessage);
    }
}