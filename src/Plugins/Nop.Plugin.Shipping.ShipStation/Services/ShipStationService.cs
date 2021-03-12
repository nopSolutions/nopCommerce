using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;

namespace Nop.Plugin.Shipping.ShipStation.Services
{
    public partial class ShipStationService : IShipStationService
    {
        #region constants

        private const string API_URL = "https://ssapi.shipstation.com/";
        private readonly CacheKey _carriersCacheKey = new CacheKey("Nop.plugins.shipping.shipstation.carrierscachekey");
        private readonly CacheKey _serviceCacheKey = new CacheKey("Nop.plugins.shipping.shipstation.servicecachekey.{0}");

        private const string CONTENT_TYPE = "application/json";
        private const string DATE_FORMAT = "MM/dd/yyyy HH:mm";

        private const string LIST_CARRIERS_CMD = "carriers";
        private const string LIST_SERVICES_CMD = "carriers/listservices?carrierCode={0}";
        private const string LIST_RATES_CMD = "shipments/getrates";

        #endregion

        #region Fields

        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly ICustomerService _customerService;
        private readonly ILogger _logger;
        private readonly IMeasureService _measureService;
        private readonly IOrderService _orderService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly IShipmentService _shipmentService;
        private readonly IShippingService _shippingService;        
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly ShipStationSettings _shipStationSettings;

        #endregion

        #region Ctor

        public ShipStationService(IAddressService addressService,
            ICountryService countryService,
            ICustomerService customerService,
            ILogger logger,
            IMeasureService measureService,
            IOrderService orderService,
            IProductAttributeParser productAttributeParser,
            IProductService productService,
            IShipmentService shipmentService,
            IShippingService shippingService,
            IStateProvinceService stateProvinceService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            ShipStationSettings shipStationSettings)
        {
            _addressService = addressService;
            _countryService = countryService;
            _customerService = customerService;
            _logger = logger;
            _measureService = measureService;
            _orderService = orderService;
            _productAttributeParser = productAttributeParser;
            _productService = productService;
            _shipmentService = shipmentService;
            _shippingService = shippingService;
            _stateProvinceService = stateProvinceService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _shipStationSettings = shipStationSettings;
        }

        #endregion

