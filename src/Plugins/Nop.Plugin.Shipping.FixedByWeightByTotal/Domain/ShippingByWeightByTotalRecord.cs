using Nop.Core;

namespace Nop.Plugin.Shipping.FixedByWeightByTotal.Domain
{
    /// <summary>
    /// Represents a shipping by weight record
    /// </summary>
    public partial class ShippingByWeightByTotalRecord : BaseEntity
    {
        /// <summary>
        /// Gets or sets the store identifier
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the warehouse identifier
        /// </summary>
        public int WarehouseId { get; set; }

        /// <summary>
        /// Gets or sets the country identifier
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// Gets or sets the state/province identifier
        /// </summary>
        public int StateProvinceId { get; set; }

        /// <summary>
        /// Gets or sets the zip
        /// </summary>
        public string Zip { get; set; }

        /// <summary>
        /// Gets or sets the shipping method identifier
        /// </summary>
        public int ShippingMethodId { get; set; }

        /// <summary>
        /// Gets or sets the "Weight from" value
        /// </summary>
        public decimal WeightFrom { get; set; }

        /// <summary>
        /// Gets or sets the "Weight to" value
        /// </summary>
        public decimal WeightTo { get; set; }

        /// <summary>
        /// Gets or sets the "Order subtotal from" value
        /// </summary>
        public decimal OrderSubtotalFrom { get; set; }

        /// <summary>
        /// Gets or sets the "Order subtotal to" value
        /// </summary>
        public decimal OrderSubtotalTo { get; set; }

        /// <summary>
        /// Gets or sets the additional fixed cost
        /// </summary>
        public decimal AdditionalFixedCost { get; set; }

        /// <summary>
        /// Gets or sets the shipping charge percentage (of subtotal)
        /// </summary>
        public decimal PercentageRateOfSubtotal { get; set; }

        /// <summary>
        /// Gets or sets the shipping charge amount (per weight unit)
        /// </summary>
        public decimal RatePerWeightUnit { get; set; }

        /// <summary>
        /// Gets or sets the lower weight limit
        /// </summary>
        public decimal LowerWeightLimit { get; set; }
    }
}