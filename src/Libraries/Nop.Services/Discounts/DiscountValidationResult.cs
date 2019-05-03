using System.Collections.Generic;

namespace Nop.Services.Discounts
{
    /// <summary>
    /// Represents a result of discount validation
    /// </summary>
    public partial class DiscountValidationResult
    {
        public DiscountValidationResult()
        {
            Errors = new List<string>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether discount is valid
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the errors that a customer should see when entering a coupon code (in case if "IsValid" is set to "false")
        /// </summary>
        public IList<string> Errors { get; set; }
    }
}