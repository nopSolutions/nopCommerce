using Humanizer;

namespace Nop.Core.Domain.Marketing
{
    /// <summary>
    /// 产品扩展标签映射表
    /// </summary>
    public partial class ProductProductExtendLabelMapping : BaseEntity
    {
        /// <summary>
        /// 【Product.Id】
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// 【ProductExtendLabel.Id】以逗号分开
        /// </summary>
        public string ProductExtendLabelIds { get; set; }
    }
}
