using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Orders;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Place order result
    /// </summary>
    public partial class PlaceOrderResult
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public PlaceOrderResult() 
        {
            this.Errors = new List<string>();
        }

        /// <summary>
        /// Gets a value indicating whether request has been completed successfully
        /// </summary>
        public bool Success
        {
            get { return (!this.Errors.Any()); }
        }

        /// <summary>
        /// Add error
        /// </summary>
        /// <param name="error">Error</param>
        public void AddError(string error)
        {
            this.Errors.Add(error);
        }

        /// <summary>
        /// Errors
        /// </summary>
        public IList<string> Errors { get; set; }
        
        /// <summary>
        /// Gets or sets the placed order
        /// </summary>
        public Order PlacedOrder { get; set; }
    }
}
