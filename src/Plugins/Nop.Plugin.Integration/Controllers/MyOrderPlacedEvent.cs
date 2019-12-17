using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Events;
using Nop.Services.Orders;

namespace Nop.Plugin.Integration.Controllers
{
    public class MyOrderPlacedEvent : IConsumer<OrderPlacedEvent>
    {
        //private readonly IPluginFinder _pluginFinder;
        private readonly IOrderService _orderService;
        private readonly IStoreContext _storeContext;

        private readonly Nop.Services.Common.IGenericAttributeService _genericAttributeService;

        public MyOrderPlacedEvent(
            //IPluginFinder pluginFinder,
            IOrderService orderService,
            IStoreContext storeContext,
            Nop.Services.Common.IGenericAttributeService genericAttributeService)
        {
            //this._pluginFinder = pluginFinder;
            this._orderService = orderService;
            this._storeContext = storeContext;
            this._genericAttributeService = genericAttributeService;
        }

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        public void HandleEvent(OrderPlacedEvent eventMessage)
        {
            eventMessage.Order.OrderStatus = OrderStatus.Pending;
            eventMessage.Order.OrderNotes.Add(new OrderNote()
            {
                CreatedOnUtc = DateTime.UtcNow,
                DisplayToCustomer = true,
                Note = "Please, confirm your order by email!"
            });
            _orderService.UpdateOrder(eventMessage.Order);
        }
    }
}