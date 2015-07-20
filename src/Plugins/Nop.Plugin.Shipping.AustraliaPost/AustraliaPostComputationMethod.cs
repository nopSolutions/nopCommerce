using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.AustraliaPost
{
    /// <summary>
    /// Australia post computation method
    /// </summary>
    public class AustraliaPostComputationMethod : BasePlugin, IShippingRateComputationMethod
    {
        #region Constants

        private const int MIN_LENGTH = 50; // 5 cm
        private const int MIN_WEIGHT = 1; // 1 g
        private const int MAX_LENGTH = 1050; // 105 cm
        private const int MAX_WEIGHT = 20000; // 20 Kg
        private const int MIN_GIRTH = 160; // 16 cm
        private const int MAX_GIRTH = 1050; // 105 cm

        #endregion

        #region Fields

        private readonly IMeasureService _measureService;
        private readonly IShippingService _shippingService;
        private readonly ISettingService _settingService;
        private readonly AustraliaPostSettings _australiaPostSettings;

        #endregion

        #region Ctor
        public AustraliaPostComputationMethod(IMeasureService measureService,
            IShippingService shippingService, ISettingService settingService,
            AustraliaPostSettings australiaPostSettings)
        {
            this._measureService = measureService;
            this._shippingService = shippingService;
            this._settingService = settingService;
            this._australiaPostSettings = australiaPostSettings;
        }
        #endregion

        #region Utilities

        private MeasureWeight GatewayMeasureWeight
        {
            get
            {
                return this._measureService.GetMeasureWeightBySystemKeyword("grams");
            }
        }

        private MeasureDimension GatewayMeasureDimension
        {
            get
            {
                return this._measureService.GetMeasureDimensionBySystemKeyword("millimetres");
            }
        }

        private string GetGatewayUrl()
        {
            return _australiaPostSettings.GatewayUrl;
        }

        private int GetWeight(GetShippingOptionRequest getShippingOptionRequest)
        {
            var totalWeigth = _shippingService.GetTotalWeight(getShippingOptionRequest);

            int value = Convert.ToInt32(Math.Ceiling(this._measureService.ConvertFromPrimaryMeasureWeight(totalWeigth, this.GatewayMeasureWeight)));
            return (value < MIN_WEIGHT ? MIN_WEIGHT : value);
        }
        
        private ShippingOption RequestShippingOption(string zipPostalCodeFrom,
            string zipPostalCodeTo, string countryCode, string serviceType,
            int weight, int length, int width, int height, int quantity)
        {
            var shippingOption = new ShippingOption();
            var sb = new StringBuilder();

            sb.AppendFormat(GetGatewayUrl());
            sb.AppendFormat("?Pickup_Postcode={0}&", zipPostalCodeFrom);
            sb.AppendFormat("Destination_Postcode={0}&", zipPostalCodeTo);
            sb.AppendFormat("Country={0}&", countryCode);
            sb.AppendFormat("Service_Type={0}&", serviceType);
            sb.AppendFormat("Weight={0}&", weight);
            sb.AppendFormat("Length={0}&", length);
            sb.AppendFormat("Width={0}&", width);
            sb.AppendFormat("Height={0}&", height);
            sb.AppendFormat("Quantity={0}", quantity);

            var request = WebRequest.Create(sb.ToString()) as HttpWebRequest;
            request.Method = "GET";

            WebResponse response = request.GetResponse();
            string rspContent;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                rspContent = reader.ReadToEnd();
            }

            string[] tmp = rspContent.Split(new [] { '\n' }, 3);
            if (tmp.Length != 3)
            {
                throw new NopException("Response is not valid.");
            }

            var rspParams = new NameValueCollection();
            foreach (string s in tmp)
            {
                string[] tmp2 = s.Split(new [] { '=' });
                if (tmp2.Length != 2)
                {
                    throw new NopException("Response is not valid.");
                }
                rspParams.Add(tmp2[0].Trim(), tmp2[1].Trim());
            }


            string errMsg = rspParams["err_msg"];
            if (!errMsg.ToUpperInvariant().StartsWith("OK"))
            {
                throw new NopException(errMsg);
            }

            var serviceName = GetServiceNameByType(serviceType);
            if (serviceName != null && !serviceName.StartsWith("Australia Post.", StringComparison.InvariantCultureIgnoreCase))
                serviceName = string.Format("Australia Post. {0}", serviceName);
            shippingOption.Name = serviceName;
            if (!_australiaPostSettings.HideDeliveryInformation)
            {
                shippingOption.Description = String.Format("{0} Days", rspParams["days"]);
            }
            shippingOption.Rate = Decimal.Parse(rspParams["charge"]);

            return shippingOption;
        }
        
        private string GetServiceNameByType(string type)
        {
            if (String.IsNullOrEmpty(type))
                return type;

            string serviceName;
            switch (type)
            {
                case "Standard":
                    serviceName = "Regular Parcels";
                    break;
                case "Express":
                    serviceName = "Express Parcels";
                    break;
                case "EXP_PLT":
                    serviceName = "Express Parcels Platinum";
                    break;
                case "Air":
                    serviceName = "Air Mail";
                    break;
                case "Sea":
                    serviceName = "Sea Mail";
                    break;
                case "ECI_D":
                    serviceName = "Express Courier International Document";
                    break;
                case "ECI_M":
                    serviceName = "Express Courier International Merchandise";
                    break;
                case "EPI":
                    serviceName = "Express Post International";
                    break;
                default:
                    //not found. return service type
                    serviceName = type;
                    break;
            }
            return serviceName;
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

            string zipPostalCodeFrom = getShippingOptionRequest.ZipPostalCodeFrom;
            string zipPostalCodeTo = getShippingOptionRequest.ShippingAddress.ZipPostalCode;
            int weight = GetWeight(getShippingOptionRequest);


            decimal lengthTmp, widthTmp, heightTmp;
            _shippingService.GetDimensions(getShippingOptionRequest.Items, out widthTmp, out lengthTmp, out heightTmp);
            int length = Math.Min(Convert.ToInt32(Math.Ceiling(this._measureService.ConvertFromPrimaryMeasureDimension(lengthTmp, this.GatewayMeasureDimension))), MIN_LENGTH);
            int width = Math.Min(Convert.ToInt32(Math.Ceiling(this._measureService.ConvertFromPrimaryMeasureDimension(widthTmp, this.GatewayMeasureDimension))), MIN_LENGTH);
            int height = Math.Min(Convert.ToInt32(Math.Ceiling(this._measureService.ConvertFromPrimaryMeasureDimension(heightTmp, this.GatewayMeasureDimension))), MIN_LENGTH);
            
            //estimate packaging
            int totalPackagesDims = 1;
            int totalPackagesWeights = 1;
            if (length > MAX_LENGTH || width > MAX_LENGTH || height > MAX_LENGTH)
            {
                totalPackagesDims = Convert.ToInt32(Math.Ceiling((decimal)Math.Max(Math.Max(length, width), height) / MAX_LENGTH));
            }
            if (weight > MAX_WEIGHT)
            {
                totalPackagesWeights = Convert.ToInt32(Math.Ceiling((decimal)weight / (decimal)MAX_WEIGHT));
            }
            var totalPackages = totalPackagesDims > totalPackagesWeights ? totalPackagesDims : totalPackagesWeights;
            if (totalPackages == 0)
                totalPackages = 1;
            if (totalPackages > 1)
            {
                //recalculate dims, weight
                weight = weight / totalPackages;
                height = height / totalPackages;
                width = width / totalPackages;
                length = length / totalPackages;
                if (weight < MIN_WEIGHT)
                    weight = MIN_WEIGHT;
                if (height < MIN_LENGTH)
                    height = MIN_LENGTH;
                if (width < MIN_LENGTH)
                    width = MIN_LENGTH;
                if (length < MIN_LENGTH)
                    length = MIN_LENGTH;
            }

            int girth = height + height + width + width;
            if (girth < MIN_GIRTH)
            {
                height = MIN_LENGTH;
                width = MIN_LENGTH;
            }
            if (girth > MAX_GIRTH)
            {
                height = MAX_LENGTH / 4;
                width = MAX_LENGTH / 4;
            }
                var country = getShippingOptionRequest.ShippingAddress.Country;
                if (country == null)
                {
                    response.AddError("Shipping country is not specified");
                    return response;
                }
                var serviceTypes = new List<string>();
                switch (country.ThreeLetterIsoCode)
                {
                    case "AUS":
                        //domestic services
                        serviceTypes.Add("Standard");
                        serviceTypes.Add("Express");
                        //discontinued
                        serviceTypes.Add("EXP_PLT");
                        break;
                    default:
                        //international services
                        serviceTypes.Add("Air");
                        serviceTypes.Add("Sea");
                        serviceTypes.Add("ECI_D");
                        serviceTypes.Add("ECI_M");
                        serviceTypes.Add("EPI");
                        break;
                }
            foreach (var serviceType in serviceTypes)
            {
                try
                {
                    response.ShippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, 
                        zipPostalCodeTo,
                        country.TwoLetterIsoCode,
                        serviceType,
                        weight, 
                        length, 
                        width, 
                        height, 
                        totalPackages));
                }
                catch (Exception ex)
                {
                    //If this plugin doesn't allow some method (e.g. sea freight) to the selected country (e.g. New Zealand),
                    //the AusPost API returns an err_msg.
                    //This throws an exception which results in no shipping options being offered at all.
                    //The result is that customers from NZ can't buy anything.
                    //That why we commented the code below (do not exit)
                    //and process each "RequestShippingOption" separately (in try-catch)
                    //response.AddError(ex.Message);
                }
            }
            foreach (var shippingOption in response.ShippingOptions)
            {
                shippingOption.Rate += _australiaPostSettings.AdditionalHandlingCharge;
            }
            return response;
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
            controllerName = "ShippingAustraliaPost";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Shipping.AustraliaPost.Controllers" }, { "area", null } };
        }
        
        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new AustraliaPostSettings
            {
                GatewayUrl = "http://drc.edeliver.com.au/ratecalc.asp",
                AdditionalHandlingCharge = 0,
                HideDeliveryInformation = false
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.GatewayUrl", "Gateway URL");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.GatewayUrl.Hint", "Specify gateway URL");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.AdditionalHandlingCharge", "Additional handling charge");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.AdditionalHandlingCharge.Hint", "Enter additional handling fee to charge your customers.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.HideDeliveryInformation", "Hide delivery information");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.HideDeliveryInformation.Hint", "Check to hide delivery information as description of returned shipping methods.");
            
            base.Install();
        }

        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<AustraliaPostSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.GatewayUrl");
            this.DeletePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.GatewayUrl.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.AdditionalHandlingCharge");
            this.DeletePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.AdditionalHandlingCharge.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.HideDeliveryInformation");
            this.DeletePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.HideDeliveryInformation.Hint");
            
            base.Uninstall();
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
            get { return null; }
        }

        #endregion
    }
}