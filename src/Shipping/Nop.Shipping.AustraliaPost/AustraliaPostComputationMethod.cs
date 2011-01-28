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
using Nop.Services.Shipping;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Core.Domain.Shipping;
using Nop.Core;
using Nop.Services.Orders;
using Nop.Core.Domain.Directory;

namespace Nop.Shipping.AustraliaPost
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

        private MeasureWeight GatewayMeasureWeight
        {
            get
            {
                return this.MeasureService.GetMeasureWeightBySystemKeyword("grams");
            }
        }

        private MeasureDimension GatewayMeasureDimension
        {
            get
            {
                return this.MeasureService.GetMeasureDimensionBySystemKeyword("millimetres");
            }
        }

        private string GetGatewayUrl()
        {
            return this.SettingService.GetSettingByKey<string>("ShippingRateComputationMethod.AustraliaPost.GatewayUrl", "http://drc.edeliver.com.au/ratecalc.asp");
        }

        private int GetWeight(GetShippingOptionRequest getShippingOptionRequest)
        {
            var totalWeigth = this.ShippingService.GetShoppingCartTotalWeight(getShippingOptionRequest.Items);

            int value = Convert.ToInt32(Math.Ceiling(this.MeasureService.ConvertFromPrimaryMeasureWeight(totalWeigth, this.GatewayMeasureWeight)));
            return (value < MIN_WEIGHT ? MIN_WEIGHT : value);
        }

        private int GetLength(GetShippingOptionRequest getShippingOptionRequest)
        {
            int value = Convert.ToInt32(Math.Ceiling(this.MeasureService.ConvertFromPrimaryMeasureDimension(getShippingOptionRequest.GetTotalLength(), this.GatewayMeasureDimension)));
            return (value < MIN_LENGTH ? MIN_LENGTH : value);
        }

        private int GetWidth(GetShippingOptionRequest getShippingOptionRequest)
        {
            int value = Convert.ToInt32(Math.Ceiling(this.MeasureService.ConvertFromPrimaryMeasureDimension(getShippingOptionRequest.GetTotalWidth(), this.GatewayMeasureDimension)));
            return (value < MIN_LENGTH ? MIN_LENGTH : value);
        }

        private int GetHeight(GetShippingOptionRequest getShippingOptionRequest)
        {
            int value = Convert.ToInt32(Math.Ceiling(this.MeasureService.ConvertFromPrimaryMeasureDimension(getShippingOptionRequest.GetTotalHeight(), this.GatewayMeasureDimension)));
            return (value < MIN_LENGTH ? MIN_LENGTH : value);
        }

        private ShippingOption RequestShippingOption(string zipPostalCodeFrom,
            string zipPostalCodeTo, string countryCode, string serviceType,
            int weight, int length, int width, int height, int quantity)
        {
            var shippingOption = new ShippingOption();
            var sb = new StringBuilder();

            sb.AppendFormat("Pickup_Postcode={0}&", zipPostalCodeFrom);
            sb.AppendFormat("Destination_Postcode={0}&", zipPostalCodeTo);
            sb.AppendFormat("Country={0}&", countryCode);
            sb.AppendFormat("Service_Type={0}&", serviceType);
            sb.AppendFormat("Weight={0}&", weight);
            sb.AppendFormat("Length={0}&", length);
            sb.AppendFormat("Width={0}&", width);
            sb.AppendFormat("Height={0}&", height);
            sb.AppendFormat("Quantity={0}&", quantity);

            string gatewayUrl = GetGatewayUrl();
            HttpWebRequest request = WebRequest.Create(gatewayUrl) as HttpWebRequest;
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

            string zipPostalCodeFrom = this.SettingService.GetSettingByKey<string>("ShippingRateComputationMethod.AustraliaPost.DefaultShippedFromZipPostalCode");
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
                        response.ShippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "Standard", weight, length, width, height, totalPackages));
                        response.ShippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "Express", weight, length, width, height, totalPackages));
                        response.ShippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "EXP_PLT", weight, length, width, height, totalPackages));
                        break;
                    default:
                        response.ShippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "Air", weight, length, width, height, totalPackages));
                        response.ShippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "Sea", weight, length, width, height, totalPackages));
                        response.ShippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "ECI_D", weight, length, width, height, totalPackages));
                        response.ShippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "ECI_M", weight, length, width, height, totalPackages));
                        response.ShippingOptions.Add(RequestShippingOption(zipPostalCodeFrom, zipPostalCodeTo, country.TwoLetterIsoCode, "EPI", weight, length, width, height, totalPackages));
                        break;
                }

                foreach (var shippingOption in response.ShippingOptions)
                {
                    shippingOption.Rate += this.SettingService.GetSettingByKey<decimal>("ShippingRateComputationMethod.AustraliaPost.AdditionalHandlingCharge");
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
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the friendly name
        /// </summary>
        public string FriendlyName
        {
            get
            {
                return "Australia Post";
            }
        }

        /// <summary>
        /// Gets or sets the system name
        /// </summary>
        public string SystemName
        {
            get
            {
                return "AustraliaPost";
            }
        }

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
        /// Gets or sets the setting service
        /// </summary>
        public ISettingService SettingService { get; set; }

        /// <summary>
        /// Gets or sets the measure service
        /// </summary>
        public IMeasureService MeasureService { get; set; }

        /// <summary>
        /// Gets or sets the shipping service
        /// </summary>
        public IShippingService ShippingService { get; set; }

        #endregion
    }
}