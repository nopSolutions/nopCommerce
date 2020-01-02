using System.Collections.Generic;

namespace Nop.Plugin.Tax.Avalara.Domain
{
    /// <summary>
    /// Represents a tax details received from the Avalara tax service
    /// </summary>
    public class TaxDetails
    {
        #region Ctor

        public TaxDetails()
        {
            TaxRates = new Dictionary<decimal, decimal>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the tax total
        /// </summary>
        public decimal? TaxTotal { get; set; }

        /// <summary>
        /// Gets or sets tax rates
        /// </summary>
        public IDictionary<decimal, decimal> TaxRates { get; set; }

        #endregion
    }
}