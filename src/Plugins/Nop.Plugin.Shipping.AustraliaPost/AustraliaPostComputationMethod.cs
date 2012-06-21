using System;
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
            var totalWeigth = _shippingService.GetShoppingCartTotalWeight(getShippingOptionRequest.Items);

            int value = Convert.ToInt32(Math.Ceiling(this._measureService.ConvertFromPrimaryMeasureWeight(totalWeigth, this.GatewayMeasureWeight)));
            return (value < MIN_WEIGHT ? MIN_WEIGHT : value);
        }

        private int GetLength(GetShippingOptionRequest getShippingOptionRequest)
        {
            int value = Convert.ToInt32(Math.Ceiling(this._measureService.ConvertFromPrimaryMeasureDimension(getShippingOptionRequest.GetTotalLength(), this.GatewayMeasureDimension)));
            return (value < MIN_LENGTH ? MIN_LENGTH : value);
        }

        private int GetWidth(GetShippingOptionRequest getShippingOptionRequest)
        {
            int value = Convert.ToInt32(Math.Ceiling(this._measureService.ConvertFromPrimaryMeasureDimension(getShippingOptionRequest.GetTotalWidth(), this.GatewayMeasureDimension)));
            return (value < MIN_LENGTH ? MIN_LENGTH : value);
        }

        private int GetHeight(GetShippingOptionRequest getShippingOptionRequest)
        {
            int value = Convert.ToInt32(Math.Ceiling(this._measureService.ConvertFromPrimaryMeasureDimension(getShippingOptionRequest.GetTotalHeight(), this.GatewayMeasureDimension)));
            return (value < MIN_LENGTH ? MIN_LENGTH : value);
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

            HttpWebRequest request = WebRequest.Create(sb.ToString()) as HttpWebRequest;
            request.Method = "GET";
            //request.ContentType = "application/x-www-form-urlencoded";
            //byte[] reqContent = Encoding.ASCII.GetBytes(sb.ToString());
            //request.ContentLength = reqContent.Length;
            //using (Stream newStream = request.GetRequestStream())
            //{
            //    newStream.Write(reqContent, 0, reqContent.Length);
            //}

            WebResponse response = request.GetResponse();
            string rspContent;
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                rspContent = reader.ReadToEnd();
            }

            string[] tmp = rspContent.Split(new char[] { '\n' }, 3);
            if (tmp.Length != 3)
            {
                throw new NopException("Response is not valid.");
            }

            var rspParams = new NameValueCollection();
            foreach (string s in tmp)
            {
                string[] tmp2 = s.Split(new char[] { '=' });
                if (tmp2.Length != 2)
                {
                    throw new NopException("Response is not valid.");
                }
                rspParams.Add(tmp2[0].Trim(), tmp2[1].Trim());
            }


            string err_msg = rspParams["err_msg"];
            if (!err_msg.ToUpperInvariant().StartsWith("OK"))
            {
                throw new NopException(err_msg);
            }

            var serviceName = GetServiceNameByType(serviceType);
            if (serviceName != null && !serviceName.StartsWith("Australia Post.", StringComparison.InvariantCultureIgnoreCase))
                serviceName = string.Format("Australia Post. {0}", serviceName);
            shippingOption.Name = serviceName;
            shippingOption.Description = String.Format("{0} Days", rspParams["days"]);
            shippingOption.Rate = Decimal.Parse(rspParams["charge"]);

            return shippingOption;
        }
        
        private string GetServiceNameByType(string type)
        {
            if (String.IsNullOrEmpty(type))
                return type;

            var serviceName = "";
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

            string zipPostalCodeFrom = _australiaPostSettings.ShippedFromZipPostalCode;
            string zipPostalCodeTo = getShippingOptionRequest.ShippingAddress.ZipPostalCode;
            int weight = GetWeight(getShippingOptionRequest);
            int length = GetLength(getShippingOptionRequest);
            int width = GetWidth(getShippingOptionRequest);
            int height = GetHeight(getShippingOptionRequest);

            var country = getShippingOptionRequest.ShippingAddress.Country;

            //estimate packaging
            int totalPackages = 1;
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
            totalPackages = totalPackagesDims > totalPackagesWeights ? totalPackagesDims : totalPackagesWeights;
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
            try
            {
                switch (country.ThreeLetterIsoCode)
                {
                    case "AUS":
                        //domestic services
                        response.ShippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "Standard", weight, length, width, height, totalPackages));
                        response.ShippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "Express", weight, length, width, height, totalPackages));
                        response.ShippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "EXP_PLT", weight, length, width, height, totalPackages));
                        break;
                    default:
                        //international services
                        response.ShippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "Air", weight, length, width, height, totalPackages));
                        response.ShippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "Sea", weight, length, width, height, totalPackages));
                        response.ShippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "ECI_D", weight, length, width, height, totalPackages));
                        response.ShippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "ECI_M", weight, length, width, height, totalPackages));
                        response.ShippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "EPI", weight, length, width, height, totalPackages));
                        break;
                }

                foreach (var shippingOption in response.ShippingOptions)
                {
                    shippingOption.Rate += _australiaPostSettings.AdditionalHandlingCharge;
                }
            }
            catch (Exception ex)
            {
                response.AddError(ex.Message);
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
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Shipping.AustraliaPost.Controllers" }, { "area", null } };
        }
        
        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new AustraliaPostSettings()
            {
                GatewayUrl = "http://drc.edeliver.com.au/ratecalc.asp",
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.GatewayUrl", "Gateway URL");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.GatewayUrl.Hint", "Specify gateway URL");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.AdditionalHandlingCharge", "Additional handling charge.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.AdditionalHandlingCharge.Hint", "Enter additional handling fee to charge your customers.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.ShippedFromZipPostalCode", "Shipped from zip");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.ShippedFromZipPostalCode.Hint", "Specify origin zip code.");
            
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
            this.DeletePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.ShippedFromZipPostalCode");
            this.DeletePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.ShippedFromZipPostalCode.Hint");
            
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