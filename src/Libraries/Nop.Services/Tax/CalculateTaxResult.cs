using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Represents a result of tax calculation
    /// </summary>
    public partial class CalculateTaxResult
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public CalculateTaxResult()
        {
            this.Errors = new List<string>();
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
        public bool Success
        {
            get 
            { 
                return !this.Errors.Any(); 
            }
        }

        /// <summary>
        /// Add error
        /// </summary>
        /// <param name="error">Error</param>
        public void AddError(string error)
        {
            this.Errors.Add(error);
        }
    }
}
