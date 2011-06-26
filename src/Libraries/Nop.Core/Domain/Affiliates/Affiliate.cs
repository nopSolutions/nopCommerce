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
        private ICollection<Customer> _affiliatedCustomers;
        private ICollection<Order> _affiliatedOrders;

        public virtual int AddressId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public virtual bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is active
        /// </summary>
        public virtual bool Active { get; set; }

        public virtual Address Address { get; set; }

        public virtual ICollection<Customer> AffiliatedCustomers 
        {
            get { return _affiliatedCustomers ?? (_affiliatedCustomers = new List<Customer>()); }
            protected set { _affiliatedCustomers = value; }
        }

        public virtual ICollection<Order> AffiliatedOrders
        {
            get { return _affiliatedOrders ?? (_affiliatedOrders = new List<Order>()); }
            protected set { _affiliatedOrders = value; }            
        }
    }
}
