using Nop.Core.Domain.Attributes;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents a checkout attribute value
    /// </summary>
    public partial class CheckoutAttributeValue : BaseAttributeValue
    {
        /// <summary>
        /// Gets or sets the color RGB value (used with "Color squares" attribute type)
        /// </summary>
        public string ColorSquaresRgb { get; set; }

        /// <summary>
        /// Gets or sets the price adjustment
        /// </summary>
        public decimal PriceAdjustment { get; set; }

        /// <summary>
        /// Gets or sets the weight adjustment
        /// </summary>
        public decimal WeightAdjustment { get; set; }
    }
}
