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

        public HomeDeliveryPlugin(
            IDeliveryService deliveryService,
            IRepository<ProductHomeDelivery> productHomeDeliveryRepo,
            IProductAttributeParser productAttributeParser,
            INopDataProvider nopContext,
            IHomeDeliveryCostService homeDeliveryCostService,
            IProductAttributeService productAttributeService
        )
        {
            _deliveryService = deliveryService;
            _baseShippingComputation = EngineContext.Current.Resolve<FedexComputationMethod>();
            _productHomeDeliveryRepo = productHomeDeliveryRepo;
            _productAttributeParser = productAttributeParser;
            _nopContext = nopContext;
            _homeDeliveryCostService = homeDeliveryCostService;
            _productAttributeService = productAttributeService;
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
                response.AddError("Shipping address is not set");
                return response;
            }

            if (getShippingOptionRequest.ShippingAddress.CountryId == null)
            {
                response.AddError("Shipping country is not set");
                return response;
            }

            //TODO clone the request to prevent issues cause by modification
            var it = getShippingOptionRequest.Items.GetEnumerator();
            var homeDeliveryList = new List<GetShippingOptionRequest.PackageItem>();
            var pickupInStoreList = new List<GetShippingOptionRequest.PackageItem>();

            //find and separately process all home delivery items
            foreach (var item in getShippingOptionRequest.Items)
            {
                //checking first for items that are pickup in store so they dont get charged as home delivery
                string itemAttributeXml = item.ShoppingCartItem.AttributesXml;
                List<ProductAttributeMapping> pam = (await _productAttributeParser.ParseProductAttributeMappingsAsync(itemAttributeXml)).ToList();
                if (await pam.Select(p => p).WhereAwait(async p => ((await _productAttributeService.GetProductAttributeByIdAsync(p.ProductAttributeId)).Name == "Pickup")).CountAsync() > 0)
                {
                    pickupInStoreList.Add(item);
                }
                // also checking product attribute (why not only check the product attribute?)
                else if (_productHomeDeliveryRepo.Table.Any(phd => phd.Product_Id == item.ShoppingCartItem.ProductId) ||
                         await pam.Select(p => p)
                            .WhereAwait(async p => ((await _productAttributeService.GetProductAttributeByIdAsync(p.ProductAttributeId)).Name == "Home Delivery"))
                            .AnyAsync())
                {
                    homeDeliveryList.Add(item);
                }
            }

            foreach (var item in pickupInStoreList)
            {
                getShippingOptionRequest.Items.Remove(item);
            }

            foreach (var item in homeDeliveryList)
            {
                getShippingOptionRequest.Items.Remove(item);
            }

            var homeDeliveryCharge = await _homeDeliveryCostService.GetHomeDeliveryCostAsync(homeDeliveryList);

            //if there are items to be shipped, use the base calculation service if items remain that will be shipped by ups
            if (getShippingOptionRequest.Items.Count > 0)
            {
                var oldProtocol = ServicePointManager.SecurityProtocol;
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
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
                if (homeDeliveryList.Count > 0)
                {
                    var zip = getShippingOptionRequest.ShippingAddress.ZipPostalCode;
                    await CheckZipcodeAsync(zip, response);
                    foreach (var shippingOption in response.ShippingOptions)
                    {
                        shippingOption.Name += " and Home Delivery";
                        shippingOption.Rate += homeDeliveryCharge;
                    }
                }
            }//else the cart contains only home delivery and/or pickup in store
            else if (homeDeliveryList.Count > 0)
            {
                var zip = getShippingOptionRequest.ShippingAddress.ZipPostalCode;
                await CheckZipcodeAsync(zip, response);
                response.ShippingOptions.Add(new ShippingOption { Name = "Home Delivery", Rate = homeDeliveryCharge });
            }//else the cart contains only pickup in store
            else
            {
                response.ShippingOptions.Add(new ShippingOption { Name = "Pickup in Store", Rate = decimal.Zero });
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
            int zipNum;
            if (int.TryParse(firstFive, out zipNum))
            {
                var isZipEligible = await _deliveryService.CheckZipcodeAsync(zipNum);

                if (isZipEligible)
                    return true;
                else
                    response.AddError("Home Delivery Shipping is not available for the given zip code.");
            }
            else
            {
                response.AddError("Invalid Zipcode");
            }
            return false;
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