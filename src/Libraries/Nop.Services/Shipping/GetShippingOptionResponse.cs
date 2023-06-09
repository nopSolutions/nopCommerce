using Nop.Core.Domain.Shipping;

namespace Nop.Services.Shipping
{
    /// <summary>
    /// Represents a response of getting shipping rate options
    /// </summary>
    public partial class GetShippingOptionResponse
    {
        public GetShippingOptionResponse()
        {
            Errors = new List<string>();
            ShippingOptions = new List<ShippingOption>();
        }

        /// <summary>
        /// Gets or sets a list of shipping options
        /// </summary>
        public IList<ShippingOption> ShippingOptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether shipping is done from multiple locations (warehouses)
        /// </summary>
        public bool ShippingFromMultipleLocations { get; set; }

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