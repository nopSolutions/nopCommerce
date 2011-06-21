using System.Collections.Generic;
using Nop.Plugin.Tax.CountryStateZip.Domain;

namespace Nop.Plugin.Tax.CountryStateZip.Services
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Finds tax rate
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="countryId">Country identifier</param>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <returns>Found tax rates</returns>
        public static IList<TaxRate> FindTaxRates(this IList<TaxRate> source,
            int countryId, int taxCategoryId)
        {
            var result = new List<TaxRate>();
            foreach (TaxRate taxRate in source)
            {
                if (taxRate.CountryId == countryId && taxRate.TaxCategoryId == taxCategoryId)
                    result.Add(taxRate);
            }
            return result;
        }
    }
}
