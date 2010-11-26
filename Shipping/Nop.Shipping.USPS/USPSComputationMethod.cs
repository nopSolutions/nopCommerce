//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): RJH 08/07/2009, mb 10/20/2010. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Measures;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.Common;

namespace NopSolutions.NopCommerce.Shipping.Methods.USPS
{
    /// <summary>
    /// US Postal Service computation method
    /// </summary>
    public class USPSComputationMethod : IShippingRateComputationMethod
    {
        #region Const

        private const int MAXPACKAGEWEIGHT = 70;
        private const string MEASUREWEIGHTSYSTEMKEYWORD = "lb";
        private const string MEASUREDIMENSIONSYSTEMKEYWORD = "inches";
        #endregion

        #region Utilities

        /// <summary>
        /// Convert pounds to ounces
        /// </summary>
        /// <param name="pounds"></param>
        /// <returns></returns>
        protected int PoundsToOunces(int pounds)
        {
            return Convert.ToInt32(pounds * 16M);
        }

        /// <summary>
        /// Is a request domestic
        /// </summary>
        /// <param name="shipmentPackage">Shipment package</param>
        /// <returns>Rsult</returns>
        protected bool IsDomesticRequest(ShipmentPackage shipmentPackage)
        {
            //Origin Country must be USA, Collect USA from list of countries
            bool result = true;
            if (shipmentPackage != null &&
                shipmentPackage.ShippingAddress != null &&
                shipmentPackage.ShippingAddress.Country != null)
            {
                result = shipmentPackage.ShippingAddress.Country.ThreeLetterIsoCode == "USA";
            }
            return result;
        }

