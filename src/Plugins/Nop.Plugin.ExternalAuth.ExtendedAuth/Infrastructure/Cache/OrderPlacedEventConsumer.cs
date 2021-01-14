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

namespace Nop.Plugin.ExternalAuth.ExtendedAuth.Infrastructure.Cache
{
    class OrderPlacedEventConsumer
        : IConsumer<OrderPlacedEvent>
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly ICustomerService _customerService;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private const int OrdersGroupId = -408158341;

        public OrderPlacedEventConsumer(ITelegramBotClient telegramBotClient,
            ICustomerService customerService, IExternalAuthenticationService externalAuthenticationService)
        {
            this._telegramBotClient = telegramBotClient;
            this._customerService = customerService;
            this._externalAuthenticationService = externalAuthenticationService;
        }

        public void HandleEvent(OrderPlacedEvent eventMessage)
        {
            var customer = _customerService.GetCustomerById(eventMessage.Order.CustomerId);
            var externalRecord = _externalAuthenticationService.GetCustomerExternalAuthenticationRecords(customer).FirstOrDefault();
            var externalDisplayIdentifier =
                externalRecord?.ExternalDisplayIdentifier ??
                _customerService.GetCustomerFullName(customer);

            if(string.IsNullOrEmpty(externalDisplayIdentifier))
                externalDisplayIdentifier = "Unknown";

            System.Threading.Tasks.Task.Run(async () => {
                var chat = await _telegramBotClient.GetChatAsync(OrdersGroupId);
                await _telegramBotClient.SendTextMessageAsync(chat, $"New order from {externalDisplayIdentifier}");
            });
        }
    }
}
