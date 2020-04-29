using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Misc.PolyCommerce.Models;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Factories;

namespace Nop.Plugin.Misc.PolyCommerce.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IOrderModelFactory _orderModelFactory;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerService _customerService;
        private readonly IRepository<Language> _languageRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICountryService _countryService;
        private readonly ILogger _logger;

        public OrdersController(IRepository<Order> orderRepository,
            IOrderModelFactory orderModelFactory,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ICustomerService customerService,
            IRepository<Language> languageRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<OrderNote> orderNoteRepository,
            IGenericAttributeService genericAttributeService,
            ILogger logger,
            ICountryService countryService)
        {
            _orderRepository = orderRepository;
            _orderModelFactory = orderModelFactory;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _customerService = customerService;
            _languageRepository = languageRepository;
            _orderItemRepository = orderItemRepository;
            _orderNoteRepository = orderNoteRepository;
            _genericAttributeService = genericAttributeService;
            _countryService = countryService;
            _logger = logger;
        }

        [HttpPost]
        [Route("api/polycommerce/orders/add")]
        public async Task<IActionResult> AddOrder([FromBody]PolyCommerceOrder model)
        {
            try
            {
                var storeToken = Request.Headers.TryGetValue("Store-Token", out var values) ? values.First() : null;
                var store = await PolyCommerceHelper.GetPolyCommerceStoreByToken(storeToken);

                if (store == null)
                {
                    return Unauthorized();
                }

                if (model == null)
                {
                    throw new Exception("Model can't be null");
                }

                if (model.Address == null)
                {
                    throw new Exception("Address can't be null");
                }

                if (model.OrderItems == null || !model.OrderItems.Any())
                {
                    throw new Exception("At least one OrderItem is required");
                }

                var country = _countryService.GetCountryByTwoLetterIsoCode(model.Address.TwoLetterCountryCode);

                var customer = new Customer
                {
                    Username = model.Email,
                    Email = model.Email,
                    Active = true,
                    Deleted = false,
                    LastActivityDateUtc = DateTime.UtcNow,
                    CreatedOnUtc = DateTime.UtcNow,
                    IsSystemAccount = false,
                    ShippingAddress = new Address
                    {
                        FirstName = model.Address.FirstName,
                        LastName = model.Address.LastName,
                        Address1 = model.Address.Address1,
                        Address2 = model.Address.Address2,
                        City = model.Address.City,
                        PhoneNumber = model.Address.PhoneNumber,
                        Email = model.Email,
                        CountryId = country.Id,
                        Company = model.Address.Company,
                        ZipPostalCode = model.Address.ZipPostalCode
                    }
                };

                // add new customer to system
                _customerService.InsertCustomer(customer);

                // assign guest role to newly added customer
                var guestRole = _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.GuestsRoleName);
                customer.AddCustomerRoleMapping(new CustomerCustomerRoleMapping { CustomerRole = guestRole });

                // save FirstName and LastName to user.

                if (!string.IsNullOrEmpty(model.Address.FirstName))
                {
                    _genericAttributeService.SaveAttribute(customer, "FirstName", model.Address.FirstName);
                }

                if (!string.IsNullOrEmpty(model.Address.LastName))
                {
                    _genericAttributeService.SaveAttribute(customer, "LastName", model.Address.LastName);
                }

                if (!Enum.IsDefined(typeof(PaymentStatus), model.PaymentStatusId))
                {
                    throw new Exception($"PaymentStatusId: {model.PaymentStatusId} not recognised");
                }

                // only english supported
                var englishLanguage = _languageRepository.Table.First(x => x.Name == "English");

                var order = new Order
                {
                    StoreId = store.Id,
                    OrderGuid = Guid.NewGuid(),
                    Customer = customer,
                    CustomerLanguageId = englishLanguage.Id,
                    CustomerIp = null,
                    OrderSubtotalInclTax = model.OrderSubtotalInclTax,
                    OrderSubtotalExclTax = model.OrderSubtotalExclTax,
                    OrderSubTotalDiscountInclTax = decimal.Zero,
                    OrderSubTotalDiscountExclTax = decimal.Zero,
                    OrderShippingInclTax = decimal.Zero,
                    OrderShippingExclTax = decimal.Zero,
                    PaymentMethodAdditionalFeeInclTax = decimal.Zero,
                    PaymentMethodAdditionalFeeExclTax = decimal.Zero,
                    TaxRates = "0:0;",
                    OrderTax = decimal.Zero,
                    OrderTotal = model.OrderTotal,
                    RefundedAmount = decimal.Zero,
                    OrderDiscount = decimal.Zero,
                    CheckoutAttributeDescription = string.Empty,
                    CheckoutAttributesXml = string.Empty,
                    CustomerCurrencyCode = model.CurrencyCode,
                    AffiliateId = 0,
                    OrderStatus = OrderStatus.Pending,
                    AllowStoringCreditCardNumber = false,
                    CardType = string.Empty,
                    CardName = string.Empty,
                    CardNumber = string.Empty,
                    MaskedCreditCardNumber = string.Empty,
                    CardCvv2 = string.Empty,
                    CardExpirationMonth = string.Empty,
                    CardExpirationYear = string.Empty,
                    PaymentMethodSystemName = model.PaymentMethodName,
                    AuthorizationTransactionId = string.Empty,
                    AuthorizationTransactionCode = string.Empty,
                    AuthorizationTransactionResult = string.Empty,
                    CaptureTransactionId = string.Empty,
                    CaptureTransactionResult = string.Empty,
                    SubscriptionTransactionId = string.Empty,
                    PaymentStatus = (PaymentStatus)model.PaymentStatusId,
                    PaidDateUtc = null,
                    BillingAddress = (Address)customer.ShippingAddress.Clone(),
                    ShippingAddress = (Address)customer.ShippingAddress.Clone(),
                    ShippingStatus = ShippingStatus.NotYetShipped,
                    ShippingMethod = model.ShippingMethod,
                    PickupInStore = false,
                    CustomValuesXml = string.Empty,
                    VatNumber = string.Empty,
                    CreatedOnUtc = DateTime.UtcNow,
                    CustomOrderNumber = model.OrderNumber
                };

                _orderRepository.Insert(order);

                // insert order items...
                foreach (var orderItem in model.OrderItems)
                {
                    var newItem = new OrderItem
                    {
                        OrderItemGuid = Guid.NewGuid(),
                        Order = order,
                        ProductId = orderItem.ExternalProductId,
                        UnitPriceInclTax = orderItem.UnitPriceInclTax,
                        UnitPriceExclTax = orderItem.UnitPriceExclTax,
                        PriceInclTax = orderItem.PriceInclTax,
                        PriceExclTax = orderItem.PriceExclTax,
                        AttributeDescription = string.Empty,
                        AttributesXml = string.Empty,
                        Quantity = orderItem.Quantity,
                        DiscountAmountInclTax = decimal.Zero,
                        DiscountAmountExclTax = decimal.Zero,
                        DownloadCount = 0,
                        IsDownloadActivated = false,
                        LicenseDownloadId = null,
                        ItemWeight = orderItem.ItemWeight
                    };

                    _orderItemRepository.Insert(newItem);
                }

                //order notes
                if (model.Notes != null && model.Notes.Any())
                {
                    foreach(var note in model.Notes)
                    {
                        _orderNoteRepository.Insert(new OrderNote
                        {
                            CreatedOnUtc = DateTime.UtcNow,
                            Note = note,
                            Order = order
                        });
                    }

                }

                return Ok(new { OrderId = order.Id });
            }
            catch (Exception ex)
            {
                _logger.Error($"Error saving PolyCommerce order. {(model != null ? Environment.NewLine + JsonConvert.SerializeObject(model) : string.Empty)}", ex);
                return BadRequest(ex.ToString());
            }
        }


        [HttpGet]
        [Route("api/polycommerce/orders/get_new_orders")]
        public async Task<IActionResult> GetNewOrders(int page, int pageSize, DateTime minCreatedDate)
        {
            var storeToken = Request.Headers.TryGetValue("Store-Token", out var values) ? values.First() : null;

            var store = await PolyCommerceHelper.GetPolyCommerceStoreByToken(storeToken);

            if (store == null)
            {
                return Unauthorized();
            }

            var skipRecords = (page - 1) * pageSize;

            var orders = await _orderRepository.Table
                .Where(x => x.CreatedOnUtc >= minCreatedDate)
                .Skip(skipRecords)
                .Take(pageSize)
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet]
        [Route("api/polycommerce/orders/get_orders_by_id")]
        public async Task<IActionResult> GetOrdersById(string commaSeparatedOrderIds)
        {
            var storeToken = Request.Headers.TryGetValue("Store-Token", out var values) ? values.First() : null;

            var store = await PolyCommerceHelper.GetPolyCommerceStoreByToken(storeToken);

            if (store == null)
            {
                return Unauthorized();
            }

            commaSeparatedOrderIds = commaSeparatedOrderIds.Trim().Replace(" ", string.Empty);

            var orderIds = commaSeparatedOrderIds.Split(',').Select(x => int.Parse(x));

            var orders = await _orderRepository.Table
                .Where(x => orderIds.Any(y => y == x.Id))
                .ToListAsync();

            var mappedOrders = orders.ConvertAll(x => _orderModelFactory.PrepareOrderModel(null, x));

            return Ok(mappedOrders);
        }

    }
}
