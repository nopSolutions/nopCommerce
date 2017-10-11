//------------------------------------------------------------------------------
// Contributor(s): mb 10/20/2010, New York 02/08/2014
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Plugin.Shipping.UPS.Domain;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Discounts;
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
        private readonly IWebHelper _webHelper;

        private readonly StringBuilder _traceMessages;

        #endregion

        #region Ctor

        public UPSComputationMethod(IMeasureService measureService,
            IShippingService shippingService,
            ISettingService settingService,
            UPSSettings upsSettings, 
            ICountryService countryService,
            ICurrencyService currencyService,
            CurrencySettings currencySettings,
            IOrderTotalCalculationService orderTotalCalculationService,
            ILogger logger,
            ILocalizationService localizationService,
            IWebHelper webHelper)
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
            this._webHelper = webHelper;

            this._traceMessages = new StringBuilder();
        }

        #endregion

        #region Utilities

        private string CreateRequest(string accessKey, string username, string password,
            GetShippingOptionRequest getShippingOptionRequest, UPSCustomerClassification customerClassification,
            UPSPickupType pickupType, UPSPackagingType packagingType, bool saturdayDelivery)
        {
            var zipPostalCodeFrom = getShippingOptionRequest.ZipPostalCodeFrom;
            var zipPostalCodeTo = getShippingOptionRequest.ShippingAddress.ZipPostalCode;
            var countryCodeFrom = getShippingOptionRequest.CountryFrom.TwoLetterIsoCode;
            var countryCodeTo = getShippingOptionRequest.ShippingAddress.Country.TwoLetterIsoCode;
            var stateCodeFrom = getShippingOptionRequest.StateProvinceFrom?.Abbreviation;
            var stateCodeTo = getShippingOptionRequest.ShippingAddress.StateProvince?.Abbreviation;

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
            sb.Append("<RequestOption>Shop</RequestOption>");
            sb.Append("</Request>");
            if (String.Equals(countryCodeFrom, "US", StringComparison.InvariantCultureIgnoreCase))
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
            sb.AppendFormat("<ShipperNumber>{0}</ShipperNumber>", _upsSettings.AccountNumber);
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
            if (!string.IsNullOrEmpty(stateCodeTo))
                sb.AppendFormat("<StateProvinceCode>{0}</StateProvinceCode>", stateCodeTo);
            sb.Append("</Address>");
            sb.Append("</ShipTo>");
            sb.Append("<ShipFrom>");
            sb.Append("<Address>");
            sb.AppendFormat("<PostalCode>{0}</PostalCode>", zipPostalCodeFrom);
            sb.AppendFormat("<CountryCode>{0}</CountryCode>", countryCodeFrom);
            if (!string.IsNullOrEmpty(stateCodeFrom))
                sb.AppendFormat("<StateProvinceCode>{0}</StateProvinceCode>", stateCodeFrom);
            sb.Append("</Address>");
            sb.Append("</ShipFrom>");
            sb.Append("<Service>");
            sb.Append("<Code>03</Code>");
            sb.Append("</Service>");

            //Saturday delivery flag
            if (saturdayDelivery)
            {
                sb.Append("<ShipmentServiceOptions>");
                sb.Append("<SaturdayDelivery></SaturdayDelivery>");
                sb.Append("</ShipmentServiceOptions>");
            }

            //negotiated rates flag
            if (!string.IsNullOrEmpty(_upsSettings.AccountNumber) && !string.IsNullOrEmpty(stateCodeFrom) && !string.IsNullOrEmpty(stateCodeTo))
            {
                sb.Append("<RateInformation>");
                sb.Append("<NegotiatedRatesIndicator/>");
                sb.Append("</RateInformation>");
            }

            var currencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;

            //get subTotalWithoutDiscountBase, for use as insured value (when Settings.InsurePackage)
            //(note: prior versions used "with discount", but "without discount" better reflects true value to insure.)
            //TODO we should use getShippingOptionRequest.Items.GetQuantity() method to get subtotal
            _orderTotalCalculationService.GetShoppingCartSubTotal(getShippingOptionRequest.Items.Select(x => x.ShoppingCartItem).ToList(),
                false, out decimal _, out List<DiscountForCaching> _, out decimal subTotalWithoutDiscountBase, out decimal _);

            if (_upsSettings.Tracing)
                _traceMessages.AppendLine(" Packing Type: " + _upsSettings.PackingType.ToString());

            switch (_upsSettings.PackingType)
            {
                case PackingType.PackByOneItemPerPackage:
                    SetIndividualPackageLineItemsOneItemPerPackage(sb, getShippingOptionRequest, packagingType, currencyCode);
                    break;
                case PackingType.PackByVolume:
                    SetIndividualPackageLineItemsCubicRootDimensions(sb, getShippingOptionRequest, packagingType, subTotalWithoutDiscountBase, currencyCode);
                    break;
                case PackingType.PackByDimensions:
                default:
                    SetIndividualPackageLineItems(sb, getShippingOptionRequest, packagingType, subTotalWithoutDiscountBase, currencyCode);
                    break;
            }

            sb.Append("</Shipment>");
            sb.Append("</RatingServiceSelectionRequest>");

            return sb.ToString();
        }

        private void AppendPackageRequest(StringBuilder sb, UPSPackagingType packagingType, decimal length, decimal height, decimal width, decimal weight, decimal insuranceAmount, string currencyCode)
        {
            if (_upsSettings.Tracing)
                _traceMessages.AppendFormat(" Package: LxHxW={0}x{1}x{2}; Weight={3}; Insured={4} {5}.", length, height, width, weight, insuranceAmount, currencyCode).AppendLine();

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

            if (insuranceAmount > Decimal.Zero)
            {
                sb.Append("<PackageServiceOptions>");
                sb.Append("<InsuredValue>");
                sb.AppendFormat("<CurrencyCode>{0}</CurrencyCode>", currencyCode);
                sb.AppendFormat("<MonetaryValue>{0}</MonetaryValue>", insuranceAmount);
                sb.Append("</InsuredValue>");
                sb.Append("</PackageServiceOptions>");
            }

            sb.Append("</Package>");
        }

        private void SetIndividualPackageLineItems(StringBuilder sb, GetShippingOptionRequest getShippingOptionRequest, UPSPackagingType packagingType, decimal orderSubTotal, string currencyCode)
        {
            // Rate request setup - Total Dimensions of Shopping Cart Items determines number of packages

            var usedMeasureWeight = GetUsedMeasureWeight();
            var usedMeasureDimension = GetUsedMeasureDimension();

            _shippingService.GetDimensions(getShippingOptionRequest.Items, out decimal widthTmp, out decimal lengthTmp, out decimal heightTmp, true);

            var length = ConvertFromPrimaryMeasureDimension(lengthTmp, usedMeasureDimension);
            var height = ConvertFromPrimaryMeasureDimension(heightTmp, usedMeasureDimension);
            var width = ConvertFromPrimaryMeasureDimension(widthTmp, usedMeasureDimension);
            var weight = ConvertFromPrimaryMeasureWeight(_shippingService.GetTotalWeight(getShippingOptionRequest, ignoreFreeShippedItems: true), usedMeasureWeight);
            if (length < 1)
                length = 1;
            if (height < 1)
                height = 1;
            if (width < 1)
                width = 1;
            if (weight < 1)
                weight = 1;

            if ((!IsPackageTooHeavy(weight)) && (!IsPackageTooLarge(length, height, width)))
            {
                if (!_upsSettings.PassDimensions)
                    length = width = height = 0;

                var insuranceAmount = _upsSettings.InsurePackage ? Convert.ToInt32(orderSubTotal) : 0;
                AppendPackageRequest(sb, packagingType, length, height, width, weight, insuranceAmount, currencyCode);
            }
            else
            {
                var totalPackagesDims = 1;
                var totalPackagesWeights = 1;
                if (IsPackageTooHeavy(weight))
                {
                    totalPackagesWeights = Convert.ToInt32(Math.Ceiling((decimal)weight / (decimal)MAXPACKAGEWEIGHT));
                }
                if (IsPackageTooLarge(length, height, width))
                {
                    totalPackagesDims = Convert.ToInt32(Math.Ceiling((decimal)TotalPackageSize(length, height, width) / (decimal)108));
                }
                var totalPackages = totalPackagesDims > totalPackagesWeights ? totalPackagesDims : totalPackagesWeights;
                if (totalPackages == 0)
                    totalPackages = 1;

                var weight2 = weight / totalPackages;
                var height2 = height / totalPackages;
                var width2 = width / totalPackages;
                var length2 = length / totalPackages;
                if (weight2 < 1)
                    weight2 = 1;
                if (height2 < 1)
                    height2 = 1;
                if (width2 < 1)
                    width2 = 1;
                if (length2 < 1)
                    length2 = 1;

                if (!_upsSettings.PassDimensions)
                    length2 = width2 = height2 = 0;

                //The maximum declared amount per package: 50000 USD.
                var insuranceAmountPerPackage = _upsSettings.InsurePackage ? Convert.ToInt32(orderSubTotal / totalPackages) : 0;

                for (var i = 0; i < totalPackages; i++)
                {
                    AppendPackageRequest(sb, packagingType, length2, height2, width2, weight2, insuranceAmountPerPackage, currencyCode);
                }
            }
        }

        private void SetIndividualPackageLineItemsOneItemPerPackage(StringBuilder sb, GetShippingOptionRequest getShippingOptionRequest, UPSPackagingType packagingType, string currencyCode)
        {
            // Rate request setup - each Shopping Cart Item is a separate package

            var usedMeasureWeight = GetUsedMeasureWeight();
            var usedMeasureDimension = GetUsedMeasureDimension();

            foreach (var packageItem in getShippingOptionRequest.Items)
            {
                var sci = packageItem.ShoppingCartItem;
                var qty = packageItem.GetQuantity();

                //get dimensions for qty 1
                _shippingService.GetDimensions(new List<GetShippingOptionRequest.PackageItem>
                                               {
                                                   new GetShippingOptionRequest.PackageItem(sci, 1)
                                               }, out decimal widthTmp, out decimal lengthTmp, out decimal heightTmp, true);

                var length = ConvertFromPrimaryMeasureDimension(lengthTmp, usedMeasureDimension);
                var height = ConvertFromPrimaryMeasureDimension(heightTmp, usedMeasureDimension);
                var width = ConvertFromPrimaryMeasureDimension(widthTmp, usedMeasureDimension);
                var weight = ConvertFromPrimaryMeasureWeight(sci.Product.Weight, usedMeasureWeight);
                if (length < 1)
                    length = 1;
                if (height < 1)
                    height = 1;
                if (width < 1)
                    width = 1;
                if (weight < 1)
                    weight = 1;

                //The maximum declared amount per package: 50000 USD.
                //TODO: Currently using Product.Price - should we use GetUnitPrice() instead?
                // Convert.ToInt32(_priceCalculationService.GetUnitPrice(sci, includeDiscounts:false))
                //One could argue that the insured value should be based on Cost rather than Price.
                //GetUnitPrice handles Attribute Adjustments and also Customer Entered Price.
                //But, even with includeDiscounts:false, it could apply a "discount" from Tier pricing.
                var insuranceAmountPerPackage = _upsSettings.InsurePackage ? Convert.ToInt32(sci.Product.Price) : 0;

                for (var j = 0; j < qty; j++)
                {
                    AppendPackageRequest(sb, packagingType, length, height, width, weight, insuranceAmountPerPackage, currencyCode);
                }
            }
        }

        private void SetIndividualPackageLineItemsCubicRootDimensions(StringBuilder sb, GetShippingOptionRequest getShippingOptionRequest, UPSPackagingType packagingType, decimal orderSubTotal, string currencyCode)
        {
            // Rate request setup - Total Volume of Shopping Cart Items determines number of packages

            //Dimensional weight is based on volume (the amount of space a package
            //occupies in relation to its actual weight). If the cubic size of your
            //package measures three cubic feet (5,184 cubic inches or 84,951
            //cubic centimetres) or greater, you will be charged the greater of the
            //dimensional weight or the actual weight.
            //This algorithm devides total package volume by the UPS settings PackingPackageVolume
            //so that no package requires dimensional weight; this could result in an under-charge.

            var usedMeasureWeight = GetUsedMeasureWeight();
            var usedMeasureDimension = GetUsedMeasureDimension();

            int totalPackagesDims;
            int length;
            int height;
            int width;

            if (getShippingOptionRequest.Items.Count == 1 && getShippingOptionRequest.Items[0].GetQuantity() == 1)
            {
                var sci = getShippingOptionRequest.Items[0].ShoppingCartItem;

                //get dimensions for qty 1
                _shippingService.GetDimensions(new List<GetShippingOptionRequest.PackageItem>
                                               {
                                                   new GetShippingOptionRequest.PackageItem(sci, 1)
                                               }, out decimal widthTmp, out decimal lengthTmp, out decimal heightTmp, true);

                totalPackagesDims = 1;
                length = ConvertFromPrimaryMeasureDimension(lengthTmp, usedMeasureDimension);
                height = ConvertFromPrimaryMeasureDimension(heightTmp, usedMeasureDimension);
                width = ConvertFromPrimaryMeasureDimension(widthTmp, usedMeasureDimension);
            }
            else
            {
                decimal totalVolume = 0;
                foreach (var item in getShippingOptionRequest.Items)
                {
                    var sci = item.ShoppingCartItem;

                    //get dimensions for qty 1
                    _shippingService.GetDimensions(new List<GetShippingOptionRequest.PackageItem>
                                               {
                                                   new GetShippingOptionRequest.PackageItem(sci, 1)
                                               }, out decimal widthTmp, out decimal lengthTmp, out decimal _, true);

                    var productLength = ConvertFromPrimaryMeasureDimension(lengthTmp, usedMeasureDimension);
                    var productHeight = ConvertFromPrimaryMeasureDimension(lengthTmp, usedMeasureDimension);
                    var productWidth = ConvertFromPrimaryMeasureDimension(widthTmp, usedMeasureDimension);
                    totalVolume += item.GetQuantity() * (productHeight * productWidth * productLength);
                }

                int dimension;
                if (totalVolume == 0)
                {
                    dimension = 0;
                    totalPackagesDims = 1;
                }
                else
                {
                    // cubic inches
                    var packageVolume = _upsSettings.PackingPackageVolume;
                    if (packageVolume <= 0)
                        packageVolume = 5184;

                    // cube root (floor)
                    dimension = Convert.ToInt32(Math.Floor(Math.Pow(Convert.ToDouble(packageVolume), (double)(1.0 / 3.0))));
                    if (IsPackageTooLarge(dimension, dimension, dimension))
                        throw new NopException("upsSettings.PackingPackageVolume exceeds max package size");

                    // adjust packageVolume for dimensions calculated
                    packageVolume = dimension * dimension * dimension;

                    totalPackagesDims = Convert.ToInt32(Math.Ceiling(totalVolume / packageVolume));
                }

                length = width = height = dimension;
            }
            if (length < 1)
                length = 1;
            if (height < 1)
                height = 1;
            if (width < 1)
                width = 1;

            var weight = ConvertFromPrimaryMeasureWeight(_shippingService.GetTotalWeight(getShippingOptionRequest, ignoreFreeShippedItems: true), usedMeasureWeight);
            if (weight < 1)
                weight = 1;

            var totalPackagesWeights = 1;
            if (IsPackageTooHeavy(weight))
            {
                totalPackagesWeights = Convert.ToInt32(Math.Ceiling((decimal)weight / (decimal)MAXPACKAGEWEIGHT));
            }

            var totalPackages = totalPackagesDims > totalPackagesWeights ? totalPackagesDims : totalPackagesWeights;

            var weightPerPackage = weight / totalPackages;

            //The maximum declared amount per package: 50000 USD.
            var insuranceAmountPerPackage = _upsSettings.InsurePackage ? Convert.ToInt32(orderSubTotal / totalPackages) : 0;

            for (var i = 0; i < totalPackages; i++)
            {
                AppendPackageRequest(sb, packagingType, length, height, width, weightPerPackage, insuranceAmountPerPackage, currencyCode);
            }

        }

        private string DoRequest(string url, string requestString)
        {
            var bytes = Encoding.ASCII.GetBytes(requestString);
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = MimeTypes.ApplicationXWwwFormUrlencoded;
            request.ContentLength = bytes.Length;
            using (var requestStream = request.GetRequestStream())
                requestStream.Write(bytes, 0, bytes.Length);
            using (var response = request.GetResponse())
            {
                string responseXml;
                using (var reader = new StreamReader(response.GetResponseStream()))
                    responseXml = reader.ReadToEnd();

                return responseXml;
            }
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
            var total = TotalPackageSize(length, height, width);
            return total > 165;
        }

        private int TotalPackageSize(int length, int height, int width)
        {
            var girth = height + height + width + width;
            var total = girth + length;
            return total;
        }

        private bool IsPackageTooHeavy(int weight)
        {
            return weight > MAXPACKAGEWEIGHT;
        }

        private MeasureWeight GetUsedMeasureWeight()
        {
            var usedMeasureWeight = _measureService.GetMeasureWeightBySystemKeyword(MEASUREWEIGHTSYSTEMKEYWORD);
            if (usedMeasureWeight == null)
                throw new NopException("UPS shipping service. Could not load \"{0}\" measure weight", MEASUREWEIGHTSYSTEMKEYWORD);
            return usedMeasureWeight;
        }

        private MeasureDimension GetUsedMeasureDimension()
        {
            var usedMeasureDimension = _measureService.GetMeasureDimensionBySystemKeyword(MEASUREDIMENSIONSYSTEMKEYWORD);
            if (usedMeasureDimension == null)
                throw new NopException("UPS shipping service. Could not load \"{0}\" measure dimension", MEASUREDIMENSIONSYSTEMKEYWORD);

            return usedMeasureDimension;
        }

        private int ConvertFromPrimaryMeasureDimension(decimal quantity, MeasureDimension usedMeasureDimension)
        {
            return Convert.ToInt32(Math.Ceiling(_measureService.ConvertFromPrimaryMeasureDimension(quantity, usedMeasureDimension)));
        }

        private int ConvertFromPrimaryMeasureWeight(decimal quantity, MeasureWeight usedMeasureWeighht)
        {
            return Convert.ToInt32(Math.Ceiling(_measureService.ConvertFromPrimaryMeasureWeight(quantity, usedMeasureWeighht)));
        }

        private IEnumerable<ShippingOption> ParseResponse(string response, bool saturdayDelivery, ref string error)
        {
            var shippingOptions = new List<ShippingOption>();

            var carrierServicesOffered = _upsSettings.CarrierServicesOffered;

            using (var sr = new StringReader(response))
            using (var tr = new XmlTextReader(sr))
                while (tr.Read())
                {
                    if ((tr.Name == "Error") && (tr.NodeType == XmlNodeType.Element))
                    {
                        var errorText = "";
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
                        var serviceCode = "";
                        var monetaryValue = "";
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
                            if ((tr.Name == "RatedShipment") && (tr.NodeType == XmlNodeType.EndElement))
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
                            //parse negotiated rates
                            if ((tr.Name == "NegotiatedRates") && (tr.NodeType == XmlNodeType.Element))
                            {
                                while (tr.Read())
                                {
                                    if ((tr.Name == "NetSummaryCharges") && (tr.NodeType == XmlNodeType.Element))
                                    {
                                        while (tr.Read())
                                        {
                                            if ((tr.Name == "GrandTotal") && (tr.NodeType == XmlNodeType.Element))
                                            {
                                                while (tr.Read())
                                                {
                                                    if ((tr.Name == "MonetaryValue") && (tr.NodeType == XmlNodeType.Element))
                                                    {
                                                        monetaryValue = tr.ReadString();
                                                        tr.ReadEndElement();
                                                    }
                                                    if ((tr.Name == "GrandTotal") && (tr.NodeType == XmlNodeType.EndElement))
                                                        break;
                                                }
                                            }
                                            if ((tr.Name == "NetSummaryCharges") && (tr.NodeType == XmlNodeType.EndElement))
                                                break;
                                        }
                                    }
                                    if ((tr.Name == "NegotiatedRates") && (tr.NodeType == XmlNodeType.EndElement))
                                        break;
                                }
                            }
                        }
                        var service = GetServiceName(serviceCode);
                        var serviceId = $"[{serviceCode}]";

                        // Go to the next rate if the service ID is not in the list of services to offer
                        if (!saturdayDelivery && !string.IsNullOrEmpty(carrierServicesOffered) && !carrierServicesOffered.Contains(serviceId))
                        {
                            continue;
                        }

                        //Weed out unwanted or unknown service rates
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
                throw new ArgumentNullException(nameof(getShippingOptionRequest));

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
                getShippingOptionRequest.CountryFrom = _countryService.GetAllCountries().FirstOrDefault();
            }

            try
            {
                var requestString = CreateRequest(_upsSettings.AccessKey, _upsSettings.Username, _upsSettings.Password, getShippingOptionRequest,
                    _upsSettings.CustomerClassification, _upsSettings.PickupType, _upsSettings.PackagingType, false);
                if (_upsSettings.Tracing)
                    _traceMessages.AppendLine("Request:").AppendLine(requestString);

                var responseXml = DoRequest(_upsSettings.Url, requestString);
                if (_upsSettings.Tracing)
                    _traceMessages.AppendLine("Response:").AppendLine(responseXml);

                var error = "";
                var shippingOptions = ParseResponse(responseXml, false, ref error);
                if (string.IsNullOrEmpty(error))
                {
                    foreach (var shippingOption in shippingOptions)
                    {
                        if (!shippingOption.Name.ToLower().StartsWith("ups"))
                            shippingOption.Name = $"UPS {shippingOption.Name}";
                        shippingOption.Rate += _upsSettings.AdditionalHandlingCharge;
                        response.ShippingOptions.Add(shippingOption);
                    }
                }
                else
                {
                    response.AddError(error);
                }

                //Saturday delivery
                if (_upsSettings.CarrierServicesOffered.Contains("[sa]"))
                {
                    requestString = CreateRequest(_upsSettings.AccessKey, _upsSettings.Username, _upsSettings.Password, getShippingOptionRequest,
                        _upsSettings.CustomerClassification, _upsSettings.PickupType, _upsSettings.PackagingType, true);
                    if (_upsSettings.Tracing)
                        _traceMessages.AppendLine("Request:").AppendLine(requestString);

                    responseXml = DoRequest(_upsSettings.Url, requestString);
                    if (_upsSettings.Tracing)
                        _traceMessages.AppendLine("Response:").AppendLine(responseXml);

                    error = string.Empty;
                    var saturdayDeliveryShippingOptions = ParseResponse(responseXml, true, ref error);
                    if (string.IsNullOrEmpty(error))
                    {
                        foreach (var shippingOption in saturdayDeliveryShippingOptions)
                        {
                            shippingOption.Name =
                                $"{(shippingOption.Name.ToLower().StartsWith("ups") ? string.Empty : "UPS ")}{shippingOption.Name} - Saturday Delivery";
                            shippingOption.Rate += _upsSettings.AdditionalHandlingCharge;
                            response.ShippingOptions.Add(shippingOption);
                        }
                    }
                    else
                        response.AddError(error);
                }

                if (response.ShippingOptions.Any())
                    response.Errors.Clear();
            }
            catch (Exception exc)
            {
                response.AddError($"UPS Service is currently unavailable, try again later. {exc.Message}");
            }
            finally
            {
                if (_upsSettings.Tracing && _traceMessages.Length > 0)
                {
                    var shortMessage =
                        $"UPS Get Shipping Options for customer {getShippingOptionRequest.Customer.Email}.  {getShippingOptionRequest.Items.Count} item(s) in cart";
                    _logger.Information(shortMessage, new Exception(_traceMessages.ToString()), getShippingOptionRequest.Customer);
                }
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
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/ShippingUPS/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new UPSSettings
            {
                Url = "https://www.ups.com/ups.app/xml/Rate",
                CustomerClassification = UPSCustomerClassification.Retail,
                PickupType = UPSPickupType.OneTimePickup,
                PackagingType = UPSPackagingType.ExpressBox,
                PackingPackageVolume = 5184,
                PackingType = PackingType.PackByDimensions,
                PassDimensions = true,
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.Url", "URL");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.Url.Hint", "Specify UPS URL.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.AccessKey", "Access Key");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.AccessKey.Hint", "Specify UPS access key.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.AccountNumber", "Account number");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.AccountNumber.Hint", "Specify UPS account number (required to get negotiated rates).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.Username", "Username");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.Username.Hint", "Specify UPS username.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.Password", "Password");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.Password.Hint", "Specify UPS password.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.AdditionalHandlingCharge", "Additional handling charge");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.AdditionalHandlingCharge.Hint", "Enter additional handling fee to charge your customers.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.InsurePackage", "Insure package");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.InsurePackage.Hint", "Check to insure packages.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.CustomerClassification", "UPS Customer Classification");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.CustomerClassification.Hint", "Choose customer classification.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PickupType", "UPS Pickup Type");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PickupType.Hint", "Choose UPS pickup type.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackagingType", "UPS Packaging Type");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackagingType.Hint", "Choose UPS packaging type.");
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
            //packing
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PassDimensions", "Pass dimensions");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PassDimensions.Hint", "Check if you want to pass package dimensions when requesting rates.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackingType", "Packing type");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackingType.Hint", "Choose preferred packing type.");
            this.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Shipping.UPS.PackingType.PackByDimensions", "Pack by dimensions");
            this.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Shipping.UPS.PackingType.PackByOneItemPerPackage", "Pack by one item per package");
            this.AddOrUpdatePluginLocaleResource("Enums.Nop.Plugin.Shipping.UPS.PackingType.PackByVolume", "Pack by volume");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackingPackageVolume", "Package volume");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackingPackageVolume.Hint", "Enter your package volume.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.Tracing", "Tracing");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.UPS.Fields.Tracing.Hint", "Check if you want to record plugin tracing in System Log. Warning: The entire request and response XML will be logged (including AccessKey/UserName,Password). Do not leave this enabled in a production environment.");

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
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.AccountNumber");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.AccountNumber.Hint");
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
            //packing
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.PassDimensions");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.PassDimensions.Hint");
            this.DeletePluginLocaleResource("Enums.Nop.Plugin.Shipping.UPS.PackingType.PackByDimensions");
            this.DeletePluginLocaleResource("Enums.Nop.Plugin.Shipping.UPS.PackingType.PackByOneItemPerPackage");
            this.DeletePluginLocaleResource("Enums.Nop.Plugin.Shipping.UPS.PackingType.PackByVolume");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackingType");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackingType.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackingPackageVolume");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.PackingPackageVolume.Hint");
            //tracing
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.Tracing");
            this.DeletePluginLocaleResource("Plugins.Shipping.UPS.Fields.Tracing.Hint");

            base.Uninstall();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a shipping rate computation method type
        /// </summary>
        public ShippingRateComputationMethodType ShippingRateComputationMethodType
        {
            get { return ShippingRateComputationMethodType.Realtime; }
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