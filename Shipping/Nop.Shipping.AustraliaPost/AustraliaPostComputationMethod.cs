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
// Contributor(s): 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Measures;
using NopSolutions.NopCommerce.BusinessLogic.Shipping;
using NopSolutions.NopCommerce.Common;

namespace NopSolutions.NopCommerce.Shipping.Methods.AustraliaPost
{
    /// <summary>
    /// Australia post computation method
    /// </summary>
    public class AustraliaPostComputationMethod : IShippingRateComputationMethod
    {
        #region Constants

        private const int MIN_LENGTH = 50; // 5 cm
        private const int MIN_WEIGHT = 1; // 1 g
        private const int MAX_LENGTH = 1050; // 105 cm
        private const int MAX_WEIGHT = 20000; // 20 Kg
        private const int MIN_GIRTH = 160; // 16 cm
        private const int MAX_GIRTH = 1050; // 105 cm

        #endregion

        #region Utilities

        private static int GetWeight(ShipmentPackage shipmentPackage)
        {
            int value = Convert.ToInt32(Math.Ceiling(MeasureManager.ConvertWeight(ShippingManager.GetShoppingCartTotalWeight(shipmentPackage.Items, shipmentPackage.Customer), MeasureManager.BaseWeightIn, AustraliaPostSettings.MeasureWeight)));
            return (value < MIN_WEIGHT ? MIN_WEIGHT : value);
        }

        private static int GetLength(ShipmentPackage shipmentPackage)
        {
            int value = Convert.ToInt32(Math.Ceiling(MeasureManager.ConvertDimension(shipmentPackage.GetTotalLength(), MeasureManager.BaseDimensionIn, AustraliaPostSettings.MeasureDimension)));
            return (value < MIN_LENGTH ? MIN_LENGTH : value);
        }

        private static int GetWidth(ShipmentPackage shipmentPackage)
        {
            int value = Convert.ToInt32(Math.Ceiling(MeasureManager.ConvertDimension(shipmentPackage.GetTotalWidth(), MeasureManager.BaseDimensionIn, AustraliaPostSettings.MeasureDimension)));
            return (value < MIN_LENGTH ? MIN_LENGTH : value);
        }

        private static int GetHeight(ShipmentPackage shipmentPackage)
        {
            int value = Convert.ToInt32(Math.Ceiling(MeasureManager.ConvertDimension(shipmentPackage.GetTotalHeight(), MeasureManager.BaseDimensionIn, AustraliaPostSettings.MeasureDimension)));
            return (value < MIN_LENGTH ? MIN_LENGTH : value);
        }
        
        private static ShippingOption RequestShippingOption(string zipPostalCodeFrom, 
            string zipPostalCodeTo, string countryCode, string serviceType, 
            int weight, int length, int width, int height, int quantity)
        {
            ShippingOption shippingOption = new ShippingOption();
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Pickup_Postcode={0}&", zipPostalCodeFrom);
            sb.AppendFormat("Destination_Postcode={0}&", zipPostalCodeTo);
            sb.AppendFormat("Country={0}&", countryCode);
            sb.AppendFormat("Service_Type={0}&", serviceType);
            sb.AppendFormat("Weight={0}&", weight);
            sb.AppendFormat("Length={0}&", length);
            sb.AppendFormat("Width={0}&", width);
            sb.AppendFormat("Height={0}&", height);
            sb.AppendFormat("Quantity={0}&", quantity);

            HttpWebRequest request = WebRequest.Create(AustraliaPostSettings.GatewayUrl) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] reqContent = Encoding.ASCII.GetBytes(sb.ToString());
            request.ContentLength = reqContent.Length;
            using (Stream newStream = request.GetRequestStream())
            {
                newStream.Write(reqContent, 0, reqContent.Length);
            }

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

            NameValueCollection rspParams = new NameValueCollection();
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

            shippingOption.Name = serviceType;
            shippingOption.Description = String.Format("{0} Days", rspParams["days"]);
            shippingOption.Rate = Decimal.Parse(rspParams["charge"]);

            return shippingOption;
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
            {
                throw new ArgumentNullException("shipmentPackage");
            }
            if (shipmentPackage.Items == null)
            {
                throw new NopException("No shipment items");
            }
            if (shipmentPackage.ShippingAddress == null)
            {
                error = "Shipping address is not set";
                return shippingOptions;
            }

            shipmentPackage.ZipPostalCodeFrom = AustraliaPostSettings.ShippedFromZipPostalCode;
            string zipPostalCodeFrom = shipmentPackage.ZipPostalCodeFrom;
            string zipPostalCodeTo = shipmentPackage.ShippingAddress.ZipPostalCode;
            int weight = GetWeight(shipmentPackage);
            int length = GetLength(shipmentPackage);
            int width = GetWidth(shipmentPackage);
            int height = GetHeight(shipmentPackage);

            Country country = shipmentPackage.ShippingAddress.Country;

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
                        shippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "Standard", weight, length, width, height, totalPackages));
                        shippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "Express", weight, length, width, height, totalPackages));
                        shippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "EXP_PLT", weight, length, width, height, totalPackages));
                        break;
                    default:
                        shippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "Air", weight, length, width, height, totalPackages));
                        shippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "Sea", weight, length, width, height, totalPackages));
                        shippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "ECI_D", weight, length, width, height, totalPackages));
                        shippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "ECI_M", weight, length, width, height, totalPackages));
                        shippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "EPI", weight, length, width, height, totalPackages));
                        break;
                }

                foreach (ShippingOption shippingOption in shippingOptions)
                {
                    shippingOption.Rate += AustraliaPostSettings.AdditionalHandlingCharge;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
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

