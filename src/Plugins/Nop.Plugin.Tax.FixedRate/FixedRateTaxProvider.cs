using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Nop.Core.Plugins;
using Nop.Services.Tax;
using Nop.Services.Configuration;

namespace Nop.Plugin.Tax.FixedRate
{
    /// <summary>
    /// Fixed rate tax provider
    /// </summary>
    public class FixedRateTaxProvider : BasePlugin, ITaxProvider
    {
        private ISettingService _settingService;

        public FixedRateTaxProvider(ISettingService settingService)
        {
            _settingService = settingService;
        }

        /// <summary>
        /// Gets or sets the friendly name
        /// </summary>
        public override string FriendlyName
        {
            get
            {
                return "Fixed tax rate provider";
            }
        }

        /// <summary>
        /// Gets or sets the system name
        /// </summary>
        public override string SystemName
        {
            get
            {
                return "Tax.FixedRate";
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
            decimal rate = this._settingService.GetSettingByKey<decimal>(string.Format("Tax.TaxProvider.FixedRate.TaxCategoryId{0}", taxCategoryId));
            return rate;
        }
    }
}
