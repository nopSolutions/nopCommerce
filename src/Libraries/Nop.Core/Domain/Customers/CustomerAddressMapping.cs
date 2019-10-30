using LinqToDB.Mapping;
using Nop.Core.Data;

namespace Nop.Core.Domain.Customers
{
    /// <summary>
    /// Represents a customer-address mapping class
    /// </summary>
    [Table(NopMappingDefaults.CustomerAddressesTable)]
    public partial class CustomerAddressMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        [Column("Customer_Id")]
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the address identifier
        /// </summary>
        [Column("Address_Id")]
        public int AddressId { get; set; }
    }
}