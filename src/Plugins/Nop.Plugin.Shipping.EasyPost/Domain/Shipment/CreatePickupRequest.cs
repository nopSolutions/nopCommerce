using System;
using Nop.Core.Domain.Common;

namespace Nop.Plugin.Shipping.EasyPost.Domain.Shipment
{
    /// <summary>
    /// Represents the details to create a pickup
    /// </summary>
    public class CreatePickupRequest
    {
        /// <summary>
        /// Gets or sets the additional text to help the driver successfully obtain the package
        /// </summary>
        public string Instructions { get; set; }

        /// <summary>
        /// Gets or sets the earliest time at which the package is available to pick up
        /// </summary>
        public DateTime MinDate { get; set; }

        /// <summary>
        /// Gets or sets the latest time at which the package is available to pick up
        /// </summary>
        public DateTime MaxDate { get; set; }

        /// <summary>
        /// Gets or sets the associated address
        /// </summary>
        public Address Address { get; set; }
    }
}