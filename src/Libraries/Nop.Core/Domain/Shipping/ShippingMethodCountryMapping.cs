using LinqToDB.Mapping;
using Nop.Core.Data;

namespace Nop.Core.Domain.Shipping
{
    /// <summary>
    /// Represents a shipping method-country mapping class
    /// </summary>
    [Table(NopMappingDefaults.ShippingMethodRestrictionsTable)]
    public partial class ShippingMethodCountryMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        [NotColumn]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the shipping method identifier
        /// </summary>
        [Column("ShippingMethod_Id")]
        public int ShippingMethodId { get; set; }

        /// <summary>
        /// Gets or sets the country identifier
        /// </summary>
        [Column("Country_Id")]
        public int CountryId { get; set; }
    }
}