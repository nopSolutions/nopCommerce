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
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Linq;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Measures;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.BusinessLogic.Utils;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.BusinessLogic.IoC;

namespace NopSolutions.NopCommerce.Shipping.Methods.CanadaPost
{
    /// <summary>
    /// FedEx computation method
    /// </summary>
    public class CanadaPostComputationMethod : IShippingRateComputationMethod
    {
        #region Const
        private const int MAXPACKAGEWEIGHT = 30;
        #endregion

        #region Utilities
        /// <summary>
        /// Sends the message to CanadaPost.
        /// </summary>
        /// <param name="eParcelMessage">The e parcel message.</param>
        /// <returns></returns>
        private string SendMessage(string eParcelMessage)
        {
            using (Socket socCanadaPost = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socCanadaPost.ReceiveTimeout = 12000;
                string url = IoCFactory.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.CanadaPost.URL");
                int port = IoCFactory.Resolve<ISettingManager>().GetSettingValueInteger("ShippingRateComputationMethod.CanadaPost.Port");
                IPEndPoint remoteEndPoint = new IPEndPoint(Dns.GetHostAddresses(url)[0], port);

                socCanadaPost.Connect(remoteEndPoint);
                byte[] data = System.Text.Encoding.ASCII.GetBytes(eParcelMessage);
                socCanadaPost.Send(data);

                string resp = String.Empty;
                byte[] buffer = new byte[8192];
                int iRx = 0;

                while (!resp.Contains("<!--END_OF_EPARCEL-->"))
                {
                    try
                    {
                        iRx = socCanadaPost.Receive(buffer, 0, 8192, SocketFlags.None);
                    }
                    catch (SocketException e)
                    {
                        if (e.SocketErrorCode == SocketError.TimedOut)
                            break;
                        else
                            throw e;
                    }
                    if (iRx > 0)
                    {
                        resp += new string((System.Text.Encoding.UTF8.GetChars(buffer, 0, iRx)));
                    }
                }
                return resp;
            }
        }

