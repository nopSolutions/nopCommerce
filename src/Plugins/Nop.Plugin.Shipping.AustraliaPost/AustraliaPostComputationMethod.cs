using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Routing;
using Newtonsoft.Json.Linq;
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
        private const int MIN_WEIGHT = 500; // 500 g
        private const int MAX_LENGTH = 1050; // 105 cm
        private const int MAX_DOMESTIC_WEIGHT = 22000; // 22 Kg
        private const int MAX_INTERNATIONAL_WEIGHT = 20000; // 20 Kg
        private const int MIN_GIRTH = 160; // 16 cm
        private const int MAX_GIRTH = 1400; // 140 cm
        private const int ONE_KILO = 1000; // 1 kg
        private const int ONE_CENTIMETER = 10; // 1 cm

        private const string GATEWAY_URL_INTERNACIONAL_ALLOWED_SERVICES = "https://digitalapi.auspost.com.au/postage/parcel/international/service.json";
        private const string GATEWAY_URL_DOMESTIC_ALLOWED_SERVICES = "https://digitalapi.auspost.com.au/postage/parcel/domestic/service.json";

        #endregion

        #region Fields

        private readonly ICurrencyService _currencyService;
        private readonly IMeasureService _measureService;
        private readonly IShippingService _shippingService;
        private readonly ISettingService _settingService;
        private readonly AustraliaPostSettings _australiaPostSettings;

        #endregion

        #region Ctor
        public AustraliaPostComputationMethod(ICurrencyService currencyService, 
            IMeasureService measureService, IShippingService shippingService, 
            ISettingService settingService, AustraliaPostSettings australiaPostSettings)
        {
            this._currencyService = currencyService;
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

        private int GetWeight(GetShippingOptionRequest getShippingOptionRequest)
        {
            var totalWeigth = _shippingService.GetTotalWeight(getShippingOptionRequest);
            int value = Convert.ToInt32(Math.Ceiling(this._measureService.ConvertFromPrimaryMeasureWeight(totalWeigth, this.GatewayMeasureWeight)));
            return (value < MIN_WEIGHT ? MIN_WEIGHT : value);
        }

        private IList<ShippingOption> RequestShippingOptions(string countryTwoLetterIsoCode, string fromPostcode, string toPostcode, decimal weight, int length, int width, int heigth, int totalPackages)
        {
            var shippingOptions = new List<ShippingOption>();
            var cultureInfo = new CultureInfo("en-AU");
            var sb = new StringBuilder();

            switch (countryTwoLetterIsoCode)
            {
                case "AU":
                    sb.AppendFormat(GATEWAY_URL_DOMESTIC_ALLOWED_SERVICES);
                    sb.AppendFormat("?from_postcode={0}&", fromPostcode);
                    sb.AppendFormat("to_postcode={0}&", toPostcode);
                    sb.AppendFormat("length={0}&", length);
                    sb.AppendFormat("width={0}&", width);
                    sb.AppendFormat("height={0}&", heigth);
                    break;
                default:
                    sb.AppendFormat(GATEWAY_URL_INTERNACIONAL_ALLOWED_SERVICES);
                    sb.AppendFormat("?country_code={0}&", countryTwoLetterIsoCode);
                    break;
            }

            sb.AppendFormat("weight={0}", weight.ToString(cultureInfo.NumberFormat));

            var request = WebRequest.Create(sb.ToString()) as HttpWebRequest;
            request.Headers.Add("AUTH-KEY", _australiaPostSettings.ApiKey);
            request.Method = "GET";
            Stream stream;

            try
            {
                var response = request.GetResponse();
                stream = response.GetResponseStream();
            }
            catch (WebException ex)
            {
                stream = ex.Response.GetResponseStream();
            }

            //parse json from response
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(json))
                {
                    var parsed = JObject.Parse(json);
                    JToken jToken;
                    try
                    {
                        jToken = parsed["services"]["service"];
                        if (jToken != null)
                        {
                            var options = (JArray)jToken;
                            foreach (var option in options)
                            {
                                var service = (JObject)option;
                                var shippingOption = service.ParseShippingOption(_currencyService);
                                if (shippingOption != null)
                                {
                                    shippingOption.Rate = shippingOption.Rate * totalPackages;
                                    shippingOptions.Add(shippingOption);
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //if the size or weight of the parcel exceeds the allowable, 
                        //and if for example the set(or not set) the wrong postal code, 
                        //the Australia Post errorMessage returns the error text.
                        //As a result, the client can not use the services of the service
                        jToken = parsed["error"]["errorMessage"];
                        if (jToken != null)
                        {
                            var error = (JValue)jToken;
                            throw new NopException(error.Value.ToString());
                        }
                        throw new Exception("Response is not valid.");
                    }
                }
                else
                {
                    throw new Exception("Response is not valid.");
                }
            }
            return shippingOptions;
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

            if (string.IsNullOrEmpty(getShippingOptionRequest.ZipPostalCodeFrom))
            {
                response.AddError("Shipping origin zip is not set");
                return response;
            }

            var country = getShippingOptionRequest.ShippingAddress.Country;
            if (country == null)
            {
                response.AddError("Shipping country is not specified");
                return response;
            }

            if (getShippingOptionRequest.ShippingAddress == null)
            {
                response.AddError("Shipping address is not set");
                return response;
            }

            if (string.IsNullOrEmpty(getShippingOptionRequest.ShippingAddress.ZipPostalCode))
            {
                response.AddError("Shipping zip (postal code) is not set");
                return response;
            }

            string zipPostalCodeFrom = getShippingOptionRequest.ZipPostalCodeFrom;
            string zipPostalCodeTo = getShippingOptionRequest.ShippingAddress.ZipPostalCode;
            int weight = GetWeight(getShippingOptionRequest);
            decimal lengthTmp, widthTmp, heightTmp;
            _shippingService.GetDimensions(getShippingOptionRequest.Items, out widthTmp, out lengthTmp, out heightTmp);
            int length = Math.Max(Convert.ToInt32(Math.Ceiling(this._measureService.ConvertFromPrimaryMeasureDimension(lengthTmp, this.GatewayMeasureDimension))), MIN_LENGTH);
            int width = Math.Max(Convert.ToInt32(Math.Ceiling(this._measureService.ConvertFromPrimaryMeasureDimension(widthTmp, this.GatewayMeasureDimension))), MIN_LENGTH);
            int height = Math.Max(Convert.ToInt32(Math.Ceiling(this._measureService.ConvertFromPrimaryMeasureDimension(heightTmp, this.GatewayMeasureDimension))), MIN_LENGTH);

            //estimate packaging
            int totalPackagesDims = 1;
            int totalPackagesWeights = 1;
            if (length > MAX_LENGTH || width > MAX_LENGTH || height > MAX_LENGTH)
            {
                totalPackagesDims = Convert.ToInt32(Math.Ceiling((decimal)Math.Max(Math.Max(length, width), height) / MAX_LENGTH));
            }

            int maxWeight = country.TwoLetterIsoCode.Equals("AU") ? MAX_DOMESTIC_WEIGHT : MAX_INTERNATIONAL_WEIGHT;
            if (weight > maxWeight)
            {
                totalPackagesWeights = Convert.ToInt32(Math.Ceiling((decimal)weight / (decimal)maxWeight));
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
            // Australia post takes the dimensions in centimeters and weight in kilograms, 
            // so dimensions should be converted and rounded up from millimeters to centimeters,
            // grams should be converted to kilograms and rounded to two decimal.
            length = length / ONE_CENTIMETER + (length % ONE_CENTIMETER > 0 ? 1 : 0);
            width = width / ONE_CENTIMETER + (width % ONE_CENTIMETER > 0 ? 1 : 0);
            height = height / ONE_CENTIMETER + (height % ONE_CENTIMETER > 0 ? 1 : 0);
            var kgWeight = Math.Round(weight / (decimal)ONE_KILO, 2);

            try
            {
                var shippingOptions = RequestShippingOptions(country.TwoLetterIsoCode,
                    zipPostalCodeFrom, zipPostalCodeTo, kgWeight, length, width, height, totalPackages);

                foreach (var shippingOption in shippingOptions)
                {
                    response.ShippingOptions.Add(shippingOption);
                }
            }
            catch (NopException ex)
            {
                response.AddError(ex.Message);
                return response;
            }
            catch (Exception)
            {
                response.AddError("Australia Post Service is currently unavailable, try again later");
                return response;
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
                AdditionalHandlingCharge = 0
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.ApiKey", "Australia Post API Key");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.ApiKey.Hint", "Specify Australia Post API Key.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.AdditionalHandlingCharge", "Additional handling charge");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.AdditionalHandlingCharge.Hint", "Enter additional handling fee to charge your customers.");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<AustraliaPostSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.ApiKey");
            this.DeletePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.ApiKey.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.AdditionalHandlingCharge");
            this.DeletePluginLocaleResource("Plugins.Shipping.AustraliaPost.Fields.AdditionalHandlingCharge.Hint");

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