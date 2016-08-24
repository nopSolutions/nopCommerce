using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.CanadaPost
{
    /// <summary>
    /// Canada post computation method
    /// </summary>
    public class CanadaPostComputationMethod : BasePlugin, IShippingRateComputationMethod
    {
        #region Fields

        private readonly CanadaPostSettings _canadaPostSettings;
        private readonly ICurrencyService _currencyService;
        private readonly ILogger _logger;
        private readonly IMeasureService _measureService;
        private readonly ISettingService _settingService;
        private readonly IShippingService _shippingService;

        #endregion

        #region Ctor

        public CanadaPostComputationMethod(CanadaPostSettings canadaPostSettings,
            ICurrencyService currencyService,
            ILogger logger,
            IMeasureService measureService,
            ISettingService settingService,
            IShippingService shippingService)
        {
            this._canadaPostSettings = canadaPostSettings;
            this._currencyService = currencyService;
            this._logger = logger;
            this._measureService = measureService;
            this._settingService = settingService;
            this._shippingService = shippingService;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a shipping rate computation method type
        /// </summary>
        public ShippingRateComputationMethodType ShippingRateComputationMethodType
        {
            get
            {
                return ShippingRateComputationMethodType.Realtime;
            }
        }

        /// <summary>
        /// Gets a shipment tracker
        /// </summary>
        public IShipmentTracker ShipmentTracker
        {
            get
            {
                return new CanadaPostShipmentTracker(_canadaPostSettings, _logger);
            }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Calculate parcel weight in kgs
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <param name="weight">Weight</param>
        private void GetWeight(GetShippingOptionRequest getShippingOptionRequest, out decimal weight)
        {
            var usedMeasureWeight = _measureService.GetMeasureWeightBySystemKeyword("kg");
            if (usedMeasureWeight == null)
                throw new NopException("CanadaPost shipping service. Could not load \"kg\" measure weight");

            weight = _shippingService.GetTotalWeight(getShippingOptionRequest);
            weight = _measureService.ConvertFromPrimaryMeasureWeight(weight, usedMeasureWeight);
        }

        /// <summary>
        /// Calculate parcel length, width, height in centimeters
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <param name="length">Length</param>
        /// <param name="width">Width</param>
        /// <param name="height">height</param>
        private void GetDimensions(GetShippingOptionRequest getShippingOptionRequest, out decimal length, out decimal width, out decimal height)
        {
            var usedMeasureDimension = _measureService.GetMeasureDimensionBySystemKeyword("meters");
            if (usedMeasureDimension == null)
                throw new NopException("CanadaPost shipping service. Could not load \"meter(s)\" measure dimension");

            _shippingService.GetDimensions(getShippingOptionRequest.Items, out width, out length, out height);

            //In the Canada Post API length is longest dimension, width is second longest dimension, height is shortest dimension
            var dimensions = new List<decimal> { length, width, height };
            dimensions.Sort();
            length = Math.Round(_measureService.ConvertFromPrimaryMeasureDimension(dimensions[2], usedMeasureDimension) * 100, 1);
            width = Math.Round(_measureService.ConvertFromPrimaryMeasureDimension(dimensions[1], usedMeasureDimension) * 100, 1);
            height = Math.Round(_measureService.ConvertFromPrimaryMeasureDimension(dimensions[0], usedMeasureDimension) * 100, 1);
        }

        /// <summary>
        /// Get shipping price in the primary store currency
        /// </summary>
        /// <param name="price">Price in CAD currency</param>
        /// <returns>Price amount</returns>
        private decimal PriceToPrimaryStoreCurrency(decimal price)
        {
            var cad = _currencyService.GetCurrencyByCode("CAD");
            if (cad == null)
                throw new Exception("CAD currency cannot be loaded");

            return _currencyService.ConvertToPrimaryStoreCurrency(price, cad);
        }
        #endregion

        #region Methods

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Represents a response of getting shipping rate options</returns>
        public GetShippingOptionResponse GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest == null)
                throw new ArgumentNullException("getShippingOptionRequest");

            if (getShippingOptionRequest.Items == null)
                return new GetShippingOptionResponse { Errors = new List<string> { "No shipment items" } };

            if (getShippingOptionRequest.ShippingAddress == null)
                return new GetShippingOptionResponse { Errors = new List<string> { "Shipping address is not set" } };

            if (getShippingOptionRequest.ShippingAddress.Country == null)
                return new GetShippingOptionResponse { Errors = new List<string> { "Shipping country is not set" } };

            if (string.IsNullOrEmpty(getShippingOptionRequest.ZipPostalCodeFrom))
                return new GetShippingOptionResponse { Errors = new List<string> { "Origin postal code is not set" } };

            //get available services
            string errors;
            var availableServices = CanadaPostHelper.GetServices(getShippingOptionRequest.ShippingAddress.Country.TwoLetterIsoCode,
                _canadaPostSettings.ApiKey, _canadaPostSettings.UseSandbox, out errors);
            if (availableServices == null)
                return new GetShippingOptionResponse { Errors = new List<string> { errors } };

            //create object for the get rates requests
            var result = new GetShippingOptionResponse();
            object destinationCountry;
            switch (getShippingOptionRequest.ShippingAddress.Country.TwoLetterIsoCode.ToLowerInvariant())
            {
                case "us":
                    destinationCountry = new mailingscenarioDestinationUnitedstates
                    {
                        zipcode = getShippingOptionRequest.ShippingAddress.ZipPostalCode
                    };
                    break;
                case "ca":
                    destinationCountry = new mailingscenarioDestinationDomestic
                    {
                        postalcode = getShippingOptionRequest.ShippingAddress.ZipPostalCode
                    };
                    break;
                default:
                    destinationCountry = new mailingscenarioDestinationInternational
                    {
                        countrycode = getShippingOptionRequest.ShippingAddress.Country.TwoLetterIsoCode
                    };
                    break;
            }

            var mailingScenario = new mailingscenario
            {
                customernumber = _canadaPostSettings.CustomerNumber,
                originpostalcode = getShippingOptionRequest.ZipPostalCodeFrom,
                destination = new mailingscenarioDestination
                {
                    Item = destinationCountry
                }
            };

            //get original parcel characteristics
            decimal originalLength;
            decimal originalWidth;
            decimal originalHeight;
            decimal originalWeight;
            GetWeight(getShippingOptionRequest, out originalWeight);
            GetDimensions(getShippingOptionRequest, out originalLength, out originalWidth, out originalHeight);

            //get rate for all available services
            var errorSummary = new StringBuilder();
            foreach (var service in availableServices.service)
            {
                var currentService = CanadaPostHelper.GetServiceDetails(_canadaPostSettings.ApiKey, service.link.href, service.link.mediatype, out errors);
                if (currentService != null)
                {
                    #region parcels count calculation

                    var totalParcels = 1;

                    //parcels count by weight
                    var maxWeight = currentService.restrictions != null
                        && currentService.restrictions.weightrestriction != null
                        && currentService.restrictions.weightrestriction.maxSpecified
                        ? currentService.restrictions.weightrestriction.max : int.MaxValue;
                    if (originalWeight * 1000 > maxWeight)
                    {
                        var parcelsOnWeight = Convert.ToInt32(Math.Ceiling(originalWeight * 1000 / maxWeight));
                        if (parcelsOnWeight > totalParcels)
                            totalParcels = parcelsOnWeight;
                    }

                    //parcels count by length
                    var maxLength = currentService.restrictions != null
                        && currentService.restrictions.dimensionalrestrictions != null
                        && currentService.restrictions.dimensionalrestrictions.length != null
                        && currentService.restrictions.dimensionalrestrictions.length.maxSpecified
                        ? currentService.restrictions.dimensionalrestrictions.length.max : int.MaxValue;
                    if (originalLength > maxLength)
                    {
                        var parcelsOnLength = Convert.ToInt32(Math.Ceiling(originalLength / maxLength));
                        if (parcelsOnLength > totalParcels)
                            totalParcels = parcelsOnLength;
                    }

                    //parcels count by width
                    var maxWidth = currentService.restrictions != null
                        && currentService.restrictions.dimensionalrestrictions != null
                        && currentService.restrictions.dimensionalrestrictions.width != null
                        && currentService.restrictions.dimensionalrestrictions.width.maxSpecified
                        ? currentService.restrictions.dimensionalrestrictions.width.max : int.MaxValue;
                    if (originalWidth > maxWidth)
                    {
                        var parcelsOnWidth = Convert.ToInt32(Math.Ceiling(originalWidth / maxWidth));
                        if (parcelsOnWidth > totalParcels)
                            totalParcels = parcelsOnWidth;
                    }

                    //parcels count by height
                    var maxHeight = currentService.restrictions != null
                        && currentService.restrictions.dimensionalrestrictions != null
                        && currentService.restrictions.dimensionalrestrictions.height != null
                        && currentService.restrictions.dimensionalrestrictions.height.maxSpecified
                        ? currentService.restrictions.dimensionalrestrictions.height.max : int.MaxValue;
                    if (originalHeight > maxHeight)
                    {
                        var parcelsOnHeight = Convert.ToInt32(Math.Ceiling(originalHeight / maxHeight));
                        if (parcelsOnHeight > totalParcels)
                            totalParcels = parcelsOnHeight;
                    }

                    //parcel count by girth
                    var lengthPlusGirthMax = currentService.restrictions != null
                        && currentService.restrictions.dimensionalrestrictions != null
                        && currentService.restrictions.dimensionalrestrictions.lengthplusgirthmaxSpecified
                        ? currentService.restrictions.dimensionalrestrictions.lengthplusgirthmax : int.MaxValue;
                    var lengthPlusGirth = 2 * (originalWidth + originalHeight) + originalLength;
                    if (lengthPlusGirth > lengthPlusGirthMax)
                    {
                        var parcelsOnHeight = Convert.ToInt32(Math.Ceiling(lengthPlusGirth / lengthPlusGirthMax));
                        if (parcelsOnHeight > totalParcels)
                            totalParcels = parcelsOnHeight;
                    }

                    //parcel count by sum of length, width and height
                    var lengthWidthHeightMax = currentService.restrictions != null
                        && currentService.restrictions.dimensionalrestrictions != null
                        && currentService.restrictions.dimensionalrestrictions.lengthheightwidthsummaxSpecified
                        ? currentService.restrictions.dimensionalrestrictions.lengthheightwidthsummax : int.MaxValue;
                    var lengthWidthHeight = originalLength + originalWidth + originalHeight;
                    if (lengthWidthHeight > lengthWidthHeightMax)
                    {
                        var parcelsOnHeight = Convert.ToInt32(Math.Ceiling(lengthWidthHeight / lengthWidthHeightMax));
                        if (parcelsOnHeight > totalParcels)
                            totalParcels = parcelsOnHeight;
                    }

                    #endregion

                    //set parcel characteristics
                    mailingScenario.services = new[] { currentService.servicecode };
                    mailingScenario.parcelcharacteristics = new mailingscenarioParcelcharacteristics
                    {
                        weight = Math.Round(originalWeight / totalParcels, 3),
                        dimensions = new mailingscenarioParcelcharacteristicsDimensions
                        {
                            length = Math.Round(originalLength / totalParcels, 1),
                            width = Math.Round(originalWidth / totalParcels, 1),
                            height = Math.Round(originalHeight / totalParcels, 1)
                        }
                    };

                    //get rate
                    var priceQuotes = CanadaPostHelper.GetShippingRates(mailingScenario, _canadaPostSettings.ApiKey, _canadaPostSettings.UseSandbox, out errors);
                    if (priceQuotes != null)
                        foreach (var option in priceQuotes.pricequote)
                        {
                            result.ShippingOptions.Add(new ShippingOption
                            {
                                Name = option.servicename,
                                Rate = PriceToPrimaryStoreCurrency(option.pricedetails.due * totalParcels),
                                Description = string.Format("Delivery {0}into {1} parcels", 
                                    option.servicestandard != null && !string.IsNullOrEmpty(option.servicestandard.expectedtransittime) 
                                    ? string.Format("in {0} days ", option.servicestandard.expectedtransittime) : string.Empty, totalParcels),
                            });
                        }
                    else
                        errorSummary.AppendLine(errors);
                }
                else
                    errorSummary.AppendLine(errors);
            }

            //write errors
            var errorString = errorSummary.ToString();
            if (!string.IsNullOrEmpty(errorString))
                _logger.Error(errorString);
            if (!result.ShippingOptions.Any())
                result.AddError(errorString);

            return result;

        }

        /// <summary>
        /// Gets fixed shipping rate (if shipping rate computation method allows it and the rate can be calculated before checkout).
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Fixed shipping rate; or null in case there's no fixed shipping rate</returns>
        public decimal? GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            return null;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "ShippingCanadaPost";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Shipping.CanadaPost.Controllers" }, { "area", null } };
        }
        
        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new CanadaPostSettings
            {
                 UseSandbox = true
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.Api", "API key");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.Api.Hint", "Specify Canada Post API key.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.CustomerNumber", "Customer number");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.CustomerNumber.Hint", "Specify customer number.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.UseSandbox", "Use Sandbox");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.UseSandbox.Hint", "Check to enable Sandbox (testing environment).");

            base.Install();
        }
        
        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<CanadaPostSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.Api");
            this.DeletePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.Api.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.CustomerNumber");
            this.DeletePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.CustomerNumber.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.UseSandbox");
            this.DeletePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.UseSandbox.Hint");

            base.Uninstall();
        }

        #endregion
    }
}