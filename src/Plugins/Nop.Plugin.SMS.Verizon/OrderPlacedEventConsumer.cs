using System;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Services.Events;
using Nop.Services.Orders;

namespace Nop.Plugin.SMS.Verizon
{
    public class OrderPlacedEventConsumer : IConsumer<OrderPlacedEvent>
    {
        private readonly VerizonSettings _verizonSettings;
        private readonly IPluginFinder _pluginFinder;
        private readonly IOrderService _orderService;
        private readonly IStoreContext _storeContext;

        public OrderPlacedEventConsumer(VerizonSettings verizonSettings,
            IPluginFinder pluginFinder, 
            IOrderService orderService,
            IStoreContext storeContext)
        {
            this._verizonSettings = verizonSettings;
            this._pluginFinder = pluginFinder;
            this._orderService = orderService;
            this._storeContext = storeContext;
        }

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        public void HandleEvent(OrderPlacedEvent eventMessage)
        {
            //is enabled?
            if (!_verizonSettings.Enabled)
                return;

            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("Mobile.SMS.Verizon");
            if (pluginDescriptor == null)
                return;
            if (!_pluginFinder.AuthenticateStore(pluginDescriptor, _storeContext.CurrentStore.Id))
                return;

            var plugin = pluginDescriptor.Instance() as VerizonSmsProvider;
            if (plugin == null)
                return;

            var order = eventMessage.Order;

            //send SMS
            if (plugin.SendSms(String.Format("New order(#{0}) has been placed.", order.Id)))
            {
                order.OrderNotes.Add(new OrderNote()
                {
                    Note = "\"Order placed\" SMS alert (to store owner) has been sent",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _orderService.UpdateOrder(order);
            }
        }
    }
}