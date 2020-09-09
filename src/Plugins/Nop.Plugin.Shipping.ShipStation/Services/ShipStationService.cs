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
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Formatting = System.Xml.Formatting;

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
        private readonly ICacheKeyService _cacheKeyService;
        private readonly ICountryService _countryService;
        private readonly ICustomerService _customerService;
        private readonly ILogger _logger;
        private readonly IMeasureService _measureService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IShipmentService _shipmentService;
        private readonly IShippingService _shippingService;        
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly ShipStationSettings _shipStationSettings;

        #endregion

        #region Ctor

        public ShipStationService(ICacheKeyService cacheKeyService,
            IAddressService addressService,
            ICountryService countryService,
            ICustomerService customerService,
            ILogger logger,
            IMeasureService measureService,
            IOrderService orderService,
            IProductService productService,
            IShipmentService shipmentService,
            IShippingService shippingService,
            IStateProvinceService stateProvinceService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            ShipStationSettings shipStationSettings)
        {
            _addressService = addressService;
            _cacheKeyService = cacheKeyService;
            _countryService = countryService;
            _customerService = customerService;
            _logger = logger;
            _measureService = measureService;
            _orderService = orderService;
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

        protected virtual async Task<string> SendGetRequest(string apiUrl)
        {
            var request = WebRequest.Create(apiUrl);

            request.Credentials = new NetworkCredential(_shipStationSettings.ApiKey, _shipStationSettings.ApiSecret);
            var resp = request.GetResponse();

            await using var rs = resp.GetResponseStream();
            if (rs == null) return string.Empty;
            using var sr = new StreamReader(rs);

            return await sr.ReadToEndAsync();
        }

        private async Task<int> ConvertFromPrimaryMeasureDimension(decimal quantity, MeasureDimension usedMeasureDimension)
        {
            return Convert.ToInt32(Math.Ceiling(await _measureService.ConvertFromPrimaryMeasureDimension(quantity, usedMeasureDimension)));
        }

        protected virtual bool TryGetError(string data)
        {
            var flag = false;
            try
            {
                var rez = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);

                if (rez.ContainsKey("message"))
                {
                    flag = true;
                    _logger.Error(rez["message"]);
                }
            }
            catch (JsonSerializationException)
            {
            }

            return flag;
        }

        protected virtual async Task<IList<ShipStationServiceRate>> GetRates(GetShippingOptionRequest getShippingOptionRequest, string carrierCode)
        {
            var usedWeight = await _measureService.GetMeasureWeightBySystemKeyword(Weight.Units);
            if (usedWeight == null)
                throw new NopException("ShipStatio shipping service. Could not load \"{0}\" measure weight", Weight.Units);

            var usedMeasureDimension = await _measureService.GetMeasureDimensionBySystemKeyword(Dimensions.Units);
            if (usedMeasureDimension == null)
                throw new NopException("ShipStatio shipping service. Could not load \"{0}\" measure dimension", Dimensions.Units);

            var weight = Convert.ToInt32(Math.Ceiling(await _measureService.ConvertFromPrimaryMeasureWeight(await _shippingService.GetTotalWeight(getShippingOptionRequest), usedWeight)));

            var postData = new RatesRequest
            {
                CarrierCode = carrierCode,
                FromPostalCode = getShippingOptionRequest.ZipPostalCodeFrom ?? getShippingOptionRequest.ShippingAddress.ZipPostalCode,
                ToState = (await _stateProvinceService.GetStateProvinceByAddress(getShippingOptionRequest.ShippingAddress)).Abbreviation,
                ToCountry = (await _countryService.GetCountryByAddress(getShippingOptionRequest.ShippingAddress)).TwoLetterIsoCode,
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
                        (widthTmp, lengthTmp, heightTmp) = await _shippingService.GetDimensions(getShippingOptionRequest.Items);

                        length = await ConvertFromPrimaryMeasureDimension(lengthTmp, usedMeasureDimension);
                        height = await ConvertFromPrimaryMeasureDimension(heightTmp, usedMeasureDimension);
                        width = await ConvertFromPrimaryMeasureDimension(widthTmp, usedMeasureDimension);
                        break;
                    case PackingType.PackByVolume:
                        if (getShippingOptionRequest.Items.Count == 1 &&
                            getShippingOptionRequest.Items[0].GetQuantity() == 1)
                        {
                            var sci = getShippingOptionRequest.Items[0].ShoppingCartItem;
                            var product = getShippingOptionRequest.Items[0].Product;

                            (widthTmp, lengthTmp, heightTmp) = await _shippingService.GetDimensions(new List<GetShippingOptionRequest.PackageItem>
                            {
                                new GetShippingOptionRequest.PackageItem(sci, product, 1)
                            });

                            length = await ConvertFromPrimaryMeasureDimension(lengthTmp, usedMeasureDimension);
                            height = await ConvertFromPrimaryMeasureDimension(lengthTmp, usedMeasureDimension);
                            width = await ConvertFromPrimaryMeasureDimension(widthTmp, usedMeasureDimension);
                        }
                        else
                        {
                            decimal totalVolume = 0;
                            foreach (var item in getShippingOptionRequest.Items)
                            {
                                var sci = item.ShoppingCartItem;
                                var product = item.Product;

                                (widthTmp, lengthTmp, heightTmp) = await _shippingService.GetDimensions(new List<GetShippingOptionRequest.PackageItem>
                                {
                                    new GetShippingOptionRequest.PackageItem(sci, product, 1)
                                });

                                var productLength = await ConvertFromPrimaryMeasureDimension(lengthTmp, usedMeasureDimension);
                                var productHeight = await ConvertFromPrimaryMeasureDimension(heightTmp, usedMeasureDimension);
                                var productWidth = await ConvertFromPrimaryMeasureDimension(widthTmp, usedMeasureDimension);
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

            return TryGetError(data) ? new List<ShipStationServiceRate>() : JsonConvert.DeserializeObject<List<ShipStationServiceRate>>(data);
        }
        
        protected virtual async Task<IList<Carrier>> GetCarriers()
        {
            var rez = await _staticCacheManager.Get(_cacheKeyService.PrepareKeyForShortTermCache(_carriersCacheKey), async () =>
            {
                var data = await SendGetRequest($"{API_URL}{LIST_CARRIERS_CMD}");
                
                return TryGetError(data) ? new List<Carrier>() : JsonConvert.DeserializeObject<List<Carrier>>(data);
            });

            if (!rez.Any())
                await _staticCacheManager.Remove(_carriersCacheKey);

            return rez;
        }
        
        protected virtual async Task<IList<Service>> GetServices()
        {
            var services = (await GetCarriers()).SelectMany(carrier =>
            {
                var cacheKey = _cacheKeyService.PrepareKeyForShortTermCache(_serviceCacheKey, carrier.Code);

                var data = _staticCacheManager.Get(cacheKey, async () => await SendGetRequest(string.Format($"{API_URL}{LIST_SERVICES_CMD}", carrier.Code))).Result;
                
                if (!data.Any())
                    _staticCacheManager.Remove(cacheKey);

                var serviceList = JsonConvert.DeserializeObject<List<Service>>(data);
                
                return serviceList;
            });

            return services.ToList();
        }

        protected virtual async Task WriteAddressToXml(XmlTextWriter writer, bool isBillingAddress, Address address)
        {
            writer.WriteElementString("Name", $"{address.FirstName} {address.LastName}");

            writer.WriteElementString("Company", address.Company);
            writer.WriteElementString("Phone", address.PhoneNumber);

            if (isBillingAddress)
                return;

            writer.WriteElementString("Address1", address.Address1);
            writer.WriteElementString("Address2", address.Address2);
            writer.WriteElementString("City", address.City);
            writer.WriteElementString("State", (await _stateProvinceService.GetStateProvinceByAddress(address))?.Name ?? string.Empty);
            writer.WriteElementString("PostalCode ", address.ZipPostalCode);
            writer.WriteElementString("Country", (await _countryService.GetCountryByAddress(address)).TwoLetterIsoCode);
        }

        protected virtual async Task WriteOrderItemsToXml(XmlTextWriter writer, ICollection<OrderItem> orderItems)
        {
            writer.WriteStartElement("Items");

            foreach (var orderItem in orderItems)
            {
                var product = await _productService.GetProductById(orderItem.ProductId);
                var order = await _orderService.GetOrderById(orderItem.OrderId);

                //is shippable
                if (!product.IsShipEnabled)
                    continue;

                writer.WriteStartElement("Item");

                var sku = product.Sku;

                writer.WriteElementString("SKU", string.IsNullOrEmpty(sku) ? product.Id.ToString() : sku);
                writer.WriteElementString("Name", product.Name);
                writer.WriteElementString("Quantity", orderItem.Quantity.ToString());
                writer.WriteElementString("UnitPrice", (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax ? orderItem.UnitPriceInclTax : orderItem.UnitPriceExclTax).ToString(CultureInfo.InvariantCulture));

                writer.WriteEndElement();
                writer.Flush();
            }

            writer.WriteEndElement();
            writer.Flush();
        }

        protected virtual async Task WriteCustomerToXml(XmlTextWriter writer, Order order, Core.Domain.Customers.Customer customer)
        {
            writer.WriteStartElement("Customer");

            writer.WriteElementString("CustomerCode", customer.Email);
            writer.WriteStartElement("BillTo");
            await WriteAddressToXml(writer, true, await _addressService.GetAddressById(order.BillingAddressId));
            writer.WriteEndElement();
            writer.WriteStartElement("ShipTo");
            await WriteAddressToXml(writer, false, await _addressService.GetAddressById(order.ShippingAddressId ?? order.BillingAddressId));
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.Flush();
        }

        protected virtual string GetOrderStatus(Order order)
        {
            switch (order.OrderStatus)
            {
                case OrderStatus.Pending:
                    return "unpaid";
                case OrderStatus.Processing:
                    return "paid";
                case OrderStatus.Complete:
                    return "shipped";
                case OrderStatus.Cancelled:
                    return "cancelled";
                default:
                    return "on_hold";
            }
        }

        protected virtual async Task WriteOrderToXml(XmlTextWriter writer, Order order)
        {
            writer.WriteStartElement("Order");
            writer.WriteElementString("OrderID", order.Id.ToString());
            writer.WriteElementString("OrderNumber", order.OrderGuid.ToString());
            writer.WriteElementString("OrderDate", order.CreatedOnUtc.ToString(DATE_FORMAT));
            writer.WriteElementString("OrderStatus ", GetOrderStatus(order));
            writer.WriteElementString("LastModified", DateTime.Now.ToString(DATE_FORMAT));
            writer.WriteElementString("OrderTotal", order.OrderTotal.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("ShippingAmount", (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax ? order.OrderShippingInclTax : order.OrderShippingExclTax).ToString(CultureInfo.InvariantCulture));

            await WriteCustomerToXml(writer, order, await _customerService.GetCustomerById(order.CustomerId));
            await WriteOrderItemsToXml(writer, await _orderService.GetOrderItems(order.Id));

            writer.WriteEndElement();
            writer.Flush();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all rates
        /// </summary>
        /// <param name="shippingOptionRequest"></param>
        /// <returns></returns>
        public virtual async Task<IList<ShipStationServiceRate>> GetAllRates(GetShippingOptionRequest shippingOptionRequest)
        {
            var services = await GetServices();

            var carrierFilter = services.Select(s => s.CarrierCode).Distinct().ToList();
            var serviceFilter = services.Select(s => s.Code).Distinct().ToList();
            var carriers = (await GetCarriers()).Where(c => carrierFilter.Contains(c.Code));

            return carriers.SelectMany(carrier =>
                GetRates(shippingOptionRequest, carrier.Code).Result.Where(r => serviceFilter.Contains(r.ServiceCode))).ToList();
        }
        
        /// <summary>
        /// Create or upadete shipping
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <param name="carrier"></param>
        /// <param name="service"></param>
        /// <param name="trackingNumber"></param>
        public async Task CreateOrUpadeteShipping(string orderNumber, string carrier, string service, string trackingNumber)
        {
            try
            {
                var order = await _orderService.GetOrderByGuid(Guid.Parse(orderNumber));

                if (order == null)
                    return;

                var shipments = await _shipmentService.GetShipmentsByOrderId(order.Id);

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

                    foreach (var orderItem in await _orderService.GetOrderItems(order.Id))
                    {
                        var product = await _productService.GetProductById(orderItem.ProductId);
                        
                        //is shippable
                        if (!product.IsShipEnabled)
                            continue;

                        //ensure that this product can be shipped (have at least one item to ship)
                        var maxQtyToAdd = await _orderService.GetTotalNumberOfItemsCanBeAddedToShipment(orderItem);
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
                            WarehouseId = warehouseId
                        };

                        await _shipmentService.InsertShipmentItem(shipmentItem);
                    }

                    shipment.TotalWeight = totalWeight;

                    await _shipmentService.InsertShipment(shipment);
                }
                else
                {
                    var shipment = shipments.FirstOrDefault();

                    if (shipment == null)
                        return;

                    shipment.TrackingNumber = trackingNumber;

                    await _shipmentService.UpdateShipment(shipment);
                }

                order.ShippingStatus = ShippingStatus.Shipped;
                order.ShippingMethod = string.IsNullOrEmpty(service) ? carrier : service;

                await _orderService.UpdateOrder(order);
            }
            catch (Exception e)
            {
                await _logger.Error(e.Message, e);
            }
        }

        /// <summary>
        /// Get XML view of orders to sending to the ShipStation service
        /// </summary>
        /// <param name="startDate">Created date from (UTC); null to load all records</param>
        /// <param name="endDate">Created date to (UTC); null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>XML view of orders</returns>
        public async Task<string> GetXmlOrders(DateTime? startDate, DateTime? endDate, int pageIndex, int pageSize)
        {
            string xml;

            await using (var stream = new MemoryStream())
            {
                using (var writer = new XmlTextWriter(stream, Encoding.UTF8))
                {
                    writer.Formatting = Formatting.Indented;
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Orders");

                    foreach (var order in await _orderService.SearchOrders(createdFromUtc: startDate, createdToUtc: endDate, storeId: (await _storeContext.GetCurrentStore()).Id, pageIndex: pageIndex, pageSize: 200))
                    {
                        await WriteOrderToXml(writer, order);
                    }

                    writer.WriteEndElement();
                }

                xml = Encoding.UTF8.GetString(stream.ToArray());
            }

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
