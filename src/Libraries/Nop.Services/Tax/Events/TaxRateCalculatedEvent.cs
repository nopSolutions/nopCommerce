namespace Nop.Services.Tax.Events
{
    /// <summary>
    /// Represents an event that raised when tax rate is calculated
    /// </summary>
    public class TaxRateCalculatedEvent
    {
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="taxRateResult">Tax rate result</param>
        public TaxRateCalculatedEvent(TaxRateResult taxRateResult)
        {
            TaxRateResult = taxRateResult;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the tax rate result
        /// </summary>
        public TaxRateResult TaxRateResult { get; }

        #endregion
    }
}