        #region Utilities

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<string> SendGetRequestAsync(string apiUrl)
        {
            var request = WebRequest.Create(apiUrl);

            request.Credentials = new NetworkCredential(_shipStationSettings.ApiKey, _shipStationSettings.ApiSecret);
            var resp = await request.GetResponseAsync();

            await using var rs = resp.GetResponseStream();
            if (rs == null) return string.Empty;
            using var sr = new StreamReader(rs);

            return await sr.ReadToEndAsync();
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task<int> ConvertFromPrimaryMeasureDimensionAsync(decimal quantity, MeasureDimension usedMeasureDimension)
        {
            return Convert.ToInt32(Math.Ceiling(await _measureService.ConvertFromPrimaryMeasureDimensionAsync(quantity, usedMeasureDimension)));
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<bool> TryGetError(string data)
        {
            var flag = false;
            try
            {
                var rez = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

                if (rez.ContainsKey("message"))
                {
                    flag = true;

                    await _logger.ErrorAsync(rez["message"]);
                }
            }
            catch (JsonSerializationException)
            {
            }

            return flag;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<IList<ShipStationServiceRate>> GetRatesAsync(GetShippingOptionRequest getShippingOptionRequest, string carrierCode)
        {
            var usedWeight = await _measureService.GetMeasureWeightBySystemKeywordAsync(Weight.Units);
            if (usedWeight == null)
                throw new NopException("ShipStatio shipping service. Could not load \"{0}\" measure weight", Weight.Units);

            var usedMeasureDimension = await _measureService.GetMeasureDimensionBySystemKeywordAsync(Dimensions.Units);
            if (usedMeasureDimension == null)
                throw new NopException("ShipStatio shipping service. Could not load \"{0}\" measure dimension", Dimensions.Units);

            var weight = Convert.ToInt32(Math.Ceiling(await _measureService.ConvertFromPrimaryMeasureWeightAsync(await _shippingService.GetTotalWeightAsync(getShippingOptionRequest), usedWeight)));

            var postData = new RatesRequest
            {
                CarrierCode = carrierCode,
                FromPostalCode = getShippingOptionRequest.ZipPostalCodeFrom ?? getShippingOptionRequest.ShippingAddress.ZipPostalCode,
                ToState = (await _stateProvinceService.GetStateProvinceByAddressAsync(getShippingOptionRequest.ShippingAddress)).Abbreviation,
                ToCountry = (await _countryService.GetCountryByAddressAsync(getShippingOptionRequest.ShippingAddress)).TwoLetterIsoCode,
                ToPostalCode = getShippingOptionRequest.ShippingAddress.ZipPostalCode,
                ToCity = getShippingOptionRequest.ShippingAddress.City,
                Weight = new Weight { Value = weight }
            };

            if (_shipStationSettings.PassDimensions)
            {
                int length, height, width;

                decimal lengthTmp, widthTmp, heightTmp;

                switch (_shipStationSettings.PackingType)
                {
                    case PackingType.PackByDimensions:
                        (widthTmp, lengthTmp, heightTmp) = await _shippingService.GetDimensionsAsync(getShippingOptionRequest.Items);

                        length = await ConvertFromPrimaryMeasureDimensionAsync(lengthTmp, usedMeasureDimension);
                        height = await ConvertFromPrimaryMeasureDimensionAsync(heightTmp, usedMeasureDimension);
                        width = await ConvertFromPrimaryMeasureDimensionAsync(widthTmp, usedMeasureDimension);
                        break;
                    case PackingType.PackByVolume:
                        if (getShippingOptionRequest.Items.Count == 1 &&
                            getShippingOptionRequest.Items[0].GetQuantity() == 1)
                        {
                            var sci = getShippingOptionRequest.Items[0].ShoppingCartItem;
                            var product = getShippingOptionRequest.Items[0].Product;

                            (widthTmp, lengthTmp, heightTmp) = await _shippingService.GetDimensionsAsync(new List<GetShippingOptionRequest.PackageItem>
                            {
                                new GetShippingOptionRequest.PackageItem(sci, product, 1)
                            });

                            length = await ConvertFromPrimaryMeasureDimensionAsync(lengthTmp, usedMeasureDimension);
                            height = await ConvertFromPrimaryMeasureDimensionAsync(lengthTmp, usedMeasureDimension);
                            width = await ConvertFromPrimaryMeasureDimensionAsync(widthTmp, usedMeasureDimension);
                        }
                        else
                        {
                            decimal totalVolume = 0;
                            foreach (var item in getShippingOptionRequest.Items)
                            {
                                var sci = item.ShoppingCartItem;
                                var product = item.Product;

                                (widthTmp, lengthTmp, heightTmp) = await _shippingService.GetDimensionsAsync(new List<GetShippingOptionRequest.PackageItem>
                                {
                                    new GetShippingOptionRequest.PackageItem(sci, product, 1)
                                });

                                var productLength = await ConvertFromPrimaryMeasureDimensionAsync(lengthTmp, usedMeasureDimension);
                                var productHeight = await ConvertFromPrimaryMeasureDimensionAsync(heightTmp, usedMeasureDimension);
                                var productWidth = await ConvertFromPrimaryMeasureDimensionAsync(widthTmp, usedMeasureDimension);
                                totalVolume += item.GetQuantity() * (productHeight * productWidth * productLength);
                            }

                            int dimension;
                            if (totalVolume == 0)
                            {
                                dimension = 0;
                            }
                            else
                            {
                                // cubic inches
                                var packageVolume = _shipStationSettings.PackingPackageVolume;
                                if (packageVolume <= 0)
                                    packageVolume = 5184;

                                // cube root (floor)
                                dimension = Convert.ToInt32(Math.Floor(Math.Pow(Convert.ToDouble(packageVolume),
                                    1.0 / 3.0)));
                            }

                            length = width = height = dimension;
                        }

                        break;
                    default:
                        length = height = width = 1;
                        break;
                }

                if (length < 1)
                    length = 1;
                if (height < 1)
                    height = 1;
                if (width < 1)
                    width = 1;

                postData.Dimensions = new Dimensions
                {
                    Length = length,
                    Height = height,
                    Width = width
                };
            }

            using var client = new WebClient
            {
                Credentials = new NetworkCredential(_shipStationSettings.ApiKey, _shipStationSettings.ApiSecret)
            };

            client.Headers.Add("Content-Type", CONTENT_TYPE);

            var data = client.UploadString($"{API_URL}{LIST_RATES_CMD}", JsonConvert.SerializeObject(postData));

            return (await TryGetError(data)) ? new List<ShipStationServiceRate>() : JsonConvert.DeserializeObject<List<ShipStationServiceRate>>(data);
        }
        
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<IList<Carrier>> GetCarriersAsync()
        {
            var rez = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForShortTermCache(_carriersCacheKey), async () =>
            {
                var data = await SendGetRequestAsync($"{API_URL}{LIST_CARRIERS_CMD}");
                
                return (await TryGetError(data)) ? new List<Carrier>() : JsonConvert.DeserializeObject<List<Carrier>>(data);
            });

            if (!rez.Any())
                await _staticCacheManager.RemoveAsync(_carriersCacheKey);

            return rez;
        }
        
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<IList<Service>> GetServicesAsync()
        {
            var services = await (await GetCarriersAsync()).SelectManyAwait(async carrier =>
            {
                var cacheKey = _staticCacheManager.PrepareKeyForShortTermCache(_serviceCacheKey, carrier.Code);

                var data = await _staticCacheManager.GetAsync(cacheKey, async () => await SendGetRequestAsync(string.Format($"{API_URL}{LIST_SERVICES_CMD}", carrier.Code)));
                
                if (!data.Any())
                    await _staticCacheManager.RemoveAsync(cacheKey);

                var serviceList = JsonConvert.DeserializeObject<IList<Service>>(data);
                
                return serviceList;
            }).ToListAsync();

            return services.ToList();
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task WriteAddressToXmlAsync(XmlWriter writer, bool isBillingAddress, Address address)
        {
            await writer.WriteElementStringAsync("Name", $"{address.FirstName} {address.LastName}");

            await writer.WriteElementStringAsync("Company", address.Company);
            await writer.WriteElementStringAsync("Phone", address.PhoneNumber);

            if (isBillingAddress)
                return;

            await writer.WriteElementStringAsync("Address1", address.Address1);
            await writer.WriteElementStringAsync("Address2", address.Address2);
            await writer.WriteElementStringAsync("City", address.City);
            await writer.WriteElementStringAsync("State", (await _stateProvinceService.GetStateProvinceByAddressAsync(address))?.Name ?? string.Empty);
            await writer.WriteElementStringAsync("PostalCode ", address.ZipPostalCode);
            await writer.WriteElementStringAsync("Country", (await _countryService.GetCountryByAddressAsync(address)).TwoLetterIsoCode);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task WriteOrderItemsToXmlAsync(XmlWriter writer, ICollection<OrderItem> orderItems)
        {
            await writer.WriteStartElementAsync("Items");

            foreach (var orderItem in orderItems)
            {
                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
                var order = await _orderService.GetOrderByIdAsync(orderItem.OrderId);

                //is shippable
                if (!product.IsShipEnabled)
                    continue;

                await writer.WriteStartElementAsync("Item");

                var sku = product.Sku;

                if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStockByAttributes)
                {
                    var attributesXml = orderItem.AttributesXml;

                    if (!string.IsNullOrEmpty(attributesXml) && product.ManageInventoryMethod ==
                        ManageInventoryMethod.ManageStockByAttributes)
                    {
                        var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
                        if (combination != null && !string.IsNullOrEmpty(combination.Sku)) 
                            sku = combination.Sku;
                    }
                }

                await writer.WriteElementStringAsync("SKU", string.IsNullOrEmpty(sku) ? product.Id.ToString() : sku);
                await writer.WriteElementStringAsync("Name", product.Name);
                await writer.WriteElementStringAsync("Quantity", orderItem.Quantity.ToString());
                await writer.WriteElementStringAsync("UnitPrice", (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax ? orderItem.UnitPriceInclTax : orderItem.UnitPriceExclTax).ToString(CultureInfo.InvariantCulture));

                await writer.WriteEndElementAsync();
                await writer.FlushAsync();
            }

            await writer.WriteEndElementAsync();
            await writer.FlushAsync();
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task WriteCustomerToXmlAsync(XmlWriter writer, Order order, Core.Domain.Customers.Customer customer)
        {
            await writer.WriteStartElementAsync("Customer");

            await writer.WriteElementStringAsync("CustomerCode", customer.Email);
            await writer.WriteStartElementAsync("BillTo");
            await WriteAddressToXmlAsync(writer, true, await _addressService.GetAddressByIdAsync(order.BillingAddressId));
            await writer.WriteEndElementAsync();
            await writer.WriteStartElementAsync("ShipTo");
            await WriteAddressToXmlAsync(writer, false, await _addressService.GetAddressByIdAsync(order.ShippingAddressId ?? order.BillingAddressId));
            await writer.WriteEndElementAsync();

            await writer.WriteEndElementAsync();
            await writer.FlushAsync();
        }

        protected virtual string GetOrderStatus(Order order)
        {
            return order.OrderStatus switch
            {
                OrderStatus.Pending => "unpaid",
                OrderStatus.Processing => "paid",
                OrderStatus.Complete => "shipped",
                OrderStatus.Cancelled => "cancelled",
                _ => "on_hold",
            };
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task WriteOrderToXmlAsync(XmlWriter writer, Order order)
        {
            await writer.WriteStartElementAsync("Order");
            await writer.WriteElementStringAsync("OrderID", order.Id.ToString());
            await writer.WriteElementStringAsync("OrderNumber", order.OrderGuid.ToString());
            await writer.WriteElementStringAsync("OrderDate", order.CreatedOnUtc.ToString(DATE_FORMAT));
            await writer.WriteElementStringAsync("OrderStatus ", GetOrderStatus(order));
            await writer.WriteElementStringAsync("LastModified", DateTime.Now.ToString(DATE_FORMAT));
            await writer.WriteElementStringAsync("OrderTotal", order.OrderTotal.ToString(CultureInfo.InvariantCulture));
            await writer.WriteElementStringAsync("ShippingAmount", (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax ? order.OrderShippingInclTax : order.OrderShippingExclTax).ToString(CultureInfo.InvariantCulture));

            await WriteCustomerToXmlAsync(writer, order, await _customerService.GetCustomerByIdAsync(order.CustomerId));
            await WriteOrderItemsToXmlAsync(writer, await _orderService.GetOrderItemsAsync(order.Id));

            await writer.WriteEndElementAsync();
            await writer.FlushAsync();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all rates
        /// </summary>
        /// <param name="shippingOptionRequest"></param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the 
        /// </returns>
        public virtual async Task<IList<ShipStationServiceRate>> GetAllRatesAsync(GetShippingOptionRequest shippingOptionRequest)
        {
            var services = await GetServicesAsync();

            var carrierFilter = services.Select(s => s.CarrierCode).Distinct().ToList();
            var serviceFilter = services.Select(s => s.Code).Distinct().ToList();
            var carriers = (await GetCarriersAsync()).Where(c => carrierFilter.Contains(c.Code));

            return await carriers.SelectManyAwait(async carrier =>
                (await GetRatesAsync(shippingOptionRequest, carrier.Code)).Where(r => serviceFilter.Contains(r.ServiceCode))).ToListAsync();
        }
        
        /// <summary>
        /// Create or update shipping
        /// </summary>
        /// <param name="orderNumber">Order number</param>
        /// <param name="carrier">Carrier</param>
        /// <param name="service">Service</param>
        /// <param name="trackingNumber">Tracking number</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task CreateOrUpdateShippingAsync(string orderNumber, string carrier, string service, string trackingNumber)
        {
            try
            {
                var order = await _orderService.GetOrderByGuidAsync(Guid.Parse(orderNumber));

                if (order == null)
                    return;

                var shipments = await _shipmentService.GetShipmentsByOrderIdAsync(order.Id);

                if (!shipments.Any())
                {
                    var shipment = new Shipment
                    {
                        CreatedOnUtc = DateTime.UtcNow,
                        ShippedDateUtc = DateTime.UtcNow,
                        OrderId = order.Id,
                        TrackingNumber = trackingNumber
                    };

                    decimal totalWeight = 0;

                    await _shipmentService.InsertShipmentAsync(shipment);

                    foreach (var orderItem in await _orderService.GetOrderItemsAsync(order.Id))
                    {
                        var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
                        
                        //is shippable
                        if (!product.IsShipEnabled)
                            continue;

                        //ensure that this product can be shipped (have at least one item to ship)
                        var maxQtyToAdd = await _orderService.GetTotalNumberOfItemsCanBeAddedToShipmentAsync(orderItem);
                        if (maxQtyToAdd <= 0)
                            continue;

                        var warehouseId = product.WarehouseId;

                        //ok. we have at least one item. let's create a shipment (if it does not exist)

                        var orderItemTotalWeight = orderItem.ItemWeight * orderItem.Quantity;
                        if (orderItemTotalWeight.HasValue)
                            totalWeight += orderItemTotalWeight.Value;

                        //create a shipment item
                        var shipmentItem = new ShipmentItem
                        {
                            OrderItemId = orderItem.Id,
                            Quantity = orderItem.Quantity,
                            WarehouseId = warehouseId,
                            ShipmentId = shipment.Id
                        };

                        await _shipmentService.InsertShipmentItemAsync(shipmentItem);
                    }

                    shipment.TotalWeight = totalWeight;

                    await _shipmentService.UpdateShipmentAsync(shipment);
                }
                else
                {
                    var shipment = shipments.FirstOrDefault();

                    if (shipment == null)
                        return;

                    shipment.TrackingNumber = trackingNumber;

                    await _shipmentService.UpdateShipmentAsync(shipment);
                }

                order.ShippingStatus = ShippingStatus.Shipped;
                order.ShippingMethod = string.IsNullOrEmpty(service) ? carrier : service;

                await _orderService.UpdateOrderAsync(order);
            }
            catch (Exception e)
            {
                await _logger.ErrorAsync(e.Message, e);
            }
        }

        /// <summary>
        /// Get XML view of orders to sending to the ShipStation service
        /// </summary>
        /// <param name="startDate">Created date from (UTC); null to load all records</param>
        /// <param name="endDate">Created date to (UTC); null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the xML view of orders
        /// </returns>
        public async Task<string> GetXmlOrdersAsync(DateTime? startDate, DateTime? endDate, int pageIndex, int pageSize)
        {
            string xml;

            var settings = new XmlWriterSettings
            {
                Async = true,
                Encoding = Encoding.UTF8,
                Indent = true,
                ConformanceLevel = ConformanceLevel.Auto
            };

            await using var stream = new MemoryStream();
            await using var writer = XmlWriter.Create(stream, settings);

            await writer.WriteStartDocumentAsync();
            await writer.WriteStartElementAsync("Orders");

            foreach (var order in await _orderService.SearchOrdersAsync(createdFromUtc: startDate, createdToUtc: endDate, storeId: (await _storeContext.GetCurrentStoreAsync()).Id, pageIndex: pageIndex, pageSize: 200))
            {
                await WriteOrderToXmlAsync(writer, order);
            }

            await writer.WriteEndElementAsync();

            xml = Encoding.UTF8.GetString(stream.ToArray());

            return xml;
        }

        /// <summary>
        /// Date format
        /// </summary>
        public string DateFormat => DATE_FORMAT;

        #endregion

        #region Nested classes

        protected class Carrier
        {
            public string Name { get; set; }

            public string Code { get; set; }
        }

        protected class Service : IEqualityComparer<Service>
        {
            public string CarrierCode { get; set; }

            public string Code { get; set; }

            public string Name { get; set; }

            public bool Domestic { get; set; }

            public bool International { get; set; }

            /// <summary>
            /// Determines whether the specified objects are equal
            /// </summary>
            /// <param name="first">The first object of type T to compare</param>
            /// <param name="second">The second object of type T to compare</param>
            /// <returns>true if the specified objects are equal; otherwise, false</returns>
            public bool Equals(Service first, Service second)
            {
                if (first == null && second == null)
                    return true;

                if (first == null)
                    return false;

                return first.Code.Equals(second?.Code);
            }

            public int GetHashCode(Service obj)
            {
                return Code.GetHashCode();
            }
        }

        protected class RatesRequest
        {
            public string CarrierCode { get; set; }

            public string FromPostalCode { get; set; }

            public string ToState { get; set; }

            public string ToCountry { get; set; }

            public string ToPostalCode { get; set; }

            public string ToCity { get; set; }

            public Weight Weight { get; set; }

            public Dimensions Dimensions { get; set; }
        }

        protected class Weight
        {
            public static string Units => "ounce";

            public int Value { get; set; }
        }

        protected class Dimensions
        {
            public static string Units => "inches";

            public int Length { get; set; }

            public int Width { get; set; }

            public int Height { get; set; }
        }

        #endregion
    }
}
