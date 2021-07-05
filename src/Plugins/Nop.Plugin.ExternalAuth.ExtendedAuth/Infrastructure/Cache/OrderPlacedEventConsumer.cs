using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Tasks;
using Telegram.Bot;
using System.Security.Claims;
using Nop.Services.Authentication.External;
using System.Linq;
using Nop.Core.Configuration;
using Nop.Services.Orders;

namespace Nop.Plugin.ExternalAuth.ExtendedAuth.Infrastructure.Cache
{
    struct VendorToChatMap
    {
        public int VendorId { get; set; }
        public long ChatGroupId { get; set; }
    }

    class OrderPlacedEventConsumer
        : IConsumer<OrderPlacedEvent>
    {
        private readonly Lazy<ITelegramBotClient> _telegramBotClient;
        private readonly ICustomerService _customerService;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly NopConfig _config;
        private readonly IOrderService _orderService;

        static private readonly List<VendorToChatMap> _vendorToChat = new List<VendorToChatMap>{
            new VendorToChatMap{ VendorId = 1, ChatGroupId = -1001213896599 },  // StrEat Kitchen
            new VendorToChatMap{ VendorId = 2, ChatGroupId = -1001440235406 },  // Root
            new VendorToChatMap{ VendorId = 4, ChatGroupId = -1001490513385 },  // 33 Pizzas
            new VendorToChatMap{ VendorId = 6, ChatGroupId = -1001378455507 },  // Kaufmann
            new VendorToChatMap{ VendorId = 7, ChatGroupId = -575045194     },  // Barbq
        };

        public OrderPlacedEventConsumer(Lazy<ITelegramBotClient> telegramBotClient,
            ICustomerService customerService, 
            IExternalAuthenticationService externalAuthenticationService,
            NopConfig config,
            IOrderService orderService)
        {
            this._telegramBotClient = telegramBotClient;
            this._customerService = customerService;
            this._externalAuthenticationService = externalAuthenticationService;
            this._config = config;
            this._orderService = orderService;
        }

        public void HandleEvent(OrderPlacedEvent eventMessage)
        {
            if (!_config.TelegramBotEnabled)
                return;

            var chatGroupsToNotify =
                _vendorToChat.Where(x => 
                    _orderService.GetOrderItems(eventMessage.Order.Id, vendorId: x.VendorId).Any())
                    .Select(x => x.ChatGroupId)
                    .ToList();

            if (!chatGroupsToNotify.Any())
                return;

            var customer = _customerService.GetCustomerById(eventMessage.Order.CustomerId);
            var externalRecord = _externalAuthenticationService.GetCustomerExternalAuthenticationRecords(customer).FirstOrDefault();
            var externalDisplayIdentifier =
                externalRecord?.ExternalDisplayIdentifier ??
                _customerService.GetCustomerFullName(customer);

            if(string.IsNullOrEmpty(externalDisplayIdentifier))
                externalDisplayIdentifier = "Unknown";

            chatGroupsToNotify.ForEach(chatGroupId => {
                System.Threading.Tasks.Task.Run(async () => {
                    var chat = await _telegramBotClient.Value.GetChatAsync(chatGroupId);
                    await _telegramBotClient.Value.SendTextMessageAsync(chat, $"New order from {externalDisplayIdentifier}");
                });
            });
        }
    }
}