        private string CreateRequest(string username, string password, ShipmentPackage shipmentPackage)
        {
            var usedMeasureWeight = IoC.Resolve<IMeasureService>().GetMeasureWeightBySystemKeyword(MEASUREWEIGHTSYSTEMKEYWORD);
            if (usedMeasureWeight == null)
                throw new NopException(string.Format("USPS shipping service. Could not load \"{0}\" measure weight", MEASUREWEIGHTSYSTEMKEYWORD));

            var usedMeasureDimension = IoC.Resolve<IMeasureService>().GetMeasureDimensionBySystemKeyword(MEASUREDIMENSIONSYSTEMKEYWORD);
            if (usedMeasureDimension == null)
                throw new NopException(string.Format("USPS shipping service. Could not load \"{0}\" measure dimension", MEASUREDIMENSIONSYSTEMKEYWORD));

            int length = Convert.ToInt32(Math.Ceiling(IoC.Resolve<IMeasureService>().ConvertDimension(shipmentPackage.GetTotalLength(), IoC.Resolve<IMeasureService>().BaseDimensionIn, usedMeasureDimension)));
            int height = Convert.ToInt32(Math.Ceiling(IoC.Resolve<IMeasureService>().ConvertDimension(shipmentPackage.GetTotalHeight(), IoC.Resolve<IMeasureService>().BaseDimensionIn, usedMeasureDimension)));
            int width = Convert.ToInt32(Math.Ceiling(IoC.Resolve<IMeasureService>().ConvertDimension(shipmentPackage.GetTotalWidth(), IoC.Resolve<IMeasureService>().BaseDimensionIn, usedMeasureDimension)));
            int weight = Convert.ToInt32(Math.Ceiling(IoC.Resolve<IMeasureService>().ConvertWeight(IoC.Resolve<IShippingService>().GetShoppingCartTotalWeight(shipmentPackage.Items, shipmentPackage.Customer), IoC.Resolve<IMeasureService>().BaseWeightIn, usedMeasureWeight)));
            if (length < 1)
                length = 1;
            if (height < 1)
                height = 1;
            if (width < 1)
                width = 1;
            if (weight < 1)
                weight = 1;


            string zipPostalCodeFrom = shipmentPackage.ZipPostalCodeFrom;
            string zipPostalCodeTo = shipmentPackage.ShippingAddress.ZipPostalCode;

            //valid values for testing. http://testing.shippingapis.com/ShippingAPITest.dll
            //Zip to = "20008"; Zip from ="10022"; weight = 2;

            int pounds = weight;
            //we don't use ounce
            //int ounces = Convert.ToInt32((weight - pounds) * 16.0M);
            int ounces = 0;
            if (pounds < 1)
                pounds = 1;

            string requestString = string.Empty;

            bool isDomestic = IsDomesticRequest(shipmentPackage);
            if (isDomestic)
            {
                #region domestic request
                var sb = new StringBuilder();
                sb.AppendFormat("<RateV3Request USERID=\"{0}\" PASSWORD=\"{1}\">", username, password);

                var xmlStrings = new USPSStrings(); // Create new instance with string array

                if ((!IsPackageTooHeavy(pounds)) && (!IsPackageTooLarge(length, height, width)))
                {
                    var packageSize = GetPackageSize(length, height, width);
                    // RJH get all XML strings not commented out for USPSStrings. 
                    // RJH V3 USPS Service must be Express, Express SH, Express Commercial, Express SH Commercial, First Class, Priority, Priority Commercial, Parcel, Library, BPM, Media, ALL or ONLINE;
                    foreach (string element in xmlStrings.Elements) // Loop over elements with property
                    {
                        sb.Append("<Package ID=\"0\">");

                        // sb.AppendFormat("<Service>{0}</Service>", USPSService.All);
                        sb.AppendFormat("<Service>{0}</Service>", element);
                        sb.AppendFormat("<ZipOrigination>{0}</ZipOrigination>", zipPostalCodeFrom);
                        sb.AppendFormat("<ZipDestination>{0}</ZipDestination>", zipPostalCodeTo);
                        sb.AppendFormat("<Pounds>{0}</Pounds>", pounds);
                        sb.AppendFormat("<Ounces>{0}</Ounces>", ounces);
                        sb.AppendFormat("<Size>{0}</Size>", packageSize);
                        sb.Append("<Machinable>FALSE</Machinable>");

                        sb.Append("</Package>");
                    }
                }
                else
                {
                    int totalPackages = 1;
                    int totalPackagesDims = 1;
                    int totalPackagesWeights = 1;
                    if (IsPackageTooHeavy(pounds))
                    {
                        totalPackagesWeights = Convert.ToInt32(Math.Ceiling((decimal)pounds / (decimal)MAXPACKAGEWEIGHT));
                    }
                    if (IsPackageTooLarge(length, height, width))
                    {
                        totalPackagesDims = Convert.ToInt32(Math.Ceiling((decimal)TotalPackageSize(length, height, width) / (decimal)108));
                    }
                    totalPackages = totalPackagesDims > totalPackagesWeights ? totalPackagesDims : totalPackagesWeights;
                    if (totalPackages == 0)
                        totalPackages = 1;

                    int pounds2 = pounds / totalPackages;
                    //we don't use ounces
                    int ounces2 = ounces / totalPackages;
                    int height2 = height / totalPackages;
                    int width2 = width / totalPackages;
                    int length2 = length / totalPackages;
                    if (pounds2 < 1)
                        pounds2 = 1;
                    if (height2 < 1)
                        height2 = 1;
                    if (width2 < 1)
                        width2 = 1;
                    if (length2 < 1)
                        length2 = 1;

                    var packageSize = GetPackageSize(length2, height2, width2);

                    for (int i = 0; i < totalPackages; i++)
                    {
                        foreach (string element in xmlStrings.Elements)
                        {
                            sb.AppendFormat("<Package ID=\"{0}\">", i.ToString());
                            // sb.AppendFormat("<Service>{0}</Service>", USPSService.All);
                            sb.AppendFormat("<Service>{0}</Service>", element);
                            sb.AppendFormat("<ZipOrigination>{0}</ZipOrigination>", zipPostalCodeFrom);
                            sb.AppendFormat("<ZipDestination>{0}</ZipDestination>", zipPostalCodeTo);
                            sb.AppendFormat("<Pounds>{0}</Pounds>", pounds2);
                            sb.AppendFormat("<Ounces>{0}</Ounces>", ounces2);
                            sb.AppendFormat("<Size>{0}</Size>", packageSize);
                            sb.Append("<Machinable>FALSE</Machinable>");
                            sb.Append("</Package>");
                        }
                    }
                }

                sb.Append("</RateV3Request>");

                requestString = "API=RateV3&XML=" + sb.ToString();
                #endregion
            }
            else
            {
                #region international request
                var sb = new StringBuilder();
                sb.AppendFormat("<IntlRateRequest USERID=\"{0}\" PASSWORD=\"{1}\">", username, password);

                if ((!IsPackageTooHeavy(pounds)) && (!IsPackageTooLarge(length, height, width)))
                {
                    //little hack here for international requests
                    length = 12;
                    width = 12;
                    height = 12;

                    string mailType = "Package"; //Package, Envelope
                    sb.Append("<Package ID=\"0\">");
                    // No use of pounds in this classes
                    sb.Append("<Pounds>0</Pounds>");
                    ounces = PoundsToOunces(pounds);
                    sb.AppendFormat("<Ounces>{0}</Ounces>", ounces);
                    sb.Append("<Machinable>FALSE</Machinable>");
                    sb.AppendFormat("<MailType>{0}</MailType>", mailType);
                    sb.Append("<GXG>");
                    sb.AppendFormat("<Length>{0}</Length>", length);
                    sb.AppendFormat("<Width>{0}</Width>", width);
                    sb.AppendFormat("<Height>{0}</Height>", height);
                    sb.Append("<POBoxFlag>N</POBoxFlag>");
                    sb.Append("<GiftFlag>N</GiftFlag>");
                    sb.Append("</GXG>");
                    sb.AppendFormat("<Country>{0}</Country>", shipmentPackage.ShippingAddress.Country.Name);

                    sb.Append("</Package>");
                }
                else
                {
                    int totalPackages = 1;
                    int totalPackagesDims = 1;
                    int totalPackagesWeights = 1;
                    if (IsPackageTooHeavy(pounds))
                    {
                        totalPackagesWeights = Convert.ToInt32(Math.Ceiling((decimal)pounds / (decimal)MAXPACKAGEWEIGHT));
                    }
                    if (IsPackageTooLarge(length, height, width))
                    {
                        totalPackagesDims = Convert.ToInt32(Math.Ceiling((decimal)TotalPackageSize(length, height, width) / (decimal)108));
                    }
                    totalPackages = totalPackagesDims > totalPackagesWeights ? totalPackagesDims : totalPackagesWeights;
                    if (totalPackages == 0)
                        totalPackages = 1;

                    int pounds2 = pounds / totalPackages;
                    //we don't use ounces
                    int ounces2 = ounces / totalPackages;
                    int height2 = height / totalPackages;
                    int width2 = width / totalPackages;
                    int length2 = length / totalPackages;
                    if (pounds2 < 1)
                        pounds2 = 1;
                    if (height2 < 1)
                        height2 = 1;
                    if (width2 < 1)
                        width2 = 1;
                    if (length2 < 1)
                        length2 = 1;

                    //little hack here for international requests
                    length2 = 12;
                    width2 = 12;
                    height2 = 12;

                    for (int i = 0; i < totalPackages; i++)
                    {
                        string mailType = "Package"; //Package, Envelope

                        sb.AppendFormat("<Package ID=\"{0}\">", i.ToString());
                        // No use of pounds in this classes
                        sb.Append("<Pounds>0</Pounds>");
                        ounces2 = PoundsToOunces(pounds2);
                        sb.AppendFormat("<Ounces>{0}</Ounces>", ounces2);
                        sb.Append("<Machinable>FALSE</Machinable>");
                        sb.AppendFormat("<MailType>{0}</MailType>", mailType);
                        sb.Append("<GXG>");
                        sb.AppendFormat("<Length>{0}</Length>", length2);
                        sb.AppendFormat("<Width>{0}</Width>", width2);
                        sb.AppendFormat("<Height>{0}</Height>", height2);
                        sb.Append("<POBoxFlag>N</POBoxFlag>");
                        sb.Append("<GiftFlag>N</GiftFlag>");
                        sb.Append("</GXG>");
                        sb.AppendFormat("<Country>{0}</Country>", shipmentPackage.ShippingAddress.Country.Name);

                        sb.Append("</Package>");
                    }
                }

                sb.Append("</IntlRateRequest>");

                requestString = "API=IntlRate&XML=" + sb.ToString();
                #endregion
            }

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

        private bool IsPackageTooLarge(int length, int height, int width)
        {
            int total = TotalPackageSize(length, height, width);
            if (total > 130)
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

        private USPSPackageSize GetPackageSize(int length, int height, int width)
        {
            int girth = height + height + width + width;
            int total = girth + length;
            if (total <= 84)
                return USPSPackageSize.Regular;
            if ((total > 84) && (total <= 108))
                return USPSPackageSize.Large;
            if ((total > 108) && (total <= 130))
                return USPSPackageSize.Oversize;
            else
                throw new NopException("Shipping Error: Package too large.");
        }

        private List<ShippingOption> ParseResponse(string response, bool isDomestic, ref string error)
        {
            var shippingOptions = new List<ShippingOption>();

            string postageStr = isDomestic ? "Postage" : "Service";
            string mailServiceStr = isDomestic ? "MailService" : "SvcDescription";
            string rateStr = isDomestic ? "Rate" : "Postage";
            string classStr = isDomestic ? "CLASSID" : "ID";
            string carrierServicesOffered = String.Empty;

            if (isDomestic)
            {
                carrierServicesOffered = IoC.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.USPS.CarrierServicesOfferedDomestic", string.Empty);
            }
            else
            {
                carrierServicesOffered = IoC.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.USPS.CarrierServicesOfferedInternational", string.Empty);
            }

            using (var sr = new StringReader(response))
            using (var tr = new XmlTextReader(sr))
            {
                do
                {
                    tr.Read();

                    if ((tr.Name == "Error") && (tr.NodeType == XmlNodeType.Element))
                    {
                        string errorText = "";
                        while (tr.Read())
                        {
                            if ((tr.Name == "Description") && (tr.NodeType == XmlNodeType.Element))
                                errorText += "Error Desc: " + tr.ReadString();
                            if ((tr.Name == "HelpContext") && (tr.NodeType == XmlNodeType.Element))
                                errorText += "USPS Help Context: " + tr.ReadString() + ". ";
                        }
                        error = "USPS Error returned: " + errorText;
                    }

                    if ((tr.Name == postageStr) && (tr.NodeType == XmlNodeType.Element))
                    {
                        string serviceId = string.Empty;

                        // Find the ID for the service
                        if (tr.HasAttributes)
                        {
                            for (int i = 0; i < tr.AttributeCount; i++)
                            {
                                tr.MoveToAttribute(i);
                                if (tr.Name.Equals(classStr))
                                {
                                    // Add delimiters [] so that single digit IDs aren't found in mutli-digit IDs                                    
                                    serviceId = String.Format("[{0}]", tr.Value);
                                    break;
                                }
                            }
                        }

                        // Go to the next rate if the service ID is not in the list of services to offer
                        if (!String.IsNullOrEmpty(serviceId) &&
                            !String.IsNullOrEmpty(carrierServicesOffered) &&
                            !carrierServicesOffered.Contains(serviceId))
                        {
                            continue;
                        }

                        string serviceCode = string.Empty;
                        string postalRate = string.Empty;

                        do
                        {

                            tr.Read();

                            if ((tr.Name == mailServiceStr) && (tr.NodeType == XmlNodeType.Element))
                            {
                                serviceCode = tr.ReadString();

                                tr.ReadEndElement();
                                if ((tr.Name == mailServiceStr) && (tr.NodeType == XmlNodeType.EndElement))
                                    break;
                            }

                            if ((tr.Name == rateStr) && (tr.NodeType == XmlNodeType.Element))
                            {
                                postalRate = tr.ReadString();
                                tr.ReadEndElement();
                                if ((tr.Name == rateStr) && (tr.NodeType == XmlNodeType.EndElement))
                                    break;
                            }

                        }
                        while (!((tr.Name == postageStr) && (tr.NodeType == XmlNodeType.EndElement)));

                        if (shippingOptions.Find((s) => s.Name == serviceCode) == null)
                        {
                            var shippingOption = new ShippingOption();
                            //TODO check whether we need to multiply rate by package quantity
                            shippingOption.Rate = Convert.ToDecimal(postalRate, new CultureInfo("en-US"));
                            shippingOption.Name = serviceCode;
                            shippingOptions.Add(shippingOption);
                        }
                    }
                }
                while (!tr.EOF);
            }
            return shippingOptions;
        }

        #endregion

        #region Methods
        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="shipmentPackage">Shipment package</param>
        /// <param name="error">Error</param>
        /// <returns>Shipping options</returns>
        public List<ShippingOption> GetShippingOptions(ShipmentPackage shipmentPackage, ref string error)
        {
            var shippingOptions = new List<ShippingOption>();

            if (shipmentPackage == null)
                throw new ArgumentNullException("shipmentPackage");
            if (shipmentPackage.Items == null)
                throw new NopException("No shipment items");
            if (shipmentPackage.ShippingAddress == null)
            {
                error = "Shipping address is not set";
                return shippingOptions;
            }

            string url = IoC.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.USPS.URL");
            string username = IoC.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.USPS.Username");
            string password = IoC.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.USPS.Password");
            decimal additionalHandlingCharge = IoC.Resolve<ISettingManager>().GetSettingValueDecimalNative("ShippingRateComputationMethod.USPS.AdditionalHandlingCharge");
            shipmentPackage.ZipPostalCodeFrom = IoC.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.USPS.DefaultShippedFromZipPostalCode");


            bool isDomestic = IsDomesticRequest(shipmentPackage);
            string requestString = CreateRequest(username, password, shipmentPackage);
            string responseXML = DoRequest(url, requestString);
            shippingOptions = ParseResponse(responseXML, isDomestic, ref error);
            foreach (var shippingOption in shippingOptions)
            {
                if (!shippingOption.Name.ToLower().StartsWith("usps"))
                    shippingOption.Name = string.Format("USPS {0}", shippingOption.Name);
                shippingOption.Rate += additionalHandlingCharge;
            }

            return shippingOptions;
        }

        /// <summary>
        /// Gets fixed shipping rate (if shipping rate computation method allows it and the rate can be calculated before checkout).
        /// </summary>
        /// <param name="shipmentPackage">Shipment package</param>
        /// <returns>Fixed shipping rate; or null if shipping rate could not be calculated before checkout</returns>
        public decimal? GetFixedRate(ShipmentPackage shipmentPackage)
        {
            return null;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets a shipping rate computation method type
        /// </summary>
        /// <returns>A shipping rate computation method type</returns>
        public ShippingRateComputationMethodTypeEnum ShippingRateComputationMethodType
        {
            get
            {
                return ShippingRateComputationMethodTypeEnum.Realtime;
            }
        }

        #endregion
    }
}