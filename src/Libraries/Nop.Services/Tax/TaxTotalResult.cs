namespace Nop.Services.Tax
{
    /// <summary>
    /// Represents a result of tax total calculation
    /// </summary>
    public partial class TaxTotalResult
    {
        #region Ctor

        public TaxTotalResult()
        {
            TaxRates = new SortedDictionary<decimal, decimal>();
            Errors = new List<string>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a tax total
        /// </summary>
        public decimal TaxTotal { get; set; }

        /// <summary>
        /// Gets or sets tax rates
        /// </summary>
        public SortedDictionary<decimal, decimal> TaxRates { get; set; }

        /// <summary>
        /// Gets or sets errors
        /// </summary>
        public IList<string> Errors { get; set; }

        /// <summary>
        /// Gets a value indicating whether request has been completed successfully
        /// </summary>
        public bool Success => !Errors.Any();

        #endregion
    }
}