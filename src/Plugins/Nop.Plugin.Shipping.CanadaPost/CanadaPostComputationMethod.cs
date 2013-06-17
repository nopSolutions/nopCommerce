using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web.Routing;
using System.Xml;
using System.Xml.Linq;
using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Plugin.Shipping.CanadaPost.Domain;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.CanadaPost
{
    /// <summary>
    /// Canada post computation method
    /// </summary>
    public class CanadaPostComputationMethod : BasePlugin, IShippingRateComputationMethod
    {
        #region Constants

        private const int MAXPACKAGEWEIGHT = 30;

        #endregion

        #region Fields

        private readonly IMeasureService _measureService;
        private readonly IShippingService _shippingService;
        private readonly ISettingService _settingService;
        private readonly CanadaPostSettings _canadaPostSettings;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor
        public CanadaPostComputationMethod(IMeasureService measureService,
            IShippingService shippingService, ISettingService settingService,
            CanadaPostSettings canadaPostSettings, IWorkContext workContext)
        {
            this._measureService = measureService;
            this._shippingService = shippingService;
            this._settingService = settingService;
            this._canadaPostSettings = canadaPostSettings;
            this._workContext = workContext;
        }
        #endregion
        
        #region Utilities

        /// <summary>
        /// Sends the message to CanadaPost.
        /// </summary>
        /// <param name="eParcelMessage">The e parcel message.</param>
        /// <returns></returns>
        private string SendMessage(string eParcelMessage)
        {
            using (var socCanadaPost = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socCanadaPost.ReceiveTimeout = 12000;
                var remoteEndPoint = new IPEndPoint(Dns.GetHostAddresses(_canadaPostSettings.Url)[0], _canadaPostSettings.Port);

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
        /// <param name="canadaPostResponse">The response from Canada Post.</param>
        /// /// <param name="language">The language.</param>
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
                XElement error = query.FirstOrDefault();
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
                XElement info = query.FirstOrDefault();
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

        private List<Item> CreateItems(GetShippingOptionRequest getShippingOptionRequest)
        {
            var result = new List<Item>();

            var usedMeasureWeight = _measureService.GetMeasureWeightBySystemKeyword("kg");
            if (usedMeasureWeight == null)
                throw new NopException("CanadaPost shipping service. Could not load \"kg\" measure weight");

            var usedMeasureDimension = _measureService.GetMeasureDimensionBySystemKeyword("meters");
            if (usedMeasureDimension == null)
                throw new NopException("CanadaPost shipping service. Could not load \"meter(s)\" measure dimension");

            foreach (var sci in getShippingOptionRequest.Items)
            {
                var product = sci.Product;

                var item = new Item();
                item.Quantity = sci.Quantity;
                //Canada Post uses kg(s)

                decimal unitWeight = _shippingService.GetShoppingCartItemTotalWeight(sci) / sci.Quantity;
                item.Weight = _measureService.ConvertFromPrimaryMeasureWeight(unitWeight, usedMeasureWeight);
                item.Weight = Math.Round(item.Weight, 2);
                if (item.Weight == decimal.Zero)
                    item.Weight = 0.01M;

                //Canada Post uses centimeters                
                item.Length = Convert.ToInt32(Math.Ceiling(_measureService.ConvertFromPrimaryMeasureDimension(product.Length, usedMeasureDimension) * 100));
                if (item.Length == decimal.Zero)
                    item.Length = 1;
                item.Width = Convert.ToInt32(Math.Ceiling(_measureService.ConvertFromPrimaryMeasureDimension(product.Width, usedMeasureDimension) * 100));
                if (item.Width == decimal.Zero)
                    item.Width = 1;
                item.Height = Convert.ToInt32(Math.Ceiling(_measureService.ConvertFromPrimaryMeasureDimension(product.Height, usedMeasureDimension) * 100));
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
            if (getShippingOptionRequest.ShippingAddress.StateProvince == null)
            {
                response.AddError("Shipping state is not set");
                return response;
            }

            try
            {
                var profile = new Profile();
                profile.MerchantId = _canadaPostSettings.CustomerId;

                var destination = new Destination();
                destination.City = getShippingOptionRequest.ShippingAddress.City;
                destination.StateOrProvince = getShippingOptionRequest.ShippingAddress.StateProvince.Abbreviation;
                destination.Country = getShippingOptionRequest.ShippingAddress.Country.TwoLetterIsoCode;
                destination.PostalCode = getShippingOptionRequest.ShippingAddress.ZipPostalCode;

                var items = CreateItems(getShippingOptionRequest);

                var lang = CanadaPostLanguageEnum.English;
                if (_workContext.WorkingLanguage.LanguageCulture.StartsWith("fr", StringComparison.InvariantCultureIgnoreCase))
                    lang = CanadaPostLanguageEnum.French;

                var requestResult = GetShippingOptionsInternal(profile, destination, items, lang);
                if (requestResult.IsError)
                {
                    response.AddError(requestResult.StatusMessage);
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
                        response.ShippingOptions.Add(so);
                    }
                }

                foreach (var shippingOption in response.ShippingOptions)
                {
                    if (!shippingOption.Name.StartsWith("canada post", StringComparison.InvariantCultureIgnoreCase))
                        shippingOption.Name = string.Format("Canada Post {0}", shippingOption.Name);
                }
            }
            catch (Exception e)
            {
                response.AddError(e.Message);
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
            controllerName = "ShippingCanadaPost";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Shipping.CanadaPost.Controllers" }, { "area", null } };
        }
        
        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new CanadaPostSettings()
            {
                Url = "sellonline.canadapost.ca",
                Port = 30000,
                //use "CPC_DEMO_XML" merchant ID for testing
                CustomerId = "CPC_DEMO_XML" 
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.Url", "Canada Post URL");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.Url.Hint", "Specify Canada Post URL.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.Port", "Canada Post Port");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.Port.Hint", "Specify Canada Post port.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.CustomerId", "Canada Post Customer ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.CustomerId.Hint", "Specify Canada Post customer identifier.");
            
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
            this.DeletePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.Url");
            this.DeletePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.Url.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.Port");
            this.DeletePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.Port.Hint");
            this.DeletePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.CustomerId");
            this.DeletePluginLocaleResource("Plugins.Shipping.CanadaPost.Fields.CustomerId.Hint");
            
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