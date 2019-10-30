using LinqToDB.Mapping;
using Nop.Core.Data;

namespace Nop.Core.Domain.Discounts
{
    /// <summary>
    /// Represents a discount-manufacturer mapping class
    /// </summary>
    [Table(NopMappingDefaults.DiscountAppliedToManufacturersTable)]
    public partial class DiscountManufacturerMapping : DiscountMapping
    {
        /// <summary>
        /// Gets or sets the manufacturer identifier
        /// </summary>
        [Column("Manufacturer_Id")]
        public int ManufacturerId { get; set; }
    }
}