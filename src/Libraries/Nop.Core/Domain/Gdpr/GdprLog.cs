using System;

namespace Nop.Core.Domain.Gdpr
{
    /// <summary>
    /// Represents a GDPR log
    /// </summary>
    public partial class GdprLog : BaseEntity
    {
        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the consent identifier (0 if not related to any consent)
        /// </summary>
        public int ConsentId { get; set; }

        /// <summary>
        /// Gets or sets the customer info (when a customer records is already deleted)
        /// </summary>
        public string CustomerInfo { get; set; }

        /// <summary>
        /// Gets or sets the request type identifier
        /// </summary>
        public int RequestTypeId { get; set; }

        /// <summary>
        /// Gets or sets the request details
        /// </summary>
        public string RequestDetails { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the request type
        /// </summary>
        public GdprRequestType RequestType
        {
            get => (GdprRequestType)RequestTypeId;
            set => RequestTypeId = (int)value;
        }
    }
}
