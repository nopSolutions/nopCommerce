//------------------------------------------------------------------------------
// Contributor(s): mb 10/20/2010. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Routing;
using System.Xml;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Plugin.Shipping.UPS.Domain;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.UPS
{
    /// <summary>
    /// UPS computation method
    /// </summary>
    public class UPSComputationMethod : BasePlugin, IShippingRateComputationMethod
    {
        #region Constants

        private const int MAXPACKAGEWEIGHT = 150;
        private const string MEASUREWEIGHTSYSTEMKEYWORD = "lb";
        private const string MEASUREDIMENSIONSYSTEMKEYWORD = "inches";


        #endregion

        #region Fields

        private readonly IMeasureService _measureService;
        private readonly IShippingService _shippingService;
        private readonly ISettingService _settingService;
        private readonly UPSSettings _upsSettings;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ILogger _logger;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor
        public UPSComputationMethod(IMeasureService measureService,
            IShippingService shippingService, ISettingService settingService,
            UPSSettings upsSettings, ICountryService countryService,
            ICurrencyService currencyService, CurrencySettings currencySettings,
            IOrderTotalCalculationService orderTotalCalculationService, ILogger logger,
            ILocalizationService localizationService)
        {
            this._measureService = measureService;
            this._shippingService = shippingService;
            this._settingService = settingService;
            this._upsSettings = upsSettings;
            this._countryService = countryService;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._logger = logger;
            this._localizationService = localizationService;
        }
        #endregion

        #region Utilities

        private string CreateRequest(string accessKey, string username, string password,
            GetShippingOptionRequest getShippingOptionRequest, UPSCustomerClassification customerClassification,
            UPSPickupType pickupType, UPSPackagingType packagingType)
        {
            var usedMeasureWeight = _measureService.GetMeasureWeightBySystemKeyword(MEASUREWEIGHTSYSTEMKEYWORD);
            if (usedMeasureWeight == null)
                throw new NopException(string.Format("UPS shipping service. Could not load \"{0}\" measure weight", MEASUREWEIGHTSYSTEMKEYWORD));

            var usedMeasureDimension = _measureService.GetMeasureDimensionBySystemKeyword(MEASUREDIMENSIONSYSTEMKEYWORD);
            if (usedMeasureDimension == null)
                throw new NopException(string.Format("UPS shipping service. Could not load \"{0}\" measure dimension", MEASUREDIMENSIONSYSTEMKEYWORD));

            int length = Convert.ToInt32(Math.Ceiling(_measureService.ConvertFromPrimaryMeasureDimension(getShippingOptionRequest.GetTotalLength(), usedMeasureDimension)));
            int height = Convert.ToInt32(Math.Ceiling(_measureService.ConvertFromPrimaryMeasureDimension(getShippingOptionRequest.GetTotalHeight(), usedMeasureDimension)));
            int width = Convert.ToInt32(Math.Ceiling(_measureService.ConvertFromPrimaryMeasureDimension(getShippingOptionRequest.GetTotalWidth(), usedMeasureDimension)));
            int weight = Convert.ToInt32(Math.Ceiling(_measureService.ConvertFromPrimaryMeasureWeight(_shippingService.GetShoppingCartTotalWeight(getShippingOptionRequest.Items), usedMeasureWeight)));
            if (length < 1)
                length = 1;
            if (height < 1)
                height = 1;
            if (width < 1)
                width = 1;
            if (weight < 1)
                weight = 1;

            string zipPostalCodeFrom = getShippingOptionRequest.ZipPostalCodeFrom;
            string zipPostalCodeTo = getShippingOptionRequest.ShippingAddress.ZipPostalCode;
            string countryCodeFrom = getShippingOptionRequest.CountryFrom.TwoLetterIsoCode;
            string countryCodeTo = getShippingOptionRequest.ShippingAddress.Country.TwoLetterIsoCode;


            decimal orderSubTotalDiscountAmount = decimal.Zero;
            Discount orderSubTotalAppliedDiscount = null;
            decimal subTotalWithoutDiscountBase = decimal.Zero;
            decimal subTotalWithDiscountBase = decimal.Zero;
            _orderTotalCalculationService.GetShoppingCartSubTotal(getShippingOptionRequest.Items,
                out orderSubTotalDiscountAmount, out orderSubTotalAppliedDiscount,
                out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);

            var sb = new StringBuilder();
            sb.Append("<?xml version='1.0'?>");
            sb.Append("<AccessRequest xml:lang='en-US'>");
            sb.AppendFormat("<AccessLicenseNumber>{0}</AccessLicenseNumber>", accessKey);
            sb.AppendFormat("<UserId>{0}</UserId>", username);
            sb.AppendFormat("<Password>{0}</Password>", password);
            sb.Append("</AccessRequest>");
            sb.Append("<?xml version='1.0'?>");
            sb.Append("<RatingServiceSelectionRequest xml:lang='en-US'>");
            sb.Append("<Request>");
            sb.Append("<TransactionReference>");
            sb.Append("<CustomerContext>Bare Bones Rate Request</CustomerContext>");
            sb.Append("<XpciVersion>1.0001</XpciVersion>");
            sb.Append("</TransactionReference>");
            sb.Append("<RequestAction>Rate</RequestAction>");
            sb.Append("<RequestOption>Shop</RequestOption>");
            sb.Append("</Request>");
            if (String.Equals(countryCodeFrom, "US", StringComparison.InvariantCultureIgnoreCase) == true)
            {
                sb.Append("<PickupType>");
                sb.AppendFormat("<Code>{0}</Code>", GetPickupTypeCode(pickupType));
                sb.Append("</PickupType>");
                sb.Append("<CustomerClassification>");
                sb.AppendFormat("<Code>{0}</Code>", GetCustomerClassificationCode(customerClassification));
                sb.Append("</CustomerClassification>");
            }
            sb.Append("<Shipment>");
            sb.Append("<Shipper>");
            sb.Append("<Address>");
            sb.AppendFormat("<PostalCode>{0}</PostalCode>", zipPostalCodeFrom);
            sb.AppendFormat("<CountryCode>{0}</CountryCode>", countryCodeFrom);
            sb.Append("</Address>");
            sb.Append("</Shipper>");
            sb.Append("<ShipTo>");
            sb.Append("<Address>");
            sb.Append("<ResidentialAddressIndicator/>");
            sb.AppendFormat("<PostalCode>{0}</PostalCode>", zipPostalCodeTo);
            sb.AppendFormat("<CountryCode>{0}</CountryCode>", countryCodeTo);
            sb.Append("</Address>");
            sb.Append("</ShipTo>");
            sb.Append("<ShipFrom>");
            sb.Append("<Address>");
            sb.AppendFormat("<PostalCode>{0}</PostalCode>", zipPostalCodeFrom);
            sb.AppendFormat("<CountryCode>{0}</CountryCode>", countryCodeFrom);
            sb.Append("</Address>");
            sb.Append("</ShipFrom>");
            sb.Append("<Service>");
            sb.Append("<Code>03</Code>");
            sb.Append("</Service>");


            if ((!IsPackageTooHeavy(weight)) && (!IsPackageTooLarge(length, height, width)))
            {
                sb.Append("<Package>");
                sb.Append("<PackagingType>");
                sb.AppendFormat("<Code>{0}</Code>", GetPackagingTypeCode(packagingType));
                sb.Append("</PackagingType>");
                sb.Append("<Dimensions>");
                sb.AppendFormat("<Length>{0}</Length>", length);
                sb.AppendFormat("<Width>{0}</Width>", width);
                sb.AppendFormat("<Height>{0}</Height>", height);
                sb.Append("</Dimensions>");
                sb.Append("<PackageWeight>");
                sb.AppendFormat("<Weight>{0}</Weight>", weight);
                sb.Append("</PackageWeight>");

                if (_upsSettings.InsurePackage)
                {
                    //The maximum declared amount per package: 50000 USD.
                    int packageInsurancePrice = Convert.ToInt32(subTotalWithDiscountBase);
                    sb.Append("<PackageServiceOptions>");
                    sb.Append("<InsuredValue>");
                    sb.AppendFormat("<CurrencyCode>{0}</CurrencyCode>", _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode);
                    sb.AppendFormat("<MonetaryValue>{0}</MonetaryValue>", packageInsurancePrice);
                    sb.Append("</InsuredValue>");
                    sb.Append("</PackageServiceOptions>");
                }
                sb.Append("</Package>");
            }
            else
            {
                int totalPackages = 1;
                int totalPackagesDims = 1;
                int totalPackagesWeights = 1;
                if (IsPackageTooHeavy(weight))
                {
                    totalPackagesWeights = Convert.ToInt32(Math.Ceiling((decimal)weight / (decimal)MAXPACKAGEWEIGHT));
                }
                if (IsPackageTooLarge(length, height, width))
                {
                    totalPackagesDims = Convert.ToInt32(Math.Ceiling((decimal)TotalPackageSize(length, height, width) / (decimal)108));
                }
                totalPackages = totalPackagesDims > totalPackagesWeights ? totalPackagesDims : totalPackagesWeights;
                if (totalPackages == 0)
                    totalPackages = 1;

                int weight2 = weight / totalPackages;
                int height2 = height / totalPackages;
                int width2 = width / totalPackages;
                int length2 = length / totalPackages;
                if (weight2 < 1)
                    weight2 = 1;
                if (height2 < 1)
                    height2 = 1;
                if (width2 < 1)
                    width2 = 1;
                if (length2 < 1)
                    length2 = 1;

                //The maximum declared amount per package: 50000 USD.
                int packageInsurancePrice = Convert.ToInt32(subTotalWithDiscountBase / totalPackages);

                for (int i = 0; i < totalPackages; i++)
                {
                    sb.Append("<Package>");
                    sb.Append("<PackagingType>");
                    sb.AppendFormat("<Code>{0}</Code>", GetPackagingTypeCode(packagingType));
                    sb.Append("</PackagingType>");
                    sb.Append("<Dimensions>");
                    sb.AppendFormat("<Length>{0}</Length>", length2);
                    sb.AppendFormat("<Width>{0}</Width>", width2);
                    sb.AppendFormat("<Height>{0}</Height>", height2);
                    sb.Append("</Dimensions>");
                    sb.Append("<PackageWeight>");
                    sb.AppendFormat("<Weight>{0}</Weight>", weight2);
                    sb.Append("</PackageWeight>");

                    if (_upsSettings.InsurePackage)
                    {
                        sb.Append("<PackageServiceOptions>");
                        sb.Append("<InsuredValue>");
                        sb.AppendFormat("<CurrencyCode>{0}</CurrencyCode>", _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode);
                        sb.AppendFormat("<MonetaryValue>{0}</MonetaryValue>", packageInsurancePrice);
                        sb.Append("</InsuredValue>");
                        sb.Append("</PackageServiceOptions>");
                    }

                    sb.Append("</Package>");
                }
            }


            sb.Append("</Shipment>");
            sb.Append("</RatingServiceSelectionRequest>");
            string requestString = sb.ToString();
            return requestString;
        }

        private string DoRequest(string url, string requestString)
        {
            byte[] bytes = new ASCIIEncoding().GetBytes(requestString);
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;
            var requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            var response = request.GetResponse();
            string responseXML = string.Empty;
            using (var reader = new StreamReader(response.GetResponseStream()))
                responseXML = reader.ReadToEnd();

            return responseXML;
        }

        private string GetCustomerClassificationCode(UPSCustomerClassification customerClassification)
        {
            switch (customerClassification)
            {
                case UPSCustomerClassification.Wholesale:
                    return "01";
                case UPSCustomerClassification.Occasional:
                    return "03";
                case UPSCustomerClassification.Retail:
                    return "04";
                default:
                    throw new NopException("Unknown UPS customer classification code");
            }
        }

        private string GetPackagingTypeCode(UPSPackagingType packagingType)
        {
            switch (packagingType)
            {
                case UPSPackagingType.Letter:
                    return "01";
                case UPSPackagingType.CustomerSuppliedPackage:
                    return "02";
                case UPSPackagingType.Tube:
                    return "03";
                case UPSPackagingType.PAK:
                    return "04";
                case UPSPackagingType.ExpressBox:
                    return "21";
                case UPSPackagingType._10KgBox:
                    return "25";
                case UPSPackagingType._25KgBox:
                    return "24";
                default:
                    throw new NopException("Unknown UPS packaging type code");
            }
        }

        private string GetPickupTypeCode(UPSPickupType pickupType)
        {
            switch (pickupType)
            {
                case UPSPickupType.DailyPickup:
                    return "01";
                case UPSPickupType.CustomerCounter:
                    return "03";
                case UPSPickupType.OneTimePickup:
                    return "06";
                case UPSPickupType.OnCallAir:
                    return "07";
                case UPSPickupType.SuggestedRetailRates:
                    return "11";
                case UPSPickupType.LetterCenter:
                    return "19";
                case UPSPickupType.AirServiceCenter:
                    return "20";
                default:
                    throw new NopException("Unknown UPS pickup type code");
            }
        }

        private string GetServiceName(string serviceId)
        {
            switch (serviceId)
            {
                case "01":
                    return "UPS Next Day Air";
                case "02":
                    return "UPS 2nd Day Air";
                case "03":
                    return "UPS Ground";
                case "07":
                    return "UPS Worldwide Express";
                case "08":
                    return "UPS Worldwide Expedited";
                case "11":
                    return "UPS Standard";
                case "12":
                    return "UPS 3 Day Select";
                case "13":
                    return "UPS Next Day Air Saver";
                case "14":
                    return "UPS Next Day Air Early A.M.";
                case "54":
                    return "UPS Worldwide Express Plus";
                case "59":
                    return "UPS 2nd Day Air A.M.";
                case "65":
                    return "UPS Saver";
                case "82": //82-86, for Polish Domestic Shipments
                    return "UPS Today Standard";
                case "83":
                    return "UPS Today Dedicated Courier";
                case "85":
                    return "UPS Today Express";
                case "86":
                    return "UPS Today Express Saver";
                default:
                    return "Unknown";
            }
        }

        private bool IsPackageTooLarge(int length, int height, int width)
        {
            int total = TotalPackageSize(length, height, width);
            if (total > 165)
                return true;
            else
                return false;
        }

        private int TotalPackageSize(int length, int height, int width)
        {
            int girth = height + height + width + width;
            int total = girth + length;
            return total;
        }

        private bool IsPackageTooHeavy(int weight)
        {
            if (weight > MAXPACKAGEWEIGHT)
                return true;
            else
                return false;
        }

        private List<ShippingOption> ParseResponse(string response, ref string error)
        {
            var shippingOptions = new List<ShippingOption>();

            string carrierServicesOffered = _upsSettings.CarrierServicesOffered;

            using (var sr = new StringReader(response))
            using (var tr = new XmlTextReader(sr))
                while (tr.Read())
                {
                    if ((tr.Name == "Error") && (tr.NodeType == XmlNodeType.Element))
                    {
                        string errorText = "";
                        while (tr.Read())
                        {
                            if ((tr.Name == "ErrorCode") && (tr.NodeType == XmlNodeType.Element))
                            {
                                errorText += "UPS Rating Error, Error Code: " + tr.ReadString() + ", ";
                            }
                            if ((tr.Name == "ErrorDescription") && (tr.NodeType == XmlNodeType.Element))
                            {
                                errorText += "Error Desc: " + tr.ReadString();
                            }
                        }
                        error = "UPS Error returned: " + errorText;
                    }
                    if ((tr.Name == "RatedShipment") && (tr.NodeType == XmlNodeType.Element))
                    {
                        string serviceCode = "";
                        string monetaryValue = "";
                        while (tr.Read())
                        {
                            if ((tr.Name == "Service") && (tr.NodeType == XmlNodeType.Element))
                            {
                                while (tr.Read())
                                {
                                    if ((tr.Name == "Code") && (tr.NodeType == XmlNodeType.Element))
                                    {
                                        serviceCode = tr.ReadString();
                                        tr.ReadEndElement();
                                    }
                                    if ((tr.Name == "Service") && (tr.NodeType == XmlNodeType.EndElement))
                                    {
                                        break;
                                    }
                                }
                            }
                            if (((tr.Name == "RatedShipment") && (tr.NodeType == XmlNodeType.EndElement)) || ((tr.Name == "RatedPackage") && (tr.NodeType == XmlNodeType.Element)))
                            {
                                break;
                            }
                            if ((tr.Name == "TotalCharges") && (tr.NodeType == XmlNodeType.Element))
                            {
                                while (tr.Read())
                                {
                                    if ((tr.Name == "MonetaryValue") && (tr.NodeType == XmlNodeType.Element))
                                    {
                                        monetaryValue = tr.ReadString();
                                        tr.ReadEndElement();
                                    }
                                    if ((tr.Name == "TotalCharges") && (tr.NodeType == XmlNodeType.EndElement))
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        string service = GetServiceName(serviceCode);
                        string serviceId = String.Format("[{0}]", serviceCode);

                        // Go to the next rate if the service ID is not in the list of services to offer
                        if (!String.IsNullOrEmpty(carrierServicesOffered) && !carrierServicesOffered.Contains(serviceId))
                        {
                            continue;
                        }

                        //Weed out unwanted or unkown service rates
                        if (service.ToUpper() != "UNKNOWN")
                        {
                            var shippingOption = new ShippingOption();
                            shippingOption.Rate = Convert.ToDecimal(monetaryValue, new CultureInfo("en-US"));
                            shippingOption.Name = service;
                            shippingOptions.Add(shippingOption);
                        }

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

            if (getShippingOptionRequest.ShippingAddress == null)
            {
                response.AddError("Shipping address is not set");
                return response;
            }

            if (getShippingOptionRequest.ShippingAddress.Country == null)
            {
                response.AddError("Shipping country is not set");
                return response;
            }

            if (getShippingOptionRequest.CountryFrom == null)
            {
                getShippingOptionRequest.CountryFrom = _countryService.GetCountryById(_upsSettings.DefaultShippedFromCountryId);
                if (getShippingOptionRequest.CountryFrom == null)
                    getShippingOptionRequest.CountryFrom = _countryService.GetAllCountries(true).ToList().FirstOrDefault();
            }
            if (String.IsNullOrEmpty(getShippingOptionRequest.ZipPostalCodeFrom))
                getShippingOptionRequest.ZipPostalCodeFrom = _upsSettings.DefaultShippedFromZipPostalCode;

            string requestString = CreateRequest(_upsSettings.AccessKey, _upsSettings.Username, _upsSettings.Password, getShippingOptionRequest,
                _upsSettings.CustomerClassification, _upsSettings.PickupType, _upsSettings.PackagingType);
            string responseXml = DoRequest(_upsSettings.Url, requestString);
            string error = "";
            var shippingOptions = ParseResponse(responseXml, ref error);
            if (String.IsNullOrEmpty(error))
            {
                foreach (var shippingOption in shippingOptions)
                {
                    if (!shippingOption.Name.ToLower().StartsWith("ups"))
                        shippingOption.Name = string.Format("UPS {0}", shippingOption.Name);
                    shippingOption.Rate += _upsSettings.AdditionalHandlingCharge;
                    response.ShippingOptions.Add(shippingOption);
                }
            }
            else
            {
                response.AddError(error);
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
            controllerName = "ShippingUPS";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Shipping.UPS.Controllers" }, { "area", null } };
        }
        
        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new UPSSettings()
            {
                Url = "https://www.ups.com/ups.app/xml/Rate",
                AccessKey = "AccessKey1",
                Username = "Username1",
                Password = "Password",
                CustomerClassification = UPSCustomerClassification.Retail,
                PickupType = UPSPickupType.OneTimePickup,
                PackagingType = UPSPackagingType.ExpressBox,
                DefaultShippedFromZipPostalCode = "10001"

            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.Url", "URL");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.Url.Hint", "Specify UPS URL.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.AccessKey", "Access Key");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.AccessKey.Hint", "Specify UPS access key.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.Username", "Username");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.Username.Hint", "Specify UPS username.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.Password", "Password");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.Password.Hint", "Specify UPS password.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.AdditionalHandlingCharge", "Additional handling charge");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.AdditionalHandlingCharge.Hint", "Enter additional handling fee to charge your customers.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.InsurePackage", "Insure package");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.InsurePackage.Hint", "Check to ensure packages.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.CustomerClassification", "UPS Customer Classification");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.CustomerClassification.Hint", "Choose customer classification.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PickupType", "UPS Pickup Type");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PickupType.Hint", "Choose UPS pickup type.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackagingType", "UPS Packaging Type");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackagingType.Hint", "Choose UPS packaging type.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.DefaultShippedFromCountry", "Shipped from country");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.DefaultShippedFromCountry.Hint", "Specify origin country.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.DefaultShippedFromZipPostalCode", "Shipped from zip");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.DefaultShippedFromZipPostalCode.Hint", "Specify origin zip code.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.AvailableCarrierServices", "Carrier Services");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.AvailableCarrierServices.Hint", "Select the services you want to offer to customers.");
            //tracker events
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Tracker.Departed", "Departed");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Tracker.ExportScanned", "Export scanned");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Tracker.OriginScanned", "Origin scanned");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Tracker.Arrived", "Arrived");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Tracker.NotDelivered", "Not delivered");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Tracker.Booked", "Booked");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Tracker.Delivered", "Delivered");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<UPSSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.Url");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.Url.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.AccessKey");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.AccessKey.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.Username");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.Username.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.Password");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.Password.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.AdditionalHandlingCharge");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.AdditionalHandlingCharge.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.InsurePackage");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.InsurePackage.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.CustomerClassification");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.CustomerClassification.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.PickupType");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.PickupType.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackagingType");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackagingType.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.DefaultShippedFromCountry");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.DefaultShippedFromCountry.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.DefaultShippedFromZipPostalCode");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.DefaultShippedFromZipPostalCode.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.AvailableCarrierServices");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.AvailableCarrierServices.Hint");
            //tracker events
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Tracker.Departed");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Tracker.ExportScanned");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Tracker.OriginScanned");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Tracker.Arrived");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Tracker.NotDelivered");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Tracker.Booked");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Tracker.Delivered");

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
            get { return new UPSShipmentTracker(_logger, _localizationService, _upsSettings); }
        }

        #endregion
    }
}