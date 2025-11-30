using System.Text;
using Newtonsoft.Json;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Events;
using Nop.Plugin.Shipping.CourierGuy.Domain;
using Nop.Plugin.Shipping.CourierGuy.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.ScheduleTasks;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using NUglify.Helpers;
using System.Linq; // added for LINQ operations

namespace Nop.Plugin.Shipping.CourierGuy;

public class CourierGuyShipmentService(
    ISettingService settingService,
    IHttpClientFactory httpClientFactory,
    IOrderService orderService,
    IProductService productService,
    IStoreService storeService,
    IShippingService shippingService,
    IAddressService addressService,
    ICustomerService customerService,
    IShipmentService shipmentService,
    ICountryService countryService,
    IEventPublisher eventPublisher,
    IOrderProcessingService orderProcessingService,
    IStateProvinceService stateProvinceService,
    IMessageTemplateService messageTemplateService,
    IQueuedEmailService queuedEmailService,
    IWarehouseService warehouseService,
    Nop.Services.Logging.ILogger logger
)
    : ICourierShipmentService
{
    private CourierGuyHttpClientFactory _shipLogicHttpClientFactory = new(httpClientFactory, settingService);

    // https://api-docs.bob.co.za/shiplogic

    public async Task HandleOrderPaidEventWithCourierGuy(OrderPaidEvent eventMessage)
    {
        var order = eventMessage.Order;
        var orderItems = await orderService.GetOrderItemsAsync(order.Id);
        var productsGroupedByWarehouse = (await productService.GetProductsByIdsAsync(orderItems.Select(x => x.ProductId).ToArray()))
            .GroupBy(x => x.WarehouseId);

        foreach (var productItemGroup in productsGroupedByWarehouse)
        {
            var products = await productService.GetProductsByIdsAsync(productItemGroup.Select(x => x.Id).ToArray());
            var warehouse = await warehouseService.GetWarehouseByIdAsync(productItemGroup.Key);
            var customer = await customerService.GetCustomerByIdAsync(order.CustomerId);
            var shipFromAddress = await addressService.GetAddressByIdAsync(warehouse.AddressId);
            var shipToAddress = await addressService.GetAddressByIdAsync(customer.ShippingAddressId.GetValueOrDefault());
            var country = await countryService.GetCountryByIdAsync(shipToAddress.CountryId.GetValueOrDefault());
            var stateProvince = await stateProvinceService.GetStateProvinceByIdAsync(shipToAddress.StateProvinceId.GetValueOrDefault());

            // Build rate request first to determine proper (cheapest) service level code
            var rateRequest = new CourierGuyRateRequest
            {
                CollectionAddress = new CourierGuyRateRequest.Address
                {
                    Type = string.IsNullOrEmpty(shipFromAddress.Company) ? CourierGuyRateRequest.AddressType.residential : CourierGuyRateRequest.AddressType.business,
                    Company = shipFromAddress.Company,
                    StreetAddress = shipFromAddress.Address1,
                    LocalArea = shipFromAddress.Address2,
                    City = shipFromAddress.City,
                    Code = shipFromAddress.ZipPostalCode,
                    Zone = stateProvince.Name,
                    Country = country.TwoLetterIsoCode,
                },
                DeliveryAddress = new CourierGuyRateRequest.Address
                {
                    Type = string.IsNullOrEmpty(shipToAddress.Company) ? CourierGuyRateRequest.AddressType.residential : CourierGuyRateRequest.AddressType.business,
                    Company = shipToAddress.Company,
                    StreetAddress = shipToAddress.Address1,
                    LocalArea = shipToAddress.Address2,
                    City = shipToAddress.City,
                    Code = shipToAddress.ZipPostalCode,
                    Zone = stateProvince.Name,
                    Country = country.TwoLetterIsoCode,
                },
                Parcels = products.Select(p =>
                {
                    if (p.Length == 0) p.Length = 1;
                    if (p.Width == 0) p.Width = 1;
                    if (p.Height == 0) p.Height = 1;
                    if (p.Weight == 0) p.Weight = 1;
                    return new CourierGuyRateRequest.Parcel
                    {
                        SubmittedLengthCm = (int)Math.Round(p.Length * 100),
                        SubmittedWidthCm = (int)Math.Round(p.Width * 100),
                        SubmittedHeightCm = (int)Math.Round(p.Height * 100),
                        SubmittedWeightKg = (int)Math.Round(p.Weight)
                    };
                }).ToList(),
                DeclaredValue = (int)Math.Round(products.Sum(x => x.Price)) > 25000 ? 25000 : (int)Math.Round(products.Sum(x => x.Price)),
                CollectionMinDate = DateTime.Today.AddDays(1),
                DeliveryMinDate = DateTime.Today.AddDays(2)
            };

            string chosenServiceLevelCode = null;
            try
            {
                using var rateRequestMessage = new HttpRequestMessage();
                using var rateContent = new StringContent(JsonConvert.SerializeObject(rateRequest), Encoding.UTF8, "application/json");
                rateRequestMessage.Content = rateContent;
                rateRequestMessage.Method = HttpMethod.Post;
                using var rateClient = await _shipLogicHttpClientFactory.RateRequestHttpClient();
                using var rateResponse = await rateClient.SendAsync(rateRequestMessage);
                if (!rateResponse.IsSuccessStatusCode)
                {
                    var rateErrorContent = await rateResponse.Content.ReadAsStringAsync();
                    await logger.WarningAsync($"CourierGuy Rate request failed for order {order.Id}, warehouse {warehouse.Id}: {rateResponse.StatusCode} {rateResponse.ReasonPhrase}. Body: {rateErrorContent}");
                }
                else
                {
                    var rateResponseJson = await rateResponse.Content.ReadAsStringAsync();
                    var rateResponseObj = JsonConvert.DeserializeObject<CourierGuyRateResponse>(rateResponseJson);
                    var cheapestRate = rateResponseObj?.Rates?.OrderBy(r => r.ShippingOptionRate).FirstOrDefault();
                    chosenServiceLevelCode = cheapestRate?.ServiceLevel?.Code;
                    if (string.IsNullOrWhiteSpace(chosenServiceLevelCode))
                        await logger.WarningAsync($"CourierGuy Rate response contained no service level code for order {order.Id}, warehouse {warehouse.Id}. Using fallback.");
                }
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync($"CourierGuy Rate request exception for order {order.Id}, warehouse {warehouse.Id}: {ex.Message}", ex);
            }

            // Fallback if rate request failed or no code – retain previous hardcoded LOF so shipment still proceeds
            if (string.IsNullOrWhiteSpace(chosenServiceLevelCode))
                chosenServiceLevelCode = "LOF"; // fallback

            var shipmentRequest = new CourierGuyShipmentRequest
            {
                CollectionAddress = new CollectionAddress
                {
                    Type = string.IsNullOrEmpty(shipFromAddress.Company) ? "residential" : "business",
                    Company = shipFromAddress.Company,
                    StreetAddress = shipFromAddress.Address1,
                    LocalArea = shipFromAddress.Address2,
                    City = shipFromAddress.City,
                    Code = shipFromAddress.ZipPostalCode,
                    Zone = stateProvince.Name,
                    Country = country.TwoLetterIsoCode,
                },
                CollectionContact = new()
                {
                    Name = $"{shipFromAddress.FirstName} {shipFromAddress.LastName}", MobileNumber = shipFromAddress.PhoneNumber, Email = shipFromAddress.Email,
                },
                DeliveryAddress = new DeliveryAddress
                {
                    Type = string.IsNullOrEmpty(shipToAddress.Company) ? "residential" : "business",
                    Company = shipToAddress.Company,
                    StreetAddress = shipToAddress.Address1,
                    LocalArea = shipToAddress.Address2,
                    City = shipToAddress.City,
                    Code = shipToAddress.ZipPostalCode,
                    Zone = stateProvince.Name,
                    Country = country.TwoLetterIsoCode,
                },
                DeliveryContact = new DeliveryContact
                {
                    Name = $"{shipToAddress.FirstName} {shipToAddress.LastName}", MobileNumber = shipToAddress.PhoneNumber, Email = shipToAddress.Email,
                },
                Parcels = products.Select(x =>
                {
                    if (x.Length == 0) x.Length = 1;
                    if (x.Width == 0) x.Width = 1;
                    if (x.Height == 0) x.Height = 1;
                    if (x.Weight == 0) x.Weight = 1;

                    return new Parcel
                    {
                        ParcelDescription = x.Name,
                        SubmittedLengthCm = (int)Math.Round(x.Length * 100),
                        SubmittedWidthCm = (int)Math.Round(x.Width * 100),
                        SubmittedHeightCm = (int)Math.Round(x.Height * 100),
                        SubmittedWeightKg = (int)Math.Round(x.Weight),
                    };
                }).ToList(),
                SpecialInstructionsCollection = "Please handle with care",
                SpecialInstructionsDelivery = "Please handle with care",
                DeclaredValue = (int)Math.Round(products.Sum(x => x.Price)) > 25000 ? 25000 : (int)Math.Round(products.Sum(x => x.Price)),
                CollectionMinDate = DateTime.Today.AddDays(1),
                CollectionAfter = "09:00",
                CollectionBefore = "17:00",
                DeliveryMinDate = DateTime.Today.AddDays(2),
                DeliveryAfter = "09:00",
                DeliveryBefore = "17:00",
                CustomerReference = $"MIDC-ORDER-{order.Id}",
                ServiceLevelCode = chosenServiceLevelCode,
            };

            using var requestMessage = new HttpRequestMessage();
            using var content = new StringContent(JsonConvert.SerializeObject(shipmentRequest), Encoding.UTF8, "application/json");
            requestMessage.Content = content;
            requestMessage.Method = HttpMethod.Post;
            using var client = await _shipLogicHttpClientFactory.ShipmentRequestHttpClient();
            using var response = await client.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var warning = new { payload = shipmentRequest, headers = requestMessage.Headers, errors = responseContent };
                await logger.WarningAsync(JsonConvert.SerializeObject(warning));
                // Skip creating shipment if API call failed
                continue;
            }
            var responseContentJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<CourierGuyShipmentResponse>(responseContentJson);

            var shipment = new Shipment
            {
                OrderId = order.Id,
                TrackingNumber = responseObject.ShortTrackingReference,
                TotalWeight = responseObject.ActualWeight,
                AdminComment = $"{responseObject.SpecialInstructionsCollection}{Environment.NewLine}{responseObject.SpecialInstructionsDelivery}\nService Level: {chosenServiceLevelCode}",
                CreatedOnUtc = responseObject.TimeCreated.GetValueOrDefault(),
            };
            await shipmentService.InsertShipmentAsync(shipment);
            await orderService.InsertOrderNoteAsync(new OrderNote
            {
                OrderId = order.Id,
                Note = $"Shipment created. ServiceLevelCode chosen: {chosenServiceLevelCode}",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            orderItems.ForEach(action);
            await eventPublisher.PublishAsync(new ShipmentCreatedEvent(shipment));
            if (!string.IsNullOrWhiteSpace(shipment.TrackingNumber))
                await eventPublisher.PublishAsync(new ShipmentTrackingNumberSetEvent(shipment));
            await orderProcessingService.ShipAsync(shipment, true);
            continue;

            async void action(OrderItem x)
            {
                var shipmentItem = new ShipmentItem
                {
                    ShipmentId = shipment.Id, OrderItemId = x.Id, Quantity = x.Quantity, WarehouseId = productItemGroup.Key,
                };
                await shipmentService.InsertShipmentItemAsync(shipmentItem);
            }
        }
        order.ShippingStatus = ShippingStatus.NotYetShipped;
        await orderService.UpdateOrderAsync(order);

        // Send an email using the vendour templare 
    }

    public async Task SendPushoverNotification(string message, string title)
    {
        const string PushoveApiUri = "https://api.pushover.net/1/messages.json";
        var pushoverHttpClient = httpClientFactory.CreateClient();
        var settings = await settingService.LoadSettingAsync<CourierGuySettings>();
        var template = BuildPushoverTemplate();
        var storeInfo = await storeService.GetStoreByIdAsync(1);
        var url = storeInfo.Url;
        var linkText = "Online Music Store";
        var formattedMessage = string.Format(template, message, url, linkText);

        var payload = new
        {
            token = settings.PushoverApiKey,
            user = settings.PushoverUserKey,
            message = formattedMessage,
            title,
            priority = 1,
            sound = "siren",
        };

        using var requestMessage = new HttpRequestMessage();
        using var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
        requestMessage.Content = content;
        requestMessage.Method = HttpMethod.Post;
        requestMessage.RequestUri = new Uri(PushoveApiUri);
        using var response = await pushoverHttpClient.SendAsync(requestMessage);
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var warning = new
            {
                payload, headers = requestMessage.Headers, errors = responseContent
            };
            await logger.WarningAsync(JsonConvert.SerializeObject(warning));
        }

    }
    private static string BuildPushoverTemplate()
    {
        StringBuilder sb = new StringBuilder();

        // Message title
        sb.AppendLine("<b>Notification:</b><br>");

        // Placeholder for the main message
        sb.AppendLine("{0}<br>"); // Replace this with your dynamic message

        // Optional link (can be added or removed as needed)
        sb.AppendLine("<a href=\"{1}\">{2}</a>"); // Replace {1} with the URL and {2} with the link text

        return sb.ToString();
    }
/*
 * token - your application's API token (required)
user - your user/group key (or that of your target user), viewable when logged into our dashboard; often referred to as USER_KEY in our documentation and code examples (required)
message - your message (required)
Some optional parameters may also be included:
attachment - a binary image attachment to send with the message (documentation)
attachment_base64 - a Base64-encoded image attachment to send with the message (documentation)
attachment_type - the MIME type of the included attachment or attachment_base64 (documentation)
device - the name of one of your devices to send just to that device instead of all devices (documentation)
html - set to 1 to enable HTML parsing (documentation)
priority - a value of -2, -1, 0 (default), 1, or 2 (documentation)
sound - the name of a supported sound to override your default sound choice (documentation)
timestamp - a Unix timestamp of a time to display instead of when our API received it (documentation)
title - your message's title, otherwise your app's name is used
ttl - a number of seconds that the message will live, before being deleted automatically (documentation)
url - a supplementary URL to show with your message (documentation)
url_title - a title for the URL specified as the url parameter, otherwise just the URL is shown (documentation)
That's it. Make sure your application is friendly to our API servers and you're all set. For more information on each parameter, keep reading or jump to a section at the left.

Need help using our API or found an error in the documentation? Drop us a line.
 *
 */
}