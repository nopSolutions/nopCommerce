using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Nop.Core.Plugins;
using Nop.Services.Tax;
using Nop.Services.Configuration;

namespace Nop.Plugin.Tax.Free
{/// <summary>
    /// Free tax provider
    /// </summary>
    public class FreeTaxProvider : BasePlugin, ITaxProvider
    {
        /// <summary>
        /// Gets or sets the friendly name
        /// </summary>
        public override string FriendlyName
        {
            get
            {
                return "Free tax rate provider";
            }
        }

        /// <summary>
        /// Gets or sets the system name
        /// </summary>
        public override string SystemName
        {
            get
            {
                return "Tax.FreeTaxRate";
            }
        }
        
        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="calculateTaxRequest">Tax calculation request</param>
        /// <returns>Tax</returns>
        public CalculateTaxResult GetTaxRate(CalculateTaxRequest calculateTaxRequest)
        {
            var result = new CalculateTaxResult()
            {
                 TaxRate = decimal.Zero
            };
            return result;
        }
    }
}
