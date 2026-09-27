using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.WorldpayHpp.Constants;
using Nop.Plugin.Payments.WorldpayHpp.Converters;
using Nop.Plugin.Payments.WorldpayHpp.Models;
using Nop.Plugin.Payments.WorldpayHpp.Models.Requests;
using Nop.Plugin.Payments.WorldpayHpp.Models.Response;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Logging;

namespace Nop.Plugin.Payments.WorldpayHpp.Services
{
    public class WorldpayHppService
    {
        private readonly HttpClient _httpClient;
        private readonly ISettingService _settingService;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly ICustomerService _customerService;
        private readonly ICountryService _countryService;
        private readonly IAddressService _addressService;
       

        public WorldpayHppService(
            HttpClient httpClient,
            ISettingService settingService,
            ILogger logger,
            IWebHelper webHelper,
            ICustomerService customerService,
            ICountryService countryService,
            IAddressService addressService
           
            )
        {
            _httpClient = httpClient;
            _settingService = settingService;
            _logger = logger;
            _webHelper = webHelper;
            _customerService = customerService;
            _countryService = countryService;
            _addressService = addressService;
            
        }

        private string GetBaseUrl(string environment)
            => environment?.Equals("LIVE", StringComparison.OrdinalIgnoreCase) == true
                ? "https://access.worldpay.com"
                : "https://try.access.worldpay.com";

        

      
        public async Task<PaymentResponse> CreateSessionAsync(Order order)
        {
            // Load settings each time
            var settings = await _settingService.LoadSettingAsync<WorldpayHppSettings>();

            var baseUrl = GetBaseUrl(settings.Environment);
            var endpoint = $"{baseUrl}/payment_pages";

           
           
            // Get a specific address by customer and address ID
            var billing = await _addressService.GetAddressByIdAsync(order.BillingAddressId);
            // var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            var country = await _countryService.GetCountryByIdAsync(billing.CountryId.Value);

           
            var request = new PaymentRequest
            {
                TransactionReference = $"{settings.TransactionReferencePrefix ??= string.Empty}{order.Id}",
                Merchant = new Merchant { Entity = settings.EntityId },
                Description = $"Order {order.Id} payment",

                Narrative = new Narrative { Line1 = settings.NarrativeLine1 ??= "NopCommerce" },
                
                Value = new Value { Amount = ((int)(order.OrderTotal * 100m)).ToString(), Currency = order.CustomerCurrencyCode },
                BillingAddress = new BillingAddress
                {
                    FirstName = billing.FirstName,
                    LastName = billing.LastName,
                     Address1 = billing.Address1,
                      City = billing.City,
                       PostalCode = billing.ZipPostalCode,
                          CountryCode = country?.TwoLetterIsoCode
                },

                ResultURLs = new ResultUrls
                {
                    SuccessURL = string.IsNullOrEmpty(settings.SuccessUrl)
                        ? $"{_webHelper.GetStoreLocation()}WorldpayHpp/Success"
                        : settings.SuccessUrl,
                    FailureURL = string.IsNullOrEmpty(settings.FailureUrl)
                        ? $"{_webHelper.GetStoreLocation()}WorldpayHpp/Failure"
                        : settings.FailureUrl,
                    CancelURL = string.IsNullOrEmpty(settings.CancelUrl)
                        ? $"{_webHelper.GetStoreLocation()}WorldpayHpp/Cancel"
                        : settings.CancelUrl,
                    ErrorURL = string.IsNullOrEmpty(settings.ErrorUrl)
                        ? $"{_webHelper.GetStoreLocation()}WorldpayHpp/Error"
                        : settings.ErrorUrl,
                    ExpiryURL = string.IsNullOrEmpty(settings.ExpiryUrl)
                        ? $"{_webHelper.GetStoreLocation()}WorldpayHpp/Expiry"
                        : settings.ExpiryUrl,
                    PendingURL = string.IsNullOrEmpty(settings.PendingUrl)
                        ? $"{_webHelper.GetStoreLocation()}WorldpayHpp/Pending"
                        : settings.PendingUrl,

                }

            };

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
               // WriteIndented = true
            };
            serializeOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            serializeOptions.Converters.Add(new BooleanStringJsonConverter());
            serializeOptions.Converters.Add(new GooglePayButtonLabelJsonConverter());
            // serializeOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

            var jsonString = System.Text.Json.JsonSerializer.Serialize(request, serializeOptions);
            var message = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(jsonString, Encoding.UTF8, "application/vnd.worldpay.payment_pages-v1.hal+json")
            };
            
            

            var credentials = $"{settings.Username}:{settings.Password}";
            //todo base64 encode the token 
            var base64Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
            message.Headers.Authorization = new AuthenticationHeaderValue("Basic", $"{base64Credentials}");
            message.Headers.UserAgent.ParseAdd("NopCommerce Worldpay HPP Plugin");
            message.Headers.Add("WP-CorrelationId", order.OrderGuid.ToString());
          
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.worldpay.payment_pages-v1.hal+json"));

            var  response = await _httpClient.SendAsync(message);
            var body = await response.Content.ReadAsStringAsync();


            if (!response.IsSuccessStatusCode)
            {
                await _logger.ErrorAsync($"Worldpay HPP session init failed with statuscode: {response.StatusCode} and body: {body}");
                throw new Exception("Worldpay HPP session init failed");
            }

            var paymentResponse =  System.Text.Json.JsonSerializer.Deserialize<PaymentResponse>(body);

            return paymentResponse;
        }
    }
}
