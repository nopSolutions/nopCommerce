namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// 关联销售
    /// Represents a cross-sell product
    /// </summary>
    public partial class CrossSellProduct : BaseEntity
    {
        /// <summary>
        /// 产品1
        /// Gets or sets the first product identifier
        /// </summary>
        public int ProductId1 { get; set; }

        /// <summary>
        /// 产品2
        /// Gets or sets the second product identifier
        /// </summary>
        public int ProductId2 { get; set; }
    }

}
