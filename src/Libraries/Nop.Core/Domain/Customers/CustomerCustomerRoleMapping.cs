using LinqToDB.Mapping;
using Nop.Core.Data;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents a customer-customer role mapping class
    /// </summary>
    [Table(NopMappingDefaults.CustomerCustomerRoleTable)]
    public partial class CustomerCustomerRoleMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        [Column("Customer_Id")]
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the customer role identifier
        /// </summary>
        [Column("CustomerRole_Id")]
        public int CustomerRoleId { get; set; }
    }
}