using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.WebhookSettings;
using Nop.Core.Models;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Logging;
using RestSharp.Portable;
using RestSharp.Portable.HttpClient;

namespace Nop.Services.Orders;

public class OrderNotifier : IConsumer<OrderPlacedEvent>
{
    private readonly IOrderService _orderService;
    private readonly ICustomerService _customerService;
    private readonly IProductService _productService;
    private readonly WebhookSettings _webhookSettings;
    private readonly ILogger _logger;

    public OrderNotifier(IOrderService orderService, ICustomerService customerService, IProductService productService,
        ILogger logger, WebhookSettings webhookSettings)
    {
        _orderService = orderService;
        _customerService = customerService;
        _productService = productService;
        _webhookSettings = webhookSettings;
        _logger = logger;
    }

    public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
    {
        try
        {
            var orderItems = await _orderService.GetOrderItemsAsync(eventMessage.Order.Id);
            var customerById = await _customerService.GetCustomerByIdAsync(eventMessage.Order.CustomerId);
            var listOfProducts = new List<ProductDetails>();

            foreach (var item in orderItems)
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);

                listOfProducts.Add(new ProductDetails()
                {
                    Id = product.Id, Name = product.Name, Price = product.Price, Total = item.Quantity
                });
            }

            var orderDetails = new OrderDetails()
            {
                Id = eventMessage.Order.Id,
                OrderTotal = eventMessage.Order.OrderTotal,
                Products = listOfProducts,
                Customer = new CustomerDetails()
                {
                    Email = customerById.Email, Id = customerById.Id, Name = customerById.FirstName
                }
            };

            if (_webhookSettings.ConfigurationEnabled)
            {
                var url = _webhookSettings.PlaceOrderEndpointUrl;

                var client = new RestClient(url);
                var request = new RestRequest(url, Method.POST);
                request.AddJsonBody(orderDetails);
                var response = await client.Execute(request);
                var output = response.Content;
                await _logger.InsertLogAsync(LogLevel.Information, "Sent .... for ... with ...", output);
            }
        }
        catch (Exception e)
        {
            await _logger.InsertLogAsync(LogLevel.Error, $"Something went wrong for {eventMessage.Order.Id}. Please check the model");
            throw;
        }
    }
}