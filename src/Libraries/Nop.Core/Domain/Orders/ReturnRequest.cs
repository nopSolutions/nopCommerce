using System;
using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents a return request
    /// </summary>
    public partial class ReturnRequest : BaseEntity
    {
        /// <summary>
        /// Gets or sets the order product variant identifier
        /// </summary>
        public virtual int OrderProductVariantId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public virtual int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public virtual int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the reason to return
        /// </summary>
        public virtual string ReasonForReturn { get; set; }

        /// <summary>
        /// Gets or sets the requested action
        /// </summary>
        public virtual string RequestedAction { get; set; }

        /// <summary>
        /// Gets or sets the customer comments
        /// </summary>
        public virtual string CustomerComments { get; set; }

        /// <summary>
        /// Gets or sets the staff notes
        /// </summary>
        public virtual string StaffNotes { get; set; }

        /// <summary>
        /// Gets or sets the return status identifier
        /// </summary>
        public virtual int ReturnRequestStatusId { get; set; }
        
        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public virtual DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity update
        /// </summary>
        public virtual DateTime UpdatedOnUtc { get; set; }
        
        /// <summary>
        /// Gets or sets the return status
        /// </summary>
        public virtual ReturnRequestStatus ReturnRequestStatus
        {
            get
            {
                return (ReturnRequestStatus)this.ReturnRequestStatusId;
            }
            set
            {
                this.ReturnRequestStatusId = (int)value;
            }
        }

        /// <summary>
        /// Gets or sets the customer
        /// </summary>
        public virtual Customer Customer { get; set; }
    }
}
