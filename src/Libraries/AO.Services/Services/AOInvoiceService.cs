using AO.Services.Domain;
using AO.Services.Models;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AO.Services.Services
{
    public class AOInvoiceService : IAOInvoiceService
    {
        #region Private variables        
        private readonly ILogger _logger;
        private readonly IRepository<AOInvoice> _invoiceRepository;
        private readonly IRepository<AOInvoiceItem> _invoiceItemRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly ICustomerService _customerService;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly IShipmentService _shipmentService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ICommonReOrderService _commonReOrderService;
        private readonly ICurrencyService _currencyService;
        private readonly IRepository<AOOrderItemSetting> _aoOrderItemSettingRepository;
        private readonly IRepository<ProductAttributeCombination> _productAttributeCombinationRepository;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IGenericAttributeService _genericAttributeService;
        #endregion

        public AOInvoiceService(IRepository<AOInvoice> invoiceRepository, IRepository<AOInvoiceItem> invoiceItemRepository, IShipmentService shipmentService, IProductService productService, IProductAttributeParser productAttributeParser, IRepository<Order> orderRepository, ICustomerService customerService, IAddressService addressService, ICountryService countryService, IOrderService orderService, IRepository<Customer> customerRepository, ICommonReOrderService commonReOrderService, IRepository<AOOrderItemSetting> aoOrderItemSettingRepository, ILogger logger, IRepository<ProductAttributeCombination> productAttributeCombinationRepository, ICurrencyService currencyService, IPriceCalculationService priceCalculationService, IRepository<Address> addressRepository, IGenericAttributeService genericAttributeService)
        {
            _invoiceRepository = invoiceRepository;
            _invoiceItemRepository = invoiceItemRepository;
            _shipmentService = shipmentService;
            _productService = productService;
            _productAttributeParser = productAttributeParser;
            _orderRepository = orderRepository;
            _customerService = customerService;
            _addressService = addressService;
            _countryService = countryService;
            _orderService = orderService;
            _customerRepository = customerRepository;
            _commonReOrderService = commonReOrderService;
            _aoOrderItemSettingRepository = aoOrderItemSettingRepository;
            _logger = logger;
            _productAttributeCombinationRepository = productAttributeCombinationRepository;
            _currencyService = currencyService;
            _priceCalculationService = priceCalculationService;
            _addressRepository = addressRepository;
            _genericAttributeService = genericAttributeService;
        }

        public async Task<AOInvoice> CreateInvoiceAsync(List<OrderItem> orderItems, Order order, bool creditNote, bool invoiceShipping, DateTime invoiceDate, DateTime paymentDate, bool isPaid, bool isManual, bool reStock = false)
        {
            var invoice = new AOInvoice()
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                InvoiceDate = invoiceDate,
                PaymentDate = order.PaidDateUtc.HasValue ? order.PaidDateUtc.Value : paymentDate,
                InvoiceIsPaid = isPaid,
                InvoiceIsManual = isManual,
                CustomerCurrencyCode = order.CustomerCurrencyCode,
                CurrencyRate = order.CurrencyRate
            };

            if (invoiceShipping)
            {
                invoice.InvoiceShipping = order.OrderShippingInclTax;
            }

            await AddTrackingNumberAsync(invoice, order.Id);

            await _invoiceRepository.InsertAsync(invoice);

            await CreateInvoiceItemsAsync(orderItems, invoice.Id, creditNote, reStock);

            await UpdateInvoiceWithPricesAsync(invoice, order, orderItems, creditNote, invoiceShipping);

            if (creditNote == false)
            {
                // Dont change the reorder list when we credit something
                await ClenupReOrderListAsync(orderItems);
            }
     
            return invoice;
        }

        private async Task ReStockItemAsync(AOInvoiceItem invoiceItem, ProductAttributeCombination combination)
        {                       
            combination.StockQuantity += invoiceItem.Quantity;
            await _productAttributeCombinationRepository.UpdateAsync(combination);            
        }

        private async Task LogStockChangeAsync(ProductAttributeCombination combination, int stockQuantityChange, int originalStockQuantity)
        {
            var product = await _productService.GetProductByIdAsync(combination.ProductId);
            if (product == null)
            {
                await _logger.ErrorAsync($"No product found to update stock history when crediting, tried with ProductId: {combination.ProductId}");
            }
            else
            {
                string message = string.Format("Stock updated by crediting orderline(s)");
                await _productService.AddStockQuantityHistoryEntryAsync(product, stockQuantityChange, originalStockQuantity, 0, message, combination.Id);
            }
        }

        /// <summary>
        /// This method is to make sure we remove items from the reorder list.
        /// This item has now been invoiced so should be removed.
        /// </summary>        
        private async Task ClenupReOrderListAsync(List<OrderItem> orderItems)
        {
            // foreach orderitem find out whether they are "taken aside" or "ordered"
            foreach (var orderItem in orderItems)
            {
                try
                {
                    // Get item from reorderlist
                    var reOrderItem = await _commonReOrderService.GetReOrderItemByOrderItemIdAsync(orderItem.Id);
                    if (reOrderItem != null)
                    {
                        // Find out if this item is either taken aside or ordered, and if yes, do nothing
                        var rep = await _aoOrderItemSettingRepository.GetAllAsync(query =>
                        {
                            return from orderSettings in query
                                   where orderSettings.OrderItemId == orderItem.Id
                                   select orderSettings;
                        });
                        var setting = rep.FirstOrDefault();
                        if (setting != null)
                        {
                            // We have a custom order setting for this orderitem
                            if (setting.IsOrdered > 0 || setting.IsTakenAside > 0)
                            {
                                // Move to the next, this one is already ordered or taken a side
                                continue;
                            }
                        }

                        // So the item is on the reorderlist (Bestillingslisten) and is not ordered or take aside
                        await _commonReOrderService.RemoveFromReOrderListAsync(orderItem.Quantity, orderItem.Id);
                    }
                }
                catch (Exception ex)
                {
                    string message = $"Error cleaning up (ClenupReOrderListAsync): {ex.Message}";
                    await _logger.ErrorAsync(message, ex);
                }
            }            
        }

        public async Task<AOInvoice> GetInvoiceByIdAsync(int Id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(Id);
            return invoice;
        }

        public async Task<List<AOInvoice>> GetInvoiceByOrderIdAsync(int orderId)
        {
            var invoices = await _invoiceRepository.GetAllAsync(query =>
            {
                return from invoice in query
                       where invoice.OrderId == orderId
                       select invoice;
            });

            return invoices.ToList();
        }

        public async Task<List<AOInvoiceItem>> GetInvoiceItemsByInvoiceIdAsync(int invoiceId)
        {
            var invoiceItems = await _invoiceItemRepository.GetAllAsync(query =>
            {
                return from invoiceItem in query
                       where invoiceItem.InvoiceId == invoiceId
                       select invoiceItem;
            });

            return invoiceItems.ToList();
        }

        /// <summary>
        /// This will get all invoiced items in an order.
        /// No matter if this order has been spread over multiple invoices.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>List of items that has been invoiced on order</returns>
        public async Task<List<AOInvoiceItem>> GetInvoiceItemsByOrderIdAsync(int orderId)
        {
            var invoices = await _invoiceRepository.GetAllAsync(query =>
            {
                return from invoice in query
                       where invoice.OrderId == orderId
                       select invoice;
            });

            var invoiceIds = invoices.Select(i => i.Id).ToList();

            var invoiceItems = await _invoiceItemRepository.GetAllAsync(query =>
            {
                return from invoiceItem in query
                       where invoiceIds.Contains(invoiceItem.InvoiceId)
                       select invoiceItem;
            });

            return invoiceItems.ToList();
        }

        public async Task<List<AOInvoiceItem>> GetInvoiceItemsByInvoiceItemIdAsync(int orderItemId, int invoiceId, bool credited = false)
        {
            var invoiceItems = await _invoiceItemRepository.GetAllAsync(query =>
            {
                if (credited)
                {
                    if (invoiceId > 0)
                    {
                        return from invoiceItem in query
                               where invoiceItem.OrderItemId == orderItemId && invoiceItem.Credited == true && invoiceItem.InvoiceId == invoiceId
                               select invoiceItem;
                    }
                    else
                    {
                        return from invoiceItem in query
                               where invoiceItem.OrderItemId == orderItemId && invoiceItem.Credited == true
                               select invoiceItem;
                    }
                }
                else
                {
                    if (invoiceId > 0)
                    {
                        return from invoiceItem in query
                               where invoiceItem.OrderItemId == orderItemId && invoiceItem.Credited == false && invoiceItem.InvoiceId == invoiceId
                               select invoiceItem;
                    }
                    else
                    {
                        return from invoiceItem in query
                               where invoiceItem.OrderItemId == orderItemId && invoiceItem.Credited == false
                               select invoiceItem;
                    }
                }
            });

            return invoiceItems.ToList();
        }

        public async Task<int> GetInvoiceItemCountOnInvoiceAsync(int invoiceItemId, int invoiceId)
        {
            var invoiceItems = await _invoiceItemRepository.GetAllAsync(query =>
            {
                return from invoiceItem in query
                       where invoiceItem.InvoiceId == invoiceId
                       && invoiceItem.OrderItemId == invoiceItemId
                       select invoiceItem;
            });

            return invoiceItems.Count;
        }

        public async Task<AOInvoiceItem> GetInvoiceItemByOrderItemIdAsync(int orderItemId, int invoiceId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
            if (invoice == null)
            {
                throw new Exception($"No invoice found with invoice id: {invoiceId}");
            }

            bool creditNote = invoice.InvoiceTotal < 0;

            var invoiceItem = await _invoiceItemRepository.GetAllAsync(query =>
            {
                return from invoiceItem in query
                       where invoiceItem.OrderItemId == orderItemId
                       && invoiceItem.Credited == creditNote
                       && invoiceItem.InvoiceId == invoiceId
                       select invoiceItem;
            });

            return invoiceItem.FirstOrDefault();
        }

        public async Task<AOInvoiceListModel> GetTopInvoicesAsync(string searchphrase, int topCount)
        {
            IQueryable<InvoiceModel> invoices = null;

            if (string.IsNullOrEmpty(searchphrase))
            {
                invoices = (from i in _invoiceRepository.Table
                            join o in _orderRepository.Table on i.OrderId equals o.Id
                            join c in _customerRepository.Table on i.CustomerId equals c.Id
                            join a in _addressRepository.Table on c.BillingAddressId equals a.Id into ia
                            from a in ia.DefaultIfEmpty()
                            orderby i.Id descending
                            select new InvoiceModel
                            {
                                InvoiceNumber = i.Id,
                                OrderId = i.OrderId,
                                CustomerId = i.CustomerId,
                                InvoiceDate = i.InvoiceDate,
                                PaymentDate = i.PaymentDate,
                                InvoiceTax = i.InvoiceTax,
                                InvoiceDiscount = i.InvoiceDiscount,
                                InvoiceTotal = i.InvoiceTotal,
                                InvoiceRefundedAmount = i.InvoiceRefundedAmount,
                                InvoiceShipping = i.InvoiceShipping,
                                TrackingNumber = i.TrackingNumber,
                                CustomerCurrencyCode = i.CustomerCurrencyCode,
                                PaymentMethod = GetPaymentMethod(o),
                                CustomerPresentationName = string.IsNullOrEmpty(a.FirstName) ? c.Username : $"{a.FirstName} {a.LastName}",
                                InvoiceIsPaid = i.InvoiceIsPaid,
                                InvoiceIsManual = i.InvoiceIsManual,
                                CurrencyRate = i.CurrencyRate,
                                BookedDate = i.BookedDate,
                                EconomicInvoiceNumber = i.EconomicInvoiceNumber
                            }).Take(topCount);
            }
            else
            {
                invoices = (from i in _invoiceRepository.Table
                            join o in _orderRepository.Table on i.OrderId equals o.Id
                            join c in _customerRepository.Table on i.CustomerId equals c.Id
                            join a in _addressRepository.Table on c.BillingAddressId equals a.Id into ia
                            from a in ia.DefaultIfEmpty()
                            where i.Id.ToString() == searchphrase || i.OrderId.ToString() == searchphrase || a.FirstName.Contains(searchphrase)  || a.LastName.Contains(searchphrase) || a.Email.Contains(searchphrase) || o.CardType.Contains(searchphrase) || o.PaymentMethodSystemName.Contains(searchphrase)
                            orderby i.Id descending
                            select new InvoiceModel
                            {
                                InvoiceNumber = i.Id,
                                OrderId = i.OrderId,
                                CustomerId = i.CustomerId,
                                InvoiceDate = i.InvoiceDate,
                                PaymentDate = i.PaymentDate,
                                InvoiceTax = i.InvoiceTax,
                                InvoiceDiscount = i.InvoiceDiscount,
                                InvoiceTotal = i.InvoiceTotal,
                                InvoiceRefundedAmount = i.InvoiceRefundedAmount,
                                InvoiceShipping = i.InvoiceShipping,
                                TrackingNumber = i.TrackingNumber,
                                CustomerCurrencyCode = i.CustomerCurrencyCode,
                                PaymentMethod = GetPaymentMethod(o),
                                CustomerPresentationName = string.IsNullOrEmpty(a.FirstName) ? c.Username : $"{a.FirstName} {a.LastName}",
                                InvoiceIsManual = i.InvoiceIsManual,
                                InvoiceIsPaid = i.InvoiceIsPaid,
                                CurrencyRate = i.CurrencyRate,
                                BookedDate = i.BookedDate,
                                EconomicInvoiceNumber = i.EconomicInvoiceNumber
                            }).Take(topCount);
            }

            var model = new AOInvoiceListModel()
            {
                InvoiceList = await invoices.ToListAsync(),
                SearchPhrase = searchphrase
            };

            return model;
        }

        /// <summary>
        /// This will return all non booked invoices.
        /// Non booked meaning, missig either BookedDate OR EconomicInvoiceNumber
        /// </summary>        
        public async Task<List<AOInvoice>> GetInvoicesNotBookedAsync()
        {
            var invoices = await _invoiceRepository.GetAllAsync(query =>
            {
                return from invoice in query
                       where (invoice.BookedDate == null || invoice.BookedDate < Convert.ToDateTime("01-01-1970"))
                             || 
                             (invoice.EconomicInvoiceNumber == null || invoice.EconomicInvoiceNumber <= 0)
                       select invoice;
            });

            return invoices.ToList();
        }        

        public async Task<AOInvoice> CreateManualInvoiceAsync(string orderItemText, decimal totalIncludingTax, string currencyCode, decimal taxRate, int customerId, DateTime invoiceDate, DateTime paymentDate)
        {
            ValidateInput(orderItemText, currencyCode, totalIncludingTax, customerId, invoiceDate);

            decimal totalWithoutTax = totalIncludingTax / (1 + taxRate / 100);
            decimal taxAmount = totalIncludingTax - totalWithoutTax;

            bool creditNote = false;
            if (totalIncludingTax < 0)
            {
                creditNote = true;
                totalIncludingTax = decimal.Negate(totalIncludingTax);
                taxAmount = decimal.Negate(taxAmount);
            }

            Address address = await GetAddressByCustomerIdAsync(customerId);
            var product = await CreateProductAsync(orderItemText, totalIncludingTax);

            var order = new Order()
            {
                StoreId = 1,
                OrderGuid = Guid.NewGuid(),
                CustomerIp = "127.0.0.1",
                OrderStatus = Nop.Core.Domain.Orders.OrderStatus.Complete,
                AllowStoringCreditCardNumber = false,
                CardType = string.Empty,
                CardName = string.Empty,
                CardNumber = string.Empty,
                MaskedCreditCardNumber = string.Empty,
                CardCvv2 = string.Empty,
                CardExpirationMonth = string.Empty,
                CardExpirationYear = string.Empty,
                PaymentMethodSystemName = "",
                AuthorizationTransactionId = string.Empty,
                AuthorizationTransactionCode = string.Empty,
                AuthorizationTransactionResult = string.Empty,
                CaptureTransactionId = string.Empty,
                CaptureTransactionResult = string.Empty,
                SubscriptionTransactionId = string.Empty,
                PaymentStatus = Nop.Core.Domain.Payments.PaymentStatus.Pending,
                PaidDateUtc = null,
                BillingAddressId = address.Id,
                ShippingAddressId = address.Id,
                ShippingStatus = Nop.Core.Domain.Shipping.ShippingStatus.Delivered,
                ShippingMethod = "",
                PickupInStore = false,
                ShippingRateComputationMethodSystemName = "",
                CustomValuesXml = string.Empty,
                VatNumber = string.Empty,
                CreatedOnUtc = DateTime.UtcNow,
                CustomerId = customerId,
                CustomOrderNumber = string.Empty,
                CustomerCurrencyCode = currencyCode,
                CurrencyRate = 1
            };
            await _orderRepository.InsertAsync(order);

            var orderItem = new OrderItem()
            {
                OrderId = order.Id,
                Quantity = 1,
                AttributeDescription = orderItemText,
                PriceInclTax = totalIncludingTax,
                UnitPriceInclTax = totalIncludingTax,
                PriceExclTax = totalIncludingTax - taxAmount,
                UnitPriceExclTax = totalIncludingTax - taxAmount,
                ProductId = product.Id
            };
            await _orderService.InsertOrderItemAsync(orderItem);

            var orderItems = new List<OrderItem>
            {
                orderItem
            };

            var invoice = await CreateInvoiceAsync(orderItems, order, creditNote, false, invoiceDate, paymentDate, false, true);

            return invoice;
        }

        private async void ValidateInput(string orderItemText, string currencyCode, decimal total, int customerId, DateTime invoiceDate)
        {
            if (string.IsNullOrEmpty(orderItemText))
            {
                throw new ArgumentException("Missing order item text");
            }

            if (string.IsNullOrEmpty(currencyCode))
            {
                throw new ArgumentException("Missing currency code");
            }

            if (total == 0)
            {
                throw new ArgumentException("Total cannot be 0");
            }

            if (customerId == 0)
            {
                throw new ArgumentException("We need customer id");
            }

            if (invoiceDate == DateTime.MinValue)
            {
                throw new ArgumentException("We need and invoice date");
            }

            var currency = await _currencyService.GetCurrencyByCodeAsync(currencyCode);
            if (currency == null)
            {
                throw new ArgumentException($"No currency found with code: '{currencyCode}'");
            }
        }

        private async Task<Product> CreateProductAsync(string orderItemText, decimal total)
        {
            var product = new Product()
            {
                Name = orderItemText,
                Price = total,
                Published = false,
                AdminComment = "Dummy produkt lavet til manuel faktura"
            };
            await _productService.InsertProductAsync(product);
            return product;
        }

        private async Task<Address> GetAddressByCustomerIdAsync(int customerId)
        {
            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            if (customer == null)
            {
                throw new ArgumentException($"No customer/guest found with id: {customerId}");
            }

            int addressId = 0;
            if (customer.BillingAddressId == null || customer.BillingAddressId.Value <= 0)
            {
                var order = _orderRepository.Table.Where(o => o.CustomerId == customerId).FirstOrDefault();
                if (order != null && order.BillingAddressId > 0)
                {
                    addressId = order.BillingAddressId;
                }
            }
            else
            {
                addressId = customer.BillingAddressId.Value;
            }

            Address address = null;

            if (addressId > 0)
            {
                address = await _addressService.GetAddressByIdAsync(addressId);
            }
            else
            {
                address = await GetAddressFromGenericAttributesAsync(customer);
            }

            if (address == null)
            {
                throw new ArgumentException($"No address found for customer with Billing address id: {customer.BillingAddressId.Value}");
            }

            return address;
        }

        private async Task<Address> GetAddressFromGenericAttributesAsync(Customer customer)
        {
            Address address = new Address()
            {
                Address1 = customer.StreetAddress,
                Address2 = customer.StreetAddress2,
                City = customer.City,
                Email = customer.Email,
                ZipPostalCode = customer.ZipPostalCode,
                CountryId = customer.CountryId,
                Company = customer.Company,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PhoneNumber = customer.Phone,
                County = customer.County
            };

            // Create new address in database to persist as billing address
            await _addressService.InsertAddressAsync(address);

            // Add the new address id as billing address id, as its needed later on
            customer.BillingAddressId = address.Id;
            await _customerService.UpdateCustomerAsync(customer);
                       
            return address;
        }

        private static string GetPaymentMethod(Order o)
        {
            if (string.IsNullOrEmpty(o.CardType) == false)
            {
                return o.CardType;
            }

            if (string.IsNullOrEmpty(o.PaymentMethodSystemName) == false)
            {
                if (o.PaymentMethodSystemName.Contains("Payments.CheckMoneyOrder"))
                {
                    return "Banktransfer";
                }
                else if (o.PaymentMethodSystemName.Contains("Kasselukning"))
                {
                    return "Kasselukning";
                }
                else if (o.PaymentMethodSystemName.Contains("Klarna"))
                {
                    return "Klarna";
                }
                else if (o.PaymentMethodSystemName.Contains("QuickPay"))
                {
                    return "QuickPay";
                }
                else
                {
                    return o.PaymentMethodSystemName;
                }
            }

            return "N/A";
        }

        public async Task<InvoiceModel> GetInvoiceModelByIdAsync(int invoiceNumber)
        {
            var invoices = from i in _invoiceRepository.Table
                           join o in _orderRepository.Table on i.OrderId equals o.Id
                           where i.Id == invoiceNumber
                           select new InvoiceModel
                           {
                               InvoiceNumber = i.Id,
                               OrderId = i.OrderId,
                               CustomerId = i.CustomerId,
                               InvoiceDate = i.InvoiceDate,
                               PaymentDate = i.PaymentDate,
                               InvoiceTax = i.InvoiceTax,
                               InvoiceDiscount = i.InvoiceDiscount,
                               InvoiceTotal = i.InvoiceTotal,
                               InvoiceRefundedAmount = i.InvoiceRefundedAmount,
                               InvoiceShipping = i.InvoiceShipping,
                               TrackingNumber = i.TrackingNumber,
                               CustomerCurrencyCode = i.CustomerCurrencyCode,
                               PaymentMethod = GetPaymentMethod(o),
                               InvoiceIsManual = i.InvoiceIsManual,
                               InvoiceIsPaid = i.InvoiceIsPaid,
                               CurrencyRate = i.CurrencyRate,
                               BookedDate = i.BookedDate,
                               EconomicInvoiceNumber = i.EconomicInvoiceNumber
                           };

            var model = await invoices.FirstOrDefaultAsync();
            model.InvoiceCustomer = await _customerService.GetCustomerByIdAsync(model.CustomerId);

            int addressId = model.InvoiceCustomer.BillingAddressId.HasValue ? model.InvoiceCustomer.BillingAddressId.Value : model.InvoiceCustomer.ShippingAddressId.Value;
            if (addressId > 0)
            {
                model.InvoiceAddress = await _addressService.GetAddressByIdAsync(addressId);
                if (string.IsNullOrEmpty(model.CustomerPresentationName))
                {
                    model.CustomerPresentationName = $"{model.InvoiceAddress.FirstName} {model.InvoiceAddress.LastName}";
                }

                if (string.IsNullOrEmpty(model.InvoiceCustomer.Email))
                {
                    model.InvoiceCustomer.Email = $"{model.InvoiceAddress.Email}";
                }
            }

            if (model.InvoiceAddress.CountryId > 0)
            {
                model.InvoiceCountry = await _countryService.GetCountryByIdAsync(model.InvoiceAddress.CountryId.Value);
            }

            model.InvoiceItems = await BuildInvoiceItemsAsync(model.InvoiceNumber, model.OrderId);

            return model;
        }

        private async Task<List<OrderItemModel>> BuildInvoiceItemsAsync(int invoiceNumber, int orderId)
        {
            var orderItems = await _orderService.GetOrderItemsAsync(orderId);

            //get products data by order items
            var itemsData = orderItems.SelectAwait(async item =>
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);

                //try to get product attribute combination
                var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, item.AttributesXml);

                //create product data
                return new
                {
                    id = item.Id,
                    name = product.Name,
                    variant_id = combination?.Id ?? product.Id,
                    variant_name = combination?.Sku ?? product.Name,
                    ean = combination?.Gtin ?? product.Sku,
                    quantity = item.Quantity,
                    price = item.PriceInclTax,
                    unitprice = item.UnitPriceInclTax,
                    variantInfo = item.AttributeDescription,
                    productId = item.ProductId
                };
            }).ToArrayAsync();

            List<OrderItemModel> items = new List<OrderItemModel>();
            foreach (var item in itemsData.Result)
            {
                items.Add(new OrderItemModel()
                {
                    OrderItemId = item.id,
                    InvoiceId = invoiceNumber,
                    ProductId = item.productId,
                    ProductName = item.name,
                    VariantInfo = string.IsNullOrEmpty(item.variantInfo) ? "" : WebUtility.HtmlDecode(item.variantInfo.Replace("<br />", ", ")),
                    EAN = item.ean,
                    Quantity = item.quantity
                });
            }

            return items;
        }

        private async Task UpdateInvoiceWithPricesAsync(AOInvoice invoice, Order order, List<OrderItem> orderItems, bool creditNote, bool invoiceShipping)
        {
            decimal total = 0, discount = 0, tax = 0;
            var currency = await _currencyService.GetCurrencyByCodeAsync(invoice.CustomerCurrencyCode);

            foreach (var item in orderItems)
            {
                if (creditNote)
                {
                    total -= item.UnitPriceInclTax * item.Quantity;
                    discount -= item.DiscountAmountInclTax * item.Quantity;
                    tax -= (item.UnitPriceInclTax - item.UnitPriceExclTax) * item.Quantity;
                }
                else
                {
                    total += item.UnitPriceInclTax * item.Quantity;
                    discount += item.DiscountAmountInclTax * item.Quantity;
                    tax += (item.UnitPriceInclTax - item.UnitPriceExclTax) * item.Quantity;
                }
            }

            if (creditNote)
            {
                invoice.InvoiceRefundedAmount = decimal.Negate(total);
                if (invoiceShipping)
                {
                    total -= invoice.InvoiceShipping;
                    tax -= (order.OrderShippingInclTax - order.OrderShippingExclTax);
                    invoice.InvoiceRefundedAmount += invoice.InvoiceShipping;
                }
            }
            else
            {
                if (invoiceShipping)
                {
                    total += invoice.InvoiceShipping;
                    tax += (order.OrderShippingInclTax - order.OrderShippingExclTax);
                }
            }

            if (invoice.CurrencyRate != 0)
            { 
                invoice.InvoiceTotal = await _priceCalculationService.RoundPriceAsync(total * invoice.CurrencyRate, currency);
                invoice.InvoiceDiscount = await _priceCalculationService.RoundPriceAsync(discount * invoice.CurrencyRate, currency);
                invoice.InvoiceTax = await _priceCalculationService.RoundPriceAsync(tax * invoice.CurrencyRate, currency);

                if (creditNote)
                {
                    invoice.InvoiceRefundedAmount = invoice.InvoiceRefundedAmount * invoice.CurrencyRate;
                }
            }
            else
            {
                invoice.InvoiceTotal = total;
                invoice.InvoiceDiscount = discount;
                invoice.InvoiceTax = tax;
            }

            
            var newPrice = await _priceCalculationService.RoundPriceAsync(total, currency);

            await _invoiceRepository.UpdateAsync(invoice, false);
        }

        private async Task CreateInvoiceItemsAsync(List<Nop.Core.Domain.Orders.OrderItem> orderItems, int invoiceId, bool creditNote, bool reStock = false)
        {
            foreach (var item in orderItems)
            {
                var invoiceItem = new AOInvoiceItem()
                {
                    InvoiceId = invoiceId,
                    OrderItemId = item.Id,
                    Quantity = item.Quantity
                };

                ProductAttributeCombination combination = null;
                if (item.ProductId > 0)
                {
                    // For manual invoice, productid will be 0
                    var product = await _productService.GetProductByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, item.AttributesXml);
                        if (combination != null)
                        {
                            invoiceItem.EAN = combination.Gtin;
                        }
                    }
                }

                if (creditNote)
                {
                    invoiceItem.Credited = true;

                    if (reStock)
                    {
                        if (combination == null)
                        {
                            await _logger.ErrorAsync($"No combination found for product: {item.ProductId}, we cannot restock this credit note product");
                        }
                        else
                        {
                            // When an item from an order has been credited, put it back in stock
                            await ReStockItemAsync(invoiceItem, combination);

                            // Log that we changed the stock
                            await LogStockChangeAsync(combination, item.Quantity, combination.StockQuantity);
                        }
                    }
                }

                await _invoiceItemRepository.InsertAsync(invoiceItem);
            }
        }

        private async Task AddTrackingNumberAsync(AOInvoice invoice, int orderId)
        {
            var shipments = await _shipmentService.GetShipmentsByOrderIdAsync(orderId);
            if (shipments != null && shipments.Count == 1)
            {
                invoice.TrackingNumber = shipments.FirstOrDefault().TrackingNumber;
            }
        }

        public async Task UpdateInvoiceAsync(AOInvoice invoice)
        {
            await _invoiceRepository.UpdateAsync(invoice);
        }
    }
}