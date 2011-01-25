using System;
using System.Collections.Generic;
using Nop.Core.Domain.Shipping;
using Nop.Services.Shipping;
using Nop.Services.Configuration;
using Nop.Services.Directory;

namespace Nop.Shipping.FixedRateShipping
{
    /// <summary>
    /// Fixed rate shipping computation method
    /// </summary>
    public class FixedRateShippingComputationMethod : IShippingRateComputationMethod
    {
        private decimal GetRate(int shippingMethodId)
        {
            string key = string.Format("ShippingRateComputationMethod.FixedRate.Rate.ShippingMethodId{0}", shippingMethodId);
            decimal rate = this.SettingService.GetSettingByKey<decimal>(key);
            return rate;
        }

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
            
            //TODO uncomment vode below after "restricting shipping by country" is implemented
            //var shippingMethods = this.ShippingService.GetAllShippingMethods(shipmentPackage.ShippingAddress.CountryId);
            var shippingMethods = this.ShippingService.GetAllShippingMethods();
            foreach (var shippingMethod in shippingMethods)
            {
                var shippingOption = new ShippingOption();
                shippingOption.Name = shippingMethod.Name;
                shippingOption.Description = shippingMethod.Description;
                shippingOption.Rate = GetRate(shippingMethod.Id);
                response.ShippingOptions.Add(shippingOption);
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
            if (getShippingOptionRequest == null)
                throw new ArgumentNullException("getShippingOptionRequest");

            if (getShippingOptionRequest.Items == null)
                return null;
            if (getShippingOptionRequest.ShippingAddress == null)
                return null;
            if (getShippingOptionRequest.ShippingAddress.Country == null)
                return null;

            //TODO uncomment vode below after "restricting shipping by country" is implemented
            //var shippingMethods = this.ShippingService.GetAllShippingMethods(shipmentPackage.ShippingAddress.CountryId);
            var shippingMethods = this.ShippingService.GetAllShippingMethods();
            var rates = new List<decimal>();
            foreach (var shippingMethod in shippingMethods)
            {
                decimal rate = GetRate(shippingMethod.Id);
                if (!rates.Contains(rate))
                    rates.Add(rate);
            }

            //return default rate if all of them equal
            if (rates.Count == 1)
                return rates[0];

            return null;
        }
        
        #region Properties

        /// <summary>
        /// Gets or sets the friendly name
        /// </summary>
        public string FriendlyName
        {
            get
            {
                return "Fixed Rate Shipping";
            }
        }

        /// <summary>
        /// Gets or sets the system name
        /// </summary>
        public string SystemName
        {
            get
            {
                return "FixedRateShipping";
            }
        }

        /// <summary>
        /// Gets a shipping rate computation method type
        /// </summary>
        public ShippingRateComputationMethodType ShippingRateComputationMethodType
        {
            get
            {
                return ShippingRateComputationMethodType.Offline;
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
