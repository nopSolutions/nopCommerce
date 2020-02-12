using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Customer activated event
    /// </summary>
    public class CustomerActivatedEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="customer">customer</param>
        public CustomerActivatedEvent(Customer customer)
        {
            Customer = customer;
        }

        /// <summary>
        /// Customer
        /// </summary>
        public Customer Customer
        {
            get;
        }
    }
}