        /// <summary>
        /// Handles the result.
        /// </summary>
        /// <param name="canadaPostResult">The result from Canada Post.</param>
        /// <returns></returns>
        private RequestResult HandleResult(string canadaPostResponse, CanadaPostLanguageEnum language)
        {
            var result = new RequestResult();
            if (String.IsNullOrEmpty(canadaPostResponse))
            {
                result.IsError = true;
                result.StatusCode = 0;
                result.StatusMessage = "Unable to connect to Canada Post.";
                return result;
            }
            var doc = new XmlDocument();
            doc.LoadXml(canadaPostResponse);

            XElement resultRates = XElement.Load(new StringReader(canadaPostResponse));
            IEnumerable<XElement> query;
            // if we have any errors
            if (doc.GetElementsByTagName("error").Count > 0)
            {
                // query using LINQ the "error" node in the XML
                query = from errors in resultRates.Elements("error")
                        select errors;
                XElement error = query.First();
                if (error != null)
                {
                    // set the status code information of the request
                    result.StatusCode = Convert.ToInt32(error.Element("statusCode").Value);
                    result.StatusMessage = error.Element("statusMessage").Value;
                    result.IsError = true;
                }
            }
            else
            {
                // query using LINQ the "ratesAndServicesResponse" node in the XML because it contains 
                // the actual status code information
                query = from response in resultRates.Elements("ratesAndServicesResponse")
                        select response;
                XElement info = query.First();
                // if we have informations
                if (info != null)
                {
                    // set the status code information of the request
                    result.StatusCode = Convert.ToInt32(info.Element("statusCode").Value);
                    result.StatusMessage = info.Element("statusMessage").Value;
                    // query using LINQ all the returned "product" nodes in the XML
                    query = from prod in resultRates.Elements("ratesAndServicesResponse").Elements("product")
                            select prod;
                    foreach (XElement product in query)
                    {
                        // set the information related to this available rate
                        var rate = new DeliveryRate();
                        rate.Sequence = Convert.ToInt32(product.Attribute("sequence").Value);
                        rate.Name = product.Element("name").Value;
                        rate.Amount = Convert.ToDecimal(product.Element("rate").Value, new CultureInfo("en-US", false).NumberFormat);
                        DateTime shipDate;
                        if (DateTime.TryParse(product.Element("shippingDate").Value, out shipDate) == true)
                        {
                            rate.ShippingDate = shipDate;
                        }
                        
                        DateTime delivDate;
                        if (DateTime.TryParse(product.Element("deliveryDate").Value, out delivDate) == true)
                        {
                            CultureInfo culture;
                            if (language == CanadaPostLanguageEnum.French)
                            {
                                culture = new CultureInfo("fr-ca");
                                rate.DeliveryDate = delivDate.ToString("d MMMM yyyy", culture);
                            }
                            else
                            {
                                culture = new CultureInfo("en-us");
                                rate.DeliveryDate = delivDate.ToString("MMMM d, yyyy", culture);
                            }
                        }
                        else
                        {
                            //rate.DeliveryDate = product.Element("deliveryDate").Value;
                            rate.DeliveryDate = string.Empty;
                        }
                        result.AvailableRates.Add(rate);
                    }
                    query = from packing in resultRates.Elements("ratesAndServicesResponse").Elements("packing").Elements("box")
                            select packing;
                    foreach (XElement packing in query)
                    {
                        var box = new BoxDetail();
                        box.Name = packing.Element("name").Value;
                        box.Weight = Convert.ToDouble(packing.Element("weight").Value, new CultureInfo("en-US", false).NumberFormat);
                        box.ExpediterWeight = Convert.ToDouble(packing.Element("expediterWeight").Value, new CultureInfo("en-US", false).NumberFormat);
                        box.Length = Convert.ToDouble(packing.Element("length").Value, new CultureInfo("en-US", false).NumberFormat);
                        box.Width = Convert.ToDouble(packing.Element("width").Value, new CultureInfo("en-US", false).NumberFormat);
                        box.Height = Convert.ToDouble(packing.Element("height").Value, new CultureInfo("en-US", false).NumberFormat);
                        box.Quantity = Convert.ToInt32(packing.Element("packedItem").Element("quantity").Value);
                        // add the box to the result
                        result.Boxes.Add(box);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the shipping options.
        /// </summary>
        /// <param name="profile">The profile.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="items">The items.</param>
        /// <param name="language">The language.</param>
        /// <returns></returns>
        private RequestResult GetShippingOptionsInternal(Profile profile, Destination destination, List<Item> items, CanadaPostLanguageEnum language)
        {
            var parcel = new eParcelBuilder(profile, destination, items, language);
            string result = SendMessage(parcel.GetMessage(true));
            return HandleResult(result, language);
        }

        private List<Item> CreateItems(ShipmentPackage ShipmentPackage)
        {
            var  result = new List<Item>();

            var usedMeasureWeight = IoCFactory.Resolve<IMeasureManager>().GetMeasureWeightBySystemKeyword("kg");
            if (usedMeasureWeight == null)
                throw new NopException("CanadaPost shipping service. Could not load \"kg\" measure weight");

            var usedMeasureDimension = IoCFactory.Resolve<IMeasureManager>().GetMeasureDimensionBySystemKeyword("meters");
            if (usedMeasureDimension == null)
                throw new NopException("CanadaPost shipping service. Could not load \"meter(s)\" measure dimension");

            foreach (var sci in ShipmentPackage.Items)
            {
                var pv = sci.ProductVariant;

                var item = new Item();
                item.Quantity = sci.Quantity;
                //Canada Post uses kg(s)
                decimal unitWeight = sci.TotalWeight / sci.Quantity;
                item.Weight = IoCFactory.Resolve<IMeasureManager>().ConvertWeight(unitWeight, IoCFactory.Resolve<IMeasureManager>().BaseWeightIn, usedMeasureWeight);
                item.Weight = Math.Round(item.Weight, 2);
                if (item.Weight == decimal.Zero)
                    item.Weight = 0.01M;
                
                //Canada Post uses centimeters                
                item.Length = Convert.ToInt32(Math.Ceiling(IoCFactory.Resolve<IMeasureManager>().ConvertDimension(pv.Length, IoCFactory.Resolve<IMeasureManager>().BaseDimensionIn, usedMeasureDimension) * 100));
                if (item.Length == decimal.Zero)
                    item.Length = 1;
                item.Width = Convert.ToInt32(Math.Ceiling(IoCFactory.Resolve<IMeasureManager>().ConvertDimension(pv.Width, IoCFactory.Resolve<IMeasureManager>().BaseDimensionIn, usedMeasureDimension) * 100));
                if (item.Width == decimal.Zero)
                    item.Width = 1;
                item.Height = Convert.ToInt32(Math.Ceiling(IoCFactory.Resolve<IMeasureManager>().ConvertDimension(pv.Height, IoCFactory.Resolve<IMeasureManager>().BaseDimensionIn, usedMeasureDimension) * 100));
                if (item.Height == decimal.Zero)
                    item.Height = 1;
                result.Add(item);
            }

            return result;
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
            if (shipmentPackage.ShippingAddress.Country == null)
            {
                error = "Shipping country is not set";
                return shippingOptions;
            }
            if (shipmentPackage.ShippingAddress.StateProvince == null)
            {
                error = "Shipping state is not set";
                return shippingOptions;
            }

            try
            {
                var profile = new Profile();
                //use "CPC_DEMO_XML" merchant ID for testing
                profile.MerchantId = IoCFactory.Resolve<ISettingManager>().GetSettingValue("ShippingRateComputationMethod.CanadaPost.CustomerID");

                var destination = new Destination();
                destination.City = shipmentPackage.ShippingAddress.City;
                destination.StateOrProvince = shipmentPackage.ShippingAddress.StateProvince.Abbreviation;
                destination.Country = shipmentPackage.ShippingAddress.Country.TwoLetterIsoCode;
                destination.PostalCode = shipmentPackage.ShippingAddress.ZipPostalCode;

                var items = CreateItems(shipmentPackage);

                var lang = CanadaPostLanguageEnum.English;
                if (NopContext.Current.WorkingLanguage.LanguageCulture.ToLowerInvariant().StartsWith("fr"))
                    lang = CanadaPostLanguageEnum.French;

                var requestResult = GetShippingOptionsInternal(profile, destination, items, lang);
                if (requestResult.IsError)
                {
                    error = requestResult.StatusMessage;
                }
                else
                {
                    foreach (var dr in requestResult.AvailableRates)
                    {
                        var so = new ShippingOption();
                        so.Name = dr.Name;
                        if (!string.IsNullOrEmpty(dr.DeliveryDate))
                            so.Name += string.Format(" - {0}", dr.DeliveryDate);
                        so.Rate = dr.Amount;
                        shippingOptions.Add(so);
                    }
                }

                foreach (var shippingOption in shippingOptions)
                {
                    if (!shippingOption.Name.ToLower().StartsWith("canada post"))
                        shippingOption.Name = string.Format("Canada Post {0}", shippingOption.Name);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                error = e.Message;
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
