using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Tax;
using Nop.Services.Configuration;

namespace Nop.Services.Tests.Tax
{
    public class FixedRateTestTaxProvider : BasePlugin, ITaxProvider
    {
        public override PluginDescriptor PluginDescriptor
        {
            get
            {
                return new PluginDescriptor()
                {
                    Author = "nopCommerce team",
                    FriendlyName = "Fixed tax test rate provider",
                    SystemName = "FixedTaxRateTest",
                    Version = "1.00"
                };
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public CalculateTaxResult GetTaxRate(CalculateTaxRequest calculateTaxRequest)
        {
            var result = new CalculateTaxResult()
            {
                TaxRate = GetTaxRate(calculateTaxRequest.TaxCategoryId)
            };
            return result;
        }

        /// <summary>
        /// Gets a tax rate
        /// </summary>
        /// <param name="taxCategoryId">The tax category identifier</param>
        /// <returns>Tax rate</returns>
        protected decimal GetTaxRate(int taxCategoryId)
        {
            decimal rate = 10;
            return rate;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = null;
            controllerName = null;
            routeValues = null;
        }
    }
}
