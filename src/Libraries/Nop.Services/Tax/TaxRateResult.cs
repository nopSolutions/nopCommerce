using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Represents a result of tax rate calculation
    /// </summary>
    public partial class TaxRateResult
    {
        public TaxRateResult()
        {
            Errors = new List<string>();
        }

        /// <summary>
        /// Gets or sets a tax rate
        /// </summary>
        public decimal TaxRate { get; set; }

        /// <summary>
        /// Gets or sets errors
        /// </summary>
        public IList<string> Errors { get; set; }

        /// <summary>
        /// Gets a value indicating whether request has been completed successfully
        /// </summary>
        public bool Success => !Errors.Any();

        /// <summary>
        /// Add error
        /// </summary>
        /// <param name="error">Error</param>
        public void AddError(string error)
        {
            Errors.Add(error);
        }
    }
}