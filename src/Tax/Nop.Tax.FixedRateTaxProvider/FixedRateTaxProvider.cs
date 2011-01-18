using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Services.Tax;
using Nop.Services.Configuration;

namespace Nop.Tax.FixedRateTaxProvider
{
    /// <summary>
    /// Fixed rate tax provider
    /// </summary>
    public class FixedRateTaxProvider: ITaxProvider
    {
        /// <summary>
        /// Gets or sets the friendly name
        /// </summary>
        public string FriendlyName
        {
            get
            {
                //TODO localize
                return "Fixed tax rate provider";
            }
        }

        /// <summary>
        /// Gets or sets the system name
        /// </summary>
        public string SystemName
        {
            get
            {
                return "FixedTaxRate";
            }
        }

        /// <summary>
        /// Gets or sets the setting service
        /// </summary>
        public ISettingService SettingService { get; set; }

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
            //TODO resolve or pass ISettingService?
            decimal rate = decimal.Zero;
            //decimal rate = IoC.Resolve<ISettingService>().GetSettingByKey<decimal>(string.Format("Tax.TaxProvider.FixedRate.TaxCategoryId{0}", taxCategoryId));
            return rate;
        }
    }
}
