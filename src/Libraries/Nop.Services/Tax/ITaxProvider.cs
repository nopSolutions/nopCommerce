using Nop.Services.Plugins;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Provides an interface for creating tax providers
    /// </summary>
    public partial interface ITaxProvider : IPlugin
    {
        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="taxRateRequest">Tax rate request</param>
        /// <returns>Tax</returns>
        TaxRateResult GetTaxRate(TaxRateRequest taxRateRequest);

        /// <summary>
        /// Gets tax total
        /// </summary>
        /// <param name="taxTotalRequest">Tax total request</param>
        /// <returns>Tax total</returns>
        TaxTotalResult GetTaxTotal(TaxTotalRequest taxTotalRequest);
    }
}
