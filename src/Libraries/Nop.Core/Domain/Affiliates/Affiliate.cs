
using System.Collections.Generic;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;

namespace Nop.Core.Domain.Affiliates
{
    /// <summary>
    /// Represents an affiliate
    /// </summary>
    public partial class Affiliate : BaseEntity
    {
        public Affiliate()
        {
            this.AffiliatedOrders = new List<Order>();
            this.AffiliatedCustomers = new List<Customer>();
        }
        public int AddressId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is active
        /// </summary>
        public bool Active { get; set; }

        public virtual Address Address { get; set; }

        public virtual ICollection<Customer> AffiliatedCustomers { get; set; }

        public virtual ICollection<Order> AffiliatedOrders { get; set; }
    }
}
