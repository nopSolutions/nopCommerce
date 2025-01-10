using System;
using System.Linq;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using Nop.Core.Infrastructure;
using System.Collections.Generic;
using Nop.Services.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Shipping;
using System.Net;
using Nop.Data;
using System.Data;
using Nop.Services.Plugins;
using LinqToDB.Data;
using LinqToDB;
using Nop.Plugin.Misc.AbcCore.Domain;
using Nop.Services.Common;
using Nop.Plugin.Misc.AbcCore.Delivery;
using Nop.Plugin.Misc.AbcCore.HomeDelivery;
using System.Threading.Tasks;
using Nop.Plugin.Shipping.Fedex;
using Nop.Core;
using Nop.Services.Logging;
using Nop.Core.Domain.Logging;

namespace Nop.Plugin.Shipping.HomeDelivery
{
    /// <summary>
    /// ABC Home Delivery computation method
    /// </summary>
    public class HomeDeliveryPlugin : BasePlugin, IShippingRateComputationMethod
    {
        private readonly IDeliveryService _deliveryService;
        private readonly IShippingRateComputationMethod _baseShippingComputation;
        private readonly IRepository<ProductHomeDelivery> _productHomeDeliveryRepo;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly INopDataProvider _nopContext;
        private readonly IHomeDeliveryCostService _homeDeliveryCostService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ILogger _logger;

        public HomeDeliveryPlugin(
            IDeliveryService deliveryService,
            IRepository<ProductHomeDelivery> productHomeDeliveryRepo,
            IProductAttributeParser productAttributeParser,
            INopDataProvider nopContext,
            IHomeDeliveryCostService homeDeliveryCostService,
            IProductAttributeService productAttributeService,
            ILogger logger
        )
        {
            _deliveryService = deliveryService;
            _baseShippingComputation = EngineContext.Current.Resolve<FedexComputationMethod>();
            _productHomeDeliveryRepo = productHomeDeliveryRepo;
            _productAttributeParser = productAttributeParser;
            _nopContext = nopContext;
            _homeDeliveryCostService = homeDeliveryCostService;
            _productAttributeService = productAttributeService;
            _logger = logger;
        }

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Represents a response of getting shipping rate options</returns>
        public async Task<GetShippingOptionResponse> GetShippingOptionsAsync(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest == null)
                throw new ArgumentNullException("getShippingOptionRequest");

            var response = new GetShippingOptionResponse();

            if (getShippingOptionRequest.Items == null)
            {
                response.AddError("No shipment items");
                return response;
            }

            if (getShippingOptionRequest.ShippingAddress == null)
            {
                // additional information for debugging
                var customer = getShippingOptionRequest.Customer;
                var items = getShippingOptionRequest.Items;
                var itemsString = string.Join(", ", items.Select(x => $"sci ID: {x.ShoppingCartItem?.Id}, product ID: {x.Product?.Id}"));
                await _logger.InsertLogAsync(
                    LogLevel.Information,
                    "Debug information for Shipping Method",
                    $"Items: {itemsString}",
                    customer
                );

                response.AddError("Shipping address is not set");
                return response;
            }

            if (getShippingOptionRequest.ShippingAddress.CountryId == null)
            {
                response.AddError("Shipping country is not set");
                return response;
            }

            // find all items that don't qualify as Delivery/Pickup
            // There are 3 possibilities
            //  1. Delivery/Pickup (gets excluded unless FedEx)
            //  2. Legacy Delivery (mattresses, just uses the old system)
            //  3. Fedex - used by default if neither above are fulfilled
            var legacyHomeDeliveryItems = new List<GetShippingOptionRequest.PackageItem>();
            var fedexDeliveryItems = new List<GetShippingOptionRequest.PackageItem>();

