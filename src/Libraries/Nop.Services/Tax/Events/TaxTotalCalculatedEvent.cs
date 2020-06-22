namespace Nop.Services.Tax.Events
{
    /// <summary>
    /// Represents an event that raised when tax total is calculated
    /// </summary>
    public class TaxTotalCalculatedEvent
    {
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="taxTotalResult">Tax total result</param>
        public TaxTotalCalculatedEvent(TaxTotalResult taxTotalResult)
        {
            TaxTotalResult = taxTotalResult;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the tax total result
        /// </summary>
        public TaxTotalResult TaxTotalResult { get; }

        #endregion
    }
}