using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Services.Tax;
using Nop.Services.Configuration;

namespace Nop.Services.Tests.Tax
{
    public class FixedRateTestTaxProvider : ITaxProvider
    {
        public string FriendlyName
        {
            get
            {
                return "Fixed tax rate provider";
            }
        }

        public string SystemName
        {
            get
            {
                return "FixedTaxRateTest";
            }
        }

        public ISettingService SettingService { get; set; }

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
    }
}