            foreach (var item in getShippingOptionRequest.Items)
            {
                var matchedType = false;
                var itemAttributeXml = item.ShoppingCartItem.AttributesXml;
                var pams = await _productAttributeParser.ParseProductAttributeMappingsAsync(itemAttributeXml);
                foreach (var pam in pams)
                {
                    var pa = await _productAttributeService.GetProductAttributeByIdAsync(pam.ProductAttributeId);
                    if (pa.Name == "Pickup")
                    {
                        matchedType = true;
                        continue;
                    }
                    if (pa.Name != AbcDeliveryConsts.DeliveryPickupOptionsProductAttributeName)
                    {
                        continue;
                    }

                    // Since this is a delivery/pickup item, it needs a single pav
                    var pavs = await _productAttributeParser.ParseProductAttributeValuesAsync(itemAttributeXml, pam.Id);
                    ProductAttributeValue pav = null;
                    try {
                        pav = pavs.Single();
                    }
                    catch (InvalidOperationException)
                    {
                        var sci = item.ShoppingCartItem;
                        await _logger.InsertLogAsync(
                            LogLevel.Error,
                            $"Delivery/Pickup shopping cart item {sci.Id} has invalid product attribute value(s). Attribute XML in full message.",
                            $"{itemAttributeXml}",
                            getShippingOptionRequest.Customer
                        );
                        throw new NopException(
                            "Failure when filtering shopping cart items for getting shipping options during shipping cost estimate."
                        );
                    }

                    // Mattresses are added to legacy
                    if (pav.Name.Contains("Home Delivery (Price in Cart)"))
                    {
                        legacyHomeDeliveryItems.Add(item);
                        matchedType = true;
                    }
                    else if (pav.Name.Contains("Home Delivery") || pav.Name.Contains("Pickup"))
                    {
                        // not included in home delivery calculations
                        matchedType = true;
                    }
                }

                if (!matchedType)
                {
                    fedexDeliveryItems.Add(item);
                }
            }

            var legacyHomeDeliveryCharge = await _homeDeliveryCostService.GetHomeDeliveryCostAsync(legacyHomeDeliveryItems);

            //if there are items to be shipped, use the base calculation service if items remain that will be shipped by ups
            if (fedexDeliveryItems.Count > 0)
            {
                var oldProtocol = ServicePointManager.SecurityProtocol;
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
                    // only check fedex items
                    getShippingOptionRequest.Items = fedexDeliveryItems;
                    response = await _baseShippingComputation.GetShippingOptionsAsync(getShippingOptionRequest);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    ServicePointManager.SecurityProtocol = oldProtocol;
                }
                if (legacyHomeDeliveryItems.Count > 0)
                {
                    var zip = getShippingOptionRequest.ShippingAddress.ZipPostalCode;
                    await CheckZipcodeAsync(zip, response);
                    foreach (var shippingOption in response.ShippingOptions)
                    {
                        shippingOption.Name += " and Mattress Delivery";
                        shippingOption.Rate += legacyHomeDeliveryCharge;
                    }
                }
            }// else check if contains only legacy home delivery
            else if (legacyHomeDeliveryItems.Count > 0)
            {
                var zip = getShippingOptionRequest.ShippingAddress.ZipPostalCode;
                await CheckZipcodeAsync(zip, response);
                response.ShippingOptions.Add(new ShippingOption { Name = "Mattress Delivery", Rate = legacyHomeDeliveryCharge });
            }
            // use the default option of no charge, since cost is built into the items
            else 
            {
                response.ShippingOptions.Add(new ShippingOption { Name = "No Additional Shipping Charge", Rate = 0M });
            }
            return response;
        }

        private async Task<bool> CheckZipcodeAsync(string zip, GetShippingOptionResponse response)
        {
            if (zip.Length < 5)
            {
                response.AddError("Zip code must be at least 5 digits.");
                return false;
            }

            var firstFive = zip.Substring(0, 5);
            var isZipEligible = await _deliveryService.CheckZipcodeAsync(firstFive);
            if (isZipEligible)
                return true;
            else
            {
                response.AddError("Home Delivery Shipping is not available for the given zip code.");
                return false;
            }
                
        }

        /// <summary>
        /// Gets fixed shipping rate (if shipping rate computation method allows it and the rate can be calculated before checkout).
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Fixed shipping rate; or null in case there's no fixed shipping rate</returns>
        public Task<decimal?> GetFixedRateAsync(GetShippingOptionRequest getShippingOptionRequest)
        {
            return Task.FromResult<decimal?>(null);
        }

        public IShipmentTracker ShipmentTracker => _baseShippingComputation.ShipmentTracker;
    }
}