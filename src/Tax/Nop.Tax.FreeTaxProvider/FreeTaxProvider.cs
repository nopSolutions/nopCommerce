using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Services.Tax;

namespace Nop.Tax.FreeTaxProvider
{/// <summary>
    /// Free tax provider
    /// </summary>
    public class FreeTaxProvider : ITaxProvider
    {
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
