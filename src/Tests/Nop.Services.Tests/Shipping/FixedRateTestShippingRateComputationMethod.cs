using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Domain.Shipping;
using Nop.Services.Directory;
using Nop.Services.Shipping;
using Nop.Services.Configuration;
using Nop.Services.Tax;

namespace Nop.Services.Tests.Shipping
{
    public class FixedRateTestShippingRateComputationMethod : IShippingRateComputationMethod
    {
        private decimal GetRate()
        {
            decimal rate = 10M;
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
            response.ShippingOptions.Add(new ShippingOption()
                {
                    Name = "Shipping option 1",
                    Description = "",
                    Rate = GetRate()
                }); 
            response.ShippingOptions.Add(new ShippingOption()
                {
                    Name = "Shipping option 2",
                    Description = "",
                    Rate = GetRate()
                });

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

            return GetRate();
        }

        #region Properties


        public string FriendlyName
        {
            get
            {
                return "Fixed rate shipping computation method";
            }
        }

        public string SystemName
        {
            get
            {
                return "FixedRateTestShippingRateComputationMethod";
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
