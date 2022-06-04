using System;
using Nop.Core.Domain.Common;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents a customer
    /// </summary>
    public partial class Customer : BaseEntity, ISoftDeletedEntity
    {
        public int CustomerProfileTypeId { get; set; }
    }
}