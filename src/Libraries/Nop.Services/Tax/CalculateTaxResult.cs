using System.Collections.Generic;

namespace Nop.Services.Tax
{
    /// <summary>
    /// Represents a result of tax calculation
    /// </summary>
    public partial class CalculateTaxResult
    {
        public CalculateTaxResult()
        {
            this.Errors = new List<string>();
        }

        /// <summary>
        /// Gets or sets a tax rate
        /// </summary>
        public decimal TaxRate { get; set; }

        /// <summary>
        /// Gets or sets an address
        /// </summary>
        public IList<string> Errors { get; set; }

        public bool Success
        {
            get 
            { 
                return this.Errors.Count == 0; 
            }
        }

        public void AddError(string error)
        {
            this.Errors.Add(error);
        }
    }
}
